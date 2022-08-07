using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_EndGameCanvasHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RLevelTransition loadingScreenPrefab = null;

        public void OnClick_ReturnToMainMenu()
        {
            RLevelTransition transition = Instantiate(loadingScreenPrefab, transform);
            transition.LoadScene("MainMenu");
        }
    }
}