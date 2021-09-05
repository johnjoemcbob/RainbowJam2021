using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCheckpointTeleport : MonoBehaviour
{
	public GameObject ButtonPrefab;

	private void Start()
	{
		// Find all checkpoints
		int ind = 0;
		foreach ( Transform child in GameObject.Find( "Main Checkpoints" ).transform )
		{
			// Instantiate a button for each
			GameObject button = Instantiate( ButtonPrefab, transform );
			int checkpoint = ind;
			button.GetComponent<Button>().onClick.AddListener( delegate { ButtonPress( checkpoint ); } );
			button.GetComponentInChildren<Text>().text = ( ind + 1 ).ToString();
			ind++;
		}
	}

	public void ButtonPress( int checkpoint )
	{
		// Find the checkpoint in the scene
		FindObjectOfType<HoverVehicle>().StoreCheckpoint( GameObject.Find( "Main Checkpoints" ).transform.GetChild( checkpoint ).GetComponent<CheckpointActivator>(), false );
		FindObjectOfType<HoverVehicle>().ResetToCheckpoint();

	}
}
