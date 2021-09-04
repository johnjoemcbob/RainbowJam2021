using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeFloat : MonoBehaviour
{
    private Vector3 startPosition;
    private float amplitude;// = 0.5f;
    private float speed;// = 0.5f;
    private float offset;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = this.transform.localPosition;
        offset = Random.value * (Mathf.PI / 2);
        amplitude = Random.Range(0.01f, 0.3f);
        speed = Random.Range(0.5f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        float yPos = startPosition.y + Mathf.Sin((Time.time + offset) * speed) * amplitude;
        transform.localPosition = new Vector3(startPosition.x, yPos, startPosition.z);
    }
}
