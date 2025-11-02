The current scene to use is called Level1_+_Obstacles.

The general layout of the level remains wholly similair. The level is effectively split in three, denotated by the three blank cubes named playerlocation, startsheephere and startshophere.
Replace these objects as needed.

There are three components that NEED to be included.

our temp player contains a prefab called SnowEffect. add this component under the camera's hierarchy, and right above it position-wise.

There is also TriggerThing. position the pyramid between the player and the camera, scale it up to fill the distance, and change the width as needed. 
Any object that has a rigidbody and is on the selected layer will dissapear while inside the trigger.

feel free to use this component between levels, just remember to put any obstacle prefab on the selected layer and make the root have a kinematic rigidbody + any connected meshes need colliders (only root needs rigidbody)

The third part is that the MusicLooper needs to remain.


for any questions, feel free to contact any and all members

--------------------

Sprint 2

VFX - PostProcessing - Lighting

VFX: 
Fog: There is a FogEffect Prefab that needs to be attatched to the Player as a child.

Snow: There is a SnowEffect Prefab that needs to be attatched to the Main Camera as a child.

FullScreenShader: There is a FogShaderMaterial that needs to be attatched as a Full Screen Pass Renderer Feature.
In order to access this Shader:
Go to Edit
Project Settings
Graphics
Click the default Render Pipeline, then click the highlighted asset.
In the inspector, click the asset in the Render List.
Lastly at the bottom in the Inspector (if the previous Full Screen Pass Renderer Feature is still there, you can simply change the material for the FogShaderMaterial) manually add the Full Screen Pass Renderer and add the FogShaderMaterial material.


PostProcessing: There is a Global Volume Prefab that needs to be added in the Hierarchy of the level. Make sure to enable PostProcessing in the Inspector of the Main Camera.

Lighting: The lighting of the scene needs to be changed.
In the Lighting tab under Environment, change the Environment Lighting Source to Gradient. These are the Color codes for each Color: 
Sky: H238 S25 V53
Equator: H207 S22 V32
Ground: H0 S0 V39

Underneath this there is a fog setting, Enable it and change the color to Black Hexcode:000000. Change the mode to Exponential and change the Density to 0.05