using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuneProject.UserInterfaceSystem
{
    /// <summary>
    /// Always use Initialize Method!
    /// </summary>
    public class RUI_Alert : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text alertTitleText = null;
        [SerializeField] private TMP_Text alertDescriptionText = null;
        [SerializeField] private TMP_Text alertAcceptButtonText = null;
        [SerializeField] private TMP_Text alertDenyButtonText = null;
        [Space]
        [SerializeField] private Button alertAcceptButton = null;
        [SerializeField] private Button alertDenyButton = null;

        private bool hasCustomDeny = false;

        private const string DEFAULT_DENY_TEXT = "Cancel";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !hasCustomDeny)
                OnClick_DestroyMe();
        }

        /// <summary>
        /// Default Alert with Self-Destroying Deny Button.
        /// </summary>
        public void Initialize(string title, string description, string acceptActionButtonText, Action acceptAction, bool showDenyButton = true)
        {
            alertTitleText.text = title;
            alertDescriptionText.text = description;
            alertAcceptButtonText.text = acceptActionButtonText;
            alertDenyButtonText.text = DEFAULT_DENY_TEXT;
            alertAcceptButton.onClick.AddListener(() => acceptAction());
            alertDenyButton.onClick.AddListener(() => OnClick_DestroyMe());
            alertDenyButton.gameObject.SetActive(showDenyButton);
        }

        /// <summary>
        /// Custom Alert with custom Deny Button.
        /// </summary>
        public void Initialize(string title, string description, string acceptActionButtonText, Action acceptAction, string denyActionButtonText, Action denyAction)
        {
            alertTitleText.text = title;
            alertDescriptionText.text = description;
            alertAcceptButtonText.text = acceptActionButtonText;
            alertDenyButtonText.text = denyActionButtonText;
            alertAcceptButton.onClick.AddListener(() => acceptAction());
            alertDenyButton.onClick.AddListener(() => denyAction());
            hasCustomDeny = true;
        }

        private void OnClick_DestroyMe()
        {
            Destroy(gameObject);
        }
    }
}