using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempCarMeshPicker : MonoBehaviour
{
    public Text UI;

    private List<GameObject> Meshes = new List<GameObject>();

    private int Current = 0;

    void Start()
    {
        // Store all
		foreach ( Transform child in transform )
        {
            Meshes.Add( child.gameObject );
            child.gameObject.SetActive( false );
        }

        // Choose random
        Current = Random.Range( 0, Meshes.Count );
        Meshes[Current].SetActive( true );
        UI.text = "Current vehicle design; " + Meshes[Current].name;
    }

    void Update()
    {
        if ( Input.GetKeyDown( KeyCode.Alpha5 ) )
        {
            // Loop through
            Meshes[Current].SetActive( false );
            Current++;
            if ( Current >= Meshes.Count )
            {
                Current = 0;
            }
            Meshes[Current].SetActive( true );

            // Update UI
            UI.text = "Current vehicle design; " + Meshes[Current].name;
        }
    }
}
