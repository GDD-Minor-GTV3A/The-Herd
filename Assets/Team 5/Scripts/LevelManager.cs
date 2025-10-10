using System;
using System.Collections;

using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab, _sheepPrefab;
    [SerializeField] private Transform _playerParent, _sheepParent;
    [SerializeField] private int _sheepCount; // Might become a field in the Player class, in which case it wouldn't be needed here.


    /// <summary>
    /// Initialization method for the dynamic parts of the level.
    /// </summary>
    void Start()
    {
        /* Places the playerprefab at position 0, 0, 0. Position will require changes later.
        *  Also, it currently uses the playerprefab, but the player will probably have a bunch of data attached to it, so it would need to have that data as well.
        */
        Instantiate(_playerPrefab, new Vector3(21.6790237f, 0.261999995f, 20.3814964f), Quaternion.identity, _playerParent);

        // Places all the sheep at position 0, 0, 0. Positions will require changes later.
        for (int i = 0; i < _sheepCount; i++)
        {
            Instantiate(_sheepPrefab, new Vector3(21.6790237f, 0.261999995f, 20.3814964f), Quaternion.identity, _sheepParent);
        }
    }
}