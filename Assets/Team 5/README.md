# Team 5 | Level 3
<!--
### Scene
The scene that should be used can be found in Team 5 -> Scenes -> Level3Concepting.

### Hierarchy
The hierarchy consists of every 3D model used in the scene, as well as some empty objects to be used as parent for the instantiated objects upon loading the scene, as done by the LevelManager script. There's also a parent object containing potential spawnpoints and a SpawnTrigger object, which is the physical trigger as well as the script to trigger the enemy spawns.

### Gameplay
The level is currently playable. The CameraFollowing script is temporary to make the level function, and the positioning of this camera will be adjusted in the future once everything is properly integrated.

The "peak scare" is located at the end of the level (top right). To trigger the scare, you must first enter the circular area and then exit it the same way. When exiting, enemies will spawn around you and the "chase" will begin. During the chase, enemies will block certain paths and force the player down a specific route. Currently, the chase doesn’t lead anywhere meaningfully, as this is just a proof of concept.

The most efficient route to the end of the level can be seen in the image below.
![Route through the level](images/levelroute.png)

Here the green dot is the player spawn, the blue line the trigger and the red line the route.

### Notes for integration team
- If needed: The LevelManager in the hierarchy needs to contain prefabs for the player, dog, sheep, enemy1 and enemy2. In case these aren't in the scene yet (they should be), they need to be added manually.
- Same goes for the transforms for the parent objects. These can just be dragged from the hierarchy, if needed.

![Setup for the level manager object](images/LevelManagerSetup.png)

- The SpawnTrigger objects in the hierarchy need to contain a list of all the spawnpoints. The "OnSpawnTriggered" event should also be linked to a function. This should already be in place, but it's put here in case it isn't.

![Setup for the spawn trigger object](images/SpawnTriggerSetup.png)

- If possible: Correct the scales of the Drekavac and the sheep. They seem way smaller then intended.
- If possible: Correct the Drekavac behaviar, they spawn and recognize the navmesh correctly but don't seem to recognize the player in our level. If something from us is needed for this, just let us know.
- Aside from this, if it's already in that state, the level could be connected to the village.

### Other things
- The dog doesn't actually follow the player yet, this too will be fixed in the future. -->

---
# Scene
The scene that should be used can be found in Team 5 -> Scenes -> Level3.

# Hierarchy
A lot of the scene is in the same state as it was at the previous demo, as we just copied the scene from the _Game folder to continue working in. There are some different things however:
- The "FogOfWar" object has been disabled as we have opted to use a different lighting/vision system (see [New additions](#new-additions)).
- The SpawnTriggers-parent object has had changes to it's children, as we're using UnityEvents to handle the chase sequence, which required a change here. The SpawnTrigger script handles all the logic for this. Conceptually, this works the same way as it did in the demo.
- The SpawnPoints-parent object contains all spawnpoints for the enemies. The triggers will spawn enemies at these places.
- The CavedWalls-parent object contains both the GameObjects for the walls and the triggers for enabling or disabling them. The CaveInWalls script will handle this logic.
- Lastly, Scare is a parent-object holding some lights to make the quest area more scary as the chase starts.


# Gameplay
With the changes to the chase sequence, a new route is introduced to the player.

![Route through the level](images/Level3Walkthrough.png)

- The player (as well as their sheep and dog) will spawn at the spot marked with "1" in the image.
- The green line marks the most efficient route to the quest area.
- At "2", the player is forced to go left, as this wall will cave in when the player tries to go right.
- At "3", the chase will begin when the player exits the quest area, causing enemies to spawn at the red dots in the quest area.
- The chase has the player follow the path marked with the yellow line.
- To force the player to follow the yellow line, enemies will spawn at "4" when the player gets near it.
- Additionally, at "5", there will be another cave-in.
- At "6" more enemies will spawn to usher the player to the right direction.
- The player can exit the level at "7". There will be a bridge here in the future, but it isn't in place yet. There is also a trigger here, which will destroy the bridge and the cave-in at 2, giving the player safety again.

# New additions
This sprint, the following things have been added:
- A new vision/lighting system. The fog of war was messing up the textures. We have asked the player team if this could be changed and we received a no from them. Turns out, it can be changed as we just did it ourselves. The new lighting now actually casts shadows and it also looks a lot more creepy in our opinion. The only thing missing here is the rendering/derendering part from the fog of war, but this can always be added later.
- A different chase sequence has been added. As explained in the "[Gameplay](#gameplay)" section of this Readme, there are now walls caving in and the spawns of the enemies have been changed to move the player into a certain direction. With both the sheep and enemy AI not being fully fleshed out, whether it's "balanced" is still up for debate, though this can always be changed later. Also, the caved in walls have no models, textures or animations yet, but this can be replaced easily.
- New assets for bridges have been added to be implemented in the level later.
- Some sound effects and atmospheric audio elements have been added to the level.

# Remaining tasks
- Bridges still need to be added to the level for it to be able to be fully played. The assets have already been made, so maybe this will be implemented at the time of the demo. It just won't be there at monday 12:00.
- The connection with the quest also still needs to be implemented. There will be a meeting with the quest team about this on tuesday (so after the deadline).
- More assets for detail and audio will be added in the future as well.
- The shaman doesn't exist in this level yet. This too will follow hopefully next sprint.

# Notes for integration
As far as we're concerned, the level can just be placed back into the game. We haven't changed much about the connections implemented in the previous sprint.

Things to keep in mind:
- The positions of the camera, player, dog and sheep are the intended locations for our level, so please keep them this way.
- For a full test run of the level, you could temporarily add a bridge or just a plane over the gap at "7" in the walkthrough image. This exit then needs to be connected to the village as well (just like the entrance where the level will start).
- Our "LevelManager" script still exists, but is currently completely unused (at least, that should be the case). Just don't worry about it :).
- Lastly, the sheep and enemy AI still could use updates. We're assuming the respective teams made changes past sprint and this could seriously impact both the difficulty and the vibe of the level. Please let us know if something is wrong with our level if there are changes to the sheep and the enemies.

Aside from that, feel free to ask if you have any more questions about our level :).
