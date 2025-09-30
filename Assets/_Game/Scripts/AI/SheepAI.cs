using UnityEngine;

namespace AI
{
    public class sheepAI : MonoBehaviour
    {
        public Transform Player;
        public int MoveSpeed = 10;
        public int MaxDist = 101;
        public int MinDist = 5;

        void FixedUpdate()
        {
            transform.LookAt(Player);

            if (Vector3.Distance(transform.position, Player.position) <= MaxDist)
            {
                if (Vector3.Distance(transform.position, Player.position) <= MinDist)
                {
                }
                else
                {
                    transform.position += transform.forward * MoveSpeed * Time.deltaTime;
                }

            }

        }
    }
}
