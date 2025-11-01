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
