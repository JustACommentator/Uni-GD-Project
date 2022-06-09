using RuneProject.UserInterfaceSystem;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class RStairsComponent : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private bool showEndOfShowcaseScreen = false;
        [SerializeField] private string nextLevel = "MainMenu";

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                if (!showEndOfShowcaseScreen)
                {
                    GameObject.Find("LevelManager").transform.GetChild(0).gameObject.SetActive(false);
                    collision.collider.GetComponentInChildren<RUI_Main>().LoadScene(nextLevel);
                }
                else
                    collision.collider.GetComponentInChildren<RUI_Main>().ShowEndOfVSlice();
            }
        }
    }
}