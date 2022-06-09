using RuneProject.ActorSystem;
using RuneProject.ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_HoldingsHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerBasicAttack basicAttack = null;
        [Space]
        [SerializeField] private GameObject staffHolding = null;
        [SerializeField] private GameObject itemHolding = null;
        [SerializeField] private Image itemHoldingImage = null;

        private void Start()
        {
            basicAttack.OnPickUp += BasicAttack_OnPickUp;
            basicAttack.OnThrow += BasicAttack_OnThrow;
        }

        private void BasicAttack_OnThrow(object sender, RWorldItem e)
        {
            staffHolding.SetActive(true);
            itemHolding.SetActive(false);
        }

        private void BasicAttack_OnPickUp(object sender, RWorldItem e)
        {
            staffHolding.SetActive(false);
            itemHolding.SetActive(true);
            itemHoldingImage.sprite = e.ItemSprite;
        }
    }
}