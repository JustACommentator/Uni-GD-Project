using RuneProject.HitboxSystem;
using RuneProject.ItemSystem;
using RuneProject.UtilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    /// <summary>
    /// Script for autoattacks and picking up world items to use as attack sticks
    /// </summary>
    public class RPlayerBasicAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<Transform> handTransforms = new List<Transform>();
        [SerializeField] private Transform characterTransform = null;
        [SerializeField] private RHitboxComponent attackHitbox = null;
        [SerializeField] private GameObject autoAttackHitboxParent = null;
        [SerializeField] private Transform autoAttackHitboxScaler = null;
        [Space]
        [SerializeField] private GameObject autoAttackIndicatorParent = null;
        [SerializeField] private Transform autoAttackIndicatorTransform = null;
        [SerializeField] private Transform autoAttackStaffStartPointTransform = null;
        [SerializeField] private Transform autoAttackStaffEndPointTransform = null;
        [SerializeField] private List<LineRenderer> autoAttackLineRenderers = new List<LineRenderer>();
        [Space]
        [SerializeField] private RPlayerHealth playerHealth = null;
        [SerializeField] private RPlayerMovement playerMovement = null;
        [SerializeField] private RPlayerDash playerDash = null;

        [Header("Values")]
        [SerializeField] private bool tryToWallCheckAutoAttacks = false;
        [SerializeField] private LayerMask playerMask = new LayerMask();                
        [SerializeField] private LayerMask defaultMask = new LayerMask();                
        [SerializeField] private float attacksPerSecond = 2f;
        [SerializeField] private float attackRange = 8f;
        [SerializeField] private float autoAttackHitboxUptime = 0.5f;
        [Space]
        [SerializeField] private Vector2 throwAwayForce = new Vector2(30f, 10f);
        [SerializeField] private float hitboxUptime = 0.1f;
        [SerializeField] private float pickupDetectionRange = 0.7f;
        [SerializeField] private LayerMask worldItemMask = new LayerMask();
        [SerializeField] private bool cantPickUpOnAttackCooldown = false;
        [SerializeField] private bool cantDropOnAttackCooldown = false;
        [SerializeField] private bool cantUseWorldItemActiveEffectOnAttackCooldown = false;

        private float currentAttackCooldown = 0f;
        private float currentAttackChargeTime = 0f;
        private bool blockAttackInput = false;
        private bool blockPickupInput = false;
        private bool blockWorldItemInput = false;
        private bool chargingAttack = false;
        private int currentPickedUpWorldItemBeforeBreakCounter = 0;
        private RWorldItem currentPickedUpWorldItem = null;

        public event System.EventHandler OnBeginCharge;
        public event System.EventHandler OnEndCharge;
        public event System.EventHandler OnFireAutoAttack;

        private const int ATTACK_MOUSE_BUTTON = 1;
        private const float MIN_AUTO_ATTACK_CHARGE_TIME = 0.5f;
        private const float MAX_AUTO_ATTACK_CHARGE_TIME = 2f;
        private const float AUTO_ATTACK_STAND_TIME = 0.6f;
        private const float AUTO_ATTACK_HITBOX_WARMUP_TIME = 0.1f;
        private const float LIGHTNING_RANDOM_INTERVAL = 0.3f;
        private const float LIGHTNING_UPDATE_INTERVAL = 0.05f;
        private const float LIGHTNING_MIN_DISTANCE = 0.01f;
        private const KeyCode PICKUP_DROP_KEYCODE = KeyCode.F;
        private const KeyCode WORLD_ITEM_ACTIVE_EFFECT_KEYCODE = KeyCode.R;

        private void Start()
        {
            playerHealth.OnDeath += PlayerHealth_OnDeath;
            playerDash.OnDash += PlayerDash_OnDash;
        }
        
        private void OnDestroy()
        {
            playerHealth.OnDeath -= PlayerHealth_OnDeath;
        }

        void Update()
        {
            HandlePickups();
            HandleAttacks();
            HandleWorldItemActiveEffects();
        }

        private void HandlePickups()
        {
            if (Input.GetKeyDown(PICKUP_DROP_KEYCODE) && !blockPickupInput)
            {
                if (currentPickedUpWorldItem)
                    DropCurrentWorldItem();
                else
                    TryPickUpWorldItem();
            }
        }

        private void HandleAttacks()
        {
            if (currentAttackCooldown > 0f)
                currentAttackCooldown -= Time.deltaTime;
            else if (Input.GetMouseButtonDown(ATTACK_MOUSE_BUTTON) && !blockAttackInput)
            {
                if (!currentPickedUpWorldItem)
                {
                    chargingAttack = true;
                    currentAttackChargeTime = 0f;
                    autoAttackIndicatorTransform.localScale = new Vector3(0.3f, 0.3f, 0f);
                    autoAttackIndicatorParent.SetActive(true);
                    OnBeginCharge?.Invoke(this, null);
                }
                else if (!currentPickedUpWorldItem.CantBeUsedAsWeapon)
                {
                    StartCoroutine(IToggleHitbox());
                    currentAttackCooldown = 1f / currentPickedUpWorldItem.AttacksPerSecond;
                    //Später: Nur bei Hit
                    //currentPickedUpWorldItemBeforeBreakCounter--;

                    //if (currentPickedUpWorldItemBeforeBreakCounter <= 0)
                    //    BreakCurrentItem();
                }
            }
            else if (Input.GetMouseButton(ATTACK_MOUSE_BUTTON) && !blockAttackInput && chargingAttack)
            {
                currentAttackChargeTime += Time.deltaTime;

                if (!currentPickedUpWorldItem)
                {
                    autoAttackIndicatorTransform.localScale = new Vector3(0.3f, 0.3f, GetAutoAttackDistance());

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                    {
                        Vector3 dir = hit.point - transform.position;
                        dir.y = 0f;
                        autoAttackIndicatorTransform.LookAt(autoAttackIndicatorTransform.position + dir);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(ATTACK_MOUSE_BUTTON) && !blockAttackInput && chargingAttack)
            {
                chargingAttack = false;

                if (!currentPickedUpWorldItem)
                {
                    autoAttackIndicatorParent.SetActive(false);
                    OnEndCharge?.Invoke(this, null);

                    if (currentAttackChargeTime >= MIN_AUTO_ATTACK_CHARGE_TIME)
                    {
                        autoAttackHitboxScaler.localScale = new Vector3(1f, 1f, GetAutoAttackDistance()/4+0.4f);
                        StartCoroutine(IToggleAutoAttackHitbox());
                        playerMovement.ResetMovementMomentum();
                        playerMovement.LookAtMouse();
                        playerMovement.BlockMovementInput(AUTO_ATTACK_STAND_TIME);
                        OnFireAutoAttack?.Invoke(this, null);
                    }
                }
            }
        }

        private void HandleWorldItemActiveEffects()
        {
            if (Input.GetKeyDown(WORLD_ITEM_ACTIVE_EFFECT_KEYCODE) && currentPickedUpWorldItem &&
                (currentAttackCooldown <= 0f || !cantUseWorldItemActiveEffectOnAttackCooldown) && !blockWorldItemInput)
            {
                switch (currentPickedUpWorldItem.ActiveEffect)
                {
                    case EWorldItemActiveEffect.CONSUME:
                        {
                            playerHealth.Heal(gameObject, Mathf.RoundToInt(currentPickedUpWorldItem.ActiveEffectParameters[0]));
                            BreakCurrentItem();
                            break;
                        }
                    case EWorldItemActiveEffect.IGNITE:
                        {
                            //Zünde zeug an
                            break;
                        }
                    case EWorldItemActiveEffect.DRAW_ATTENTION:
                        {
                            RaycastHit[] hits = Physics.SphereCastAll(transform.position, currentPickedUpWorldItem.ActiveEffectParameters[0], transform.forward, float.MaxValue, ~playerMask);
                            //Für alle: Falls alertable / Gegner: Alerte auf transform.position
                            break;
                        }
                    case EWorldItemActiveEffect.REGENERATE_MANA:
                        {
                            //Stelle Mana wieder her
                            break;
                        }
                    case EWorldItemActiveEffect.PULL:
                        {
                            //Ziehe einen Gegner mit der Angelrute zum Spieler
                            break;
                        }
                    case EWorldItemActiveEffect.FREEZE_IN_RADIUS:
                        {
                            RaycastHit[] hits = Physics.SphereCastAll(transform.position, currentPickedUpWorldItem.ActiveEffectParameters[0], transform.forward, float.MaxValue, ~playerMask);
                            //Friere alle Gegner im Radius ein
                            break;
                        }
                }
            }
        }

        private void DropCurrentWorldItem(bool throwItem = true)
        {
            if (cantDropOnAttackCooldown && currentAttackCooldown > 0f) return;

            currentPickedUpWorldItem.transform.SetParent(null);
            currentPickedUpWorldItem.EnableAllColliders();
            currentPickedUpWorldItem.Rigidbody.isKinematic = false;
            attackHitbox.Damage = 0;
            if (throwItem)
                currentPickedUpWorldItem.Rigidbody.AddForce(characterTransform.forward * throwAwayForce.x + Vector3.up * throwAwayForce.y, ForceMode.VelocityChange);

            currentPickedUpWorldItem = null;
        }

        private void CancelAttackCharge()
        {
            chargingAttack = false;
            autoAttackIndicatorParent.SetActive(false);
            OnEndCharge?.Invoke(this, null);
        }

        private void TryPickUpWorldItem()
        {
            if (cantPickUpOnAttackCooldown && currentAttackCooldown > 0f) return;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, pickupDetectionRange, transform.forward, float.MaxValue, worldItemMask);
            RWorldItem nearestInRange = null;
            float nearestInRangeDistance = 0f;
            for (int i=0; i<hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent<RWorldItem>(out RWorldItem current) && !current.CantBePickedUp)
                {
                    float currentDistance = Vector3.Distance(transform.position, current.transform.position);

                    if ((currentDistance/2) <= current.PickupRange && (!nearestInRange || currentDistance < nearestInRangeDistance))
                    {
                        nearestInRange = current;
                        nearestInRangeDistance = currentDistance;
                    }
                }
            }

            if (nearestInRange)
            {
                currentPickedUpWorldItem = nearestInRange;
                currentPickedUpWorldItem.DisableAllColliders();
                currentPickedUpWorldItem.transform.SetParent(handTransforms[0]);
                currentPickedUpWorldItem.transform.localPosition = Vector3.zero;
                currentPickedUpWorldItem.Rigidbody.velocity = Vector3.zero;
                currentPickedUpWorldItem.Rigidbody.isKinematic = true;
                currentPickedUpWorldItemBeforeBreakCounter = currentPickedUpWorldItem.AttacksBeforeBeingBroken;
                attackHitbox.Damage = currentPickedUpWorldItem.AttackDamage;
            }
        }

        private void BreakCurrentItem()
        {
            GameObject worldItemObject = currentPickedUpWorldItem.gameObject;
            DropCurrentWorldItem(false);
            Destroy(worldItemObject);
        }

        private void SetLightningPositions()
        {
            Vector3 startPos = autoAttackStaffStartPointTransform.position;
            Vector3 endPos = autoAttackStaffEndPointTransform.position;
            Vector3 dir = (endPos - startPos).normalized;

            Vector3 raycastStartPos = transform.position + Vector3.up * autoAttackStaffStartPointTransform.localPosition.y;
            if (Physics.Raycast(raycastStartPos, dir, out RaycastHit hit, Vector3.Distance(raycastStartPos, endPos), defaultMask))
                endPos = hit.point;

            if (Vector3.Distance(startPos, endPos) < LIGHTNING_MIN_DISTANCE && tryToWallCheckAutoAttacks) return;

            for (int i=0; i< autoAttackLineRenderers.Count; i++)
            {
                int randomPoints = Random.Range(4, 8);              

                float segmentLength = Vector3.Distance(startPos, endPos) / randomPoints;
                List<Vector3> finalPositions = new List<Vector3>() { characterTransform.InverseTransformPoint(startPos) };

                for (int j=0; j<randomPoints; j++)
                {
                    Vector3 currentPos = startPos + (j + 1) * segmentLength * dir + Random.insideUnitSphere * LIGHTNING_RANDOM_INTERVAL;
                    finalPositions.Add(characterTransform.InverseTransformPoint(currentPos));
                }

                finalPositions.Add(characterTransform.InverseTransformPoint(endPos));

                autoAttackLineRenderers[i].positionCount = randomPoints + 2;
                autoAttackLineRenderers[i].SetPositions(finalPositions.ToArray());
            }
        }

        private float GetAutoAttackDistance()
        {
            if (currentAttackChargeTime < MIN_AUTO_ATTACK_CHARGE_TIME)
                return 0f;

            return Mathf.Clamp(Mathf.Lerp(0f, attackRange, (currentAttackChargeTime-MIN_AUTO_ATTACK_CHARGE_TIME) / MAX_AUTO_ATTACK_CHARGE_TIME), 0f, attackRange);            
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, pickupDetectionRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f + transform.forward * LIGHTNING_MIN_DISTANCE);
        }

        private void PlayerHealth_OnDeath(object sender, GameObject e)
        {
            blockAttackInput = true;
            blockPickupInput = true;
            blockWorldItemInput = true;
        }

        private void PlayerDash_OnDash(object sender, System.EventArgs e)
        {
            CancelAttackCharge();
        }

        private IEnumerator IToggleHitbox()
        {
            attackHitbox.gameObject.SetActive(true);
            yield return new WaitForSeconds(hitboxUptime);
            attackHitbox.gameObject.SetActive(false);
        }

        private IEnumerator IToggleAutoAttackHitbox()
        {
            yield return new WaitForSeconds(AUTO_ATTACK_HITBOX_WARMUP_TIME);            
            autoAttackHitboxParent.SetActive(true);
            StartCoroutine(IChangeLightnings());
            yield return new WaitForSeconds(autoAttackHitboxUptime);
            autoAttackHitboxParent.SetActive(false);
        }

        private IEnumerator IChangeLightnings()
        {
            while (autoAttackHitboxParent.activeInHierarchy)
            {
                SetLightningPositions();
                yield return new WaitForSeconds(LIGHTNING_UPDATE_INTERVAL);
            }
        }
    }
}