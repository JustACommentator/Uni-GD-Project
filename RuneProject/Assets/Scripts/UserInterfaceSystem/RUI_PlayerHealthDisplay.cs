using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_PlayerHealthDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerHealth playerHealth = null;
        [Space]
        [SerializeField] private List<Image> hearts = new List<Image>();
        [SerializeField] private Transform heartParent = null;

        [Header("Prefabs")]
        [SerializeField] private GameObject heartPrefab = null;

        private const int HP_DISPLAYED_PER_HEART = 4;

        private void Start()
        {
            playerHealth.OnDamageTaken += PlayerHealth_OnHealthChanged;
            playerHealth.OnHealReceived += PlayerHealth_OnHealthChanged;
            playerHealth.OnGainAdditionalHealth += PlayerHealth_OnGainAdditionalHealth;
        }

        private void PlayerHealth_OnGainAdditionalHealth(object sender, int e)
        {
            Transform instance = Instantiate(heartPrefab, heartParent).transform;
            instance.transform.SetAsLastSibling();
            hearts.Add(instance.GetChild(1).GetComponent<Image>());
            PlayerHealth_OnHealthChanged(null, 0);
        }

        private void OnDestroy()
        {
            playerHealth.OnDamageTaken -= PlayerHealth_OnHealthChanged;
            playerHealth.OnHealReceived -= PlayerHealth_OnHealthChanged;
        }

        private void PlayerHealth_OnHealthChanged(object sender, int e)
        {
            for (int i=0; i<hearts.Count; i++)            
                hearts[i].fillAmount = playerHealth.CurrentHealth * (1f / HP_DISPLAYED_PER_HEART) - i;            
        }
    }
}