using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDistance : MonoBehaviour
{
	#region Variables - Inspector
	[Header( "Variables" )]
    public float DistMin = -5;
    public float DistMax = -20;
    public float SpeedMin = 0;
    public float SpeedMax = 100;
    public float YDistMin = 0;
    public float YDistMax = 1;
    public float YSpeedMin = 0;
    public float YSpeedMax = 100;
    public float FOVMin = 70;
    public float FOVMax = 90;
    public float FOVSpeedMin = 0;
    public float FOVSpeedMax = 100;
    public float FOVLerp = 5;
    public float YDampner = 1;

    [Header( "References" )]
    public HoverVehicle Vehicle;
    public Transform CameraLookAt;
    #endregion

    #region Variables - Public
    [HideInInspector]
    public Vector3 Offset;
	#endregion

	#region Variables - Private
	private Vector3 InitialPos;
	private Vector3 InitialLookAtPos;
	#endregion

	#region MonoBehaviour
	void Start()
    {
        InitialPos = transform.localPosition;
        InitialLookAtPos = CameraLookAt.localPosition;
    }

    void FixedUpdate()
    {
        // Lerp offset back to zero (anti wall poke through)
        Offset = Vector3.Lerp( Offset, Vector3.zero, Time.deltaTime * 5 );

        // Look up/down when travelling in that direction for a better view
        float y = InitialPos.y - Vehicle.GetVelocity().y * YDampner;

        // LookAt Y also affected by speed
        float lookAtY = InitialLookAtPos.y + Vehicle.GetSpeed().RemapClamped( YSpeedMin, YSpeedMax, YDistMin, YDistMax );
        CameraLookAt.localPosition = new Vector3(InitialLookAtPos.x, lookAtY, InitialLookAtPos.z);

        // Lerp camera distance by speed
        float z = Vehicle.GetSpeed().RemapClamped( SpeedMin, SpeedMax, DistMin, DistMax );
        transform.localPosition = new Vector3( InitialPos.x, y, z ) + Offset;

        // Lerp camera fov by speed
        float fov = Vehicle.GetSpeed().RemapClamped( FOVSpeedMin, FOVSpeedMax, FOVMin, FOVMax );
        Camera.main.fieldOfView = Mathf.Lerp( Camera.main.fieldOfView, fov, Time.deltaTime * FOVLerp );
    }
	#endregion
}
