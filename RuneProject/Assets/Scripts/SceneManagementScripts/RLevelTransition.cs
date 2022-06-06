using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuneProject.UserInterfaceSystem
{
    public class RLevelTransition : MonoBehaviour
    {
        [SerializeField] private string targetSceneName = "SampleScene";

        private bool isLoading = false;

        public void LoadScene()
        {
            if (!isLoading)
            {
                isLoading = true;

                StartCoroutine(ILoadScene());
            }
        }

        private IEnumerator ILoadScene()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName);

            while (!op.isDone)
            {
                yield return null;
            }
        }
    }
}