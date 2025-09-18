using System;
using System.Collections;
using System.Resources;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using YooAsset;

namespace TJ.Scripts
{
    public class SoundController : MonoBehaviour
    {
        public static SoundController Instance;
        public AudioSource audioSource;
        //出发，
        [FormerlySerializedAs("tapSound")] public AudioClip dragonPop;
        public AudioClip buttonSound, hitSound, sort, full, win, fail, nocoinPOP;

        public AudioSource bgAudioSource;
        public AudioClip bgSound;
        public AudioClip moving;

        private bool isFullSoundPlaying = false;
        public float bgVolume = 1.0f;
        public float soundEffectVolume = 0.8f;
        private void Awake()
        {
            Instance = this;

        }

        private void Start()
        {
            bgVolume = 5.0f;
            soundEffectVolume = 0.8f;
            Invoke("PlayBgSound", 0.2f);
           //PlayBgSound();
        }

        void InitSoundSource()
        {
            //buttonSound = YooAssets.LoadAssetAsync<AudioClip>("Sound/buttonSound");
        }

        void PlayBgSound()
        {
            if (bgSound)
            {
                bgAudioSource.clip = bgSound;
                bgAudioSource.volume= bgVolume;
                bgAudioSource.loop = true;
                bgAudioSource.Play();
            }
        }

        void StopBgSound()
        {
            bgAudioSource.Stop();
        }

        public void PlayOneShot(AudioClip clip, float volume)
        {
            audioSource.PlayOneShot(clip, soundEffectVolume);
        }
        public void PlayOneShot(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        public void HandleGameResultAudio(bool isFailing)
        {
            StopBgSound();
            if (isFailing)
            {
                PlayOneShot(fail);
            }
            else
            {
                PlayOneShot(win);
            }
        }

        public void PlayFullSound()
        {
            if (!isFullSoundPlaying)
            {
                audioSource.PlayOneShot(full);
                isFullSoundPlaying = true;
                DOVirtual.DelayedCall(full.length, ()=>isFullSoundPlaying = false);
            }
        }
    }
}