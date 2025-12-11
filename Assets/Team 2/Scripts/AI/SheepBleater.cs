using UnityEngine;

namespace Core.AI.Sheep
{
    public class SheepBleater : MonoBehaviour
    {
        /*
        How to use this script
        1. Put this script on an individual sheep object
        2. Add an AudioSource component to the same sheep and connect it to the script
        3. Put all the heavy bleats in Heavy Clips, medium bleats in Medium Clips, and light bleats in Light Clips
        4. Select the category of sheep sounds you want in Category
        5. Adjust Min Time and Max Time if you want different bleat intervals
         */

        public enum Category
        {
            Heavy,
            Medium,
            Light
        }

        public Category sheepCategory;

        public AudioSource source;

        public AudioClip[] heavyClips;
        public AudioClip[] mediumClips;
        public AudioClip[] lightClips;

        [Header("Timer Settings")]
        public float minTime = 4f; // Don't know how much minium or maxium is a good range, but we can experiment with this later
        public float maxTime = 12f;

        private float timer; // internal timer so that the sheep bleat at random times

        void Start()
        {
            PickNewTime();
        }

        void Update()
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
            }

            if (timer <= 0f)
            {
                PlayRandomBleat();
                PickNewTime();
            }
        }

        void PickNewTime()
        {
            // adding 2 seconds on top of the random range, because it takes 1 to 2 seconds to play the bleats themselfs
            timer = Random.Range(minTime, maxTime) + 2f;
        }

        void PlayRandomBleat()
        {
            if (source == null) return;

            AudioClip clipToPlay = null;

            switch (sheepCategory)
            {
                case Category.Heavy:
                    if (heavyClips.Length > 0)
                        clipToPlay = heavyClips[Random.Range(0, heavyClips.Length)];
                    break;

                case Category.Medium:
                    if (mediumClips.Length > 0)
                        clipToPlay = mediumClips[Random.Range(0, mediumClips.Length)];
                    break;

                case Category.Light:
                    if (lightClips.Length > 0)
                        clipToPlay = lightClips[Random.Range(0, lightClips.Length)];
                    break;
            }
            // vibe coded this a bit, but seems to work okay :), 2nd unity thing I made! - Tigo
            if (clipToPlay != null)
            {
                source.PlayOneShot(clipToPlay);
            }
        }
    }
}
