Scripts used in the current scene, how to use them and where to find them.


Outline
Attached directly unto the player object, and set to Outline hidden (outline mode) and a width of 10 (outline width). Color does not matter currently and can be changed.
Usage of the Outline script is very easy. The modes specify under what circumstance the outline shows up. 
The color is what color the outline will be and the width is how thick the outline will be. An important aspect of the Outline is that it is always visible over other objects.
Script is placed under Team 3/Mirco/MircoScript

CollisionHandlerWithHook
Attached unto the Event cubes in TriggerPlaceholders. All Event objects are set up to trigger only gameobjects.
The script will only react to the player entering and nothing else (done through player tag condition). 
There are two configurable parts, whether the event is a one-time thing, and then a blank UnityEvent to fill.
Script is placed under Team 3/Final Export






The usable scene for level 1 can be found under Team 3/Final Export.

Level design documentation for level 1 is split into 3 parts. Components that must remain, Components that must be replaced, and design intention.

Components that must remain: scripts or components attached to objects that can or must be replaced. So if said object is replaced, refer to this document to see what additions must be ensured.

Components that must be replaced: stand ins for objects that other teams are responsible for. This is simply a list of components that should at the very least not appear in the final version for test n chill 1.

Design intention: a broad explanation of how the level should work and play out. This serves to inform others when they make changes to the level.



Components that must remain

there should be an audioplayer object with an audiosource component. Set the resource to “Ambient 2 WIP” and play it on awake and loop, rest can be changed.
Player should have an outline script with specs documented in Tech documentation.
The hierarchy has a folder called InvisibleBounds, they should be invisible and force the player roughly towards the roads.



Components that should be replaced
Player.
Replace it with the player made by player team.

the Sheep.
Replace each sheep with one as made by sheep team. They should remain around their assigned locations until the player approaches them or triggers the SheepEvent (decision up to you)

placeholder shaman
Replace it with the shaman as made by the shaman team.

placeholder player start
This is simply where the player should start.

all events found under TriggersPlaceholder
While they shouldn’t be replaced, the events should be defined. Whatever should happen at that level section should be called from that level section.



Design intention
The level is split into three parts with each dedicated to a mechanic.

The first part consists of thin pathways leading towards a little drop.
 The player should take this moment to get used to the controls, the aesthetic and of the fact that gravity is used in this game.

The second section starts from the SheepEvent object onwards. Consisting of a broad open area. Players learn about the sheep here and get the room to experiment with them.

The third section is another thin section that leads towards the shaman. The thin road will force the player to interact with the shaman.

Then the level ends at the gates.
