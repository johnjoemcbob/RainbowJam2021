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

	#region Variables - Private
	private GhostRecorder Recorder;

    private int Frame = 0;
	private List<GhostRecorder.KeyFrame> KeyFrames = new List<GhostRecorder.KeyFrame>();
	#endregion

	#region MonoBehaviour
	private void Start()
	{
        Recorder = FindObjectOfType<GhostRecorder>();

		if ( JSON != "" )
		{
			var array = JsonUtility.FromJson<GhostRecorder.KeyFramesArray>( JSON );
			KeyFrames = new List<GhostRecorder.KeyFrame>( array.array );
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
		if ( KeyFrames.Count > DelayedStart && KeyFrames.Count > Frame )
		{
			GhostRecorder.KeyFrame frame = KeyFrames[Frame];
			transform.position = Vector3.MoveTowards( transform.position, frame.Pos, Time.deltaTime * frame.Vel.magnitude * MoveSpeed );
			transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.Euler( frame.Ang ), Time.deltaTime * TurnSpeed );

			float dist = ( transform.position - frame.Pos ).sqrMagnitude;
			if ( dist <= MinSqrDistance )
			{
				Frame++;
			}
		}
    }
	#endregion
}
