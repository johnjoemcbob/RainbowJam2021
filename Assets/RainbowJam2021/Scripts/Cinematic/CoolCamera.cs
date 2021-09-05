using UnityEngine;

public class CoolCamera : MonoBehaviour
{
    public Camera CameraToSwitchTo;
    public Camera OriginalCamera;
    public Transform CameraPivot;

    public float MinDistance = 35;
    public float MaxDistance = 65;
    public float MinFOV = 25;
    public float MaxFOV = 75;

    public float MaxCamTimeSeconds = 10f;
    private float CurrentCamTimeSeconds;

    private Transform ObjectOfInterest;
    private Camera PreviousCamera;
    

    public void EnterCoolMode(Transform objectOfInterest, Camera originalCamera)
    {
        ObjectOfInterest = objectOfInterest;
        OriginalCamera = originalCamera;
        OriginalCamera.enabled = false;
        CameraToSwitchTo.enabled = true;

        CurrentCamTimeSeconds = MaxCamTimeSeconds;
    }

    public void ExitCoolMode()
    {
        ObjectOfInterest = null;
        OriginalCamera.enabled = true;
        CameraToSwitchTo.enabled = false;
    }

    public void Update()
    {
        if(ObjectOfInterest != null)
        {
            if(CurrentCamTimeSeconds > 0)
            {
                CurrentCamTimeSeconds -= Time.deltaTime;
            }
            else
            {
                ExitCoolMode();
            }


            Vector3 objectLookVector = ObjectOfInterest.position - gameObject.transform.position;
            
            CameraPivot.rotation = Quaternion.LookRotation(objectLookVector.normalized, Vector3.up);

            CameraToSwitchTo.fieldOfView = objectLookVector.magnitude.RemapRangeClamped(MinDistance, MaxDistance, MaxFOV, MinFOV);
        }
    }
}