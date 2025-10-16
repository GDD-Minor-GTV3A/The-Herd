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