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
        [SerializeField] private Transform virtualCam = null;
        [SerializeField] private Transform moveParent = null;

        [Header("Values")]
        [SerializeField] private Vector3 camOffset = Vector3.zero;
        [SerializeField] private Vector2 tilt = new Vector2(65f, 0f);
        [SerializeField] private Vector2 maxTiltDifference = Vector2.one;

        private int currentShakePriority = -1;
        private Coroutine currentShakeRoutine = null;
        private Coroutine currentMoveRoutine = null;
        private Transform playerTransform = null;

        private const float CAMERA_TRANSITION_TIME = 0.4f;
        private const float TILT_THRESHOLD = 0.8f;

        private static RPlayerCameraComponent singleton = null;

        public Transform VirtualCam { get => virtualCam; }
        public static RPlayerCameraComponent Singleton { get { if (singleton == null) singleton = FindObjectOfType<RPlayerCameraComponent>(); return singleton; } }     

        private void Update()
        {
            HandleFindPlayerTransform();
            HandleCameraTilt();
        }

        private void HandleCameraTilt()
        {
            Vector3 vp = (Camera.main.WorldToViewportPoint(playerTransform.position) - new Vector3(0.5f, 0.5f, 0f)) * 2f;

            moveParent.transform.localEulerAngles = new Vector3(tilt.x + maxTiltDifference.x * -vp.y, tilt.y + maxTiltDifference.y * vp.x, 0f);     
        }

        private void HandleFindPlayerTransform()
        {
            if (!playerTransform)
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

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

        public void EnterRoom(GameObject room)
        {
            if (currentMoveRoutine != null)
                StopCoroutine(currentMoveRoutine);

            currentMoveRoutine = StartCoroutine(IMoveToNewRoom(room.transform.position + camOffset));
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

        private IEnumerator IMoveToNewRoom(Vector3 targetPos)
        {
            Vector3 startPos = moveParent.position;
            float timePassed = 0f;
            while(moveParent.position != targetPos)
            {
                timePassed += Time.deltaTime;
                moveParent.position = Vector3.Lerp(startPos, targetPos, timePassed / CAMERA_TRANSITION_TIME);
                yield return null;
            }
        }
    }
}