# Car Go Space?

## No, car no do that, car no fly

Working on training an agent to drive a car in 2d space hopefully going from very simple to progressively more complex scenarios.

Trying to use a camera simulation set up. It's a 2d space so just calculating how much of the raycast is road vs not.

Also including distance to target, if I can figure out how to make it more complex I'll add some route planning.

## Current State 31st Dec, 2024

I have it making some decent progress towards the target going from A to B. After 100,000+ ish steps is falls apart.

### Later the same day

Changed a few things, like the end tile being seen as road, the sensors seeing what percentage of their vision is road, a sensor mimicking steering wheel input I think, and a sensor for normalized distance to target. 

I got tensorboard set up and have been learning how to read it, and that helped. At first I thought the learning rate may be too high, but as it turns out I just needed to let it train more. So I trained in increments of 50,000. Figured out how to train in the background with multi environments so it goes faster as well.

At Around 200,000 steps the model is performing in a way I think we could introduce the next steps and have some fun with it. That and I just kinda want to.

## 1st Jan, 2025! Happy New Year!

Attempting to add the next phase or level of learning for the car. I'm going to attempt curriculum learning.

### Later

Took a while to get things figured out, went back and made the check if the car was on track based on 2d colliders instead of tile cells.

Starting to see the value of tensorboard! It is sweet!

## 2nd Jan, 2025 a little after midnight

The curves track is being pretty well mastered by the little car as I watch it visually. Looking forward to some more advanced challenges for it in the days to come. That'll be my near term focus of getting a decent progression through curriculum until it can do like a little city or mini highway kind of thing. Then, with a decent foundation like that, I want to pull out the mlagents function and write the training code myself.

## 8 and 9th Jan, 2025

Working on the third scene. For now just seeing how we do with one 90 degree turn. The challenge I see is that I need to be able to go either direction and have some sensible reason for going there. My thought is I can randomly move the end point of the third scene and give as input something like "right" or "left" to the agent matching where the destination is for it to learn this.

Moving forward though, I'll need some sort of navigation input. I'm thinking their has got to be a decent way of doing like A* and giving the next step as input and rewarding the car for being on track? Or, maybe it's saying what the next node should be and rewarding for getting closer to that?

## 16th and 17th of Jan

The right or left idea seems to be okay, and I'm working on nav input. Getting weird results with the gizmo to help with debugging so my understanding of overlapcircleall works and .lerp are not as good as I'd like. This may not be the way. I'm trying to find a way that emulates how a nav system would determine that data so that I can hand that back to the agent in a meaningful sensor way.

If I use this updated script and make the composite operation "none" rather than "merge" I get closer to some userful concept. The problem is then the car always thinks it is crashing haha so maybe we can use "none" and figure out how to get the car to drive on it still, since I think the collider stuff seems to "smooth" out to much on merge. When it's set to merge the most I get is a few nodes at the far end of the track right now.

## 18th of Jan

I think this is a better basis for the navigation concept. Just looking at the tilemap and putting nodes in the center, which works when we don't have lanes or turn lanes so it'll get more complex, but I think this is a better starting point. I will have to figure out how to address the curved points and some better edge connection logic, but I like the direction this is heading.

One thing I think could be helpful as the complexity increases is doing a rectangle between the car and the final point and creating the graph based on that, or finding a way to limit the adacency graph and nodes to just those that fall within the the boxed area.

### Later

Getting closer but weird issue with curves. O okay now I got it, time for path finding.

## 5 and 6th of Feb

Been sooo long with work stuff taking up lots of time as we navigated some interesting times.

Tonight I got started on learning A-star. It's been a while since I've done greedy search, but some of the concepts are coming back thankfully. Hopefully tomorrow night I can actually finish the implementation and get started on using it in training.

I am trying to get chatgpt to teach me, not just code stuff for me on this project so sometimes progress feels weirdly slow compared to when I just have it crank out some boiler plate code or something I already know how to do. But, I love working on this. I really want to get it to where it can navigate a simple city scene and at that point I want to implement the underlying AI training code into the real details.

## Feb 10th

Last weekend I got A* to a decent enough place I'm going to start using it for training. The third scene is where it actually means something for now since that is the only one it has any real directional choice to make. Haven't done the training yet, had to mess with the colliders for a while until I understood a little better what was happening and I think I got it now.

Time to start training with the nav data and then to go build a little neighborhood is the next step I think.

## Feb 11th and 12th

Okay so the nav data needed some tweaks as did the camera sensor. With that done training on scene 3 is much improved! Ooof but I made a change and now have issues with the end adjuster. Should be simple, so I will get back to it soon.

## Feb 15th

Fixed the issue that was actually with edge connections messing with path finding in A*. Still some work to do on end adjuster, but with a working A* we are improving!

## Feb 17th

End adjuster is working now and I think we are getting pretty close! Definitely seeing some issues where it learns to just go slow and move forward since that reward is slightly larger than the time step penalty. I think I need to balance that out better and also find a way where I can increment learning to do 90 degree turns and such.

Even before I made changes, around step 750,000 the ai was beginning to find the destination! One change that was screwing it up though was every crash was reseting the destination and I meant for it to be held until it found it so it could learn to use nav data.

## Feb 18th

I think it's actually learning to follow the nav now, but on scene three it starts forgetting because it goes too long and get's too negative of a reward. I think we may need to use a transformer or set time limits on runs or maybe a "crash" like result for going back. I also think I could set a time limit on how far it needs to get before it is a "crash" equivalent

## Feb 20th and 21st

The reward function is a little more solid now! Giving it feedback as it goes towards the nodes is helping! The time limit is huge too, It felt like it was running into or off of an absolute cliff in the 90 degree point, so lesson learned, don't let it despair haha 

What's legit is it was getting it figured out before I added any vector stack to the observation. I am testing that now to see if we learn any faster.

So far testing the increased vector stack it seems slower, but in a sense more methodical. I suppose this makes sense, more weights to fit and so it is more complex function it is estimating. Probably a good sign is what I feel. We will keep watching and see!

Interesting, it seems like the curves mean more to it now and it gets a little tripped up there at times, but cool to see it not just like cheating the curve. This makes me very excited to get my own transformer as the model.

Ah the end adjuster was too adjusted lol gonna have to step back to an early model and try again.

## April 16

O it has been too long. I was able to get on and work for a little a few weeks ago and then again tonight I was able to just get started on how to connect my own python training code to the simulation. I figure I can keep tweaking with ML agents or I can start to learn myself and tweak my own training code and learn from being in it where I really want to be. So, yes I am jumping in here!

