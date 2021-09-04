using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HillGizmo : MonoBehaviour
{

    #if UNITY_EDITOR

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 10);
        Gizmos.DrawIcon(transform.position, "hillhere.png", true);
    }

    #endif
}
