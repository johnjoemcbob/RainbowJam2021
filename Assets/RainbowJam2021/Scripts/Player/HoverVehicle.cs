using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoverVehicle : MonoBehaviour
{
    public const bool DEBUG = false;

    #region Variables - Inspector
    [Header( "Variables" )]
    public float ThrustForce = 400;
    public float TurnForce = 300;
    public float EngineForce = 250;
    public float TurboForce = 800;
    public float TurboDownForce = 400;
    public float TurboConsumeRate = 0.05f;
    public float DriftArcForce = 400;
    public float DriftArcForwardForce = 400;
    public float DriftSlowMultiplier = 1;
    public float DriftTurnMultiplier = 1;
    public float DriftBuildVelocity = 15;
    public float DriftBuildAngular = 1;
    public float DriftBuildMultiplierInitial = 0.5f;
    public float DriftBuildMultiplierSpeed = 1;
    public float DriftBuildMultiplierMax = 1;
    public float DriftVisualTiltAngle = 30;
    public float DriftTurboEpsilon = 0.01f;
    public float AngleLerpSpeed = 5;
    public bool ConstantTurboPunch = true;
    public Vector3 DriftPunch = Vector3.one;
    public Vector3 TurboStartPunch = Vector3.one;
    public Vector3 TurboFinishPunch = Vector3.one;
    public bool NoVerticalInput = false;
    public float VerticalBoostInputDownMultiplier = 0;
    public float VerticalBoostInputUpMultiplier = 0;
    public float YRespawnHeight = -60;
    public float ExtraGravityHeight = 2;
    public float ExtraGravityTime = 1;
    public float ExtraGravityMultiplier = 1000;
    public Vector3 RespawnAngleOffset;

    [Header( "References" )]
    public List<GameObject> Engines;
    public GameObject Propulsion;
    public GameObject BoostPropulsion;
    public GameObject CenterMass;
    public GameObject[] VisualEngines;

    public FMODUnity.StudioEventEmitter TurboSoundEmitter;
    public FMODUnity.StudioEventEmitter HornSoundEmitter;
    #endregion

    #region Variables - Public
    [HideInInspector]
    public float DriftTurbo = 0;
    [HideInInspector]
    public bool Boosting = false;
    #endregion

    #region Variables - Private
    private Rigidbody rb;
    private Punchable PunchMesh;

    private bool Drifting = false;
    private float DriftBuildMultiplier = 0;
    private float AirTime = 0;
    private float ExtraGravityForce = 0;
    private bool InputTryDrift = false;
    private bool InputTryBoost = false;
    private bool DebugInfiniteTurbo = false;
    private Vector3 TargetDriftAngle = Vector3.zero;
    private CheckpointActivator LastCheckpoint;
    private Vector3 LastCheckpointPos;
    private Vector3 LastCheckpointAng;
    private Vector3[] VisualEngineInitialPos;
    private float JustRespawned = 0;
    private bool Respawning = false;

    [HideInInspector]
    public bool CreditsLockout = false;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        rb.centerOfMass = CenterMass.transform.localPosition;

        PunchMesh = GetComponentInChildren<Punchable>();

        ToggleEngineVisualise();
        ToggleStabiliserVisualise();

        LastCheckpointPos = transform.position;

        // Store initial visual engine positions
        VisualEngineInitialPos = new Vector3[VisualEngines.Length];
        int ind = 0;
        foreach ( var engine in VisualEngines )
		{
            VisualEngineInitialPos[ind] = engine.transform.localPosition;
            ind++;
        }
    }

	private void Update()
	{
        if ( DEBUG )
        {
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

            // Debug return to menu
            if ( Input.GetKeyDown( KeyCode.Alpha0 ) )
            {
                SceneManager.LoadScene( 0, LoadSceneMode.Single );
            }
        }

        // Respawn if fall!
        if ( transform.localPosition.y < YRespawnHeight )
        {
            Respawn();
        }

        // Input
        InputTryDrift = Input.GetButton( "Drift" );
        InputTryBoost = Input.GetButton( "Boost" );

        if (Input.GetButtonDown("Horn"))
        {
            HornSoundEmitter.Play();
        }

        if (Input.GetButtonDown("Reset"))
        {
            LastCheckpoint.Reset();
        }

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("HornPressed",Input.GetButton("Horn") ? 1 : 0);

        UpdateEngineVisuals();
    }

	void FixedUpdate()
    {
        if ( !Respawning && JustRespawned <= 0 && !CreditsLockout )
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

            // Gravity
            UpdateGravity();
        }
		else
		{
            JustRespawned -= Time.deltaTime;
        }
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

    public void ToggleRespawning( bool respawn )
	{
        Respawning = respawn;
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
        DriftBuildMultiplier = DriftBuildMultiplierInitial;
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
        if ( DriftTurbo > DriftTurboEpsilon )
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
        // Cancel horizontal velocity
        Vector3 vel = rb.velocity;
        vel.y = 0;
        rb.AddForceAtPosition( -vel, Propulsion.transform.position );

        // Move forward in new direction
        Vector3 forward = transform.TransformDirection( Vector3.forward );
        if ( forward.y < 0 )
        {
            forward.y *= VerticalBoostInputDownMultiplier;
        }
		else
		{
            forward.y *= VerticalBoostInputUpMultiplier;
		}
        rb.AddForceAtPosition( Time.deltaTime * forward * TurboForce, BoostPropulsion.transform.position );

        // Stick to ground
        // If not already in air
        float dist = 10000;
        Vector3 start = transform.position + Vector3.up * dist / 2;
        int layermask =~ LayerMask.GetMask( "Player" );
        RaycastHit hit;
        //if ( Physics.Raycast( start, Vector3.down, out hit, dist, layermask ) )
        {
            //float y = hit.point.y - transform.position.y;
            //if ( y < 1 )
            {
                // Check the raycast against ground in front
                // If its lower than the vehicle then press down
                dist = 10000;
                start = transform.position + transform.forward * 10 + Vector3.up * dist / 2;
                if ( Physics.Raycast( start, Vector3.down, out hit, dist ) )
                {
                    float y = hit.point.y - transform.position.y;
                    if ( y > -15 && y < 0 )
                    {
                        rb.AddForceAtPosition( Time.deltaTime * transform.TransformDirection( Vector3.up ) * y * TurboDownForce, transform.position );
                    }
                }
            }
        }

        // Turn!
        float TurboArrestAngularMultiplier = 1;
        rb.angularVelocity = Vector3.Lerp( rb.angularVelocity, Vector3.zero, Time.deltaTime * TurboArrestAngularMultiplier );

        // Consume the turbo
        DriftTurbo -= TurboConsumeRate;

        // Sound
        if ( TurboSoundEmitter != null && !TurboSoundEmitter.IsPlaying() )
        {
            TurboSoundEmitter.Play();
        }

        // Visual scale punch
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

    #region Gravity
    void UpdateGravity()
    {
        float dist = ExtraGravityHeight;
        Vector3 start = transform.position;
        int layermask =~ LayerMask.GetMask( "Player" );
        RaycastHit hit;
        bool test = Physics.Raycast( start, Vector3.down, out hit, dist, layermask );
        bool grav = false;
        if ( test )
        {
            float y = hit.point.y - transform.position.y;
            if ( y > dist / 2 ) // What
            {
                grav = true;
            }
        }
		else
		{
            grav = true;
		}

        if ( grav )
        {
            // Count timer
            // If counter above var then mult grav
            AirTime += Time.deltaTime;
            if ( AirTime >= ExtraGravityTime )
            {
                ExtraGravityForce += Time.deltaTime * ExtraGravityMultiplier;
                rb.AddForce( Time.deltaTime * Vector3.up * ExtraGravityForce );
            }
            
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Falling", 1);
        }
		else
        {
            // Reset counter & force
            AirTime = 0;
            ExtraGravityForce = 0;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Falling", 0);
        }
    }
    #endregion

    #region Checkpoint
    public void StoreCheckpoint( CheckpointActivator checkpoint, bool real = true )
	{
        if ( LastCheckpoint && LastCheckpoint != checkpoint && real )
		{
            LastCheckpoint.ReachedNext();
        }

        LastCheckpoint = checkpoint;
        LastCheckpointPos = transform.position;
        LastCheckpointAng = transform.eulerAngles;
	}

    public void ResetToCheckpoint()
	{
        //transform.position = LastCheckpointPos;
        //transform.eulerAngles = LastCheckpointAng;

        // Stop physics logic for a second to reset position+angles
        JustRespawned = 1;

        transform.position = LastCheckpoint.SpawnPoint.position;
        transform.eulerAngles = LastCheckpoint.SpawnPoint.eulerAngles + RespawnAngleOffset;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        DriftTurbo = 1;

        // Camera immediately
        FindObjectOfType<CameraFollow>().Teleport();
	}
    #endregion

    #region Engine Visuals
    public float VisualEngineSpeedMultiplier = 0.01f;
    public float VisualEngineAngleMultiplier = 5;
    public float VisualEngineDriftMultiplier = 5;
    public float VisualEngineLerpSpeed = 5;
    public void UpdateEngineVisuals()
	{
        int ind = 0;
		foreach ( var engine in VisualEngines )
		{
            // If turning then face direction change
            engine.transform.localRotation = Quaternion.Lerp( engine.transform.localRotation, Quaternion.Euler( new Vector3( 0, 1, 0 ) * Input.GetAxis( "Horizontal" ) * VisualEngineAngleMultiplier ), Time.deltaTime * VisualEngineLerpSpeed );

            // Move forward/back with speed
            Vector3 target = VisualEngineInitialPos[ind] + ( Vector3.forward * GetSpeed() * VisualEngineSpeedMultiplier );
            // Drifting, move apart
            if ( Drifting )
            {
                target += Vector3.right * ( ind == 0 ? -1 : 1 ) * VisualEngineDriftMultiplier;
            }
            engine.transform.localPosition = Vector3.Lerp( engine.transform.localPosition, target, Time.deltaTime * VisualEngineLerpSpeed );

            // Up and down with terrain a bit?


            ind++;
		}
	}
	#endregion
}
