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

        rb = FindObjectOfType<HoverVehicle>().GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {
        VelocityText.text = (rb.velocity.magnitude * 3.6f).ToString("F1") + "km/h";
    }
}
