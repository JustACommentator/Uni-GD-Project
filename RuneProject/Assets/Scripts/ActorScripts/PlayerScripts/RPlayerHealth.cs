using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RuneProject.ActorSystem
{
    public class RPlayerHealth : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody healthRigidbody = null;
        [SerializeField] private List<Renderer> characterRenderers = null;
        [SerializeField] private Material damageMaterial = null;
        [SerializeField] private GameObject shieldIndicator = null;

        [Header("Values")]
        [SerializeField] private bool isAI = true;
        [SerializeField] private int maxHealth = 10;
        [SerializeField] private float receivedKnockbackMultiplier = 1f;
     
        private int currentHealth = 0;
        private int currentShield = 0;
        private bool isAlive = false;
        private bool isInvincible = false;
        private NavMeshAgent agent = null;
        private Coroutine currentKnockbackRoutine = null;
        private Coroutine currentDamageMaterialRoutine = null;
        private Coroutine currentInvincibilityRoutine = null;

        public int MaxHealth { get => maxHealth; }
        public float ReceivedKnockbackMultiplier { get => receivedKnockbackMultiplier; }
        public int CurrentHealth { get => currentHealth; }
        public bool IsAlive { get => isAlive; }
        public int CurrentShield { get => currentShield; set { currentShield = value; if (shieldIndicator) { shieldIndicator.SetActive(currentShield > 0); } OnShieldChanged?.Invoke(this, currentShield); } }

        public event System.EventHandler<int> OnDamageTaken;
        public event System.EventHandler<int> OnHealReceived;
        public event System.EventHandler<int> OnGainAdditionalHealth;
        public event System.EventHandler<int> OnShieldChanged;
        public event System.EventHandler<GameObject> OnDeath;

        private const float MIN_KNOCKBACK_MAGNITUDE_TO_APPLY_KNOCKBACK_ON_DAMAGE_TAKEN = 0.1f;
        private const float MIN_KNOCKBACK_MAGNITUDE_TO_CONTINUE_DISABLING_NAVMESHAGENT = 0.01f;
        private const float MIN_KNOCKBACK_TIME = 0.5f;
        private const float DAMAGE_MATERIAL_DISPLAY_TIME = 0.15f;
        private const float ON_HIT_INVINCIBILITY = 0.2f;

        private void Start()
        {
            isAlive = true;
            currentHealth = maxHealth;

            if (isAI && healthRigidbody.TryGetComponent<NavMeshAgent>(out NavMeshAgent navmesh))
                agent = navmesh;
        }

        public void TakeDamage(GameObject source, int damage, Vector3 knockback, bool dontGiveInvincibility = false)
        {
            if (!isAlive || isInvincible)
                return;

            int shieldedDamage = Mathf.Clamp(currentShield - damage, 0, damage);
            CurrentShield -= shieldedDamage;
            currentHealth -= damage - shieldedDamage;

            if (currentDamageMaterialRoutine == null)
                currentDamageMaterialRoutine = StartCoroutine(IDisplayDamageMaterial());

            knockback *= receivedKnockbackMultiplier;
            if (knockback.magnitude > MIN_KNOCKBACK_MAGNITUDE_TO_APPLY_KNOCKBACK_ON_DAMAGE_TAKEN)
            {
                healthRigidbody.AddForce(knockback);

                if (isAI && agent)
                {
                    if (currentKnockbackRoutine != null)
                        StopCoroutine(currentKnockbackRoutine);

                    currentKnockbackRoutine = StartCoroutine(IDisableNavmeshAgentForKnockbackDuration());
                }
            }

            if (currentHealth < 1)            
                Die(source);            
            else if (!dontGiveInvincibility)
                SetInvincible(ON_HIT_INVINCIBILITY);

            OnDamageTaken?.Invoke(this, damage);
        }

        public void Heal(GameObject source, int heal)
        {
            if (!isAlive)
                return;
            currentHealth += heal;
            currentHealth = Mathf.Min(maxHealth, currentHealth);
            OnHealReceived?.Invoke(this, heal);
        }

        public void SetInvincible(float time)
        {
            if (currentInvincibilityRoutine != null)
                StopCoroutine(currentInvincibilityRoutine);

            currentInvincibilityRoutine = StartCoroutine(ISetInvincible(time));
        }

        public void ForceKill()
        {
            Die(null);
        }

        public void GainMaxHealth(int amount)
        {
            maxHealth += amount;
            OnGainAdditionalHealth?.Invoke(this, amount);
        }

        private void Die(GameObject source)
        {
            currentHealth = 0;
            isAlive = false;
            OnDeath?.Invoke(this, source);
        }

        private IEnumerator IDisableNavmeshAgentForKnockbackDuration()
        {
            agent.enabled = false;
            yield return new WaitForSeconds(MIN_KNOCKBACK_TIME);

            while (healthRigidbody.velocity.magnitude > MIN_KNOCKBACK_MAGNITUDE_TO_CONTINUE_DISABLING_NAVMESHAGENT)
                yield return null;

            agent.enabled = true;
        }

        private IEnumerator IDisplayDamageMaterial()
        {
            if (characterRenderers != null && characterRenderers.Count > 0)
            {
                List<Material[]> materials = new List<Material[]>();

                for (int i = 0; i < characterRenderers.Count; i++)
                {
                    materials.Add(characterRenderers[i].materials);
                    characterRenderers[i].materials = new Material[] { damageMaterial };
                }

                yield return new WaitForSeconds(DAMAGE_MATERIAL_DISPLAY_TIME);

                for (int i = 0; i < characterRenderers.Count; i++)
                    characterRenderers[i].materials = materials[i];
            }

            currentDamageMaterialRoutine = null;         
        }

        private IEnumerator ISetInvincible(float time)
        {
            isInvincible = true;
            yield return new WaitForSeconds(time);
            isInvincible = false;
        }
    }
}