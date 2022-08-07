using RuneProject.SaveGameSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace RuneProject.MainMenuSystem
{
    public class RMainMenuControlsHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text buttonText = null;

        private bool rollInWalkingDirection = false;
        
        private void Start()
        {
            rollInWalkingDirection = RSaveGameContainer.Instance.SaveData.rollInWalkingDirection;

            UpdateText();
        }

        public void OnClick_DashDirection()
        {
            rollInWalkingDirection = !rollInWalkingDirection;
            RSaveGameContainer.SetRoll(rollInWalkingDirection);

            UpdateText();
        }

        private void UpdateText()
        {
            buttonText.text = rollInWalkingDirection ? "Dash in walking direction" : "Dash in mouse direction";
        }
    }
}