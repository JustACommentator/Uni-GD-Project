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

        [Header("References")]
        [SerializeField] private List<Collider> colliders = new List<Collider>();
        [SerializeField] private List<MonoBehaviour> behavioursToDisableOnPreview = new List<MonoBehaviour>();
        [SerializeField] private GameObject placeholder = null;
        [SerializeField] private GameObject worldMesh = null;
        [SerializeField] private new Rigidbody rigidbody = null;

        private string itemName = string.Empty;
        private string itemDescription = string.Empty;

        public EWorldItemCategory ItemCategory { get => itemCategory; }
        public List<EPlaceCondition> PlaceConditions { get => placeConditions; }
        public Vector3 SpawnOffset { get => spawnOffset; }
        public Rigidbody Rigidbody { get => rigidbody; }
        public bool DontTumbleOnInstantiate { get => dontTumbleOnInstantiate; }
        public bool DontRotateOnInstantiate { get => dontRotateOnInstantiate; }

        private void Awake()
        {
            ReadDataFromCSV();
        }

        private void ReadDataFromCSV()
        {
            Tuple<string, string> infos = RItemIdentifierLibrary.GetItemInfos();
            itemName = infos.Item1;
            itemDescription = infos.Item2;
        }

        /// <summary>
        /// Sets this World Item to be a preview.
        /// </summary>
        public void SetAsPreview()
        {
            for (int i = 0; i < colliders.Count; i++)
                colliders[i].enabled = false;

            for (int i = 0; i < behavioursToDisableOnPreview.Count; i++)
                behavioursToDisableOnPreview[i].enabled = false;

            placeholder.SetActive(true);
            worldMesh.SetActive(false);
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
}