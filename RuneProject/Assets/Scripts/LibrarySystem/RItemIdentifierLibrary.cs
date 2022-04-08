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
        [SerializeField] private TextAsset itemCSVFile = null;
        [SerializeField] [Tooltip("List Index = Item ID")]private List<RWorldItem> items = new List<RWorldItem>();

        private static RItemIdentifierLibrary singleton = null;

        public static RItemIdentifierLibrary Singleton { get { if (singleton == null) singleton = FindObjectOfType<RItemIdentifierLibrary>(); return singleton; } }

        public static Tuple<string, string> GetItemInfos()
        {
            return new Tuple<string, string>("Platzhalter (Name)", "Platzhalter (Beschreibung)");
        }

        public static RWorldItem GetItem(int id) => Singleton.items[id];
    }
}