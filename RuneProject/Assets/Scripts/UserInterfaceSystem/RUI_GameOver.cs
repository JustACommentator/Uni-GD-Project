using RuneProject.ActorSystem;
using RuneProject.EnvironmentSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_GameOver : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerHealth playerHealth = null;
        [SerializeField] private CanvasGroup gameOverCanvas = null;
        [SerializeField] private CanvasGroup vignette = null;
        [Space]
        [SerializeField] private TMP_Text gameOverDeathCauseText = null;
        [SerializeField] private GameObject nextLevelButton = null;
        [Space]
        [SerializeField] private RLevelTransition levelTransitionPrefab = null;

        private bool isLoading = false;

        private void Start()
        {
            playerHealth.OnDeath += PlayerHealth_OnDeath;
            if (SceneManager.GetActiveScene().buildIndex >= (SceneManager.sceneCountInBuildSettings - 1))
                nextLevelButton.SetActive(false);
        }

        private void OnDestroy()
        {
            playerHealth.OnDeath -= PlayerHealth_OnDeath;
        }

        private void PlayerHealth_OnDeath(object sender, GameObject e)
        {            
            gameOverDeathCauseText.text = e == null ? $"You were defeated." : $"Defeated by {e.name}";
            StartCoroutine(IFadeInGameOverScreen());
        }

        public void OnClick_Respawn()
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            FindObjectOfType<RPlayerSpawnPoint>().SpawnPlayer();
        }

        public void OnClick_ReloadScene()
        {
            if (!isLoading)
            {
                isLoading = true;
                RLevelTransition instance = Instantiate(levelTransitionPrefab, transform);
                instance.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public void OnClick_NextLevel()
        {
            if (!isLoading)
            {
                isLoading = true;
                RLevelTransition instance = Instantiate(levelTransitionPrefab, transform);
                instance.LoadScene(SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex + 1).name);
            }
        }

        public void OnClick_MainMenu()
        {
            if (!isLoading)
            {
                isLoading = true;
                RLevelTransition instance = Instantiate(levelTransitionPrefab, transform);
                instance.LoadScene(SceneManager.GetSceneAt(0).name);
            }
        }

        public void OnClick_CloseApplication()
        {
            Application.Quit();
        }

        private IEnumerator IFadeInGameOverScreen()
        {
            vignette.gameObject.SetActive(true);
            gameOverCanvas.gameObject.SetActive(true);
            gameOverCanvas.alpha = 0f;

            yield return new WaitForSeconds(4f);

            while (gameOverCanvas.alpha < 1f)
            {
                gameOverCanvas.alpha += Time.deltaTime * 0.6f;
                vignette.alpha -= Time.deltaTime * 0.6f;
                yield return null;
            }

            vignette.gameObject.SetActive(false);
        }
    }
}