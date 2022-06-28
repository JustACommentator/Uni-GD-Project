using RuneProject.EnvironmentSystem;
using RuneProject.ItemSystem;
using RuneProject.LibrarySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuneProject.ActorSystem
{
    public class RPlayerInventory : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerMovement playerMovement = null;
        [SerializeField] private RPlayerHealth playerHealth = null;
        [SerializeField] private RPlayerDash playerDash = null;
        [SerializeField] private RPlayerBasicAttack playerAttack = null;
        [SerializeField] private Transform characterTransform = null;
        [Space]
        [SerializeField] private GameObject playerShowOffParent = null;
        [SerializeField] private Image playerShowOffImage = null;

        private Coroutine currentChestRoutine = null;
        private List<RPowerUpItem> powerUps = new List<RPowerUpItem>();
        private int currentKeys = 0;
        private int currentBossKeys = 0;

        public event System.EventHandler<int> OnKeyCountChange;
        public event System.EventHandler<int> OnBossKeyCountChange;
        public event System.EventHandler<RTreasureChestComponent> OnOpenChest;
        public event System.EventHandler<RPowerUpItem> OnClaimChestReward;
        public event System.EventHandler OnEndItemShowOff;        

        public int CurrentKeys { get => currentKeys; set { currentKeys = value; OnKeyCountChange?.Invoke(this, CurrentKeys); } }
        public int CurrentBossKeys { get => currentBossKeys; set { currentBossKeys = value; OnKeyCountChange?.Invoke(this, CurrentBossKeys); } }

        public void OpenChest(RTreasureChestComponent chest)
        {
            if (currentChestRoutine == null)
                currentChestRoutine = StartCoroutine(IExecuteOpenChest(chest));
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
                case 0: //Health
                    playerHealth.GainMaxHealth(4);
                    playerHealth.Heal(null, 100);
                    break;
                case 1: //Knockback
                    playerAttack.CurrentAdditionalAutoAttackKnockback += 0.1f;
                    break;
                case 2: //Movement-Speed
                    playerMovement.CurrentAdditionalMovementSpeed += 0.15f;
                    break;
                case 3: //Dash-Distance
                    playerDash.CurrentAdditionalDashPower += 0.15f;
                    break;
                case 4: //World Items
                    playerAttack.CurrentWorldItemBreakDenierPowerupCount++;
                    break;
                case 5: //Shield
                    playerHealth.CurrentShield += 3;
                    break;
            }
        }

        private IEnumerator IExecuteOpenChest(RTreasureChestComponent chest)
        {
            playerMovement.BlockMovementInput();
            OnOpenChest?.Invoke(this, chest);
            transform.position = chest.PlayerOpenTransform.position;
            playerMovement.LookAt(chest.transform.position);

            yield return new WaitForSeconds(1.2f);

            playerShowOffParent.SetActive(true);
            playerShowOffImage.sprite = chest.Content.powerUpIcon;

            yield return new WaitForSeconds(0.25f);

            characterTransform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
            OnClaimChestReward?.Invoke(this, chest.Content);
            AddPowerUp(chest.Content);

            bool done = false;
            while(!done)
            {
                if (Input.anyKeyDown)
                    done = true;

                yield return null;
            }

            OnEndItemShowOff?.Invoke(this, null);

            playerShowOffParent.SetActive(false);
            playerMovement.UnBlockMovementInput();
            currentChestRoutine = null;
        }
    }
}