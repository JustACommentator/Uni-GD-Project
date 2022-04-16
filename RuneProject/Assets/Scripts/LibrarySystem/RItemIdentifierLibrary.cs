using RuneProject.ItemSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.LibrarySystem
{
    /// <summary>
    /// Handles the Interaction with items, like accessing them from a numeric ID.
    /// </summary>
    public class RItemIdentifierLibrary : MonoBehaviour
    {
        [Header("Items")]
        [SerializeField] private TextAsset worldItemCSVFile = null;
        [SerializeField] private TextAsset powerUpItemCSVFile = null;
        [SerializeField] [Tooltip("List Index = Item ID")]private List<RWorldItem> worldItems = new List<RWorldItem>();
        [SerializeField] [Tooltip("List Index = Item ID")]private List<RPowerUpItem> powerUpItems = new List<RPowerUpItem>();

        private static RItemIdentifierLibrary singleton = null;

        public static RItemIdentifierLibrary Singleton { get { if (singleton == null) singleton = FindObjectOfType<RItemIdentifierLibrary>(); return singleton; } }

        public static Tuple<string, string> GetWorldItemInfos(string csv_key)
        {
            return new Tuple<string, string>("Platzhalter (Name)", "Platzhalter (Beschreibung)");
        }

        public static Tuple<string, string> GetPowerUpItemInfos(string csv_key)
        {
            return new Tuple<string, string>("Platzhalter (Name)", "Platzhalter (Beschreibung)");
        }

        public static RWorldItem GetWorldItem(int id) => Singleton.worldItems[id];
        public static RPowerUpItem GetPowerUpItem(int id) => Singleton.powerUpItems[id];
        public static int GetPowerUpID(RPowerUpItem item) => Singleton.powerUpItems.IndexOf(item); 
    }
}