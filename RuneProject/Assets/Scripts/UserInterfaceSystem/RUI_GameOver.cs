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

        private void Start()
        {
            playerHealth.OnDeath += PlayerHealth_OnDeath;
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnClick_NextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void OnClick_MainMenu()
        {
            SceneManager.LoadScene(0);
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