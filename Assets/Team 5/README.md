# Team 5 | Level 3
*That's your team folder! Here you can work freely with your team.*

*Enjoy!*

---
### Scene
The scene that should be used can be found in Team 5 -> Scenes -> LayOut1stDraft.

Floors and backwalls all have correct collisions, there are no walls coming from the players camera angle. Updated floors and walls are in the works.

Warning: there were issues with scaling in 3d programs and now everything in unity is scaled up at least 50 times.<br>
&emsp;&emsp; This will NOT be final, but lack of time causes me to make this the only option, will be fixed in future.

Temporary enemy#2 spawnpoints and walltraps are grayboxed and visible in hierarchy.

There are some InsideOutsideTriggers grayboxed around. these areas would be entrances/exits from the cave system, increasing/decreasing length of view (when going inside decrease and when going outside increase), changing sound effects and visual effects (falling snow for example).

The hierarchy contains an object LevelManager, containing a script used for spawning in the player and the sheep. In the inspector of this object, both the player and sheep prefab need to be added in the corresponding fields, as well as the number of sheep. This will spawn the objects accordingly upon starting the scene. This does currently use Unity's Start()-method, which will need to be changed later on. Either way, it needs to run upon loading the scene.