using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameUtility : MonoBehaviour
{ 
    public void PlayFXSound(string SoundName)
    {
        if (SoundManager.instance.IsSoundFXMuted() == false)
        {
            SoundManager.instance.PlaySFX(SoundName);
        }
    }

    void CloseDialogScreen()
    {
        SceneManager.LoadScene("Bunker");
    }
}
