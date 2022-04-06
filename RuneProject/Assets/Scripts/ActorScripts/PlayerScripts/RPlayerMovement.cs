using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerMovement : MonoBehaviour
    {
        [Header("Values")]        
        [SerializeField] private float walkSpeed = 700f;
        [SerializeField] private float runSpeed = 1000f;
        [SerializeField] private float inputRunThreshold = 0.8f;
        [SerializeField] private bool alwaysRun = false;
        [Space]
        [SerializeField] private bool smoothTurnaround = false;
        [SerializeField] private float characterTurnaroundDelta = 30f;
        [Space]
        [SerializeField] private bool looseImpulseMomentumOnCollision = false;
        [SerializeField] private float baseGravityMultiplier = 1f;
        [SerializeField] private float groundCheckSize = 0.3f;
        [SerializeField] private Vector3 groundCheckOffset = Vector3.zero;
        [SerializeField] private LayerMask playerLayer = new LayerMask();

        [Header("References")]
        [SerializeField] private Transform cameraTransform = null;
        [SerializeField] private Transform characterParentTransform = null;
        [SerializeField] private Rigidbody playerRigidbody = null;

        private Vector3 currentDesiredMovement = Vector3.zero;
        private Vector3 currentImpulseMovement = Vector3.zero;
        private bool isGrounded = false;
        private float airTime = 0f;

        public event System.EventHandler<Vector2> OnMove;
        public event System.EventHandler OnLand;
        public event System.EventHandler OnLeaveGround;

        private const string INPUT_AXIS_HORIZONTAL = "Horizontal";
        private const string INPUT_AXIS_VERTICAL = "Vertical";
        private const float MAX_AIRTIME_MULTIPLIER = 30f;
        private const float AIRTIME_DELTA = 10f;
        private const float IMPACT_DECREASE_DELTA = 10f;
        private const float IMPULSE_THRESHOLD = 0.1f;

        public bool IsGrounded { get => isGrounded; }
        public Vector3 Forward { get => characterParentTransform.forward; }
        public Vector3 Right { get => characterParentTransform.right; }

        private void Update()
        {
            HandleMovementAndTurnaround();
        }

        private void FixedUpdate()
        {
            HandleGravity();
            HandleGroundCheck();
            HandleApplyMovement();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckSize);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (looseImpulseMomentumOnCollision && !collision.collider.GetComponent<Rigidbody>())
                currentImpulseMovement = Vector3.zero;
        }      

        private void HandleMovementAndTurnaround()
        {
            Vector2 input = new Vector2(Input.GetAxis(INPUT_AXIS_HORIZONTAL), Input.GetAxis(INPUT_AXIS_VERTICAL));

            if (playerRigidbody.velocity != Vector3.zero) playerRigidbody.velocity = Vector3.zero;

            if (input == Vector2.zero)
            {
                currentDesiredMovement = Vector3.zero;
                return;
            }

            float usedSpeed = alwaysRun || input.magnitude >= inputRunThreshold ? runSpeed : walkSpeed;

            input.Normalize();

            OnMove?.Invoke(this, input);

            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = cameraTransform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 targetDir = forward * input.y + right * input.x;

            currentDesiredMovement = usedSpeed * targetDir * Time.deltaTime;

            Quaternion targetRotation = smoothTurnaround ? Quaternion.Slerp(characterParentTransform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * characterTurnaroundDelta) : Quaternion.LookRotation(targetDir);
            characterParentTransform.rotation = targetRotation;
        }       

        private void HandleGravity()
        {
            playerRigidbody.AddForce(baseGravityMultiplier * Physics.gravity, ForceMode.Acceleration);
        }

        private void HandleApplyMovement()
        {
            if (currentImpulseMovement != Vector3.zero)
            {
                playerRigidbody.velocity += currentImpulseMovement;

                if (currentImpulseMovement.magnitude < IMPULSE_THRESHOLD)
                    currentImpulseMovement = Vector3.zero;
                else
                    currentImpulseMovement = Vector3.Lerp(currentImpulseMovement, Vector3.zero, Time.fixedDeltaTime * IMPACT_DECREASE_DELTA);
            }

            if (currentDesiredMovement.magnitude != 0f)
                playerRigidbody.MovePosition(transform.position + currentDesiredMovement);           
        }

        private void HandleGroundCheck()
        {
            bool wasGrounded = isGrounded;

            isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckSize, ~playerLayer);

            if (wasGrounded && !isGrounded)
                OnLeaveGround?.Invoke(this, null);
            else if (!wasGrounded && isGrounded)
            {
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 2f, ~playerLayer))
                    transform.position = hit.point;

                airTime = 0f;
                OnLand?.Invoke(this, null);
            }

            if (!isGrounded)
                airTime += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Fügt dieser Einheit Rückstoß zu.
        /// </summary>
        /// <param name="impulse">Der Impuls, der an diese Einheit weitergegeben wird</param>
        public void AddImpulse(Vector3 impulse)
        {
            AddImpulse(impulse.normalized, impulse.magnitude);
        }

        /// <summary>
        /// Fügt dieser Einheit Rückstoß zu.
        /// </summary>
        ///<param name="dir">Die Richtung des Rückstoßes</param>
        ///<param name="strength">Die Stärke des Rückstoßes</param>
        public void AddImpulse(Vector3 dir, float strength)
        {
            dir.Normalize();

            currentImpulseMovement += dir * strength;
        }
    }
}