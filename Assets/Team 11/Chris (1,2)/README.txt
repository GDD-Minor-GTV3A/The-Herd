Notes:
- Walking with WASD works properly
- You can press M1 to make Dog follow mouse position, or M2 to follow Playerr
- Sheeps follow player properly with no issues so far
- Press "E" to spawn enemy
- Press "Q" to open shop when close to shop keep
- Enemy drags sheep away, but for some reason the grab position is off when Navmesh Agent is enabled
- Level 1's navmesh breaks Dog's behavior and messes with Enemy's, but Sheep seem to have no issue

Changes:
- Changed Player and Dog scale to 0.5
- Changed Camera_1 lens to 5
- Edited DrekavacAI.cs
- Added collider and rigidbody to Sheep
- Added Ignore Collision layer and put it on Player and Sheep to prevent them from knocking each other away
- Changed canvas to match with screen size and set X to 2600
- Made an "OpenShop" script for enabling and disabling shop UI
- Changed Player walk speed to 4 and sprint speed to 10

When you set up new scene:
Because sheeps and dog are different type of agent, you need to currently create two navmeshes for them. You can do so by selecting Plane (or any object) and attaching 'NavMesh Surface' to it and pressing "Bake".
