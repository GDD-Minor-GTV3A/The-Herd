Build from the scene called "ReworkedLevel1". It should be duplicated and updated with other team's changes. Noted below are the additional changes that need to be done and checked.



Lighting:   (check lighting video)

Gradient:
Sky Color: H219 S40 V75
Equator Color H203 S40 V75
Ground Color H0 S0 V43

Fog Lighting (in the same light tab):

Color: H0 S0 V25
Mode: Exponential
Density 0.01

Full Screen Shader:
Make sure it's turned on. (not sure if it remained on during previous sprint, otherwise there are previous documentations explaining how they should add it back).

Post Processing:
Not sure if this remained from the previous sprint, but simply drag and drop the Global Volume Level 1 Prefab in the level and you'll be fine.

Fog VFX:

Drag and drop the Prefab FogEffect onto the player as a child. Position should be X0 Y3 Z0. If the Y is on 0, the particles will clip into the floor.

Snow VFX:
There is a new Prefab called SnowEffectRework. Make sure to REPLACE the old SnowEffect Prefab with it. Don't delete the old SnowEffect Prefab in the Prefab folder (in the scene it's fine), because the new prefab is a parent containing multiple SnowEffect Prefabs.

Grass Sway Shader:

There is a Grass model which has the Shader attatched to it. There is a prefab called SwayingGrass that can be edited for the strength of the wind and the color of the grass (not that it's needed).





REQUIRED

when you replace the player with its new version, place the fog vfx back according to specifications.
There are 4 instances of SoftsceneCaller where the "speaker" reference needs to be reset to the player. cutsceneTrigger under wolf, the cylinder of the ravinescene, and the dialoguetriggers of the cart and house dialogues. all found under Cutscenes.

There are no dog or sheep placed in the scene yet, we have designated spots for the sheep.