using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] float PushStrength = 80f;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("In");
        if (other.gameObject.GetComponentInParent<PlayerController>() != null)
        {
            Debug.Log("Fanned");
            other.gameObject.GetComponentInParent<PlayerController>().ExternalForceAdd(transform.up * PushStrength);
        }
    }
}
