using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsCameraTakeover : MonoBehaviour
{
    public enum TakeoverState
    {
        Waiting,
        TakingOver,
        CreditsAnimation
    }

    private TakeoverState CurrentState;
    private GameObject PlayerVehicle;
    private Transform PlayerCameraToSteal;
    public Camera CameraToSwitchTo;

    

    public void OnTriggerEnter(Collider other)
    {
        // Stealing time!
        if ( other.CompareTag( "Player" ) )
        {
            PlayerVehicle = other.gameObject;
            PlayerCameraToSteal = PlayerVehicle.transform.parent.parent.Find("CameraFollower").Find("Camera");
            var hoverVehicle = PlayerVehicle.transform.parent.parent.GetComponentInChildren<HoverVehicle>();
            hoverVehicle.CreditsLockout = true;
        }

        // Thefted.
        PlayerCameraToSteal.SetParent(this.transform);

        CurrentState = TakeoverState.TakingOver;
    }

    public void Update()
    {
        switch(CurrentState)
        {
            case TakeoverState.Waiting:
            {

            
                break;
            }
            case TakeoverState.TakingOver:
            {
                var camPos = PlayerCameraToSteal.transform.position;
                camPos = Vector3.Lerp(camPos, CameraToSwitchTo.transform.position, 0.005f);
                PlayerCameraToSteal.transform.position = camPos;

                var camRot = PlayerCameraToSteal.transform.rotation;
                camRot = Quaternion.RotateTowards(camRot, CameraToSwitchTo.transform.rotation, 3);
                PlayerCameraToSteal.transform.rotation = camRot;

                if((CameraToSwitchTo.transform.position - camPos).magnitude < 5)
                {
                    PlayerCameraToSteal.gameObject.SetActive(false);
                    CameraToSwitchTo.gameObject.SetActive(true);
                    var anim = CameraToSwitchTo.gameObject.GetComponent<Animation>();
                    anim.Play();
                    CurrentState = TakeoverState.CreditsAnimation;
                }

                break;
            } 
            case TakeoverState.CreditsAnimation:
            {

                break;
            }
        }
    }
}
