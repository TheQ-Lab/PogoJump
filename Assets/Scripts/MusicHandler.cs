using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public enum BGMType { Cave, Jungle, Mushroom, Danger, GameOver };
    private float volumeMultiplierLocal = 1f;

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

    private void FixedUpdate()
    {
        UpdateVolumeMultiplier();
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
        audioSource.volume = newClip.volume * volumeMultiplierLocal;

        currentlyPlaying = newClip;
        PlayClip();
    }

    private void PlayClip()
    {
        audioSource.Play();
    }

    private void UpdateVolumeMultiplier()
    {
        if (volumeMultiplierLocal != GameManager.Instance.VolumeMultiplier)
        {
            volumeMultiplierLocal = GameManager.Instance.VolumeMultiplier;
            audioSource.volume = currentlyPlaying.volume * volumeMultiplierLocal;
        }
    }
}
