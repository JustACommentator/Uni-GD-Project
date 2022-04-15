using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    /// <summary>
    /// Objects that can burn.
    /// </summary>
    public class RBurnableComponent : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private float timeToCatchFire = 1.5f;
        [SerializeField] private float burnTimeUntilDestroy = 5f;

        private float burnTimeLeft = 0f;
        private float timeUntilCatchFire = 0f;
        private bool cindering = false;
        private bool burning = false;

        private void Update()
        {
            if (cindering)
            {
                if (timeUntilCatchFire > 0f)
                    timeUntilCatchFire -= Time.deltaTime;
                else if (!burning)
                    CatchFire();
            }
            else if (burning)
            {
                if (burnTimeLeft > 0f)
                    burnTimeLeft -= Time.deltaTime;
                else
                    Destroy(gameObject);
            }
        }

        /// <summary>
        /// Starts cindering an object. If it stays like this for a certain time, it starts to burn.
        /// </summary>
        public void StartCinder()
        {
            timeUntilCatchFire = timeToCatchFire;
            cindering = true;
        }

        /// <summary>
        /// Cancels cindering of an object. If it stays cindering for a certain time, it starts to burn.
        /// </summary>
        public void CancelCinder()
        {
            timeUntilCatchFire = timeToCatchFire;
            cindering = false;
        }

        /// <summary>
        /// Once an object starts to burn, nothing can stop it (yet).
        /// </summary>
        public void CatchFire()
        {
            burnTimeLeft = burnTimeUntilDestroy;
            burning = true;
            cindering = false;
        }
    }
}