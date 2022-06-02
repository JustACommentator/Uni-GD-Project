using RuneProject.CameraSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerDash : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private float dashCooldown = 1f;
        [SerializeField] private float dashPower = 20f;
        [SerializeField] private float movementBlockTime = 0.4f;

        [Header("References")]
        [SerializeField] private RPlayerMovement movement = null;
        [SerializeField] private RPlayerHealth playerHealth = null;
        [SerializeField] private Transform playerCharacterTransform = null;
        [SerializeField] private RPlayerCameraComponent cameraComponent = null;
        [Space]
        [SerializeField] private Transform dashDirectionIndicator = null;
        [SerializeField] private GameObject dashTrailIndicator = null;

        public event System.EventHandler OnDash;

        private const KeyCode DASH_INPUT = KeyCode.LeftShift;

        private float currentDashCooldown = 0f;
        private bool isDead = false;

        private void Start()
        {
            playerHealth.OnDeath += PlayerHealth_OnDeath;
        }

        private void PlayerHealth_OnDeath(object sender, GameObject e)
        {
            isDead = true;
            Destroy(dashDirectionIndicator.gameObject);
        }

        private void Update()
        {
            HandleDashCooldown();
            HandleDash();
            HandleIndicator();
        }

        private void HandleDashCooldown()
        {
            if (currentDashCooldown > 0f)
                currentDashCooldown -= Time.deltaTime;
        }

        private void HandleDash()
        {
            if (Input.GetKeyDown(DASH_INPUT) && CanDash())
            {
                currentDashCooldown += dashCooldown + movementBlockTime;
                Vector3 dir = playerCharacterTransform.forward;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))                
                    dir = hit.point - transform.position;                

                dir.y = 0f;
                dir.Normalize();

                StartCoroutine(IDisableTrailAfter());
                movement.BlockMovementInput(movementBlockTime);
                movement.ResetMovementMomentum();
                movement.LookAtMouse();
                movement.AddImpulse(dir * dashPower);
                //cameraComponent.Shake(5f, 2f, 6f, 0.2f, 0);
                OnDash?.Invoke(this, null);
            }
        }

        private void HandleIndicator()
        {
            if (isDead) return;

            Vector3 dir = playerCharacterTransform.forward;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                dir = hit.point - transform.position;

            dir.y = 0f;
            dir.Normalize();

            dashDirectionIndicator.rotation = Quaternion.LookRotation(dir);
        }

        private bool CanDash()
        {
            return currentDashCooldown <= 0f;
        }

        private IEnumerator IDisableTrailAfter()
        {
            dashTrailIndicator.SetActive(true);
            yield return new WaitForSeconds(movementBlockTime);
            dashTrailIndicator.SetActive(false);
        }
    }
}