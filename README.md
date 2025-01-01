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