HEALTH SYSTEM INTEGRATION:
1. To main class of the object add instance of the Health class. It is not MonoBehaviour, so you need to add it in script using constructor.
2. If you want to make object damageable, add implementation of IDamageable interface to your main class. Make sure that you have a collider on this object.
2. If you want to make object healable, add implementation of IHealable interface to your main class. Make sure that you have a collider on this object.
2. If you want to make object killable, add implementation of IKillable interface to your main class. Make sure that you have a collider on this object.


FOG OF WAR INTEGRATION:
1. Add fog of war prefab on scene.
2. To make any object a fog reveler - add Fog Revelaer component. In componnet add revealer configs - ecah config represent one shape. So, if u need object to reveal circle and cone(like player) add 2 configs NOT two components.
3. To make object hidden in the fog - add Hidden In Fog component. In componnet assign MeshRenderer of the hidden object. If u want to dynamically add new hidden objects(spawn enemy or smth like that) - on spawn object has to broadcast AddHiddenObjectEvent, on despawn - RemoveHiddenObjectEvent.
