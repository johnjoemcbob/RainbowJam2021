using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverVehicle : MonoBehaviour
{
    #region Variables - Inspector
    [Header( "Variables" )]
    public float ThrustForce = 400;
    public float TurboForce = 800;
    public float TurnForce = 300;
    public float EngineForce = 250;
    public float DriftBuildMultiplier = 1;
    public bool NoVerticalInput = false;

    [Header( "References" )]
    public List<GameObject> Engines;
    public GameObject Propulsion;
    public GameObject BoostPropulsion;
    public GameObject CenterMass;
    #endregion

    #region Variables - Public
    [HideInInspector]
    public float DriftTurbo = 0;
    #endregion

    #region Variables - Private
    private Rigidbody rb;

    private bool InputTryBoost = false;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        rb.centerOfMass = CenterMass.transform.localPosition;

        ToggleEngineVisualise();
        ToggleStabiliserVisualise();
    }

	private void Update()
	{
        // Debug reset
		if ( Input.GetKeyDown( KeyCode.R ) )
		{
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
		}

        // Debug visualise engines
        if ( Input.GetKeyDown( KeyCode.F1 ) )
		{
            ToggleEngineVisualise();
		}

        // Debug visualise stabilisers
        if ( Input.GetKeyDown( KeyCode.F2 ) )
        {
            ToggleStabiliserVisualise();
        }

        // Debug stabilisers
        if ( Input.GetKeyDown( KeyCode.F3 ) )
        {
            ToggleStabilisers();
        }

        // Input
        InputTryBoost = Input.GetKey( KeyCode.Space ) || Input.GetButton( "Jump" );
    }

	void FixedUpdate()
    {
        // Directional Input
        Vector3 forward = transform.TransformDirection( Vector3.forward );
            if ( NoVerticalInput )
		    {
                forward.y = 0;
		    }
        rb.AddForceAtPosition( Time.deltaTime * forward * Input.GetAxis( "Vertical" ) * ThrustForce, Propulsion.transform.position );
        rb.AddTorque( Time.deltaTime * transform.TransformDirection( Vector3.up ) * Input.GetAxis( "Horizontal" ) * TurnForce );
        foreach ( GameObject engine in Engines )
		{
            RaycastHit hit;
            if ( Physics.Raycast( engine.transform.position, transform.TransformDirection( Vector3.down ), out hit, 3f ) )
			{
                rb.AddForceAtPosition( Time.deltaTime * transform.TransformDirection( Vector3.up ) * Mathf.Pow( 3f - hit.distance, 2 ) / 2f * EngineForce, engine.transform.position );

                engine.transform.localScale = Vector3.one;
            }

            engine.transform.localScale = Vector3.Lerp( engine.transform.localScale, Vector3.one * 0.1f, Time.deltaTime * 5 );
		}

        // ???
        rb.AddForce( -Time.deltaTime * transform.TransformVector( Vector3.right ) * transform.InverseTransformVector( rb.velocity ).x * 5f );

        // Boost Input
        if ( DriftTurbo > 0 )
        {
            if ( InputTryBoost )
            {
                Vector3 vel = rb.velocity;
                vel.y = 0;
                rb.AddForceAtPosition( -vel, Propulsion.transform.position );
                rb.AddForceAtPosition( Time.deltaTime * forward * TurboForce, BoostPropulsion.transform.position );
                float TurboArrestAngularMultiplier = 1;
                rb.angularVelocity = Vector3.Lerp( rb.angularVelocity, Vector3.zero, Time.deltaTime * TurboArrestAngularMultiplier );
                DriftTurbo -= 0.1f;
            }
        }
        else
		{
            DriftTurbo = 0;
		}

        // Drifting
        // Turning and slowing down a lot
        // Builds up DriftTurbo
        if ( rb.velocity.magnitude > 15 && rb.angularVelocity.magnitude > 1 )
        {
            DriftTurbo += Time.deltaTime * DriftBuildMultiplier;
            DriftTurbo = Mathf.Clamp( DriftTurbo, 0, 1 );
        }
    }

	private void OnCollisionEnter( Collision collision )
	{
        // Screenshake
        FindObjectOfType<CameraFollow>().Shake( collision.relativeVelocity.magnitude );
    }
	#endregion

	#region Getters
	public float GetSpeed()
	{
        return rb.velocity.magnitude;
	}

    public Vector3 GetVelocity()
	{
        return rb.velocity;
	}
    #endregion

    #region Debug
    void ToggleEngineVisualise()
	{
		foreach ( GameObject engine in Engines )
		{
            engine.GetComponent<MeshRenderer>().enabled = !engine.GetComponent<MeshRenderer>().enabled;
        }
	}

    void ToggleStabiliserVisualise()
    {
        foreach ( var stabiliser in FindObjectsOfType<VehicleUpright>( true ) )
        {
            stabiliser.GetComponent<MeshRenderer>().enabled = !stabiliser.GetComponent<MeshRenderer>().enabled;
        }
    }

    void ToggleStabilisers()
    {
        foreach ( var stabiliser in FindObjectsOfType<VehicleUpright>( true ) )
        {
            stabiliser.enabled = !stabiliser.enabled;
            stabiliser.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    #endregion
}
