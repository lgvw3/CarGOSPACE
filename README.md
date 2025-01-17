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