using RuneProject.ActorSystem;
using RuneProject.ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class RTreasureChestComponent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerOpenTransform = null;
        [SerializeField] private Animator chestAnimator = null;
        [SerializeField] private AudioSource chestAudioSource = null;

        [Header("Values")]
        [SerializeField] private List<RPowerUpItem> possibleContents = new List<RPowerUpItem>();

        private bool open = false;

        public RPowerUpItem Content => possibleContents[Random.Range(0, possibleContents.Count)];
        public Transform PlayerOpenTransform => playerOpenTransform;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player") && !open)
            {
                open = true;
                collision.collider.GetComponent<RPlayerInventory>().OpenChest(this);
                chestAnimator.SetTrigger("open");
                chestAudioSource.PlayDelayed(0.82f);
            }
        }
    }
}