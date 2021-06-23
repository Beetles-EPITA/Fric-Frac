
using System;
using UnityEngine;

public class PlayerJumpAction : MonoBehaviour
{

    public bool isOnGround;
    public bool inHouse;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != GetComponentInParent<BoxCollider>().gameObject)
        {
            isOnGround = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != GetComponentInParent<BoxCollider>().gameObject)
        {
            isOnGround = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != GetComponentInParent<BoxCollider>().gameObject)
        {
            isOnGround = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != GetComponentInParent<BoxCollider>().gameObject)
        {
            isOnGround = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject != GetComponentInParent<BoxCollider>().gameObject)
        {
            isOnGround = false;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject != GetComponentInParent<BoxCollider>().gameObject)
        {
            isOnGround = true;
        }
        
    }
}