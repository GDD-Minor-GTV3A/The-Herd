using UnityEngine;
using UnityEngine.Serialization;

#nullable enable // Chris: This needs to be added for nullable values to work. Otherwise it will show a warning
namespace Team_7.Scripts.AI.Phantom
{
    [CreateAssetMenu(fileName = "PhantomStats", menuName = "Scriptable Objects/PhantomStats")]
    public class PhantomStats : ScriptableObject
    {
        [Header("Health/Respawn Settings")] 
        [Tooltip("The amount of times the enemy can respawn")]
        public int health = 3;
        [Tooltip("The amount of clones the phantom spawns whenever it appears")]
        public int initialCloneAmount = 4;
        [Tooltip("The maximum amount of clones a single phantom will summon")]
        public int maxCloneAmount = 7;
        [Tooltip("The amount of seconds between spawning a new clone while in the shooting state")]
        public float cloneSpawnDelay = 5;
        [FormerlySerializedAs("clone")] [Tooltip("The clone prefab")]
        public PhantomFake? fake; // Chris: Made this nullable to avoid a warning. You can delete this comment once its fixed.
        [Tooltip("How long the player should continuously look at the enemy before it loses 1 health.")]
        public float lookDuration = 2;
        [Tooltip("How many seconds the Phantom is stunned by a dog bark")]
        public float stunDuration = 10;
        [Tooltip("The minimum distance from it's previous location the enemy will respawn at.")]
        public float minRespawnDistance = 50;
        [Tooltip("The maximum distance the enemy will respawn at.")]
        public float maxRespawnDistance = 100;
        [Tooltip("Should be the same as the angle of the player's vision cone, the enemy takes damage if it's too long inside this area.")]
        public float DamageAngle = 30;
        [Tooltip("Should be the same as the vision length of the player view cone")]
        public float damageDistance = 30;
        [Tooltip("How far away from the player the enemy should at least be when repositioning")] 
        public float minRepositionPlayerDistance = 35;
        [Tooltip("How far away from It's original location the enemy can move when it's within the users cone of view")]
        public float repositionDistance = 45;
        
        [Header("Movement Settings")]
        [Tooltip("The regular movement speed of the enemy.")]
        public float moveSpeed = 7f; 
        [Tooltip("The movement speed the enemy has while it's running away from the player")]
        public float sprintSpeed = 15f;
        
        [Header("Shooting Settings")]
        [Tooltip("The distance from the player from where the enemy starts shooting.")]
        public float shootRange = 25f;
        [Tooltip("The delay between each shot")]
        public float shootCooldown= 1.5f;
        [Tooltip("How long the enemy charges it's attack before it's shot ")]
        public float chargeDuration = 4.2f; //TODO Scale the animation duration with this.

        [Header("Projectile Settings")] 
        [Tooltip("The projectile prefab")]
        public PhantomProjectile? projectile; // Chris: Made this nullable to avoid a warning. You can delete this comment once its fixed.
        [Tooltip("The scale of the projectile after it is fully charged")]
        public Vector3 maxProjectileScale = new (0.35f, 0.35f, 0.35f);
        [Tooltip("The amount of damage the projectile does to the player whenever it hits.")]
        public int damage = 10;
        [Tooltip("The speed of the projectile.")]
        public float projectileSpeed = 15f;
        [Tooltip("How far the projectile travels before it gets deleted.")]
        public float projectileRange = 50f;
        [Tooltip("How strong the homing effect is")]
        public float homingStrength = 1.5f;
        
        [Header("Audio Settings")] 
        public AudioClip? screechSound;
        public AudioClip? projectileChargeSound;
        public AudioClip? projectileShootSound;
        public AudioClip? whooshSound;
    }
}
