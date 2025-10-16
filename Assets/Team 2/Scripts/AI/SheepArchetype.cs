using UnityEngine;
using Core.AI.Sheep.Personality;
using Core.AI.Sheep.Personality.Types;

namespace Core.AI.Sheep.Config
{
    /// <summary>
    /// Character/temperament of the sheep
    /// </summary>
    [CreateAssetMenu(fileName = "SheepArchetype", menuName = "Scriptable Objects/SheepArchetype")]
    public class SheepArchetype : ScriptableObject
    {
        [SerializeField] private float _skittishness = 0.0f;
        [SerializeField] private float _idleWanderRadius = 1.0f;
        [SerializeField] private float _followDistance = 1.8f;
        [SerializeField] private float _gettingLostChance = 0.005f;

        [Header("Animation Overrides")]
        [SerializeField] private AnimatorOverrideController _animationOverrides;
        //[SerializeField] private int _idleVariantCount = 2;

        [Header("Grazing interval in seconds")]
        [SerializeField]
        private float _grazeIntervalMin = 3.0f;
        [SerializeField]
        private float _grazeIntervalMax = 5.0f;

        [Header("Personality")]
        [SerializeField]
        private PersonalityType _personalityType = PersonalityType.Normal;

        //Getters
        public float Skittishness => _skittishness;
        public float IdleWanderRadius => _idleWanderRadius;
        public float FollowDistance => _followDistance;
        public float GettingLostChance => _gettingLostChance;

        public float GrazeIntervalMin => _grazeIntervalMin;
        public float GrazeIntervalMax => _grazeIntervalMax;

        public AnimatorOverrideController AnimationOverrides => _animationOverrides;
        
        //public int IdleVariantCount => Mathf.Max(1, _idleVariantCount);
        public PersonalityType PersonalityType => _personalityType;
        
        public ISheepPersonality CreatePersonality(SheepStateManager sheep)
        {
            return _personalityType switch
            {
                PersonalityType.Normal => new NormalPersonality(sheep),
                PersonalityType.Lazy => new LazyPersonality(sheep),
                PersonalityType.Energetic => new EnergeticPersonality(sheep),
                PersonalityType.Nervous => new NervousPersonality(sheep),
                PersonalityType.Stubborn => new StubbornPersonality(sheep),
                _ => new NormalPersonality(sheep)
            };
        }
    }

    /// <summary>
    /// Available personality types for sheep
    /// </summary>
    public enum PersonalityType
    {
        Normal,
        Lazy,
        Energetic,
        Nervous,
        Stubborn
    }
}