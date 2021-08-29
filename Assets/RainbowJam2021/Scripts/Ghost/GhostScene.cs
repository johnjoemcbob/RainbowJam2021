using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScene : MonoBehaviour
{
	public GameObject GhostPrefab;
	public string JSON;

	public GameObject Activate( CheckpointActivator checkpoint )
	{
		// Spawn the ghost and start its recorded path
		GameObject ghost = Instantiate( GhostPrefab );
		{
			ghost.transform.position = Vector3.zero;
			ghost.transform.eulerAngles = Vector3.zero;
			ghost.GetComponentInChildren<GhostFollower>().JSON = JSON;
			ghost.GetComponentInChildren<GhostFollower>().Checkpoint = checkpoint;
		}
		return ghost;
	}
}
