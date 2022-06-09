using RuneProject.ActorSystem;
using RuneProject.ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_Main : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerMovement playerMovement = null;
        [SerializeField] private RPlayerSoundHandler soundHandler = null;
        [SerializeField] private RPlayerHealth playerHealth = null;
        [Space]
        [SerializeField] private GameObject endOfVSliceParent = null;
        [SerializeField] private GameObject pauseParent = null;
        [Space]
        [SerializeField] private RLevelTransition loadingScreenPrefab = null;

        private bool isWaitingForEndOfVSliceInput = false;
        private bool blockPlayerPause = false;
        private bool isLoading = false;
        private bool isPaused = false;
        private Coroutine currentPauseRoutine = null;

        public event System.EventHandler OnReachEnd;

        private const string MENU_LEVEL = "MainMenu";

        private void Start()
        {
            playerHealth.OnDeath += PlayerHealth_OnDeath;
        }

        private void PlayerHealth_OnDeath(object sender, GameObject e)
        {
            blockPlayerPause = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                if (isWaitingForEndOfVSliceInput)
                    LoadScene(MENU_LEVEL);
                else
                    Pause();
            }
        }

        private void Pause()
        {
            if (isPaused || blockPlayerPause) return;

            pauseParent.SetActive(true);
            isPaused = true;
            Time.timeScale = 0f;

            if (currentPauseRoutine != null)
                StopCoroutine(currentPauseRoutine);

            currentPauseRoutine = StartCoroutine(IPauseUpdate());
        }

        private void UnPause()
        {
            if (!isPaused) return;

            Time.timeScale = 1f;
            pauseParent.SetActive(false);
            isPaused = false;
        }       

        public void LoadScene(string levelName)
        {
            if (!isLoading)
            {
                isLoading = true;
                RLevelTransition instance = Instantiate(loadingScreenPrefab, transform);
                instance.LoadScene(levelName);
            }
        }

        public void ShowEndOfVSlice()
        {
            isWaitingForEndOfVSliceInput = true;
            blockPlayerPause = true;
            playerMovement.BlockMovementInput();
            endOfVSliceParent.SetActive(true);
            OnReachEnd?.Invoke(this, null);
        }

        private IEnumerator IPauseUpdate()
        {
            yield return new WaitForSecondsRealtime(0.0016f);

            while (isPaused)
            {
                if (Input.GetKeyDown(KeyCode.Escape))                
                    UnPause();
                
                yield return new WaitForSecondsRealtime(0.00016f);
            }
        }
    }
}