using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuneProject.UserInterfaceSystem
{
    public class RLevelTransition : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private bool loadOnStart = false;
        [SerializeField] private string targetSceneName = "SampleScene";                

        private bool isLoading = false;

        private void Start()
        {
            if (loadOnStart)
                LoadScene();
        }

        public void LoadScene()
        {
            if (!isLoading)
            {
                isLoading = true;

                StartCoroutine(ILoadScene());
            }
        }

        public void LoadScene(string sceneName)
        {
            targetSceneName = sceneName;
            LoadScene();
        }

        private IEnumerator ILoadScene()
        {
            transform.SetAsLastSibling();
            DontDestroyOnLoad(gameObject);
            AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName);

            while (!op.isDone)
            {
                yield return null;
            }

            Destroy(gameObject, 0.1f);
        }
    }
}