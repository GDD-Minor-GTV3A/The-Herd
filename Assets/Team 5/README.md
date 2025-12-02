# Team 5 | Level 3
# Scene
The scene that should be used can be found in Team 5 -> Scenes -> Level3.

# Hierarchy
The hierarchy has a few notable things for this sprint:
- The "NavMesh" object is disabled. We've written a script (Level3Manager.cs) that will automatically enable it upon running the scene. We only disabled it so we could see what we're doing.
- CavedWalls has some new child objects for the newly added animation/particles (see [New Additions](#new-additions)).
- "anim bridge 2" is the new animation for the also added bridge at the end of the level.
- At the bottom of the hierarchy are some parent objects for deers, trees, stones and stonepaths. These have been added to make the scene more immersive.


# Gameplay
The walkthrough is still the same as previous sprints.

![Route through the level](images/Level3Walkthrough.png)

- The player (as well as their sheep and dog) will spawn at the spot marked with "1" in the image.
- The green line marks the most efficient route to the quest area.
- At "2", the player is forced to go left, as this wall will cave in when the player tries to go right.
- At "3", the chase will begin when the player exits the quest area, causing enemies to spawn at the red dots in the quest area.
- The chase has the player follow the path marked with the yellow line.
- To force the player to follow the yellow line, enemies will spawn at "4" when the player gets near it.
- Additionally, at "5", there will be another cave-in.
- At "6" more enemies will spawn to usher the player to the right direction.
- The player can exit the level at "7". There is a bridge here to give the player a way out. There is also a trigger here, which will destroy the bridge and the cave-in at 2, giving the player safety again.

# New additions
This sprint, the following things have been added:
- New level elements to make the scene more immersive. Assets like stones, tree stumps, trees and dead animals have been scattered throughout the scene.
- All walls have received a collider so the player can no longer walk into them.
- Before the chase, some initial enemies are added into the level to give the player a bit of a challenge and preventing the level from becoming a "walking simulator".
- A bridge has been placed at the end of the level with an animation of it breaking.
- Wall cave-ins have received a swift animation and particles, as well as a new look through proper boulder and pebble assets.

# Remaining tasks
- There seems to be an issue with enemy spawns. This is our fault and we'll fix this soon.
- There's no audio for the wall cave-ins.
- Quest implementation.
- New enemy (enemy 4, I think it was?) needs to be added to the level and the level will probably need some adjustments to facilitate this new enemy.

# Notes for integration
As far as we're concerned, the level can just be placed back into the game. We haven't changed much about the connections implemented in the previous sprint.

One thing that is important is that the exit and "7" in the image in [Gameplay](#gameplay) gets connected to the village level. It can end up at the same place as where the entrance at "1" ends up. That shouldn't really matter.

Aside from that, feel free to ask if you have any more questions about our level :).
