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
    public class RMainMenuVideoHandler : MonoBehaviour
    {
        public void OnSet_Resolution(TMP_Dropdown dropdown)
        {
            switch (dropdown.value)
            {
                case 0:
                    Screen.SetResolution(1920,1080, Screen.fullScreen);
                    break;
            }
        }

        public void OnSet_WindowMode(TMP_Dropdown dropdown)
        {
            //Fullscreen, Borderless Window, Window
            switch(dropdown.value)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
        }

        public void OnSet_Quality(Slider slider)
        {
            QualitySettings.SetQualityLevel((int)slider.value * QualitySettings.names.Length);
        }

        public void OnSet_FPS(TMP_Dropdown dropdown)
        {
            switch (dropdown.value)
            {
                case 0:
                    Application.targetFrameRate = 30;
                    break;
                case 1:
                    Application.targetFrameRate = 60;
                    break;
                case 2:
                    Application.targetFrameRate = 140;
                    break;
            }
        }

        public void OnClick_VSync()
        {
            QualitySettings.vSyncCount = (QualitySettings.vSyncCount == 1) ? 0 : 1;
        }

        public void OnClick_Screenshake()
        {
            //TBI
        }
    }
}