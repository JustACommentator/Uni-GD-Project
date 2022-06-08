using RuneProject.AudioSystem;
using RuneProject.UserInterfaceSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


namespace RuneProject.MainMenuSystem
{
    public class RMainMenuAudioHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioMixer audioMixer = null;

        float masterOriginalVolume;
        float musicOriginalVolume;
        float sfxOriginalVolume;
        float playerVoiceOriginalVolume;
        float enemyVoiceOriginalVolume;


        private void Start()
        {
            audioMixer.GetFloat("Master", out masterOriginalVolume);
            audioMixer.GetFloat("Music", out musicOriginalVolume);
            audioMixer.GetFloat("SFX", out sfxOriginalVolume);
            audioMixer.GetFloat("PlayerVoice", out playerVoiceOriginalVolume);
            audioMixer.GetFloat("EnemyVoice", out enemyVoiceOriginalVolume);
        }

        public void OnMasterVolumeChange(Slider slider)
        {
            audioMixer.SetFloat("Master", slider.value * (masterOriginalVolume + 80) - 80);
        }

        public void OnMusicVolumeChange(Slider slider)
        {
            audioMixer.SetFloat("Music", slider.value * (musicOriginalVolume + 80) - 80);
        }

        public void OnVoicesVolumeChange(Slider slider)
        {
            audioMixer.SetFloat("PlayerVoice", slider.value * (playerVoiceOriginalVolume + 80) - 80);
            audioMixer.SetFloat("EnemyVoice", slider.value * (enemyVoiceOriginalVolume + 80) - 80);
        }

        public void OnSFXVolumeChange(Slider slider)
        {
            audioMixer.SetFloat("SFX", slider.value * (sfxOriginalVolume + 80) - 80);
        }

        public void OnClick_MuteInBackground()
        {
            Application.runInBackground = !Application.runInBackground;
        }
    }
}