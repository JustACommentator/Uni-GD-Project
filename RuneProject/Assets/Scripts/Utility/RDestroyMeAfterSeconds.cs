using UnityEngine;

namespace RuneProject.UtilitySystem
{
    public class RDestroyMeAfterSeconds : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private float destroyAfter = 1f;
        [Space]
        [SerializeField] private bool shrinkOverLifetime = false;

        private float destructionTimer = 0f;
        private Vector3 startScale = Vector3.zero;

        private void Start()
        {
            Destroy(gameObject, destroyAfter);

            if (shrinkOverLifetime)
                startScale = transform.localScale;
        }

        private void Update()
        {
            if (shrinkOverLifetime)
            {
                destructionTimer += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, destructionTimer / destroyAfter);
            }
        }
    }
}