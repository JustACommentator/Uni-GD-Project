using System.Collections;
using UnityEngine;

namespace GravityProject.CameraSystem
{
    public class PlayerCameraComponent : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private Vector2 mouseSensitivity = Vector2.zero;
        [SerializeField] private bool invertYAxis = false;
        [SerializeField] private Vector2 cameraVerticalLimits = new Vector2(-89f, 89f);
        [Space]
        [SerializeField] private float cameraDistance = 4.5f;
        [SerializeField] private float cameraShoulderHeight = 1f;
        [SerializeField] [Range(0f, 1f)] private float cameraPercentageToNear = 0.75f;
        [Space]
        [SerializeField] private bool enableCameraCollision = false;
        [SerializeField] private bool enableCameraDollying = false;
        [SerializeField] private LayerMask ignoreCameraCollisionLayerMask = new LayerMask();
        [SerializeField] private float cameraColliderSize = 0.5f;
        [SerializeField] private float cameraDollyDelta = 10f;

        [Header("References")]
        [SerializeField] private Transform cameraRotationCenter = null;
        [SerializeField] private Transform cameraHolderTransform = null;
        [SerializeField] private Transform characterTransform = null;

        private Vector2 currentCameraAngles = Vector2.zero;
        private Vector2 defaultCameraAngles = Vector2.zero;
        private Coroutine currentReturnToDefaultAnglesRoutine = null;
        private int currentBlockState = 0;

        private const string INPUT_AXIS_MOUSE_X = "Mouse X";
        private const string INPUT_AXIS_MOUSE_Y = "Mouse Y";
        private const string INPUT_BUTTON_RESET_CAMERA = "Reset Camera";
        private const float INPUT_THRESHOLD = 0.05f;
        private const float ANGLE_WRAP_AROUND = 180f;
        private const float LERP_BACK_SPEED = 20f;

        private void Start()
        {
            StartCoroutine(IResetCameraAfterFrame());
        }

        private void Update()
        {
            HandleCameraMovement();
            HandleCameraDistance();
            HandleCameraReset();
        }

        private void FixedUpdate()
        {
            HandleCameraCollision();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cameraHolderTransform.position, cameraColliderSize);
        }

        private void HandleCameraMovement()
        {
            if (currentBlockState > 0) return;

            Vector2 input = new Vector2(Input.GetAxis(INPUT_AXIS_MOUSE_Y) * (invertYAxis ? -1f : 1f), Input.GetAxis(INPUT_AXIS_MOUSE_X));

            float magnitude = input.magnitude;

            if (input == Vector2.zero || magnitude < INPUT_THRESHOLD) return;

            if (magnitude > 1f)
                input.Normalize();

            currentCameraAngles += input * mouseSensitivity * Time.deltaTime;

            #region VerticalLimits
            float x = currentCameraAngles.x < -ANGLE_WRAP_AROUND ? currentCameraAngles.x + 360f : currentCameraAngles.x > ANGLE_WRAP_AROUND ? currentCameraAngles.x - 360f : currentCameraAngles.x;
            x = Mathf.Clamp(x, cameraVerticalLimits.x, cameraVerticalLimits.y);
            currentCameraAngles.x = x;
            #endregion

            cameraRotationCenter.localEulerAngles = currentCameraAngles;
        }

        private void HandleCameraReset()
        {
            if (Input.GetButtonDown(INPUT_BUTTON_RESET_CAMERA) && currentReturnToDefaultAnglesRoutine == null)
                currentReturnToDefaultAnglesRoutine = StartCoroutine(ILerpCameraToDefaultPosition());
        }

        private void HandleCameraDistance()
        {
            float y = cameraShoulderHeight;
            float a = cameraVerticalLimits.y * cameraPercentageToNear;

            if (currentCameraAngles.x > a)
            {
                float rest = cameraVerticalLimits.y - a;
                float lerp = (rest - (cameraVerticalLimits.y - currentCameraAngles.x)) / rest;
                y = Mathf.Lerp(cameraShoulderHeight, 0f, lerp);
            }
            if (y != cameraHolderTransform.localPosition.y)
                cameraHolderTransform.localPosition = new Vector3(cameraHolderTransform.localPosition.x, y, cameraHolderTransform.localPosition.z);            
        }

        private void HandleCameraCollision()
        {
            if (!enableCameraCollision) return;

            if (Physics.CheckSphere(cameraHolderTransform.position, cameraColliderSize, ~ignoreCameraCollisionLayerMask))
            {
                if (!enableCameraDollying) return;

                Vector3 startPos = transform.position + Vector3.up;

                if (Physics.Raycast(startPos, cameraHolderTransform.position - startPos, out RaycastHit hit, cameraDistance, ~ignoreCameraCollisionLayerMask))
                {
                    Vector3 targetPos = hit.point;
                    startPos.y = targetPos.y;

                    cameraHolderTransform.position = targetPos;
                }
            }
            else if (Vector3.Distance(cameraHolderTransform.position, transform.position + Vector3.up) > -cameraDistance)
            {
                cameraHolderTransform.localPosition = new Vector3(cameraHolderTransform.localPosition.x, cameraHolderTransform.localPosition.y, Mathf.Lerp(cameraHolderTransform.localPosition.z, -cameraDistance, Time.fixedDeltaTime * cameraDollyDelta));
            }
        }

        /// <summary>
        /// Setzt die Kameraansicht auf den Standard (Hinter dem Spieler) zurück.
        /// </summary>
        public void ResetCameraAngles()
        {
            currentCameraAngles = defaultCameraAngles;
            cameraRotationCenter.localEulerAngles = currentCameraAngles;
        }

        /// <summary>
        /// Fügt eine Block-Ebene für das Bewegen der Kamera hinzu.
        /// Nur wenn alle Block-Ebenen von der Kamera entfernt sind, kann diese bewegt werden.
        /// </summary>
        public void BlockCameraMovement()
        {
            currentBlockState++;
        }

        /// <summary>
        /// Löst eine Block-Ebene für das Bewegen der Kamera.
        /// Nur wenn alle Block-Ebenen von der Kamera entfernt sind, kann diese bewegt werden.
        /// </summary>
        public void UnblockCameraMovement()
        {
            currentBlockState = Mathf.Max(0, currentBlockState - 1);
        }

        private IEnumerator IResetCameraAfterFrame()
        {
            yield return new WaitForSeconds(0.1f); //Bei Start
            ResetCameraAngles();
        }

        private IEnumerator ILerpCameraToDefaultPosition()
        {
            BlockCameraMovement();

            float timer = 0.5f;
            Vector3 target = Quaternion.LookRotation(characterTransform.forward).eulerAngles;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                currentCameraAngles = Vector2.Lerp(currentCameraAngles, target, Time.deltaTime * LERP_BACK_SPEED);
                cameraRotationCenter.localEulerAngles = currentCameraAngles;
                yield return null;
            }

            UnblockCameraMovement();
            currentReturnToDefaultAnglesRoutine = null;
        }
    }
}