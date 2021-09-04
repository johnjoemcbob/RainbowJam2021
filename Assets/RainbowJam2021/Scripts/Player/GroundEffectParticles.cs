using System.Collections.Generic;
using UnityEngine;

public class GroundEffectParticles : MonoBehaviour
{
    public HoverVehicle Vehicle;
    public Transform GroundParticles;
    public Transform DustParticles;
    public Transform FrontBlastParticlesContainer;
    public Transform FrontSideBlastParticlesContainer;
    public Transform SideTrailsParticleContainer;
    public Transform OverallBlastContainer;

    private ParticleSystem GroundParticleSystem;
    private ParticleSystem FrontBlastParticles;
    private ParticleSystem[] FrontSideBlastParticles;
    private ParticleSystem[] SideTrailsParticles;
    private ParticleSystem DustParticleSystem;

    private Transform SideTrailLeft;
    private Transform SideTrailRight;


    public void Start()
    {
        Debug.Assert(FrontBlastParticlesContainer != null && FrontSideBlastParticlesContainer != null && GroundParticles != null, "Please set up the particle containers for this GroundEffectParticles instance.");
        
        FrontBlastParticles = FrontBlastParticlesContainer.GetComponentInChildren<ParticleSystem>();
        FrontSideBlastParticles = FrontSideBlastParticlesContainer.GetComponentsInChildren<ParticleSystem>();
        SideTrailsParticles = SideTrailsParticleContainer.GetComponentsInChildren<ParticleSystem>();

        GroundParticleSystem = GroundParticles.GetComponentInChildren<ParticleSystem>();
        DustParticleSystem = DustParticles.GetComponentInChildren<ParticleSystem>();

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

        UpdateOverallBlastContainer();
    }

    private void UpdateGroundCenterParticles(RaycastHit rayHit)
    {
        GroundParticles.position = rayHit.point + (Vector3.up * 0.1f);
        GroundParticles.rotation = Quaternion.LookRotation(rayHit.normal, Vector3.up) * Quaternion.Euler(90, 0, 0);

        float groundDistanceFade = 1.0f - rayHit.distance.RemapClamp01(1, 5);
        float dustScaleFade = 1.0f - rayHit.distance.RemapClamp01(1, 4);

        var groundScale = GroundParticles.localScale;
        groundScale.x = 0.5f + groundDistanceFade;
        groundScale.z = 0.5f + groundDistanceFade;
        GroundParticles.localScale = groundScale;

        if(rayHit.collider.gameObject.CompareTag("Terrain") && rayHit.distance < 5.0f)
        {
            DustParticleSystem.Play();
        }
        else
        {
            DustParticleSystem.Stop();
        }
    }

    private void UpdateOverallBlastContainer()
    {
        var xzVel = Vehicle.GetVelocity();
        xzVel.y = 0;
        var rayResult = Physics.Raycast(gameObject.transform.position + xzVel.normalized, Vector3.down, out RaycastHit rayHit, float.MaxValue);

        if(rayResult)
        {
            OverallBlastContainer.position = rayHit.point + (-0.1f * rayHit.normal);
            OverallBlastContainer.rotation = Quaternion.LookRotation(Vehicle.GetVelocity().normalized, rayHit.normal);
        }
    }

    private void UpdateFrontBlastParticles(RaycastHit rayHit)
    {
        float frontBlastSpeedScalar = Vehicle.GetSpeed().RemapClamp01(20, 60) * 1.5f;
        float groundDistanceFade = 1.0f - rayHit.distance.RemapClamp01(3, 4);

        var frontBlastPos = FrontBlastParticlesContainer.localPosition;
        frontBlastPos.y = frontBlastSpeedScalar;
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
        frontSideBlastPos.y = frontSideBlastSpeedScalar;
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

        SideTrailsParticleContainer.rotation = Quaternion.LookRotation(Vehicle.GetVelocity().normalized, rayHit.normal);

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