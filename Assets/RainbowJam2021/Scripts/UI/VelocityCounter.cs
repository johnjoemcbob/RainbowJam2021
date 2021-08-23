using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VelocityCounter : MonoBehaviour
{
    Text VelocityText;
    Rigidbody rb;

    void Start()
    {
        VelocityText = GetComponent<Text>();

        rb = GameObject.FindGameObjectWithTag( "Player" ).GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {
        VelocityText.text = "Velocity: " + rb.velocity.magnitude;
    }
}
