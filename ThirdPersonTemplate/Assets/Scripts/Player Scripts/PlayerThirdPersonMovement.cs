using UnityEngine;

namespace GravityProject.PlayerSystem
{
    /// <summary>
    /// Behandelt das Bewegen des Spieler-Charakters.
    /// </summary>
    public class PlayerThirdPersonMovement : MonoBehaviour
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
        [SerializeField] private int jumpCount = 0;
        [SerializeField] private float jumpForce = 90f;
        [Space]
        [SerializeField] private bool looseImpulseMomentumOnCollision = false;
        [SerializeField] private float baseGravityMultiplier = 1f;
        [SerializeField] private float currentGravityMultiplier = 1f;
        [SerializeField] private float groundCheckSize = 0.3f;
        [SerializeField] private Vector3 groundCheckOffset = Vector3.zero;
        [SerializeField] private LayerMask playerLayer = new LayerMask();

        [Header("References")]
        [SerializeField] private Transform cameraTransform = null;
        [SerializeField] private Transform characterParentTransform = null;
        [SerializeField] private Rigidbody playerRigidbody = null;

        private Vector3 currentGravityDirection = Vector3.down;
        private Vector3 currentDesiredMovement = Vector3.zero;
        private Vector3 currentImpulseMovement = Vector3.zero;
        private int currentJumpCount = 0;
        private bool isGrounded = false;
        private float airTime = 0f;
        private float currentJumpCooldown = 0f;

        public event System.EventHandler<Vector3> OnGravityDirectionChange;
        public event System.EventHandler OnLand;
        public event System.EventHandler OnLeaveGround;

        private const string INPUT_AXIS_HORIZONTAL = "Horizontal";
        private const string INPUT_AXIS_VERTICAL = "Vertical";
        private const string INPUT_BUTTON_JUMP = "Jump";
        private const float PLAYER_BASE_GRAVITY = 250.9f;
        private const float MAX_AIRTIME_MULTIPLIER = 30f;
        private const float AIRTIME_DELTA = 10f;
        private const float IMPULSE_THRESHOLD = 0.1f;
        private const float JUMP_COOLDOWN = 0.2f;

        public bool IsGrounded { get => isGrounded; }
        public Vector3 Forward { get => characterParentTransform.forward; }
        public Vector3 Right { get => characterParentTransform.right; }

        private void Update()
        {
            HandleMovementCooldowns();
            HandleJumping();
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

            if (input == Vector2.zero) return;

            float usedSpeed = alwaysRun || input.magnitude >= inputRunThreshold ? runSpeed : walkSpeed;

            input.Normalize();

            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = cameraTransform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 targetDir = forward * input.y + right * input.x;

            currentDesiredMovement += Time.deltaTime * usedSpeed * targetDir;

            Quaternion targetRotation = smoothTurnaround ? Quaternion.Slerp(characterParentTransform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * characterTurnaroundDelta) : Quaternion.LookRotation(targetDir);
            characterParentTransform.rotation = targetRotation;
        }

        private void HandleJumping()
        {
            if (Input.GetButtonDown(INPUT_BUTTON_JUMP) && currentJumpCount > 0 && currentJumpCooldown <= 0f)
            {
                if (!IsGrounded)
                {
                    currentDesiredMovement.y = 0f;
                    playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);
                }

                currentJumpCount--;
                currentJumpCooldown += JUMP_COOLDOWN;
                AddImpulse(new Vector3(0f, jumpForce, 0f));
            }
        }

        private void HandleGravity()
        {
            if (currentGravityMultiplier != 0f)
                currentDesiredMovement += currentGravityDirection * PLAYER_BASE_GRAVITY * baseGravityMultiplier * currentGravityMultiplier * Time.fixedDeltaTime * 
                    Mathf.Max(1f, Mathf.Min(MAX_AIRTIME_MULTIPLIER, airTime * AIRTIME_DELTA));            
        }        

        private void HandleApplyMovement()
        {
            if (currentDesiredMovement.magnitude != 0f)
            {
                playerRigidbody.velocity += currentDesiredMovement + currentImpulseMovement;

                currentDesiredMovement = Vector3.zero;

                if (currentImpulseMovement != Vector3.zero)
                {
                    if (currentImpulseMovement.magnitude < IMPULSE_THRESHOLD)
                        currentImpulseMovement = Vector3.zero;
                    else
                        currentImpulseMovement = Vector3.Lerp(currentImpulseMovement, Vector3.zero, Time.fixedDeltaTime * 10f);
                }
            }
        }

        private void HandleGroundCheck()
        {
            bool wasGrounded = isGrounded;

            isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckSize, ~playerLayer);

            if (wasGrounded && !isGrounded)            
                OnLeaveGround?.Invoke(this, null);            
            else if (!wasGrounded && isGrounded)
            {
                currentJumpCount = jumpCount;

                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 2f, ~playerLayer))                
                    transform.position = hit.point;                

                airTime = 0f;
                OnLand?.Invoke(this, null);
            }

            if (!isGrounded)
                airTime += Time.fixedDeltaTime;
        }

        private void HandleMovementCooldowns()
        {
            if (currentJumpCooldown > 0f)
                currentJumpCooldown -= Time.deltaTime;
        }

        /// <summary>
        /// Verändert die Gravitationsrichtung zur angegebenen Richtung.
        /// </summary>
        /// <param name="newDir">Die neue Richtung der Gravitation für den Spieler (wird normalisiert)</param>
        public void PlayerChangeGravityDirection(Vector3 newDir)
        {
            newDir.Normalize();
            currentGravityDirection = newDir;
            OnGravityDirectionChange?.Invoke(this, newDir);
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