Notes:
- Walking with WASD works properly
- You can press M1 to make Dog follow mouse position, or M2 to follow Playerr
- Sheeps follow player properly with no issues so far

Changes:
- Changed Player and Dog scale to 0.5
- Changed Camera_1 lens to 10
- Edited DrekavacAI.cs
- Added collider and rigidbody to Sheep
- Added Ignore Collision layer and put it on Player and Sheep to prevent them from knocking each other away

When you set up new scene:
Because sheeps and dog are different type of agent, you need to currently create two navmeshes for them. You can do so by selecting Plane (or any object) and attaching 'NavMesh Surface' to it and pressing "Bake".
