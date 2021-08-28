using System.Collections.Generic;
using UnityEngine;

public class GroundEffectParticles : MonoBehaviour
{
    public HoverVehicle Vehicle;
    public Transform GroundParticles;
    public Transform FrontBlastParticlesContainer;
    public Transform FrontSideBlastParticlesContainer;
    public Transform SideTrailsParticleContainer;

    private ParticleSystem FrontBlastParticles;
    private ParticleSystem[] FrontSideBlastParticles;
    private ParticleSystem[] SideTrailsParticles;

    private Transform SideTrailLeft;
    private Transform SideTrailRight;


    public void Start()
    {
        Debug.Assert(FrontBlastParticlesContainer != null && FrontSideBlastParticlesContainer != null && GroundParticles != null, "Please set up the particle containers for this GroundEffectParticles instance.");
        
        FrontBlastParticles = FrontBlastParticlesContainer.GetComponentInChildren<ParticleSystem>();
        FrontSideBlastParticles = FrontSideBlastParticlesContainer.GetComponentsInChildren<ParticleSystem>();
        SideTrailsParticles = SideTrailsParticleContainer.GetComponentsInChildren<ParticleSystem>();

        SideTrailLeft = SideTrailsParticleContainer.Find("Left");
        SideTrailRight = SideTrailsParticleContainer.Find("Right");
    }

    public void Update()
    {
        var rayResult = Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit rayHit, float.MaxValue);

        if(rayResult)
        {
            UpdateGroundCenterParticles(rayHit);
            UpdateFrontBlastParticles(rayHit);
            UpdateFrontSideBlastParticles(rayHit);
            UpdateSideTrailsParticles(rayHit);
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

        var frontSideBlastPos = FrontSideBlastParticlesContainer.localPosition;
        frontSideBlastPos.y = (rayHit.point.y - gameObject.transform.position.y) + (1.0f * frontSideBlastSpeedScalar);
        frontSideBlastPos.z = 1.0f + stretchScalar;
        FrontSideBlastParticlesContainer.localPosition = frontSideBlastPos;

        var frontSideBlastScale = FrontSideBlastParticlesContainer.localScale;
        frontSideBlastScale.y = frontSideBlastSpeedScalar;
        frontSideBlastScale.z = stretchScalar;
        FrontSideBlastParticlesContainer.localScale = frontSideBlastScale;

        foreach(var frontSideBlastParticles in FrontSideBlastParticles)
        {
            var particles = frontSideBlastParticles.main;
            particles.startColor = new ParticleSystem.MinMaxGradient(new Color(1.0f, 1.0f, 1.0f, groundDistanceFade));
        }
        
    }

    private void UpdateSideTrailsParticles(RaycastHit rayHit)
    {
        float sideTrailsSpeedScalar = Vehicle.GetSpeed().RemapClamp01(20, 45) * 50.0f;
        float sideTrailsTiltScalar = Vehicle.GetSpeed().RemapRangeClamped(30, 70, 20, 45);
        
        float groundDistanceFade = 1.0f - rayHit.distance.RemapClamp01(3, 4);

        var sideTrailsPos = SideTrailsParticleContainer.localPosition;
        sideTrailsPos.y = (rayHit.point.y - gameObject.transform.position.y) + 0.5f;
        SideTrailsParticleContainer.localPosition = sideTrailsPos;


        var xzVel = Vehicle.GetVelocity();
        xzVel.y = 0;

        SideTrailsParticleContainer.transform.rotation = Quaternion.LookRotation(xzVel.normalized, Vector3.up);

        foreach(var sideTrailsParticles in SideTrailsParticles)
        {
            var particles = sideTrailsParticles.main;
            var emission = sideTrailsParticles.emission;

            emission.rateOverTime = sideTrailsSpeedScalar;
            particles.startColor = new ParticleSystem.MinMaxGradient(new Color(1.0f, 1.0f, 1.0f, groundDistanceFade));
        }

        SideTrailLeft.localRotation = Quaternion.Euler(-1.0f * sideTrailsTiltScalar, 90, 0);
        SideTrailRight.localRotation = Quaternion.Euler(sideTrailsTiltScalar, 90, 0);
    }


}