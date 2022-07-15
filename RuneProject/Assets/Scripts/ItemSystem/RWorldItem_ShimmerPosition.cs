using RuneProject.CameraSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ItemSystem
{
    public class RWorldItem_ShimmerPosition : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RWorldItem worldItem = null;
        [SerializeField] private ParticleSystem shimmerParticles = null;
        
        private RPlayerCameraComponent cameraComponent = null;
        private Transform parentTransform = null;

        private const float MOVE_DISTANCE = 1f;

        private void Start()
        {
            parentTransform = transform.parent;

            if (!worldItem)
                worldItem = GetComponentInParent<RWorldItem>();

            worldItem.OnPickUp += WorldItem_OnPickUp;
            worldItem.OnDrop += WorldItem_OnDrop;
        }

        private void WorldItem_OnDrop(object sender, GameObject e)
        {
            shimmerParticles.Play();
        }

        private void WorldItem_OnPickUp(object sender, GameObject e)
        {
            shimmerParticles.Stop();
        }

        private void LateUpdate()
        {
            HandleCameraReference();
            HandlePosition();
            HandleLookAtCamera();
        }

        private void HandleCameraReference()
        {
            if (!cameraComponent && RPlayerCameraComponent.Singleton)
                cameraComponent = RPlayerCameraComponent.Singleton;
        }

        private void HandlePosition()
        {
            if (cameraComponent)
                transform.position = parentTransform.position + (cameraComponent.VirtualCam.position - parentTransform.position).normalized * MOVE_DISTANCE;
        }

        private void HandleLookAtCamera()
        {
            if (cameraComponent)
                transform.LookAt(cameraComponent.VirtualCam);
        }
    }
}