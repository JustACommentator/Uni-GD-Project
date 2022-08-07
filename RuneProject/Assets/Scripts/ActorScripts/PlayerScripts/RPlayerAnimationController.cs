using RuneProject.UserInterfaceSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerAnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator playerAnimator = null;
        [SerializeField] private RPlayerMovement movement = null;
        [SerializeField] private RPlayerDash dash = null;
        [SerializeField] private RPlayerBasicAttack basicAttack = null;
        [SerializeField] private RPlayerHealth playerHealth = null;
        [SerializeField] private RPlayerInventory playerInventory = null;
        [SerializeField] private RUI_Main userInterface = null;
        [Space]
        [SerializeField] private GameObject secondCamParent = null;

        private float lastTimeSinceMove = 0f;

        public Animator PlayerAnimator { get => playerAnimator; set => playerAnimator = value; }
        public RPlayerMovement Movement { get => movement; set => movement = value; }
        public RPlayerDash Dash { get => dash; set => dash = value; }
        public RPlayerBasicAttack BasicAttack { get => basicAttack; set => basicAttack = value; }
        public RPlayerHealth PlayerHealth { get => playerHealth; set => playerHealth = value; }
        public RPlayerInventory PlayerInventory { get => playerInventory; set => playerInventory = value; }
        public RUI_Main UserInterface { get => userInterface; set => userInterface = value; }
        public float LastTimeSinceMove { get => lastTimeSinceMove; set => lastTimeSinceMove = value; }

        private void Start()
        {
            movement.OnMove += Movement_OnMove;
            dash.OnDash += Dash_OnDash;
            basicAttack.OnBeginCharge += BasicAttack_OnBeginCharge;
            basicAttack.OnEndCharge += BasicAttack_OnEndCharge;
            basicAttack.OnFireAutoAttack += BasicAttack_OnFireAutoAttack;
            basicAttack.OnFireItemAttack += BasicAttack_OnFireItemAttack;
            playerHealth.OnDeath += PlayerHealth_OnDeath;
            playerInventory.OnOpenChest += PlayerInventory_OnOpenChest;
            playerInventory.OnEndItemShowOff += PlayerInventory_OnEndItemShowOff;
            userInterface.OnReachEnd += UserInterface_OnReachEnd;
        }

        private void PlayerInventory_OnEndItemShowOff(object sender, System.EventArgs e)
        {
            playerAnimator.SetTrigger("putAway");
            secondCamParent.SetActive(false);
        }

        private void PlayerInventory_OnOpenChest(object sender, EnvironmentSystem.RTreasureChestComponent e)
        {
            playerAnimator.Play("OpenChest");
        }

        private void UserInterface_OnReachEnd(object sender, System.EventArgs e)
        {
            playerAnimator.Play("Excited");
        }

        private void BasicAttack_OnFireItemAttack(object sender, EPlayerAttackAnimationType e)
        {
            switch (e)
            {
                case EPlayerAttackAnimationType.ONE_HANDED:
                    playerAnimator.Play("ItemAttack");
                    break;
                case EPlayerAttackAnimationType.TWO_HANDED:
                    playerAnimator.Play("2HandedItemAttack");
                    break;
                case EPlayerAttackAnimationType.CHARGED:
                    playerAnimator.Play("ItemSpinAttack");
                    break;
            }
        }

        private void PlayerHealth_OnDeath(object sender, GameObject e)
        {
            playerAnimator.Play("Death");
        }

        private void BasicAttack_OnFireAutoAttack(object sender, System.EventArgs e)
        {
            playerAnimator.Play("CastSpell");
        }

        private void BasicAttack_OnEndCharge(object sender, bool e)
        {
            playerAnimator.SetBool("charging", false);
        }

        private void BasicAttack_OnBeginCharge(object sender, bool e)
        {
            playerAnimator.SetBool("charging", true);
        }

        private void Dash_OnDash(object sender, System.EventArgs e)
        {
            playerAnimator.Play("Roll");
        }

        private void Update()
        {
            lastTimeSinceMove += Time.deltaTime;

            if (lastTimeSinceMove > 0.03f)
                playerAnimator.SetFloat("inputMagnitude", 0f);
        }

        private void Movement_OnMove(object sender, Vector2 e)
        {
            playerAnimator.SetFloat("inputMagnitude", e.magnitude);
            lastTimeSinceMove = 0f;
        }
    }
}