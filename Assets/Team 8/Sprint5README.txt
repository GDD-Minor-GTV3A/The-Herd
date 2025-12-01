Sprint 5 — For the Integration Team

There is a prefab called “SHAMAN.”
The prefab includes shaman animations, sounds, spawning, and dialogue.

All you need to do is place the prefab in the scene, and when the player interacts with it (triggers it), the shaman will spawn. Everything is already assigned except for the UI part.

When you place the prefab in the hierarchy, go to SHAMAN → Shaman Dialogue Manager, and assign the UI elements (these should already be in the hierarchy as well, Team 9 UI).
If you get confused, check the scene “Shaman Interaction” in our scenes folder.

How do sounds work?
The prefab will look for the Sound Manager in the scene, disable all other sounds when the shaman appears, and play specific sounds instead. You don’t need to do anything for this, just make sure an instance of the Audio Manager exists in the scene.



Inventory, collectables, and everything related to trading with the shaman are fully implemented as independent features, BUT we didn’t have time to integrate them with the dialogue to make them functional (we’ll do that ASAP).

What you should have in the scene:

Shaman first interaction fully functional dialogue

Shaman spawning after triggering a specific area

Shaman sounds

If you need any help, reach out to me:
Jafar Chiha — Discord: shiha777