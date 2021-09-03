using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointActivator : MonoBehaviour
{
	#region Variables - Inspector
	[Header( "Variables" )]
    public float FlagLerpSpeed = 5;
    public Vector3 FlagTarget;
    public bool LastCheckpoint = false;
    public bool VisualOnly = false; // For testing/mockups

    [Header( "References" )]
    public Transform Flag;
	#endregion

	#region Variables - Private
	private List<GameObject> Ghosts;

    private int RemainingGhosts = -1;
    private bool Activated = false;
	#endregion

	#region MonoBehaviour
	private void Update()
	{
		if ( RemainingGhosts == 0 && !LastCheckpoint )
		{
            Reset();
            StartCoroutine( DelayedActivate( 3 ) );
		}

        if ( Activated )
		{
            Flag.localPosition = Vector3.Lerp( Flag.localPosition, FlagTarget, Time.deltaTime * FlagLerpSpeed );
		}
	}

	public void OnTriggerEnter( Collider other )
    {
        if ( other.CompareTag( "Player" ) )
        {
            Activate();

            if ( !VisualOnly )
            {
                // Play dialogue (only the first time)
                GetComponent<DialogueScene>().Activate();

                // Store player position in case of reset to checkpoint
                FindObjectOfType<HoverVehicle>().StoreCheckpoint( this );
            }
        }
    }

    private void Reset()
    {
        // Delete old ghosts
        RemoveGhosts();

        // Reset player position
        FindObjectOfType<HoverVehicle>().ResetToCheckpoint();
    }
    #endregion

    #region Checkpoint
    void Activate()
    {
        // Stop further activations
        GetComponent<Collider>().enabled = false;
        Activated = true;

        if ( !VisualOnly )
        {
            // Play ghost paths
            Ghosts = new List<GameObject>();
            RemainingGhosts = 0;
            foreach ( var ghost in GetComponentsInChildren<GhostScene>() )
            {
                GameObject instance = ghost.Activate( this );
                Ghosts.Add( instance );
                RemainingGhosts++;
            }
        }
    }

    public void GhostFinished( GhostFollower ghost )
	{
        RemainingGhosts--;
	}

    public void ReachedNext()
	{
        GetComponent<DialogueScene>().Stop();
        RemoveGhosts();
    }

    void RemoveGhosts()
	{
        foreach ( var ghost in Ghosts )
        {
            Destroy( ghost );
        }
        Ghosts.Clear();
        RemainingGhosts = -1;
    }

    IEnumerator DelayedActivate( float seconds )
	{
        yield return new WaitForSeconds( seconds );

        Activate();
	}
    #endregion
}
