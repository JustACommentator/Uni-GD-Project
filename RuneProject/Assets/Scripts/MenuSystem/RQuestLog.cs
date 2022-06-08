using System.Collections;
using System.Collections.Generic;
using RuneProject.EnvironmentSystem;
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

        private List<string> quests = new List<string>();

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

            if (Input.GetKey(KeyCode.LeftShift))
            {
                RemoveQuests( new List<string> { "Dash with LEFT SHIFT" } );
            }
        }

        public void AddQuests(List<string> newQuests)
        {
            Open();
            quests.AddRange(newQuests);
            questsDisplay.text = RenderQuests(quests);
        }

        public void RemoveQuests(List<string> oldQuests)
        {
            foreach (string q in oldQuests)
                if (quests.Contains(q))
                {
                    quests.Remove(q);
                }
            if (quests.Count > 0)
            {
                Open();
            }
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
            currentlyOpenFor = 0f;
        }

        private void Close()
        {
            if (isOpened)
                animator.SetBool("Open", false);
            isOpened = false;
        }
    }
}