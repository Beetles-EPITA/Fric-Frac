using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Dictionary<KeyType, KeyCode> inputs = new Dictionary<KeyType, KeyCode>();
    

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }else if (Instance != this)
        {
            Destroy(gameObject);
        }

        foreach (KeyType type in Enum.GetValues(typeof(KeyType)))
        {
            if (!PlayerPrefs.HasKey("inputs." + type))
            {
                PlayerPrefs.SetString("inputs."+type, Enum.GetName(typeof(KeyCode), (int) type));
            }
            inputs.Add(type, (KeyCode) Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inputs."+type)));
        }
    }

    public Vector3 getMoveDirection()
    {
        Vector3 vector3 = new Vector3();
        if (GetKey(KeyType.Forward) && !GetKey(KeyType.Backward))
            vector3.z = 1;
        else if(!GetKey(KeyType.Forward) && GetKey(KeyType.Backward))
            vector3.z = -1;
        if (GetKey(KeyType.Left) && !GetKey(KeyType.Right))
            vector3.x = -1;
        else if(!GetKey(KeyType.Left) && GetKey(KeyType.Right))
            vector3.x = 1;
        return vector3;
    }


    public bool GetKey(KeyType keyType)
    {
        return Input.GetKey(inputs[keyType]);
    }

    public void SetKey(KeyType keyType, KeyCode keyCode)
    {
        inputs[keyType] = keyCode;
        PlayerPrefs.SetString("inputs."+keyType, keyCode.ToString());
    }
    
    public enum  KeyType
    {
        Forward = KeyCode.W,
        Backward = KeyCode.S,
        Left = KeyCode.A,
        Right = KeyCode.D,
        Sprint = KeyCode.LeftShift,
        Jump = KeyCode.Space,
        Interaction = KeyCode.Mouse1,
        Inventory = KeyCode.E
    }
}
