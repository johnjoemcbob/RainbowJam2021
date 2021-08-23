using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDistance : MonoBehaviour
{
	#region Variables - Inspector
	[Header( "Variables" )]
    public float Min = 5;
    public float Max = 20;
    public float SpeedMin = 0;
    public float SpeedMax = 100;
    public float YDampner = 1;

    [Header( "References" )]
    public HoverVehicle Vehicle;
	#endregion

	#region Variables - Private
	private Vector3 InitialPos;
	#endregion

	#region MonoBehaviour
	void Start()
    {
        InitialPos = transform.localPosition;
    }

    void FixedUpdate()
    {
        Debug.Log( Vehicle.GetVelocity().y );
        float y = InitialPos.y - Vehicle.GetVelocity().y * YDampner;
        float z = Vehicle.GetSpeed().RemapClamped( SpeedMin, SpeedMax, Min, Max );
        transform.localPosition = new Vector3( InitialPos.x, y, z );
    }
	#endregion
}
