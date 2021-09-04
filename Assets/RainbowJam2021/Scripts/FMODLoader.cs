using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FMODLoader : MonoBehaviour
{
    public GameObject[] Buttons;

    bool audioResumed = false;

    void Start()
    {
        foreach ( var button in Buttons )
        {
            button.SetActive( false );
        }
    }

    void Update()
    {
        if ( FMODUnity.RuntimeManager.HasBankLoaded( "Master" ) && !Buttons[0].activeSelf )
        {
			foreach ( var button in Buttons )
            {
                button.SetActive( true );
            }
        }
    }

    public void ButtonIntro()
    {
        foreach ( var button in Buttons )
        {
            button.SetActive( false );
        }
        FindObjectOfType<Text>().text = "Loading.";
        SceneManager.LoadSceneAsync( 3, LoadSceneMode.Single );

        RestartFMOD();
    }

    public void ButtonPlay()
    {
        foreach ( var button in Buttons )
        {
            button.SetActive( false );
        }
        FindObjectOfType<Text>().text = "Loading.";

        GameObject.Instantiate(Resources.Load("MusicHolder") as GameObject);

        SceneManager.LoadSceneAsync( 1, LoadSceneMode.Single );

        RestartFMOD();
    }

    public void ButtonDebug()
    {
        foreach ( var button in Buttons )
        {
            button.SetActive( false );
        }
        FindObjectOfType<Text>().text = "Loading.";
        SceneManager.LoadSceneAsync( 2, LoadSceneMode.Single );

        RestartFMOD();
    }

    void RestartFMOD()
    {
        if ( !audioResumed )
        {
            var result = FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
            result = FMODUnity.RuntimeManager.CoreSystem.mixerResume();
            audioResumed = true;
        }
    }
}
