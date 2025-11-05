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
4. within this loop, it selects a random candidate room or hallway and then tries to align them together to checkoverlap


