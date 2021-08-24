using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriftTurboCounter : MonoBehaviour
{
    #region Variables - Inspector
    public float[] Values;
	#endregion

	#region Variables - Private
	Slider Display;
    HoverVehicle veh;
	#endregion

	void Start()
    {
        Display = GetComponent<Slider>();

        veh = FindObjectOfType<HoverVehicle>();
    }

    void Update()
    {
        Debug.Log( veh.DriftTurbo );
        Debug.Log( veh.DriftTurbo * 11 + 0.1f );
        int ind = Mathf.FloorToInt( veh.DriftTurbo * ( Values.Length + 0 ) );
        if ( ind >= 0 && ind < Values.Length )
        {
            Display.value = Values[ind];
        }
    }
}
