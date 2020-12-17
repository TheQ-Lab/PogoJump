using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public enum BGMType { Cave, Jungle, Mushroom, Danger, GameOver };

    [System.Serializable]
    public class BGMSample
    {
        public string name;
        public BGMType bGMType;
        public AudioClip audioClip;
        public float volume;

        BGMSample(string _name, BGMType _bGMType, AudioClip _audioClip)
        {
            name = _name;
            bGMType = _bGMType;
            audioClip = _audioClip;
            volume = 0.15f;
        }

        public static BGMSample zero()
        {
            BGMSample ret = new BGMSample("zero", BGMType.Cave, null);
            return ret;
        }
    }

    public List<BGMSample> bgm;
    //public Dictionary<BiomeType.Biome, BGMSample> biomeBGM;
    public BGMSample currentlyPlaying;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = BGMSample.zero().audioClip;
        audioSource.loop = true;
        SetAndPlay(BGMType.Cave);
    }

    public void SetAndPlay(BGMType newType)
    {
        BGMSample newClip = BGMSample.zero();
        foreach (BGMSample sample in bgm)
        {
            if(sample.bGMType == newType)
            {
                newClip = sample;
                break;
            }
        }

        audioSource.clip = newClip.audioClip;
        audioSource.volume = newClip.volume;

        currentlyPlaying = newClip;
        PlayClip();
    }

    private void PlayClip()
    {
        audioSource.Play();
    }
}
