using UnityEngine;
using UnityEngine.SceneManagement;

public class FMODLoader : MonoBehaviour
{
    public GameObject[] Buttons;

    bool audioResumed = false;
    private FMOD.Studio.EventInstance menuMusic;

    void Start()
    {
        foreach ( var button in Buttons )
        {
            button.SetActive( false );
        }
    }

    void Update()
    {
        if ( !audioResumed && !Buttons[0].activeSelf && FMODUnity.RuntimeManager.HasBankLoaded( "Master" ) )
        {
			foreach ( var button in Buttons )
            {
                button.SetActive( true );
            }

            menuMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Menu Track");
            menuMusic.start();
        }
    }

    public void ButtonIntro()
    {
        foreach ( var button in Buttons )
        {
            button.SetActive( false );
        }

        SceneManager.LoadSceneAsync( 3, LoadSceneMode.Single );

        RestartFMOD();
    }

    public void ButtonPlay()
    {
        foreach ( var button in Buttons )
        {
            button.SetActive( false );
        }

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

        SceneManager.LoadSceneAsync( 2, LoadSceneMode.Single );

        RestartFMOD();
    }

    void RestartFMOD()
    {
        menuMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        menuMusic.release();

        if ( !audioResumed )
        {
            var result = FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
            result = FMODUnity.RuntimeManager.CoreSystem.mixerResume();
            audioResumed = true;
        }
    }
}
