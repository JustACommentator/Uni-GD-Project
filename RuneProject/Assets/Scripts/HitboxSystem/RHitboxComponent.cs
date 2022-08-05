using RuneProject.ActorSystem;
using RuneProject.UtilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.HitboxSystem
{
    public class RHitboxComponent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected GameObject owner;

        [Header("Values")]
        [SerializeField] protected int damage = 0;
        [SerializeField] protected bool isMultihitHitbox = false;
        [SerializeField] protected bool resetDamageInstanceOnTriggerExit = false;
        [SerializeField] protected float maxDamageInstancesPerSecond = 1f;
        [SerializeField] protected Vector3 knockback = Vector3.zero;

        protected Dictionary<Collider, Tuple<RPlayerHealth, float>> damageDictionary = new Dictionary<Collider, Tuple<RPlayerHealth, float>>();

        public event EventHandler<RPlayerHealth> OnHitTarget;

        public GameObject Owner { get => owner; set => owner = value; }
        public int Damage { get => damage; set => damage = value; }
        public bool IsMultihitHitbox { get => isMultihitHitbox; set => isMultihitHitbox = value; }
        public bool ResetDamageInstanceOnTriggerExit { get => resetDamageInstanceOnTriggerExit; set => resetDamageInstanceOnTriggerExit = value; }
        public float MaxDamageInstancesPerSecond { get => maxDamageInstancesPerSecond; set => maxDamageInstancesPerSecond = value; }
        public Vector3 Knockback { get => knockback; set => knockback = value; }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (other.gameObject == owner) return;

            if (damageDictionary.ContainsKey(other))
            {
                Tuple<RPlayerHealth, float> currentTuple = damageDictionary[other];
                if (Time.time >= currentTuple.Item2)                
                    damageDictionary.Remove(other);                
            }

            if (!damageDictionary.ContainsKey(other))
            {
                if (other.TryGetComponent<RPlayerHealth>(out RPlayerHealth otherHealth))
                {
                    otherHealth.TakeDamage(owner, damage, RVectorUtility.ConvertKnockbackToWorldSpace(owner ? owner.transform.position : transform.position, other.transform.position, knockback));
                    damageDictionary.Add(other, new Tuple<RPlayerHealth, float>(otherHealth, Time.time + 1f / maxDamageInstancesPerSecond));
                    OnHitTarget?.Invoke(this, otherHealth);
                }
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (resetDamageInstanceOnTriggerExit && damageDictionary.ContainsKey(other))
                damageDictionary.Remove(other);
        }
    }
}