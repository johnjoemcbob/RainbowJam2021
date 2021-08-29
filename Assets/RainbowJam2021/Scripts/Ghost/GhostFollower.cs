using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFollower : MonoBehaviour
{
	#region Variables - Inspector
	[Header( "Variables" )]
	public int DelayedStart = 5;
	public string JSON = "";

	public float MinSqrDistance = 0.1f;
	public float MoveSpeed = 1;
	public float TurnSpeed = 1;
	#endregion

	#region Variables - Public
	[HideInInspector]
	public CheckpointActivator Checkpoint;
	#endregion

	#region Variables - Private
	private GhostRecorder Recorder;
	private FMODUnity.StudioEventEmitter EngineThrum;

    private int Frame = 0;
	private List<GhostRecorder.KeyFrame> KeyFrames = new List<GhostRecorder.KeyFrame>();
	#endregion

	#region MonoBehaviour
	private void Start()
	{
        Recorder = FindObjectOfType<GhostRecorder>();
		EngineThrum = FindObjectOfType<FMODUnity.StudioEventEmitter>();

		if ( JSON != "" )
		{
			var array = JsonUtility.FromJson<GhostRecorder.KeyFramesArray>( JSON );
			KeyFrames = new List<GhostRecorder.KeyFrame>( array.array );

			// Initial position
			transform.position = KeyFrames[0].Pos;
			transform.eulerAngles = KeyFrames[0].Ang;
		}
    }

	void Update()
	{
		// If no JSON, then follow the current player
		if ( JSON == "" )
		{
			KeyFrames = Recorder.KeyFrames;
		}

		// Follow
		if ( KeyFrames.Count > DelayedStart )
		{
			if ( KeyFrames.Count > Frame )
			{
				GhostRecorder.KeyFrame frame = KeyFrames[Frame];
				transform.position = Vector3.MoveTowards( transform.position, frame.Pos, Time.deltaTime * frame.Vel.magnitude * MoveSpeed );
				transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.Euler( frame.Ang ), Time.deltaTime * TurnSpeed );

				EngineThrum.SetParameter( "IndividualVehicleSpeed", 100 );

				float dist = ( transform.position - frame.Pos ).sqrMagnitude;
				if ( dist <= MinSqrDistance )
				{
					Frame++;
				}
			}
			else if ( Checkpoint != null )
			{
				Checkpoint.GhostFinished( this );
				Checkpoint = null;

				EngineThrum.SetParameter( "IndividualVehicleSpeed", 0 );
			}
		}
    }
	#endregion
}
