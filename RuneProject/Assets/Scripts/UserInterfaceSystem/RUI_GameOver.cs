using RuneProject.ActorSystem;
using RuneProject.EnvironmentSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_GameOver : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerHealth playerHealth = null;
        [SerializeField] private GameObject gameOverCanvas = null;

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
            gameOverCanvas.SetActive(true);
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
    }
}