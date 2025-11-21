HEALTH SYSTEM INTEGRATION:
NOT FOR INTEGRATION TEAM!!!
1. To main class of the object add instance of the Health class. It is not MonoBehaviour, so you need to add it in script using constructor.
2. If you want to make object damageable, add implementation of IDamageable interface to your main class. Make sure that you have a collider on this object.
2. If you want to make object healable, add implementation of IHealable interface to your main class. Make sure that you have a collider on this object.
2. If you want to make object killable, add implementation of IKillable interface to your main class. Make sure that you have a collider on this object.


FOG OF WAR INTEGRATION:
1. Add fog of war prefab on scene.
2. Assign player transform.
3. Check config for Fog Of War and adjust plane size if needed.
4. Create LevelData for each level. It contains map highest and lowest points, you need to adjust it. You can turn on gizmos on fog of war manager to see these points in edit mode.
5. To make any object a fog reveler - add Fog Revelaer component. In componnet add revealer configs - ecah config represent one shape. So, if u need object to reveal circle and cone(like player) add 2 configs NOT two components. Player and dog already have revealers set up.
6. To make object hidden in the fog - add Hidden In Fog component. In componnet assign MeshRenderer of the hidden object. If u want to dynamically add new hidden objects(spawn enemy or smth like that) - on spawn object has to broadcast AddHiddenObjectEvent, on despawn - RemoveHiddenObjectEvent.


TOOL SLOTS:
Slots can be changed on mouse wheel or number keys(1 and 2 for now)
Right now, player has 2 slots:
1. Whistle gives commands to dog
    LMB - go to cursor position
    RMB - set standart mode(follow the player and if there are free sheep, add them to heard)
    R - bark(not used for now, only writes in console. Latter will be used to scare enemies)
2. Rifle
    LMB - fire(only one bullet per click, has delay between shots)
    R - reload
    RMB - Aiming



EFFECTS:
Check screenshots in discord