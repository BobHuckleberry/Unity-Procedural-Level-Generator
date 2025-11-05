# **Unity-Procedural-Level-Generator**
A room-based, breadth first search designed level generator made to generate branched-out maze rooms.


![Alt text](screenshot.png)
## Overview
This level generator is designed for games that need a randomised, maze like structure to it, like horror games or rogue-likes.
It is also infintitely generated as each level has a room with a button to the next level, which restarts the generation cycle.

## Explanation of the code
1. The scene is loaded, triggering the level generations startGeneration(). (this is the core loop of the program.)
2. startGeneration instantiates the enterance room, and the roomQueue that is used for BFS.
3. The algorithm then loops all the possible available entry points for that parent room until there is no more available.
4. within this loop, it selects a random candidate room or hallway and then tries to align them together to checkoverlap.
5. if there is overlap, the new room is removed and the loop is repeated, if there isnt the room is added to the queue.
6. once the room has all its available entry points filled, the room first added to the queue is put in.
7. this process is all looped within the while loop until max room count is reached or max attempts reaches zero.

## Further improvements
I want to rework this algorithm into one that uses some form of 3D grid. This would allow the usage of more complicated algorithms such as A*. This would make gameplay more engaging with rooms connecting into eachother and creating loops the player can get lost in.


