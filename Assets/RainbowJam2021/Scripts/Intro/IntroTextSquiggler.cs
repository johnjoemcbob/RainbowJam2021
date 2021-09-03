using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroTextSquiggler : MonoBehaviour
{

    private const string RandomizedChars = "0123456789XABCDEF?!@#";
    public TMPro.TMP_Text TextElement;

    // Update is called once per frame
    void Update()
    {
        char X1 = RandomizedChars[Random.Range(0, RandomizedChars.Length)];
        char X2 = RandomizedChars[Random.Range(0, RandomizedChars.Length)];

        TextElement.text = $"{X1}{X2}";
    }
}
