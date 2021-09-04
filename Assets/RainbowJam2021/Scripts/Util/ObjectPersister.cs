using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPersister : MonoBehaviour
{
    void Start()
    {
        Object.DontDestroyOnLoad(this.gameObject);      
    }

}
