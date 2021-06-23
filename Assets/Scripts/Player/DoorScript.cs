using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    // Animation:
    private Animator anim;
    private int nb;

    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        nb = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        nb++;
        print(nb);
        anim.SetInteger("isOpen", nb);
    }

    private void OnTriggerExit(Collider other)
    {
        nb--;
        print(nb);
        anim.SetInteger("isOpen", nb);
    }
}
