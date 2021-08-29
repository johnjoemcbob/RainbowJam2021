using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainAnimator : MonoBehaviour
{
    public GameObject ReplacementPrefab;

    public float Length = 10;
    public float LerpTime = 1;
    public float LerpAngleSpeed = 5;

    Vector3 StartPos;
    Vector3 FinishPos;
    private float CurrentTime = 0;

    void Start()
    {
        CurrentTime = Random.Range( -10.0f, -5 );

        FinishPos = transform.position;
        StartPos = transform.position + transform.up * -Length;

        transform.position = StartPos;

		foreach ( Transform child in transform )
		{
            if ( child.name.Contains( "Cube" ) )
            {
                Vector3 pos = child.transform.localPosition;
                Vector3 ang = child.transform.localEulerAngles;

                Destroy( child.gameObject );

                GameObject link = Instantiate( ReplacementPrefab, transform );
                link.transform.localPosition = pos;
                link.transform.localEulerAngles = ang;
            }
		}
	}

    void Update()
    {
        CurrentTime += Time.deltaTime;
        CurrentTime = Mathf.Clamp( CurrentTime, 0, LerpTime );

        transform.position = Vector3.Lerp( StartPos, FinishPos, CurrentTime / LerpTime );
        transform.localEulerAngles += new Vector3( 0, 1, 0 ) * Time.deltaTime * LerpAngleSpeed;
    }
}
