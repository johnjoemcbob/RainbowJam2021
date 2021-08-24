using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleUpright : MonoBehaviour
{
    private HoverVehicle Vehicle;

    private Vector3 InitialScale;

    void Start()
    {
        Vehicle = FindObjectOfType<HoverVehicle>();

        InitialScale = transform.localScale;
    }

    void Update()
    {
        // Raycast from vehicle center to this uprighter
        // If hits, then need to rotate towards the normal
        Vector3 start = Vehicle.transform.position;
        Vector3 finish = transform.position;
        Vector3 dir = ( finish - start ).normalized;
        RaycastHit hit;
        float dist = Vector3.Distance( start, finish );
        int layermask =~ LayerMask.GetMask( "Player" );
        if ( Physics.Raycast( start, dir, out hit, dist, layermask ) )
		{
            Quaternion target = Quaternion.LookRotation( Vehicle.transform.forward, hit.normal );
            Vehicle.transform.rotation = Quaternion.Lerp( Vehicle.transform.rotation, target, Time.deltaTime * 5 );

            // Visualisation for debug
            transform.localScale = InitialScale * 10;
        }

        // Visualisation for debug
        transform.localScale = Vector3.Lerp( transform.localScale, InitialScale, Time.deltaTime * 15 );
    }
}
