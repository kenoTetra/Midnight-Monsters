using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteZoneScript : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("DeleteZone"))
        {
            Debug.Log("Delete zone hit.");
            Destroy(gameObject);
        }
    }
}
