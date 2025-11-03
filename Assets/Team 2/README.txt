Welcome!
        --- Sprint 1 ---
To make sheep work all you have to do is 
- make a herd controller game object and attach "HerdController" script to it, give the half-extents something like x = 8, y = 8 and attach a player transform
- make a NavMesh surface on the area where you need the sheep to walk
- place sheep prefabs and test if they follow player
- please make a tag "Sheep" and place it on sheep prefab (enemy 1 team requested that)

        --- Sprint 2 ---
Please create a collision layer for enemies, in the "Sheep" prefab look for script called "Threat Sensor" and in the layer mask variable choose an enemy layer ( was set to obstacle for testing).
In the herd controller make the player's center be dog transform, that is temporary until we find a new solution for dog barking implementation with Player team
If you want a sheep to be a straggler (herdless) set the variable "Start as straggler" to true on SheepStateManager under the sheep parent

        --- Sanity System ---
SanityTracker.cs is currently in Scripts/AI/ but should probably be moved to player code for consistency.
To use: attach SanityTracker to a GameObject, optionally enable debug mode and assign a TextMeshProUGUI component for on-screen display.

        --- Event Demo Simulator (Testing) ---
EventDemoSimulator.cs allows other teams to test sheep events without full game setup.
Attach to GameObject, assign sheep prefab and player transform in inspector.
Keybinds: Tab (select sheep), F (freeze), T (threat), K (kill sheep), J (spawn sheep), H (help)
Use this to test SheepDeathEvent, SheepJoinEvent, and SanityChangeEvent integration.
