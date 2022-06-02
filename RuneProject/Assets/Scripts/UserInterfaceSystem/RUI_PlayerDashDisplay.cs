using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_PlayerDashDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerDash playerDash = null;
        [SerializeField] private Image dashIndicator = null;

        private void Update()
        {
            dashIndicator.fillAmount = 1 - playerDash.CurrentDashCooldown;            
        }
    }
}