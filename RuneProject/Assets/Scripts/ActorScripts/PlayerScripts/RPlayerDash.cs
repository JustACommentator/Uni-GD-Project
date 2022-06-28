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
        [SerializeField] private bool dashInWalkingDirection = false;

        [Header("References")]
        [SerializeField] private RPlayerMovement movement = null;
        [SerializeField] private RPlayerHealth playerHealth = null;
        [SerializeField] private RPlayerBasicAttack basicAttack = null;
        [SerializeField] private Transform playerCharacterTransform = null;
        [SerializeField] private RPlayerCameraComponent cameraComponent = null;
        [Space]
        [SerializeField] private Transform dashDirectionIndicator = null;
        [SerializeField] private GameObject dashTrailIndicator = null;

        public event System.EventHandler OnDash;

        private const KeyCode DASH_INPUT = KeyCode.LeftShift;

        private float currentDashCooldown = 0f;
        private float baseDashPower = 0f;
        private float currentAdditionalDashPower = 0f;
        private bool isDead = false;

        public float CurrentDashCooldown { get => currentDashCooldown; set => currentDashCooldown = value; }
        public bool DashInWalkingDirection { get => dashInWalkingDirection; set => dashInWalkingDirection = value; }
        public float CurrentAdditionalDashPower { get => currentAdditionalDashPower; set { currentAdditionalDashPower = value; dashPower = baseDashPower * (1f + currentAdditionalDashPower); } }

        private void Start()
        {
            baseDashPower = dashPower;
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

                StartCoroutine(IDisableTrailAfter());
                basicAttack.ForceDisableAllHitboxes();
                movement.BlockMovementInput(movementBlockTime);
                movement.ResetMovementMomentum();
                if (!dashInWalkingDirection)
                {
                    movement.LookAtMouse();
                    movement.AddImpulse(movement.MouseDirection * dashPower);
                }
                else
                {
                    movement.AddImpulse(movement.Forward * dashPower);
                }
                //cameraComponent.Shake(5f, 2f, 6f, 0.2f, 0);
                OnDash?.Invoke(this, null);
            }
        }

        private void HandleIndicator()
        {
            if (isDead) return;

            dashDirectionIndicator.rotation = Quaternion.LookRotation(movement.MouseDirection);
        }

        private bool CanDash()
        {
            return currentDashCooldown <= 0f && movement.CanMove;
        }

        private IEnumerator IDisableTrailAfter()
        {
            dashTrailIndicator.SetActive(true);
            yield return new WaitForSeconds(movementBlockTime);
            dashTrailIndicator.SetActive(false);
        }
    }
}