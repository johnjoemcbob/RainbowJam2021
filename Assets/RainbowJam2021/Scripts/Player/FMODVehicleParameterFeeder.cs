using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODVehicleParameterFeeder : MonoBehaviour
{
    #region Variables - Inspector
    [Header( "References" )]
    public HoverVehicle Vehicle;
    #endregion

    void Start()
    {

    }

	private void Update()
	{
		FMODUnity.RuntimeManager.StudioSystem.setParameterByName("PlayerVehicleSpeed", Vehicle.GetSpeed());
	}
}
