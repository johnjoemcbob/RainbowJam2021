using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCheckpointsEnabler : MonoBehaviour
{
    public GameObject Buttons;

    void Update()
    {
        if ( Input.GetKeyDown( KeyCode.F11 ) )
		{
            Buttons.SetActive( !Buttons.activeSelf );
        }
    }
}
