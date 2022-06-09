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
                RemoveQuests( new List<string> { "Dash with <sprite name=\"left_shift\">" } );
            }
        }

        public void AddQuests(List<string> newQuests)
        {
            Open();
            quests.AddRange(newQuests);
            questsDisplay.text = RenderQuests(quests, newQuests, "yellow");
        }

        public void RemoveQuests(List<string> oldQuests)
        {
            questsDisplay.text = RenderQuests(quests, oldQuests, "green");

            foreach (string q in oldQuests) {
                if (quests.Contains(q))
                {
                    quests.Remove(q);
                    Open();
                }
            }
        }

        private string RenderQuests(List<string> quests, List<string> completed, string color)
        {
            string o = "";
            foreach (string q in quests)
            {
                if (completed.Contains(q))
                    o += "<color=\"" + color + "\">";
                o += "\n- " + q;
                if (completed.Contains(q))
                    o += "</color>";
            }

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