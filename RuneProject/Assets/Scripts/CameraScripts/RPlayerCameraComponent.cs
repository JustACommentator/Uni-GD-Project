using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.CameraSystem
{
    public class RPlayerCameraComponent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraShakeParent = null;

        private int currentShakePriority = -1;
        private Coroutine currentShakeRoutine = null;

        /// <summary>
        /// Work In Progress, Don't Use!
        /// </summary>
        /// <param name="intensity"></param>
        /// <param name="magnitude"></param>
        /// <param name="swayPower"></param>
        /// <param name="time"></param>
        /// <param name="priority"></param>
        [System.Obsolete("Work In Progress, Don't Use")]
        public void Shake(float intensity, float magnitude, float swayPower, float time, int priority)
        {
            if (priority < currentShakePriority) return;

            if (currentShakeRoutine != null)
                StopCoroutine(currentShakeRoutine);

            currentShakeRoutine = StartCoroutine(IShakeCamera(intensity, magnitude, swayPower, time));
        }

        private IEnumerator IShakeCamera(float intensity, float magnitude, float swayPower, float time)
        {
            float timeLeft = time;
            float magnitudeTime = 1f / magnitude;
            float magnitudeTimeLeft = 0f;
            Vector3 currentTargetEulers = Vector3.zero;

            while (timeLeft > 0f)
            {
                magnitudeTimeLeft -= Time.deltaTime;
                timeLeft -= Time.deltaTime;

                if (magnitudeTimeLeft <= 0f)
                {
                    magnitudeTimeLeft += magnitudeTime;
                    currentTargetEulers = Random.insideUnitSphere * intensity;
                    currentTargetEulers = new Vector3(Mathf.Abs(currentTargetEulers.x), Mathf.Abs(currentTargetEulers.y), Mathf.Abs(currentTargetEulers.z));
                }

                cameraShakeParent.localEulerAngles += (currentTargetEulers- cameraShakeParent.localEulerAngles) * Time.deltaTime * swayPower;

                yield return null;
            }

            timeLeft = 0.3f;
            while (timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
                cameraShakeParent.localEulerAngles = Vector3.Lerp(cameraShakeParent.localEulerAngles, Vector3.zero, 1f - (timeLeft/0.3f));
                yield return null;
            }

            cameraShakeParent.localEulerAngles = Vector3.zero;

            currentShakePriority = -1;
            currentShakeRoutine = null;
        }
    }
}