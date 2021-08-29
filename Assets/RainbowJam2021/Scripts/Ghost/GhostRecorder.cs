using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
	#region Defines
	[Serializable]
	public struct KeyFramesArray
	{
		public KeyFrame[] array;
	}

	[Serializable]
	public struct KeyFrame
	{
        public Vector3 Pos;
        public Vector3 Ang;
        public Vector3 Vel;
	}
	#endregion

	#region Variables - Inspector
	[Header( "Variables" )]
	public float MinSqrDistance = 1;
	#endregion

	#region Variables - Public
	public List<KeyFrame> KeyFrames = new List<KeyFrame>();
	#endregion

	#region Variables - Private
	private Rigidbody rb;

	private Vector3 LastFramePos = Vector3.zero;
	#endregion

	#region MonoBehaviour
	private void Start()
	{
		rb = transform.GetComponent<Rigidbody>();
	}

	void Update()
    {
		float dist = ( LastFramePos - transform.position ).sqrMagnitude;
		if ( dist >= MinSqrDistance )
		{
			KeyFrame frame = new KeyFrame();
			{
				frame.Pos = transform.position;
				frame.Ang = transform.eulerAngles;
				frame.Vel = rb.velocity;
			}
			KeyFrames.Add( frame );

			LastFramePos = transform.position;
		}
    }
	#endregion

	#region Exporting
	public void Export()
	{
		if ( KeyFrames.Count < 5 ) return;

		List<string> jsons = new List<string>();
		foreach ( var frame in KeyFrames )
		{
			jsons.Add( JsonUtility.ToJson( frame ) );
		}
		string output = "";
		{
			output += "{\n";
			output += "\"array\": [\n";
			// First item outside of loop, to help with commas
			output += jsons[0];
			for ( int i = 1; i < jsons.Count; i++ )
			{
				output += ",\n";
				output += jsons[i];
			}
			output += "\n]\n";
			output += "}";
		}
		Save( output );
		//Upload( output );

		// Reset
		KeyFrames.Clear();
	}

	void Save( string json )
	{
		string path = Application.persistentDataPath;
		string filename = "test.txt";
		var sr = File.CreateText( Path.Combine( path, filename ) );
		{
			sr.Write( json );
		}
		sr.Close();
	}

	void Upload( string json )
	{
		NetworkRequest.Instance.PostGhostPath( json, new PostGhostPathHandler() { } );

		FindObjectOfType<GhostDownloader>().Reset();
	}
	#endregion
}
