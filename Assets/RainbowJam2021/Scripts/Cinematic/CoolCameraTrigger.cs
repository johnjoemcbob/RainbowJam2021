using UnityEngine;

public class CoolCameraTrigger : MonoBehaviour
{
    public enum TriggerMode
    {
        ENTER,
        EXIT
    }

    public TriggerMode CoolCameraTriggerMode = TriggerMode.ENTER;
    public CoolCamera CoolCameraController;

    public void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag( "Player" ) )
        {
            if(CoolCameraTriggerMode == TriggerMode.ENTER)
            {
                CoolCameraController.EnterCoolMode(other.gameObject.transform, Camera.current);
            }
            else
            {
                CoolCameraController.ExitCoolMode();
            }
        }
    }
}