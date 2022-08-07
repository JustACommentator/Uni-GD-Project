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
    public class RMainMenuControlsHandler : MonoBehaviour
    {
        private bool rollInWalkingDirection = RuneProject.SaveGameSystem.RSaveGameContainer.Instance.SaveData.rollInWalkingDirection;

        public void OnClick_DashDirection(Button button)
        {
            rollInWalkingDirection = !rollInWalkingDirection;

            button.GetComponentInChildren<TMP_Text>().text = rollInWalkingDirection ? "Dash in walking direction" : "Dash in mouse direction";
        }
    }
}