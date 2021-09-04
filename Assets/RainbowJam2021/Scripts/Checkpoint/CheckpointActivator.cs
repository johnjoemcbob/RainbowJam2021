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
    public bool NoGhosts = false; // For testing/mockups

    [Header( "References" )]
    public Transform Flag;
	#endregion

	#region Variables - Private
	private List<GameObject> Ghosts;

    private int RemainingGhosts = -1;
    private bool Activated = false;
    private bool Respawning = false;
    private UIManager UIManager;
	#endregion

	#region MonoBehaviour
    public void Start()
    {
        UIManager = GameObject.FindObjectOfType<UIManager>();
    }

	private void Update()
	{
		if ( !Respawning && !NoGhosts && RemainingGhosts == 0 && !LastCheckpoint )
		{
            ShowFailureScreen();
		}

        if ( Activated )
		{
            Flag.localPosition = Vector3.Lerp( Flag.localPosition, FlagTarget, Time.deltaTime * FlagLerpSpeed );
		}

        if ( Respawning && Input.GetButton( "Boost" ) )
        {
            Reset();
        }
	}

	public void OnTriggerEnter( Collider other )
    {
        if ( other.CompareTag( "Player" ) )
        {
            Activate();

            // Play dialogue (only the first time)
            GetComponent<DialogueScene>().Activate();

            // Store player position in case of reset to checkpoint
            FindObjectOfType<HoverVehicle>().StoreCheckpoint( this );
        }
    }

    private void ShowFailureScreen()
    {
        Respawning = true;
        UIManager.SetFailureScreenActive(true);
    }

    private void Reset()
    {
        Respawning = false;
        UIManager.SetFailureScreenActive(false);

        // Delete old ghosts
        RemoveGhosts();

        // Reset player position
        FindObjectOfType<HoverVehicle>().ResetToCheckpoint();

        // Start again, delayed
        StartCoroutine( DelayedActivate( 3 ) );
    }
    #endregion

    #region Checkpoint
    void Activate()
    {
        // Stop further activations
        GetComponent<Collider>().enabled = false;
        Activated = true;

        if ( !NoGhosts )
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
        if ( !NoGhosts )
        {
            foreach ( var ghost in Ghosts )
            {
                Destroy( ghost );
            }
            Ghosts.Clear();
            RemainingGhosts = -1;
        }
    }

    IEnumerator DelayedActivate( float seconds )
	{
        yield return new WaitForSeconds( seconds );

        Activate();
	}
    #endregion
}
