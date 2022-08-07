using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RuneProject.DebugSystem
{
    public class RDebugAnimationInfo : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text infoText = null;
        [Space]
        [SerializeField] private RPlayerAnimationController animationController = null;

        private void Update()
        {
            infoText.text = $"IM :\t\t{animationController.PlayerAnimator.GetFloat("inputMagnitude"):F2}\n" +
                $"State :\t{(animationController.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walking") ? "Walking" : animationController.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running") ? "Running" : "Misc")}\n" +
                $"Last Move :\t{animationController.LastTimeSinceMove:F2}";
        }
    }
}