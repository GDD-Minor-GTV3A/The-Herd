using UnityEngine;

public class OpenShop : MonoBehaviour
{
    [SerializeField] private GameObject _shopCanvas;
    [SerializeField] private GameObject _player;



    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) < 10 && Input.GetKeyUp(KeyCode.Q))
        {
            _shopCanvas.SetActive(!_shopCanvas.activeInHierarchy);
        }
    }
}