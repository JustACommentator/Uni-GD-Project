using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.Events;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_QuestTrigger : MonoBehaviour
    {
        [SerializeField] private List<string> Quests = new List<string>();
        [SerializeField] private bool absolve = false;
        [SerializeField] private UnityAction absolveCheck;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!absolve)
                    other.GetComponentInChildren<RQuestLog>().AddQuests(Quests);
                else
                    other.GetComponentInChildren<RQuestLog>().RemoveQuests(Quests);
            }
        }
    }
}
