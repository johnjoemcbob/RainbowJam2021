using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDownloader : MonoBehaviour
{
    [Header( "References" )]
    public Transform GhostParent;
    public GameObject GhostPrefab;

    void Start()
    {
        NetworkRequest.Instance.GetGhostPaths( new GetGhostPathsHandler() { } );
    }

    public void Reset()
	{
        // Clear old ghosts
		foreach ( Transform child in GhostParent )
		{
            Destroy( child.gameObject );
		}

        // Download again
        Start();
	}

	public void InstantiateGhosts( string[] ghostpaths )
	{
		foreach ( var ghostpath in ghostpaths )
        {
            var ghost = Instantiate( GhostPrefab, GhostParent );
            ghost.GetComponentInChildren<GhostFollower>().JSON = ghostpath;
        }
    }
}
