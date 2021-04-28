using System;
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
        return IsTargetVisible(iaCamera, player.gameObject) && Distance(ia, player, distanceMin);
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

    static bool IsTargetVisible(Camera c,GameObject go)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var point = go.transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }
        return true;
    }
}
