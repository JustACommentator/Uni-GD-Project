using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.CameraSystem
{
    public class RPlayerCameraComponent : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private bool resetForeshadowing = true;
        [SerializeField] private float foreshadowingResetDelay = 0.05f;
        [SerializeField] private float foreshadowingResetDelta = 10f;
        [SerializeField] private float foreshadowingMoveDelta = 10f;
        [SerializeField] private float foreshadowingMoveRange = 1f;
        [Space]
        [SerializeField] private float foreshadowingMaximumDistanceFront = 1f;
        [SerializeField] private float foreshadowingMaximumDistanceBehind = 1f;
        [SerializeField] private float foreshadowingMaximumDistanceSide = 1f;
        
        [Header("References")]
        [SerializeField] private Transform cameraViewTarget = null;
        [Space]
        [SerializeField] private RPlayerMovement playerMovement = null;

        private Vector2 currentForeshadowingDirection = Vector2.zero;
        private float resetCooldown = 0f;

        private void Start()
        {
            playerMovement.OnMove += PlayerMovement_OnMove;
        }

        private void OnDestroy()
        {
            playerMovement.OnMove -= PlayerMovement_OnMove;
        }

        private void Update()
        {
            if (resetForeshadowing)
            {
                if (resetCooldown > 0f)
                    resetCooldown -= Time.deltaTime;
                else
                    currentForeshadowingDirection = Vector2.Lerp(currentForeshadowingDirection, Vector2.zero, Time.deltaTime * foreshadowingResetDelta);
            }
            
            cameraViewTarget.localPosition = Vector3.Lerp(cameraViewTarget.localPosition, 
                new Vector3(foreshadowingMaximumDistanceSide * currentForeshadowingDirection.x, 0f,
                (currentForeshadowingDirection.y > 0f ? foreshadowingMaximumDistanceFront : foreshadowingMaximumDistanceBehind) * currentForeshadowingDirection.y)
                * foreshadowingMoveRange, 
                Time.deltaTime * foreshadowingMoveDelta);
        }

        private void PlayerMovement_OnMove(object sender, Vector2 e)
        {
            resetCooldown = foreshadowingResetDelay;
            currentForeshadowingDirection = e.normalized;
        }
    }
}