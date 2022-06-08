using RuneProject.AudioSystem;
using RuneProject.UserInterfaceSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RuneProject.MainMenuSystem
{
    public class RMainMenuHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RMainMenuCreditsHandler creditsHandler = null;
        [SerializeField] private RAudioEmitComponent audioEmitter = null;
        [SerializeField] private AudioSource musicSource = null;
        [SerializeField] private AudioSource rainSFXSource = null;
        [Space]
        [SerializeField] private Transform canvasTransform = null;
        [SerializeField] private Transform alertParentTransform = null;
        [SerializeField] private List<GameObject> menuPanels = new List<GameObject>();
        [Space]
        [SerializeField] private TMP_Text creditsContentText = null;

        [Header("Prefabs")]
        [SerializeField] private RUI_Alert alertPrefab = null;
        [SerializeField] private RLevelTransition levelTransitionPrefab = null;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip startUpSFX = null;
        [SerializeField] private AudioClip clickSFX = null;
        [SerializeField] private AudioClip hoverSFX = null;
        [SerializeField] private AudioClip alertSFX = null;

        private bool isLoading = false;
        private RUI_Alert currentAlert = null;
        private Coroutine currentLoadCreditsContentRoutine = null;
        private EMainMenuState currentMenuState = EMainMenuState.INITIAL_MENU;

        public event EventHandler<Tuple<EMainMenuState, EMainMenuState>> OnTransitionMenuState;

        private const KeyCode ESCAPE_KEY_CODE = KeyCode.Escape;
        private const string CREDITS_PATH = "TextFileData/credits";
        private const string LOADING_LEVEL = "SampleScene";

        private void Start()
        {
            print("Bitte noch einfügen: Zurücksetzen des LockModes, je nach Einstellung! (Methode: FreeCursor)");

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            LoadCreditsContent();

            creditsHandler.OnEndCredits += CreditsHandler_OnEndCredits;
        }

        private void Update()
        {
            HandleMenuState();
        }

        private void HandleMenuState()
        {
            switch (currentMenuState)
            {
                case EMainMenuState.INITIAL_MENU:
                    {
                        if (Input.anyKeyDown)
                        {
                            TransitionTo(EMainMenuState.MAIN_PANEL);
                        }
                    }
                    break;
                case EMainMenuState.MAIN_PANEL:
                    {
                        if (Input.GetKeyDown(ESCAPE_KEY_CODE) && !currentAlert)
                        {
                            OnClick_ShowEndGameDecision();
                        }
                    }
                    break;
                case EMainMenuState.OPTIONS_PANEL:
                    {
                        if (Input.GetKeyDown(ESCAPE_KEY_CODE) && !currentAlert)
                        {
                            //Alert: Optionen �bernehmen? - Falls ver�ndert
                            TransitionTo(EMainMenuState.MAIN_PANEL);
                        }
                    }
                    break;
                case EMainMenuState.AUDIO_PANEL:
                    {
                        if (Input.GetKeyDown(ESCAPE_KEY_CODE) && !currentAlert)
                        {
                            TransitionTo(EMainMenuState.OPTIONS_PANEL);
                        }
                    }
                    break;
                case EMainMenuState.VIDEO_PANEL:
                    {
                        if (Input.GetKeyDown(ESCAPE_KEY_CODE) && !currentAlert)
                        {
                            TransitionTo(EMainMenuState.OPTIONS_PANEL);
                        }
                    }
                    break;
                case EMainMenuState.CONTROLLS_PANEL:
                    {
                        if (Input.GetKeyDown(ESCAPE_KEY_CODE) && !currentAlert)
                        {
                            TransitionTo(EMainMenuState.OPTIONS_PANEL);
                        }
                    }
                    break;
                case EMainMenuState.CREDITS_PANEL:
                    {
                        if (Input.GetKeyDown(ESCAPE_KEY_CODE) && !currentAlert)
                        {
                            TransitionTo(EMainMenuState.MAIN_PANEL);
                        }
                    }
                    break;
            }
        }

        private void TransitionTo(EMainMenuState newState)
        {
            if (currentMenuState == newState) return;

            EMainMenuState oldState = currentMenuState;
            currentMenuState = newState;

            if (oldState == EMainMenuState.INITIAL_MENU)
            {
                FreeCursor();
                musicSource.Play();
                rainSFXSource.Play();
                audioEmitter.PlayClip(startUpSFX, true);
            }

            for (int i = 0; i<menuPanels.Count; i++)            
                menuPanels[i].SetActive(i == (int)newState);

            OnTransitionMenuState?.Invoke(this, new Tuple<EMainMenuState, EMainMenuState>(oldState, newState));
        }

        private void FreeCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = Application.isEditor ? CursorLockMode.None : CursorLockMode.None; //Zweites None durch SaveState-Getter ersetzen
        }

        private void LoadCreditsContent()
        {
            if (currentLoadCreditsContentRoutine == null)
                currentLoadCreditsContentRoutine = StartCoroutine(ILoadCreditsContent());
        }

        #region OnClicks

        public void OnClick_LoadLevel()
        {
            if (!isLoading)
            {
                isLoading = true;

                audioEmitter.PlayClip(clickSFX, true);
                musicSource.Stop();
                //rainSFXSource.Stop();
                RLevelTransition instance = Instantiate(levelTransitionPrefab, canvasTransform);
                instance.LoadScene(LOADING_LEVEL);
            }
        }

        public void OnClick_ShowEndGameDecision()
        {
            audioEmitter.PlayClip(clickSFX, true);
            CreateAlert("Really Quit?", "Do you really want to quit the game? Unsaved progress will be <b>deleted</b>.", "Quit Game", Application.Quit);
        }

        public void OnClick_OpenCredits()
        {
            audioEmitter.PlayClip(clickSFX, true);
            TransitionTo(EMainMenuState.CREDITS_PANEL);
        }

        public void OnClick_OpenOptions()
        {
            audioEmitter.PlayClip(clickSFX, true);
            TransitionTo(EMainMenuState.OPTIONS_PANEL);
        }

        public void OnClick_OpenAudio()
        {
            audioEmitter.PlayClip(clickSFX, true);
            TransitionTo(EMainMenuState.AUDIO_PANEL);
        }

        public void OnClick_OpenVideo()
        {
            audioEmitter.PlayClip(clickSFX, true);
            TransitionTo(EMainMenuState.VIDEO_PANEL);
        }

        public void OnClick_OpenControlls()
        {
            audioEmitter.PlayClip(clickSFX, true);
            TransitionTo(EMainMenuState.CONTROLLS_PANEL);
        }

        public void OnClick_OpenMain()
        {
            audioEmitter.PlayClip(clickSFX, true);
            TransitionTo(EMainMenuState.MAIN_PANEL);
        }

        public void OnHover_PlayHoverSFX()
        {
            audioEmitter.PlayClip(hoverSFX, true);
        }

        #endregion

        #region Callbacks

        private void CreditsHandler_OnEndCredits(object sender, EventArgs e)
        {
            TransitionTo(EMainMenuState.MAIN_PANEL);
        }

        #endregion

        private void CreateAlert(string title, string description, string acceptActionButtonText, Action acceptAction)
        {
            if (!currentAlert)
            {
                audioEmitter.PlayClip(alertSFX, true);
                currentAlert = Instantiate(alertPrefab, alertParentTransform);
                currentAlert.Initialize(title, description, acceptActionButtonText, acceptAction);
            }
        }

        private void CreateAlert(string title, string description, string acceptActionButtonText, Action acceptAction, string denyActionButtonText, Action denyAction)
        {
            if (!currentAlert)
            {
                audioEmitter.PlayClip(alertSFX, true);
                currentAlert = Instantiate(alertPrefab, alertParentTransform);
                currentAlert.Initialize(title, description, acceptActionButtonText, acceptAction, denyActionButtonText, denyAction);
            }
        }

        private IEnumerator ILoadCreditsContent()
        {
            ResourceRequest op = Resources.LoadAsync<TextAsset>(CREDITS_PATH);
            yield return op;
            creditsContentText.text = ((TextAsset)op.asset).text;
        }
    }

    public enum EMainMenuState
    {
        INITIAL_MENU,
        MAIN_PANEL,
        OPTIONS_PANEL,
        AUDIO_PANEL,
        VIDEO_PANEL,
        CONTROLLS_PANEL,
        CREDITS_PANEL
    }
}