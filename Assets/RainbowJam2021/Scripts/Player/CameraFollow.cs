using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	#region Variables - Inspector
	[Header( "Variables" )]
    public float LerpSpeed = 5;
    public float VelocityMin = 15;
    public float VelocityMax = 30;
    public float ShakeMin = 0;
    public float ShakeMax = 0.3f;
    public float ShakeExternal = 2;
    public float ShakeDropoff = 100;

    [Header( "References" )]
    public Transform CameraTarget;
    public Transform LookAt;
	#endregion

	#region Variables - Private
	Rigidbody rb;

    private float CurrentShake = 0;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        rb = FindObjectOfType<HoverVehicle>().GetComponentInChildren<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 testpos = Vector3.Lerp( transform.position, CameraTarget.position, Time.deltaTime * LerpSpeed );

        // Camera screenshake
        //testpos += Random.insideUnitSphere * rb.velocity.magnitude.RemapClamped( VelocityMin, VelocityMax, ShakeMin, ShakeMax );

        // Check for camera out of bounds
        Vector3 start = LookAt.position;
        Vector3 dir = ( testpos - start ).normalized;
        RaycastHit hit;
        float dist = Vector3.Distance( start, testpos );
        int layermask =~ LayerMask.GetMask( "Player" );
        if ( Physics.Raycast( start, dir, out hit, dist, layermask ) )
		{
            Vector3 offset = hit.point - dir * 1 + hit.normal * Time.deltaTime * LerpSpeed;
            CameraTarget.GetComponent<CameraDistance>().Offset = offset - testpos;
            testpos = offset;
		}
        transform.position = testpos;

        // Now look at the vehicle again
        transform.LookAt( LookAt );

        // Camera screenshake
        transform.localPosition += Random.insideUnitSphere * rb.velocity.magnitude.RemapClamped( VelocityMin, VelocityMax, ShakeMin, ShakeMax ) * CurrentShake;
        ShakeTime += Time.deltaTime * ShakeDropoff;
        CurrentShake = Mathf.Lerp( ShakeStart, 0, ShakeTime );
    }
    #endregion

    float ShakeStart = 0;
    float ShakeTime = 0;
    public void Shake( float shake )
	{
        float temp = shake * ShakeExternal;
        if ( temp > CurrentShake )
        {
            ShakeStart = temp;
            ShakeTime = 0;
            CurrentShake = ShakeStart;
            Debug.Log( CurrentShake );
        }
    }
}
