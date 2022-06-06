using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RuneProject.MainMenuSystem
{    
    public class RMainMenuAdditionalElementsHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text versionText = null;

        private void Start()
        {
            versionText.text = $"Version {Application.version}";
        }
    }
}