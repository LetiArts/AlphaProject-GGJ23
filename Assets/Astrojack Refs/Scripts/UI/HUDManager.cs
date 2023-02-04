using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{

    private void Awake() {
        if(SceneManager.GetSceneByName("HUD_UI").isLoaded == false)
        {
            SceneManager.LoadSceneAsync("HUD_UI", LoadSceneMode.Additive);
        }else{
            SceneManager.UnloadSceneAsync("HUD_UI");
        }
    }
}
