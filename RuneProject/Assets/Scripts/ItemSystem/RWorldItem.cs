using RuneProject.LibrarySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ItemSystem
{
    /// <summary>
    /// Handles the items that can be found in the world.
    /// E.g. Books, Cheese, etc.
    /// </summary>
    public class RWorldItem : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private string csv_key = string.Empty;
        [SerializeField] private EWorldItemCategory itemCategory = EWorldItemCategory.NONE;
        [SerializeField] private List<EPlaceCondition> placeConditions = new List<EPlaceCondition>();
        [SerializeField] private Vector3 spawnOffset = Vector3.up;
        [SerializeField] private bool dontRotateOnInstantiate = false;
        [SerializeField] private bool dontTumbleOnInstantiate = false;
        [SerializeField] private bool cantBePickedUp = false;
        [SerializeField] private float pickupRange = 0.25f;

        [Header("Weapon Values")]
        [SerializeField] private bool cantBeUsedAsWeapon = false;
        [SerializeField] private bool isRangedWeapon = false;
        [SerializeField] private bool cantBeBroken = false;
        [SerializeField] private GameObject rangedProjectilePrefab = null;
        [SerializeField] private Vector3 rangedProjectileSpawnOffset = Vector3.forward;
        [SerializeField] private int attackDamage = 1;
        [SerializeField] private float attacksPerSecond = 2f;
        [SerializeField] private int attacksBeforeBeingBroken = 8;
        [Space]
        [SerializeField] private EWorldItemActiveEffect activeEffect = EWorldItemActiveEffect.NONE;
        [SerializeField] private List<float> activeEffectParameters = new List<float>();

        [Header("References")]
        [SerializeField] private List<Collider> colliders = new List<Collider>();
        [SerializeField] private List<MonoBehaviour> behavioursToDisableOnPreview = new List<MonoBehaviour>();
        [SerializeField] private GameObject placeholder = null;
        [SerializeField] private GameObject worldMesh = null;
        [SerializeField] private new Rigidbody rigidbody = null;

        private string itemName = string.Empty;
        private string itemDescription = string.Empty;
        private bool isPreview = false;

        public EWorldItemCategory ItemCategory { get => itemCategory; }
        public List<EPlaceCondition> PlaceConditions { get => placeConditions; }
        public Vector3 SpawnOffset { get => spawnOffset; }
        public Rigidbody Rigidbody { get => rigidbody; }
        public bool DontTumbleOnInstantiate { get => dontTumbleOnInstantiate; }
        public bool DontRotateOnInstantiate { get => dontRotateOnInstantiate; }
        public bool CantBePickedUp { get => cantBePickedUp; }
        public float PickupRange { get => pickupRange; }
        public bool CantBeUsedAsWeapon { get => cantBeUsedAsWeapon; }
        public bool IsRangedWeapon { get => isRangedWeapon; }
        public int AttackDamage { get => attackDamage; }
        public float AttacksPerSecond { get => attacksPerSecond; }
        public GameObject RangedProjectilePrefab { get => rangedProjectilePrefab; }
        public Vector3 RangedProjectileSpawnOffset { get => rangedProjectileSpawnOffset; }
        public EWorldItemActiveEffect ActiveEffect { get => activeEffect; }
        public List<float> ActiveEffectParameters { get => activeEffectParameters; }
        public int AttacksBeforeBeingBroken { get => attacksBeforeBeingBroken; }

        private void Awake()
        {
            ReadDataFromCSV();
        }

        private void ReadDataFromCSV()
        {
            Tuple<string, string> infos = RItemIdentifierLibrary.GetWorldItemInfos(csv_key);
            itemName = infos.Item1;
            itemDescription = infos.Item2;
        }

        /// <summary>
        /// Sets this World Item to be a preview.
        /// </summary>
        public void SetAsPreview()
        {
            isPreview = true;

            DisableAllColliders();

            for (int i = 0; i < behavioursToDisableOnPreview.Count; i++)
                behavioursToDisableOnPreview[i].enabled = false;

            placeholder.SetActive(true);
            worldMesh.SetActive(false);
        }

        public void EnableAllColliders()
        {
            for (int i = 0; i < colliders.Count; i++)
                colliders[i].enabled = true;
        }

        public void DisableAllColliders()
        {
            for (int i = 0; i < colliders.Count; i++)
                colliders[i].enabled = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRange);
        }
    }

    public enum EWorldItemCategory
    {
        NONE,
        EARTH,
        WATER,
        FIRE,
        AIR,
        FAUNA,
        FLORA
    }

    public enum EPlaceCondition
    {
        NO_PLAYER,
        NO_ENEMY,
        CAN_CLIP
    }

    public enum EWorldItemActiveEffect
    {
        NONE,
        CONSUME,
        IGNITE,
        DRAW_ATTENTION,
        REGENERATE_MANA,
        PULL,
        FREEZE_IN_RADIUS
    }
}