using UnityEngine;

namespace RuneProject.UtilitySystem
{
    public class RDestroyMeAfterSeconds : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private float destroyAfter = 1f;

        private void Start()
        {
            Destroy(gameObject, destroyAfter);
        }
    }
}