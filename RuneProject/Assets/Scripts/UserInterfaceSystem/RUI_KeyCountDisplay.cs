using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_KeyCountDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerInventory playerInventory = null;
        [Space]
        [SerializeField] private GameObject normalKeysParent = null;
        [SerializeField] private GameObject bossKeysParent = null;
        [Space]
        [SerializeField] private TMP_Text normalKeysCountText = null;
        [SerializeField] private TMP_Text bossKeysCountText = null;

        private void Start()
        {
            playerInventory.OnKeyCountChange += PlayerInventory_OnKeyCountChange;
            playerInventory.OnBossKeyCountChange += PlayerInventory_OnBossKeyCountChange;
        }

        private void PlayerInventory_OnBossKeyCountChange(object sender, int e)
        {
            normalKeysParent.SetActive(e > 0);
            normalKeysCountText.text = $"{e}";
        }

        private void PlayerInventory_OnKeyCountChange(object sender, int e)
        {
            bossKeysParent.SetActive(e > 0);
            bossKeysCountText.text = $"{e}";
        }
    }
}