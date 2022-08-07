using RuneProject.UserInterfaceSystem;
using UnityEngine;
using UnityEngine.Video;

namespace RuneProject.MainMenuSystem
{
    public class RPreTutorialHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VideoPlayer player = null;
        [Space]
        [SerializeField] private Transform canvasTransform = null;
        [SerializeField] private RLevelTransition loadingScreenPrefab = null;

        private bool started = false;
        private bool loading = false;

        private const string TUTORIAL_NAME = "Tutorial";

        private void Update()
        {
            if (!started && player.isPlaying)
                started = true;
            else if (started && !loading && (!player.isPlaying || Input.GetKeyDown(KeyCode.Escape)))
            {
                loading = true;
                player.Stop();
                RLevelTransition transition = Instantiate(loadingScreenPrefab, canvasTransform);
                transition.LoadScene(TUTORIAL_NAME);
            }
        }
    }
}