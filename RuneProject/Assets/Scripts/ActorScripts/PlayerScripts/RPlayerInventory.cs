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

        private List<RPowerUpItem> powerUps = new List<RPowerUpItem>();
        private int currentKeys = 0;
        private int currentBossKeys = 0;

        public int CurrentKeys { get => currentKeys; set => currentKeys = value; }
        public int CurrentBossKeys { get => currentBossKeys; set => currentBossKeys = value; }

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