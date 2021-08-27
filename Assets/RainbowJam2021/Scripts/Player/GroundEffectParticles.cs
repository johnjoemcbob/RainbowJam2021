using System.Collections.Generic;
using UnityEngine;

public class GroundEffectParticles : MonoBehaviour
{
    public HoverVehicle Vehicle;
    public Transform GroundParticles;
    public Transform FrontBlastParticlesContainer;
    public Transform SideBlastParticlesContainer;

    public void Update()
    {
        if(GroundParticles != null)
        {
            UpdateGroundCenterParticles();
        }
    }

    private void UpdateGroundCenterParticles()
    {
        var rayResult = Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit rayHit, float.MaxValue);
        if(rayResult)
        {
            GroundParticles.transform.position = rayHit.point + (Vector3.up * 0.1f);
            GroundParticles.transform.rotation = Quaternion.LookRotation(rayHit.normal, Vector3.up) * Quaternion.Euler(90, 0, 0);
        }
    }


}