# Team 5 | Level 3
*That's your team folder! Here you can work freely with your team.*

*Enjoy!*

---
### Scene
The scene that should be used can be found in Team 5 -> Scenes -> Level3Concepting.

<!-- Floors and backwalls all have correct collisions, there are no walls coming from the players camera angle. Updated floors and walls are in the works.

Warning: there were issues with scaling in 3d programs and now everything in unity is scaled up at least 50 times.<br>
&emsp;&emsp; This will NOT be final, but lack of time causes me to make this the only option, will be fixed in future.

Temporary enemy#2 spawnpoints and walltraps are grayboxed and visible in hierarchy.

There are some InsideOutsideTriggers grayboxed around. these areas would be entrances/exits from the cave system, increasing/decreasing length of view (when going inside decrease and when going outside increase), changing sound effects and visual effects (falling snow for example). -->


### Hierarchy
The hierarchy consists of every 3D model used in the scene, as well as some empty objects to be used as parent for the instantiated objects upon loading the scene, as done by the LevelManager script. There's also a parent object containing potential spawnpoints and a SpawnTrigger object, which is the physical trigger as well as the script to trigger the enemy spawns.

### Gameplay
The level is currently playable. The CameraFollowing script is temporary to make the level function, and the positioning of this camera will be adjusted in the future once everything is properly integrated.

The "peak scare" can be triggered by heading to the end of the level, where the trigger is located. This will cause enemies to spawn, creating a chase back towards the village. (Enemy AI isn't implemented in this scene yet, so they don't actually move.) The most efficient route can be seen in the image below.

![Route through the level](images/levelroute.png)

Here the green dot is the player spawn, the blue line the trigger and the red line the route.

### Notes for integration team
- If needed: The LevelManager in the hierarchy needs to contain prefabs for the player, dog, sheep, enemy1 and enemy2. In case these aren't in the scene yet (they should be), they need to be added manually.
- Same goes for the transforms for the parent objects. These can just be dragged from the hierarchy, if needed.

![Setup for the level manager object](images/LevelManagerSetup.png)

- The SpawnTrigger object in the hierarchy needs to contain a list of all the spawnpoints, as well as the "OnSpawnTriggered" event. This should already be in place, but it's put here in case it isn't.

![Setup for the spawn trigger object](images/SpawnTriggerSetup.png)

- If possible: provide the enemies (enemy2) with AI. If something from us is needed for this, just let us know.
- Aside from this, if it's already in that state, the level could be connected to the village.

### Other things
- There seems to be some offset in the sheep-prefab. They do spawn, but not on the assigned position. This will be fixed in the future; don't worry about this for now.
- The dog doesn't actually follow the player yet, this too will be fixed in the future.