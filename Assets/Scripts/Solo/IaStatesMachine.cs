using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class IaStatesMachine : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool Distance(Component ia, Component player, float distanceMin)
    {
        Vector3 a = ia.transform.position;

        Vector3 b = player.transform.position;
        return Vector3.Distance(a, b) < distanceMin;
    }

    public static bool CanSeeThePlayer(NavMeshAgent ia,Camera iaCamera , Component player, float distanceMin)
    {
        Renderer render = player.GetComponentInChildren<SkinnedMeshRenderer>();
        if (render.isVisible) Debug.Log("Visible");
        else Debug.Log("Not visible");
        Vector3 iaView = ia.GetComponent<Transform>().forward;
        return false;
    }

    public static bool IsObjectBetween(Component ia, Component player)
    {
        Ray ray = new Ray(ia.transform.position, player.transform.localPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return true;
        }

        return false;
    }
}
