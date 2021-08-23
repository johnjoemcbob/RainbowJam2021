using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	#region Variables - Inspector
	[Header( "Variables" )]
    public float LerpSpeed = 5;

    [Header( "References" )]
    public Transform CameraTarget;
    public Transform LookAt;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp( transform.position, CameraTarget.position, Time.deltaTime * LerpSpeed );
        transform.LookAt( LookAt );
    }
	#endregion
}
