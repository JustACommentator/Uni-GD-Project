using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_ChestResultDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerInventory playerInventory = null;
        [Space]
        [SerializeField] private GameObject chestResultParent = null;
        [SerializeField] private TMP_Text chestResultItemNameText = null;
        [SerializeField] private TMP_Text chestResultItemDescriptionText = null;
        [SerializeField] private Image chestResultItemImage = null;

        private void Start()
        {
            playerInventory.OnClaimChestReward += PlayerInventory_OnClaimChestReward;
            playerInventory.OnEndItemShowOff += PlayerInventory_OnEndItemShowOff;
        }

        private void PlayerInventory_OnEndItemShowOff(object sender, System.EventArgs e)
        {
            chestResultParent.SetActive(false);
        }

        private void PlayerInventory_OnClaimChestReward(object sender, ItemSystem.RPowerUpItem e)
        {
            chestResultParent.SetActive(true);
            chestResultItemNameText.text = e.itemName;
            chestResultItemDescriptionText.text = e.itemDescription;
            chestResultItemImage.sprite = e.powerUpIcon;
        }
    }
}