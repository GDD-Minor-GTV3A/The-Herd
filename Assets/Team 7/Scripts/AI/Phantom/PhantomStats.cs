using UnityEngine;

namespace Team_7.Scripts.AI.Phantom
{
    [CreateAssetMenu(fileName = "PhantomStats", menuName = "Scriptable Objects/PhantomStats")]
    public class PhantomStats : ScriptableObject
    {
        [Header("Health/Respawn Settings")] 
        [Tooltip("The amount of times the enemy can respawn")]
        public float health = 3;
        [Tooltip("How long the player should continuously look at the enemy before it loses 1 health.")]
        public float lookDuration = 2;
        [Tooltip("The minimum distance from it's previous location the enemy will respawn at.")]
        public float minRespawnDistance = 50;
        [Tooltip("The maximum distance the enemy will respawn at.")]
        public float maxRespawnDistance = 100;
        [Tooltip("Should be the same as the angle of the player's vision cone, the enemy takes damage if it's too long inside this area.")]
        public float DamageAngle = 50;
        [Tooltip("Should be the same as the vision length of the player view cone")]
        public float damageDistance = 30;
        
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
        public PhantomProjectile projectile;
        [Tooltip("The scale of the projectile after it is fully charged")]
        public Vector3 maxProjectileScale = new (0.35f, 0.35f, 0.35f);
        [Tooltip("The amount of damage the projectile does to the player whenever it hits.")]
        public float damage = 10;
        [Tooltip("The speed of the projectile.")]
        public float projectileSpeed = 15f;
        [Tooltip("How far the projectile travels before it gets deleted.")]
        public float projectileRange = 50f;
    }
}
