using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
	#region Variables - Inspector
	public float Forward = 2;
	public float Angle = 30;
	public float ScaleMult = 2.5f;
	public float FOVDiv = 60;
	#endregion

	#region Variables - Private
	private Camera Camera;
	private RectTransform WorldCanvas;

	private Vector3 InitialScale;
	#endregion

	#region MonoBehaviour
	private void Start()
	{
		Camera = Camera.main;
		WorldCanvas = GetComponent<RectTransform>();

		InitialScale = transform.localScale;
	}

	void Update()
    {
		// Always resize to resolution of window
		WorldCanvas.sizeDelta = new Vector2( Screen.width, Screen.height );
		InitialScale = Vector3.one / Screen.width * ScaleMult;
		
		// Follow cam
		Vector3 pos = Camera.transform.position + Camera.transform.forward * Forward;
		transform.position = pos;
		transform.localScale = InitialScale * Camera.fieldOfView / FOVDiv;

		// Angle cam
		Quaternion ang = Camera.transform.rotation;
		//ang.
		transform.rotation = ang;
		transform.eulerAngles += new Vector3( 0, Angle, 0 );
	}
	#endregion
}
