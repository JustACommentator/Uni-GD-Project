using RuneProject.ActorSystem;
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
        [SerializeField] private GameObject endOfVSliceParent = null;
        [Space]
        [SerializeField] private RLevelTransition loadingScreenPrefab = null;

        private bool isWaitingForEndOfVSliceInput = false;
        private bool blockPlayerPause = false;
        private bool isLoading = false;

        private const string MENU_LEVEL = "MainMenu";

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isWaitingForEndOfVSliceInput)
            {
                LoadScene(MENU_LEVEL);   
            }
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
            soundHandler.Laugh();
        }
    }
}