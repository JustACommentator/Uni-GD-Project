using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_PowerUpDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerInventory playerInventory = null;
        [Space]
        [SerializeField] private Transform displayParent = null;

        [Header("Prefabs")]
        [SerializeField] private Image displayPrefab = null;

        private void Start()
        {
            playerInventory.OnClaimChestReward += PlayerInventory_OnClaimChestReward;
        }

        private void PlayerInventory_OnClaimChestReward(object sender, ItemSystem.RPowerUpItem e)
        {
            Instantiate(displayPrefab, displayParent).sprite = e.powerUpIcon;
        }
    }
}