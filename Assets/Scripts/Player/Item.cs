
using System;
using UnityEngine;

[Serializable]
public class Item : MonoBehaviour
{

    [SerializeField] public string itemName;
    [SerializeField] public GameObject objectPrefab;
    [SerializeField] public Sprite icon;
    
    
    public static bool operator ==(Item item, Item other)
    {
        if (item is null || other is null)
            return item is null && other is null;
        return item.itemName == other.itemName;
    }
    
    public static bool operator !=(Item item, Item other)
    {
        return !(item == other);
    }

}
