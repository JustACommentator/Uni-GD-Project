using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.MainMenuSystem
{
    public class RMainMenuCreditsHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform creditsParent = null;

        [Header("Values")]
        [SerializeField] private float startY = 0f;
        [SerializeField] private float endY = 0f;
        [Space]
        [SerializeField] private float scrollDelta = 0f;

        public event System.EventHandler OnBeginCredits;
        public event System.EventHandler OnEndCredits;

        private void OnEnable()
        {
            creditsParent.localPosition = Vector3.up * startY;
            //Musik und Co. zurücksetzen
            OnBeginCredits?.Invoke(this, null);
        }

        private void Update()
        {
            HandleScrolling();
        }

        private void HandleScrolling()
        {
            creditsParent.localPosition = new Vector3(0f, Mathf.Clamp(creditsParent.localPosition.y + Time.deltaTime * scrollDelta, startY, endY));

            if (creditsParent.localPosition.y >= endY)
                OnClick_EndCredits();
        }

        public void OnClick_EndCredits()
        {
            OnEndCredits?.Invoke(this, null);
        }
    }
}