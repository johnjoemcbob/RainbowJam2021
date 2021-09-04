using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroLoadGame : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadSceneAsync( 1, LoadSceneMode.Single );
    }
}
