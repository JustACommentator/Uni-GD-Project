using RuneProject.ItemSystem;
using RuneProject.LibrarySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerInventory : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerHealth playerHealth = null;

        [Header("Values")]
        [SerializeField] private int initialCrystals = 0;

        private List<RPowerUpItem> powerUps = new List<RPowerUpItem>();
        private int currentCrystals = 0;

        private void Start()
        {
            currentCrystals = initialCrystals;
        }

        public void RemoveCrystals(int amount)
        {
            currentCrystals = currentCrystals - Mathf.Clamp(amount, -currentCrystals, 0);
        }

        public void AddCrystals(int amount)
        {
            currentCrystals += amount;
        }

        public void AddPowerUp(RPowerUpItem item)
        {
            if (item.hasLimit && powerUps.Contains(item))
            {
                int count = 0;
                for (int i = 0; i < powerUps.Count; i++)
                    if (powerUps[i] == item)
                        count++;

                if (count >= item.limit)
                    return;
            }

            powerUps.Add(item);
            ResolveItem(item);
        }

        private void ResolveItem(RPowerUpItem item)
        {
            switch(RItemIdentifierLibrary.GetPowerUpID(item))
            {
                case 0:
                    playerHealth.MaxHealth += 4;
                    playerHealth.Heal(null, 4);
                    break;
            }
        }
    }
}