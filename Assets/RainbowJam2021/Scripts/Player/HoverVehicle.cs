using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverVehicle : MonoBehaviour
{
    #region Variables - Inspector
    [Header( "Variables" )]
    public float ThrustForce = 400;
    public float TurnForce = 300;
    public float EngineForce = 250;

    [Header( "References" )]
    public List<GameObject> Engines;
    public GameObject Propulsion;
    public GameObject CenterMass;
	#endregion

	#region Variables - Private
	private Rigidbody rb;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        rb.centerOfMass = CenterMass.transform.localPosition;
    }

    void FixedUpdate()
    {
        rb.AddForceAtPosition( Time.deltaTime * transform.TransformDirection( Vector3.forward ) * Input.GetAxis( "Vertical" ) * ThrustForce, Propulsion.transform.position );
        rb.AddTorque( Time.deltaTime * transform.TransformDirection( Vector3.up ) * Input.GetAxis( "Horizontal" ) * TurnForce );
        foreach ( GameObject engine in Engines )
		{
            RaycastHit hit;
            if ( Physics.Raycast( engine.transform.position, transform.TransformDirection( Vector3.down ), out hit, 3f ) )
			{
                rb.AddForceAtPosition( Time.deltaTime * transform.TransformDirection( Vector3.up ) * Mathf.Pow( 3f - hit.distance, 2 ) / 2f * EngineForce, engine.transform.position );
			}
		}
        rb.AddForce( -Time.deltaTime * transform.TransformVector( Vector3.right ) * transform.InverseTransformVector( rb.velocity ).x * 5f );
    }
	#endregion

    public float GetSpeed()
	{
        return rb.velocity.magnitude;
	}

    public Vector3 GetVelocity()
	{
        return rb.velocity;
	}
}
