using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace RuneProject.UserInterfaceSystem
{
    public class RLevelTransition : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private bool loadOnStart = false;
        [SerializeField] private float minLoadTime = 3f;
        [SerializeField] private string targetSceneName = "SampleScene";

        [SerializeField] private AudioMixer mixer;

        private bool isLoading = false;

        private const float MAX_PROGRESS_WITHOUT_SCENE_ACTIVATION = 0.9f;

        private void Start()
        {
            if (loadOnStart)
                LoadScene();
        }

        public void LoadScene()
        {
            mixer.FindSnapshot("Loading").TransitionTo(1f);
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

            yield return null;

            AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName);
            op.allowSceneActivation = false;
            float time = 0f;

            while (op.progress < MAX_PROGRESS_WITHOUT_SCENE_ACTIVATION)
            {
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            while(time < minLoadTime)
            {
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            if (Time.timeScale != 1f)
                Time.timeScale = 1f;

            mixer.FindSnapshot("Default").TransitionTo(1f);

            op.allowSceneActivation = true;

            while (!op.isDone)
                yield return null;

            Destroy(gameObject);
        }
    }
}