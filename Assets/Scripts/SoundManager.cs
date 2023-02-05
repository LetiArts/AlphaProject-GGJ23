using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    /// ///////////////
    public string ClipName;
    public bool isBGSound, isDefaultBG;
    [HideInInspector]
    public bool isCurrentBG;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;
    public bool loop = false;
    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;
    /// ///////////////

    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }

    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.Play();
    }
    public void Stop()
    {
        source.Stop();
    }

    public bool isPlaying()
    {
        return source.isPlaying;
    }

    public void SetVolume(float amount)
    {
        source.volume = amount * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
    }
}

public class SoundManager : MonoBehaviour
{
    private bool _muteBackgroundMusic = false;
    private bool _muteSoundFx = false;    

    [SerializeField]
    Sound[] sounds;
    public static SoundManager instance = null;
    bool hasReducedBG = false;

    private void Awake ()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        { Destroy(this); }
    }


    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].ClipName);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
        PlayDefualtBG();
    }

    public void PlayDefualtBG(){
        foreach(var sound in sounds)
        {
            if (sound.isBGSound && sound.isDefaultBG)
            {
                sound.Play();
            }else if (sound.isBGSound && !sound.isDefaultBG)
            {
                sound.Stop();
            }
        }
    }

    public void StopDefaultBG(){
        foreach(var sound in sounds)
        {
            if (sound.isBGSound && sound.isPlaying() && sound.isDefaultBG)
            {
                sound.isCurrentBG = false;
                sound.Stop();
            }
        }
    }

    public void PlaySFX(string _name)
    {
        if (!IsSoundFXMuted())
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].ClipName == _name)
                {
                    sounds[i].Play();
                    return;
                }
            }
        }

        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list, " + _name);
    }

    public void SetCurrentBG(string ClipName)
    {
        foreach(var sound in sounds)
        {
            if (sound.isBGSound)
            {
                if (sound.ClipName == ClipName)
                {
                    sound.isCurrentBG = true;
                    if (IsBackgroundMusicMuted() == false && sound.isPlaying() == false)
                    {
                        sound.Play();
                    }
                }
                else{
                    sound.isCurrentBG = false;
                    sound.Stop();
                }
            }

        }
    }

    public void CheckDefaultBGVolume()
    {
        if (!_muteBackgroundMusic)
        {
            foreach(var sound in sounds)
            {
                if (sound.isDefaultBG && hasReducedBG)
                {  
                    IncreaseBG();
                }
            }
        }
    }

    public void ReduceBG()
    {
        foreach(var sound in sounds)
        {
            if (sound.isBGSound)
            {
                sound.SetVolume(0f);
                hasReducedBG = true;
            }
        }
    }


    public void IncreaseBG()
    {
        foreach(var sound in sounds)
        {
            if (sound.isBGSound)
            {
                sound.SetVolume(0.6f);
                hasReducedBG = false;
            }
        }    
    }

    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].ClipName == _name)
            {
                sounds[i].Stop();
                return;
            }
        }

        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list, " + _name);
    }

    public void ToggleBackgroundMusic()
    {
        _muteBackgroundMusic = !_muteBackgroundMusic;
        if (_muteBackgroundMusic)
        {
            foreach(var sound in sounds)
            {
                if (sound.isBGSound && sound.isPlaying())
                {
                    sound.isCurrentBG = true;
                    sound.Stop();
                }
            }
        }
        else
        {
            foreach(var sound in sounds)
            {
                if (sound.isBGSound && sound.isCurrentBG)
                {
                    sound.isCurrentBG = false;
                    sound.Play();
                }
            }        
        }
    }

    public void ToggleSoundFX()
    {
        _muteSoundFx = !_muteSoundFx;
    }

    public bool IsBackgroundMusicMuted()
    {
        return _muteBackgroundMusic;
    }

    public bool IsSoundFXMuted()
    {
        return _muteSoundFx;
    }
}
