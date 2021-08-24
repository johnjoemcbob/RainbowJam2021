using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FMODLoader : MonoBehaviour
{
    public GameObject Button;

    bool audioResumed = false;

    void Start()
    {
        Button.SetActive( false );
    }

    void Update()
    {
        if ( FMODUnity.RuntimeManager.HasBankLoaded( "Master" ) && !Button.activeSelf )
        {
            Debug.Log( "Master Bank Loaded" );
            Button.SetActive( true );
        }
    }

    public void ButtonPlay()
	{
        Button.SetActive( false );
        FindObjectOfType<Text>().text = "Loading.";
        SceneManager.LoadSceneAsync( 1, LoadSceneMode.Single );

        if ( !audioResumed )
        {
            var result = FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
            Debug.Log( result );
            result = FMODUnity.RuntimeManager.CoreSystem.mixerResume();
            Debug.Log( result );
            audioResumed = true;
        }
    }
}
