
using System;
using UnityEngine;

[Serializable]
public class Item : MonoBehaviour
{

    [SerializeField] public string itemName;
    [SerializeField] public GameObject objectPrefab;
    [SerializeField] public Sprite icon;
}
