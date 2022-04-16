using RuneProject.HitboxSystem;
using RuneProject.ItemSystem;
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
        [Space]
        [SerializeField] private RPlayerHealth playerHealth = null;

        [Header("Values")] 
        [SerializeField] private Vector3 offsetHitSphere = Vector3.zero;
        [SerializeField] private float range = 1.5f;
        [SerializeField] private LayerMask playerMask = new LayerMask();
        [SerializeField] private int damage = 1;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Vector3 offsetLROrigin = Vector3.zero;
        [SerializeField] private Vector3 offsetLRTarget = Vector3.zero;
        [SerializeField] private float attacksPerSecond = 2f;
        [Space]
        [SerializeField] private Vector2 throwAwayForce = new Vector2(30f, 10f);
        [SerializeField] private float hitboxUptime = 0.1f;
        [SerializeField] private float pickupDetectionRange = 0.7f;
        [SerializeField] private LayerMask worldItemMask = new LayerMask();
        [SerializeField] private bool cantPickUpOnAttackCooldown = false;
        [SerializeField] private bool cantDropOnAttackCooldown = false;
        [SerializeField] private bool cantUseWorldItemActiveEffectOnAttackCooldown = false;

        private float currentAttackCooldown = 0f;
        private int currentPickedUpWorldItemBeforeBreakCounter = 0;
        private RWorldItem currentPickedUpWorldItem = null;

        private const int ATTACK_MOUSE_BUTTON = 1;
        private const KeyCode PICKUP_DROP_KEYCODE = KeyCode.F;
        private const KeyCode WORLD_ITEM_ACTIVE_EFFECT_KEYCODE = KeyCode.R;

        void Update()
        {
            HandlePickups();
            HandleAttacks();
            HandleWorldItemActiveEffects();
        }

        private void HandlePickups()
        {
            if (Input.GetKeyDown(PICKUP_DROP_KEYCODE))
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
            else if (Input.GetMouseButtonDown(ATTACK_MOUSE_BUTTON))
            {
                if (!currentPickedUpWorldItem)
                {
                    RaycastHit[] hits = Physics.SphereCastAll(transform.position + offsetHitSphere, range, transform.forward, float.MaxValue, ~playerMask);
                    RPlayerHealth nearestTarget = null;
                    float nearestDistance = float.MaxValue;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].collider.TryGetComponent<RPlayerHealth>(out RPlayerHealth current))
                        {
                            if (hits[i].distance < nearestDistance)
                            {
                                nearestDistance = hits[i].distance;
                                nearestTarget = hits[i].collider.GetComponent<RPlayerHealth>();
                            }
                        }
                    }

                    if (nearestTarget != null)
                    {
                        StartCoroutine(IDrawLine(transform.position, nearestTarget.transform.position));
                        nearestTarget.TakeDamage(gameObject, damage);
                        currentAttackCooldown = 1f / attacksPerSecond;
                    }
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
        }

        private void HandleWorldItemActiveEffects()
        {
            if (Input.GetKeyDown(WORLD_ITEM_ACTIVE_EFFECT_KEYCODE) && currentPickedUpWorldItem &&
                (currentAttackCooldown <= 0f || !cantUseWorldItemActiveEffectOnAttackCooldown))
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + offsetHitSphere, range);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, pickupDetectionRange);
        }

        private IEnumerator IDrawLine(Vector3 a, Vector3 b)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, a + offsetLROrigin);
            lineRenderer.SetPosition(1, b + offsetLRTarget);
            yield return new WaitForSeconds(0.25f);
            lineRenderer.enabled = false;
        }

        private IEnumerator IToggleHitbox()
        {
            attackHitbox.gameObject.SetActive(true);
            yield return new WaitForSeconds(hitboxUptime);
            attackHitbox.gameObject.SetActive(false);
        }
    }
}