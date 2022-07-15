using RuneProject.CameraSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ItemSystem
{
    public class RWorldItemPickupTooltip : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RWorldItem worldItem = null;
        [SerializeField] private Transform tooltip = null;
        [SerializeField] private Collider checkTrigger = null;

        private Coroutine currentDisplayTooltipRoutine = null;
        private RPlayerCameraComponent cameraComponent = null;

        private const float LERP_TIME = 0.125f;
        private const float DEFAULT_HEIGHT = 0.4f;

        private void Start()
        {
            if (!worldItem)
                worldItem = GetComponentInParent<RWorldItem>();

            worldItem.OnPickUp += WorldItem_OnPickUp;
            worldItem.OnDrop += WorldItem_OnDrop;
        }

        private void Update()
        {
            HandleTooltipPosition();
        }

        private void LateUpdate()
        {
            HandleLookAtCamera();
        }

        private void OnDestroy()
        {
            if (worldItem)
            {
                worldItem.OnPickUp -= WorldItem_OnPickUp;
                worldItem.OnDrop += WorldItem_OnDrop;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EnableTooltip();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                DisableTooltip();
            }
        }

        private void HandleLookAtCamera()
        {
            if (!cameraComponent && RPlayerCameraComponent.Singleton)
                cameraComponent = RPlayerCameraComponent.Singleton;

            if (cameraComponent)
                tooltip.LookAt(cameraComponent.VirtualCam);
        }

        private void HandleTooltipPosition()
        {
            Vector3 targetPos = worldItem.transform.position + Vector3.up * DEFAULT_HEIGHT;

            if (transform.position != targetPos)
                transform.position = targetPos;
        }

        private void WorldItem_OnPickUp(object sender, GameObject e)
        {
            checkTrigger.enabled = false;
            DisableTooltip();
        }

        private void WorldItem_OnDrop(object sender, GameObject e)
        {
            StartCoroutine(IEnableColliderAfterTime());
        }

        private void DisableTooltip()
        {
            if (worldItem.CantBePickedUp) return;

            if (currentDisplayTooltipRoutine != null)
                StopCoroutine(currentDisplayTooltipRoutine);

            currentDisplayTooltipRoutine = StartCoroutine(IDisplayTooltipTransition(false));
        }

        private void EnableTooltip()
        {
            if (worldItem.CantBePickedUp) return;

            if (currentDisplayTooltipRoutine != null)
                StopCoroutine(currentDisplayTooltipRoutine);

            currentDisplayTooltipRoutine = StartCoroutine(IDisplayTooltipTransition(true));
        }

        private IEnumerator IDisplayTooltipTransition(bool enable)
        {
            float startScale = enable ? 0f : 1f;
            float endScale = 1f - startScale;
            float usedDelta = 1f / LERP_TIME;

            if (enable)
                tooltip.gameObject.SetActive(true);

            tooltip.localScale = new Vector3(startScale, startScale, 1f);

            float currentScale = startScale;
            while (tooltip.localScale.x != endScale)
            {
                currentScale = Mathf.Clamp01(currentScale + (startScale > endScale ? -1f : 1f) * usedDelta * Time.deltaTime);
                tooltip.localScale = new Vector3(currentScale, currentScale, 1f);

                yield return null;
            }

            if (!enable)
                tooltip.gameObject.SetActive(false);         
        }

        private IEnumerator IEnableColliderAfterTime()
        {
            yield return new WaitForSeconds(0.7f);
            checkTrigger.enabled = true;
        }
    }
}