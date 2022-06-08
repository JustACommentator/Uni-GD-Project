using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.UserInterfaceSystem
{
    public class RQuestLog : MonoBehaviour
    {
        [SerializeField] private float staysOpenFor = 2f;

        [Header("References")]
        [SerializeField] private Animator animator = null;
        [SerializeField] private TMPro.TextMeshProUGUI questsDisplay = null;

        private bool isOpened = false;
        private float currentlyOpenFor = 0;

        private List<string> quests;

        private void Update()
        {
            if (isOpened)
            {
                currentlyOpenFor += Time.deltaTime;
            }

            if (currentlyOpenFor > staysOpenFor || Input.GetKey(KeyCode.Escape))
            {
                Close();
            }
        }

        public void AddQuests(List<string> newQuests)
        {
            Open();
            quests.AddRange(newQuests);
            questsDisplay.text = RenderQuests(quests);
        }

        public void RemoveQuests(List<string> newQuests)
        {
            Open();
            foreach (string q in newQuests)
                quests.Remove(q);
            questsDisplay.text = RenderQuests(quests);
        }

        private string RenderQuests(List<string> quests)
        {
            string o = "";
            foreach (string q in quests)
                o += "\n- " + q;

            return o;
        }

        private void Open()
        {
            if (!isOpened)
                animator.SetBool("Open", true);
            isOpened = true;
        }

        private void Close()
        {
            if (isOpened)
                animator.SetBool("Open", false);
            isOpened = false;
        }
    }
}