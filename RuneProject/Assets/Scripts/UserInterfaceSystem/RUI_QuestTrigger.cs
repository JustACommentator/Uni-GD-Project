using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_QuestTrigger : MonoBehaviour
    {
        [SerializeField] private List<string> Quests = new List<string>();
        [SerializeField] private bool absolve = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                return;

                if (!absolve)
                    other.GetComponentInChildren<RQuestLog>().AddQuests(Quests);
                else
                    other.GetComponentInChildren<RQuestLog>().RemoveQuests(Quests);

                Destroy(gameObject);
            }
        }
    }
}
