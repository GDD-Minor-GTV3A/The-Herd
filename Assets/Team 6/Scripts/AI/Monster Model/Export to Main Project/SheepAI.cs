using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class sheepAI : MonoBehaviour
{
    public Transform Player;
    int MoveSpeed = 10;
    int MaxDist = 101;
    int MinDist = 5;

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