The scene to be used is called "Level1 from Integration Team". It should be duplicated and updated with other team's changes. Noted below are the additional changes that need to be done and checked.



VFX - PostProcessing - Lighting

VFX: 

Snow:
The SnowEffect Prefab is updated, hopefully just merging branches will update the already placed VFX with the new version of the Snow.

Fog: 
There is a FogEffect Prefab that needs to be attached to the Player as a child.

PostProcessing: 
There is a Global Volume Prefab that needs to be added in the Hierarchy of the level. Make sure to enable PostProcessing in the Inspector of the Main Camera.


Lighting: 
The lighting of the scene needs to be changed.
In the Lighting tab under Environment, change the Environment Lighting Source to Gradient. These are the Color codes for each Color: 
Sky: H238 S25 V53
Equator: H207 S22 V32
Ground: H0 S0 V39

Underneath this there is a fog setting, Enable it and change the color to Black Hexcode:000000. Change the mode to Exponential and change the Density to 0.05