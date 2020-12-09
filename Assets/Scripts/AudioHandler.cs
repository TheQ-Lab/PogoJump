using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [System.Serializable]
    public class SFXSample
    {
        public string name;
        public AudioClip audioClip;
        public int priority;
        public float volume;
        
        SFXSample(string _name, AudioClip _audioClip, int _priority, float _volume)
        {
            name = _name;
            audioClip = _audioClip;
            priority = _priority;
            volume = _volume;
        }

        public static SFXSample zero()
        {
            SFXSample ret = new SFXSample("zero", null, 0, 0);
            return ret;
        }
    }



    //public SFXSample LaunchMusic, LandMusic, DamageMusic, ExtraLifeMusic, GameOverMusic;
    public List<SFXSample> sfx;

    /**/public SFXSample currentlyPlaying;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentlyPlaying = SFXSample.zero();
    }

    public void SetAndPlay(string type)
    {
        SFXSample newClip = SFXSample.zero();
        //find SFXSample from type
        foreach (SFXSample sample in sfx)
        {
            if (sample.name == type)
            {
                newClip = sample;
                break;
            }
        }

        //if done playing give free spot to less prioritized
        if (!audioSource.isPlaying)
            currentlyPlaying = SFXSample.zero();

        // Play if it is higher or same prio, else don't
        if (newClip.priority >= currentlyPlaying.priority)
        {
            audioSource.clip = newClip.audioClip;
            audioSource.volume = newClip.volume;
            currentlyPlaying = newClip;
        }
        else
            return;

        PlayClip();
    }

    private void PlayClip()
    {
        audioSource.loop = false;
        audioSource.Play();
    }
}
