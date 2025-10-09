using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ViewClearing : MonoBehaviour
{
    [SerializeField]
    private Transform Player;

    //reference to the layer where all big objects that can obscure the player are
    [SerializeField]
    private LayerMask ObstacleLayer;

    private List<Transform> HandledObjects;

    void Awake()
    {
        HandledObjects = new List<Transform>();

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //resets previously affected components
        if (HandledObjects.Count > 0)
        {
            foreach (Transform t in HandledObjects)
            { t.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On; }

            HandledObjects.Clear();

        }


        //the ray that checks everything between this point and the player
        RaycastHit[] raycastHits = Physics.RaycastAll(transform.position, Player.transform.position - transform.position, Vector3.Distance(Player.transform.position, transform.position), ObstacleLayer);

        if (raycastHits.Length > 0)
        {
            foreach (RaycastHit hit in raycastHits)
            {
                if (hit.collider.gameObject.transform != Player && hit.collider.transform.root != Player)
                {
                    hit.collider.transform.root.gameObject.TryGetComponent<ViewRecieve>(out ViewRecieve observed);
                    if (observed != false)
                    {
                        hit.collider.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                        HandledObjects.Add(hit.collider.gameObject.transform);
                    }
                }
            }
        }
    }
}