using RuneProject.ActorSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.HitboxSystem
{
    public class RHitboxComponent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected GameObject owner = null;

        [Header("Values")]
        [SerializeField] protected int damage = 0;
        [SerializeField] protected bool isMultihitHitbox = false;
        [SerializeField] protected bool resetDamageInstanceOnTriggerExit = false;
        [SerializeField] protected float maxDamageInstancesPerSecond = 1f;

        protected Dictionary<Collider, Tuple<RPlayerHealth, float>> damageDictionary = new Dictionary<Collider, Tuple<RPlayerHealth, float>>();

        public int Damage { get => damage; set => damage = value; }
        public bool IsMultihitHitbox { get => isMultihitHitbox; set => isMultihitHitbox = value; }
        public bool ResetDamageInstanceOnTriggerExit { get => resetDamageInstanceOnTriggerExit; set => resetDamageInstanceOnTriggerExit = value; }
        public float MaxDamageInstancesPerSecond { get => maxDamageInstancesPerSecond; set => maxDamageInstancesPerSecond = value; }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (other.gameObject == owner) return;

            if (!damageDictionary.ContainsKey(other))
            {
                if (other.TryGetComponent<RPlayerHealth>(out RPlayerHealth otherHealth))
                {
                    otherHealth.TakeDamage(owner, damage);
                    damageDictionary.Add(other, new Tuple<RPlayerHealth, float>(otherHealth, Time.time + 1f / maxDamageInstancesPerSecond));
                }
            }
            else
            {
                Tuple<RPlayerHealth, float> currentTuple = damageDictionary[other];
                if (currentTuple.Item2 >= Time.time)
                    damageDictionary.Remove(other);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (resetDamageInstanceOnTriggerExit && damageDictionary.ContainsKey(other))
                damageDictionary.Remove(other);
        }
    }
}