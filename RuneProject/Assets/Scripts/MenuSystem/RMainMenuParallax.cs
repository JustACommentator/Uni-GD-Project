using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.MainMenuSystem
{
    public class RMainMenuParallax : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private bool enableParallax = true;
        [SerializeField] private bool enableTintShift = true;
        [Space]
        [SerializeField] private Color initialTintShift = new Color();
        [SerializeField] private Color mainTintShift = new Color();
        [SerializeField] private Color secondTintShift = new Color();

        [Header("References")]
        [SerializeField] private RMainMenuHandler mainMenuHandler = null;
        [SerializeField] private SpriteRenderer backgroundSpriteRenderer = null;
        [SerializeField] private Transform parallaxTransform = null;
        [SerializeField] private Vector2 boundLimits = Vector2.zero;
        [Space]
        [SerializeField] private ParticleSystem rainParticleSystem = null;

        private Vector3 parallaxStartPos = Vector3.zero;
        private bool tintToMain = false;
        private bool afterInitialMenu = false;
        private float currentTintTimer = 0f;

        private const float PARALLAX_DELTA = 3f;
        private const float MAX_TINT_TIME = 8f;

        private void Start()
        {
            mainMenuHandler.OnTransitionMenuState += MainMenuHandler_OnTransitionMenuState;
            parallaxStartPos = parallaxTransform.position;
            SetupTintShift();
        }

        private void Update()
        {
            HandleParallax();
            HandleTintShift();
        }

        private void HandleParallax()
        {
            if (enableParallax && afterInitialMenu)
            {
                Vector3 viewportPoint = (Camera.main.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f, 0f)) * 2f;
                parallaxTransform.position = Vector3.Lerp(parallaxTransform.position,
                        new Vector3(Mathf.Clamp(parallaxStartPos.x + ((viewportPoint.x - parallaxStartPos.x) / (boundLimits.x * 2)), parallaxStartPos.x - boundLimits.x, parallaxStartPos.x + boundLimits.x),
                        Mathf.Clamp(parallaxStartPos.y + ((viewportPoint.y - parallaxStartPos.y) / (boundLimits.y * 2)), parallaxStartPos.y - boundLimits.y, parallaxStartPos.y + boundLimits.y), 0),
                        Time.deltaTime * PARALLAX_DELTA);
            }
        }

        private void HandleTintShift()
        {
            if (enableTintShift && afterInitialMenu)
            {
                if (currentTintTimer >= MAX_TINT_TIME)
                {
                    currentTintTimer = 0f;
                    tintToMain = !tintToMain;
                }
                else
                    currentTintTimer += Time.deltaTime;

                if (tintToMain)
                {
                    backgroundSpriteRenderer.color = Color.Lerp(secondTintShift, mainTintShift, currentTintTimer / MAX_TINT_TIME);
                }
                else
                {
                    backgroundSpriteRenderer.color = Color.Lerp(mainTintShift, secondTintShift, currentTintTimer / MAX_TINT_TIME);
                }
            }
        }

        private void SetupTintShift()
        {
            backgroundSpriteRenderer.color = initialTintShift;
        }

        #region Callbacks

        private void MainMenuHandler_OnTransitionMenuState(object sender, System.Tuple<EMainMenuState, EMainMenuState> e)
        {
            if (e.Item1 == EMainMenuState.INITIAL_MENU)
            {
                rainParticleSystem.Play();
                backgroundSpriteRenderer.color = mainTintShift;
                afterInitialMenu = true;
            }
        }

        #endregion
    }
}