using UnityEngine;
using System;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    /* This script will contain the main logic that's needed for all events occurring during the level playtime. */
    [SerializeField] private GameObject _playerPrefab, _sheepPrefab;
    [SerializeField] private Transform _playerParent, _sheepParent;
    [SerializeField] private int _sheepCount;

    void Initialize()
    {
        // Method for initializing the level.

        // Places the playerprefab at position 0, 0, 0.
        Instantiate(_playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, _playerParent);

        // Places all the sheep at position 0, 0, 0.
        for (int i = 0; i < _sheepCount; i++)
        {
            Instantiate(_sheepPrefab, new Vector3(0, 0, 0), Quaternion.identity, _sheepParent);
        }
    }
}
