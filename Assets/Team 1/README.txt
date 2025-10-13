HEALTH SYSTEM INTEGRATION:
1. To main class of the object add instance of the Health class. It is not MonoBehaviour, so you need to add it in script using constructor.
2. If you want to make object damageable, add implementation of IDamageable interface to your main class. Make sure that you have a collider on this object.
2. If you want to make object healable, add implementation of IHealable interface to your main class. Make sure that you have a collider on this object.
2. If you want to make object killable, add implementation of IKillable interface to your main class. Make sure that you have a collider on this object.
