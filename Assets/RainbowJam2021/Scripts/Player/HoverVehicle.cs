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
    public float TurboForce = 800;
    public float TurboConsumeRate = 0.05f;
    public float DriftArcForce = 400;
    public float DriftArcForwardForce = 400;
    public float DriftSlowMultiplier = 1;
    public float DriftTurnMultiplier = 1;
    public float DriftBuildVelocity = 15;
    public float DriftBuildAngular = 1;
    public float DriftBuildMultiplierSpeed = 1;
    public float DriftBuildMultiplierMax = 1;
    public float DriftVisualTiltAngle = 30;
    public float AngleLerpSpeed = 5;
    public bool ConstantTurboPunch = true;
    public Vector3 DriftPunch = Vector3.one;
    public Vector3 TurboStartPunch = Vector3.one;
    public Vector3 TurboFinishPunch = Vector3.one;
    public bool NoVerticalInput = false;

    [Header( "References" )]
    public List<GameObject> Engines;
    public GameObject Propulsion;
    public GameObject BoostPropulsion;
    public GameObject CenterMass;

    public FMODUnity.StudioEventEmitter TurboSoundEmitter;
    #endregion

    #region Variables - Public
    [HideInInspector]
    public float DriftTurbo = 0;
    #endregion

    #region Variables - Private
    private Rigidbody rb;
    private Punchable PunchMesh;

    private bool Drifting = false;
    private float DriftBuildMultiplier = 0;
    private bool InputTryDrift = false;
    private bool InputTryBoost = false;
    private bool Boosting = false;
    private bool DebugInfiniteTurbo = false;
    private Vector3 TargetDriftAngle = Vector3.zero;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        rb.centerOfMass = CenterMass.transform.localPosition;

        PunchMesh = GetComponentInChildren<Punchable>();

        ToggleEngineVisualise();
        ToggleStabiliserVisualise();
    }

	private void Update()
	{
        // Debug reset
		if ( Input.GetKeyDown( KeyCode.R ) )
		{
            Respawn();
		}

        // Debug visualise engines
        if ( Input.GetKeyDown( KeyCode.Alpha1 ) )
		{
            ToggleEngineVisualise();
		}

        // Debug visualise stabilisers
        if ( Input.GetKeyDown( KeyCode.Alpha2 ) )
        {
            ToggleStabiliserVisualise();
        }

        // Debug stabilisers
        if ( Input.GetKeyDown( KeyCode.Alpha3 ) )
        {
            ToggleStabilisers();
        }

        // Debug ghosts
        if ( Input.GetKeyDown( KeyCode.Alpha4 ) )
		{
            FindObjectOfType<GhostRecorder>().Export();
		}

        // Debug infinite turbo
        if ( Input.GetKeyDown( KeyCode.Alpha5 ) )
        {
            DebugInfiniteTurbo = !DebugInfiniteTurbo;
        }

        // Debug respawn
        if ( transform.localPosition.y < -60 )
		{
            Respawn();
		}

        // Input
        InputTryDrift = Input.GetKey( KeyCode.LeftShift ) || Input.GetButton( "Fire1" );
        InputTryBoost = Input.GetKey( KeyCode.Space ) || Input.GetButton( "Jump" );
    }

	void FixedUpdate()
    {
        UpdateVehiclePhysics();

        // Debug turbo
        if ( DebugInfiniteTurbo )
		{
            DriftTurbo = 1;
		}

        // Boost Input
        UpdateBoosting();

        // Drifting
        UpdateDrifting();
    }

	private void OnCollisionEnter( Collision collision )
	{
        // Screenshake
        //FindObjectOfType<CameraFollow>().Shake( collision.relativeVelocity.magnitude );
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

	#region HoverVehicle
    void UpdateVehiclePhysics()
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
    }

	void Respawn()
	{
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    #endregion

    #region Drifting
    void UpdateDrifting()
    {
        Vector3 turn = rb.angularVelocity;
        turn.z = 0;
        if ( InputTryDrift && rb.velocity.magnitude > DriftBuildVelocity && turn.magnitude > DriftBuildAngular && !Boosting )
        {
            ToggleDrifting( true );

            RunDrifting();
        }
        else
        {
            ToggleDrifting( false );
        }

        // Visually tilt the vehicle mesh
        PunchMesh.transform.localRotation = Quaternion.Lerp( PunchMesh.transform.localRotation, Quaternion.Euler( TargetDriftAngle ), Time.deltaTime * AngleLerpSpeed );
    }

    void ToggleDrifting( bool toggle )
	{
        if ( Drifting != toggle )
		{
            Drifting = toggle;
            if ( Drifting )
			{
                StartDrifting();
            }
			else
			{
                FinishDrifting();
            }
		}
	}

    void StartDrifting()
	{
        
	}

    void RunDrifting()
	{
        // Build turbo by drifting, which clamps at 1
        DriftTurbo += Time.deltaTime * DriftBuildMultiplier;
        DriftTurbo = Mathf.Clamp( DriftTurbo, 0, 1 );

        // Build turbo faster for longer drifts
        DriftBuildMultiplier += Time.deltaTime * DriftBuildMultiplierSpeed;
        DriftBuildMultiplier = Mathf.Clamp( DriftBuildMultiplier, 0, DriftBuildMultiplierMax );

        // Slow forward velocity into the drift
        Vector3 vel = rb.velocity;
        vel.y = 0;
        rb.AddForceAtPosition( -vel * vel.magnitude * DriftSlowMultiplier, Propulsion.transform.position );

        // Apply arc force in new direction
        Vector3 forward = transform.TransformDirection( Vector3.forward );
        Vector3 right = Vector3.Cross( forward.normalized, Vector3.up.normalized );
        if ( NoVerticalInput )
        {
            right.y = 0;
        }
        if ( Input.GetAxis( "Horizontal" ) < 0 )
        {
            right *= -1;
        }
        rb.AddForceAtPosition( Time.deltaTime * right * vel.magnitude * DriftArcForce, Propulsion.transform.position );
        rb.AddForceAtPosition( Time.deltaTime * forward * vel.magnitude * DriftArcForwardForce, Propulsion.transform.position );

        // Turn faster when drifting
        rb.AddTorque( Time.deltaTime * transform.TransformDirection( Vector3.up ) * Input.GetAxis( "Horizontal" ) * TurnForce * DriftTurnMultiplier );

        // Angle the vehicle to drift by speed
        TargetDriftAngle = new Vector3( 0, 0, 1 ) * DriftVisualTiltAngle * Input.GetAxis( "Horizontal" ) * rb.velocity.magnitude;
        PunchMesh.PunchScale = DriftPunch;
        PunchMesh.Punch();
    }

    void FinishDrifting()
    {
        DriftBuildMultiplier = 0;
        TargetDriftAngle = Vector3.zero;
    }
    #endregion

    #region Turbo Boosting
    void UpdateBoosting()
	{
        if ( DriftTurbo > 0.1f )
        {
            if ( InputTryBoost )
            {
                ToggleBoosting( true );
                RunBoosting();
            }
            else
            {
                ToggleBoosting( false );
            }
        }
        else
        {
            DriftTurbo = 0;
            ToggleBoosting( false );
        }
    }

    void ToggleBoosting( bool toggle )
	{
        if ( Boosting != toggle )
		{
            Boosting = toggle;
            if ( Boosting )
			{
                StartBoosting();
			}
            else
			{
                FinishBoosting();
			}
		}
	}

    void StartBoosting()
	{
        PunchMesh.PunchScale = TurboStartPunch;
        PunchMesh.Punch();
	}

    void RunBoosting()
    {
        // Directional Input
        Vector3 forward = transform.TransformDirection( Vector3.forward );
        if ( NoVerticalInput )
        {
            forward.y = 0;
        }

        // Cancel horizontal velocity
        Vector3 vel = rb.velocity;
        vel.y = 0;
        rb.AddForceAtPosition( -vel, Propulsion.transform.position );

        rb.AddForceAtPosition( Time.deltaTime * forward * TurboForce, BoostPropulsion.transform.position );
        float TurboArrestAngularMultiplier = 1;
        rb.angularVelocity = Vector3.Lerp( rb.angularVelocity, Vector3.zero, Time.deltaTime * TurboArrestAngularMultiplier );
        DriftTurbo -= TurboConsumeRate;

        if ( TurboSoundEmitter != null && !TurboSoundEmitter.IsPlaying() )
        {
            TurboSoundEmitter.Play();
        }

        if ( ConstantTurboPunch )
        {
            PunchMesh.Punch();
        }
    }

    void FinishBoosting()
    {
        PunchMesh.PunchScale = TurboFinishPunch;
        PunchMesh.Punch();
    }
	#endregion
}
