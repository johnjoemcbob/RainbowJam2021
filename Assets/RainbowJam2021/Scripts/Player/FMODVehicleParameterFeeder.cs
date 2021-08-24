using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODVehicleParameterFeeder : MonoBehaviour
{
    #region Variables - Inspector
    [Header( "References" )]
    public HoverVehicle Vehicle;
    public FMODUnity.StudioEventEmitter EngineSoundEmitter;
    public bool IsPlayer;
    #endregion

    void Start()
    {

    }

	private void Update()
	{
        if(IsPlayer)
        {
		    FMODUnity.RuntimeManager.StudioSystem.setParameterByName("PlayerVehicleSpeed", Vehicle.GetSpeed());
        }
        
        EngineSoundEmitter.SetParameter("IndividualVehicleSpeed", Vehicle.GetSpeed());
	}
}
