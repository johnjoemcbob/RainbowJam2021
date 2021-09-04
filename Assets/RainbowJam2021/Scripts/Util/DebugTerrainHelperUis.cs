using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class DebugTerrainHelperUis : MonoBehaviour
{
    public GameObject UIPrefab;

    void Start()
    {
		foreach ( Transform child in transform )
        {
            if ( child.childCount == 0 )
            {
                GameObject ui = Instantiate( UIPrefab, child );
                ui.transform.localPosition = Vector3.zero;
                ui.GetComponentInChildren<Text>().text = child.GetSiblingIndex().ToString();
            }
        }
    }
}
