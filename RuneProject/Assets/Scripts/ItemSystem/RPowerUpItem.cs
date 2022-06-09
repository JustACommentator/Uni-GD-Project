using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ItemSystem
{
    [CreateAssetMenu(fileName = "New Power-Up", menuName = "Add/Items/Power-Up")]
    public class RPowerUpItem : ScriptableObject
    {
        [Header("Values")]
        public string itemName = string.Empty;
        public string itemDescription = string.Empty;
        public Sprite powerUpIcon = null;
        public bool hasLimit = false;
        public int limit = 1;        
    }
}