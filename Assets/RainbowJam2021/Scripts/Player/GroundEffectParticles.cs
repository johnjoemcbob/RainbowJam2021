using System.Collections.Generic;
using UnityEngine;

public class GroundEffectParticles : MonoBehaviour
{
    public HoverVehicle Vehicle;
    public Transform GroundParticles;
    public Transform FrontBlastParticlesContainer;
    public Transform SideBlastParticlesContainer;

    private ParticleSystem FrontBlastParticles;
    private ParticleSystem[] FrontSideBlastParticles;


    public void Start()
    {
        Debug.Assert(FrontBlastParticlesContainer != null && SideBlastParticlesContainer != null && GroundParticles != null, "Please set up the particle containers for this GroundEffectParticles instance.");
        
        FrontBlastParticles = FrontBlastParticlesContainer.GetComponentInChildren<ParticleSystem>();
        FrontSideBlastParticles = SideBlastParticlesContainer.GetComponentsInChildren<ParticleSystem>();
    }

    public void Update()
    {
        var rayResult = Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit rayHit, float.MaxValue);

        if(rayResult)
        {
            UpdateGroundCenterParticles(rayHit);
            UpdateFrontBlastParticles(rayHit);
            UpdateFrontSideBlastParticles(rayHit);
        }
    }

    private void UpdateGroundCenterParticles(RaycastHit rayHit)
    {
        GroundParticles.position = rayHit.point + (Vector3.up * 0.1f);
        GroundParticles.rotation = Quaternion.LookRotation(rayHit.normal, Vector3.up) * Quaternion.Euler(90, 0, 0);
    }

    private void UpdateFrontBlastParticles(RaycastHit rayHit)
    {
        float frontBlastSpeedScalar = Vehicle.GetSpeed().RemapClamp01(20, 60) * 1.5f;
        float groundDistanceFade = 1.0f - rayHit.distance.RemapClamp01(3, 4);

        var frontBlastPos = FrontBlastParticlesContainer.localPosition;
        frontBlastPos.y = (rayHit.point.y - gameObject.transform.position.y) + (1.0f * frontBlastSpeedScalar);
        FrontBlastParticlesContainer.localPosition = frontBlastPos;

        var frontBlastScale = FrontBlastParticlesContainer.localScale;
        frontBlastScale.y = frontBlastSpeedScalar;
        FrontBlastParticlesContainer.localScale = frontBlastScale;

        var frontBlastParticles = FrontBlastParticles.main;
        frontBlastParticles.startColor = new ParticleSystem.MinMaxGradient(new Color(1.0f, 1.0f, 1.0f, groundDistanceFade));
        
    }

    private void UpdateFrontSideBlastParticles(RaycastHit rayHit)
    {
        float frontSideBlastSpeedScalar = Vehicle.GetSpeed().RemapClamp01(20, 60) * 1.5f;
        float stretchScalar = Vehicle.GetSpeed().RemapRangeClamped(35,50, 1.0f, 2.0f);
        float groundDistanceFade = 1.0f - rayHit.distance.RemapClamp01(3, 4);

        var frontBlastPos = SideBlastParticlesContainer.localPosition;
        frontBlastPos.y = (rayHit.point.y - gameObject.transform.position.y) + (1.0f * frontSideBlastSpeedScalar);
        frontBlastPos.z = 1.0f + stretchScalar;
        SideBlastParticlesContainer.localPosition = frontBlastPos;

        var frontSideBlastScale = SideBlastParticlesContainer.localScale;
        frontSideBlastScale.y = frontSideBlastSpeedScalar;
        frontSideBlastScale.z = stretchScalar;
        SideBlastParticlesContainer.localScale = frontSideBlastScale;

        foreach(var frontSideBlastParticles in FrontSideBlastParticles)
        {
            var particles = frontSideBlastParticles.main;
            particles.startColor = new ParticleSystem.MinMaxGradient(new Color(1.0f, 1.0f, 1.0f, groundDistanceFade));
        }
        
    }


}