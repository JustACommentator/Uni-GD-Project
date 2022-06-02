using RuneProject.ActorSystem;
using RuneProject.EnvironmentSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_GameOver : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerHealth playerHealth = null;
        [SerializeField] private CanvasGroup gameOverCanvas = null;

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

        public void OnClick_CloseApplication()
        {
            Application.Quit();
        }

        private IEnumerator IFadeInGameOverScreen()
        {
            gameOverCanvas.gameObject.SetActive(true);
            gameOverCanvas.alpha = 0f;

            yield return new WaitForSeconds(3.7f);

            while (gameOverCanvas.alpha < 1f)
            {
                gameOverCanvas.alpha += Time.deltaTime * 0.5f;
                yield return null;
            }
        }
    }
}