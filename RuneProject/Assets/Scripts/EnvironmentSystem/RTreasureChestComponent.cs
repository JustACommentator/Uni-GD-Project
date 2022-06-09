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

        [Header("Values")]
        [SerializeField] private RPowerUpItem content = null;

        private bool open = false;

        public RPowerUpItem Content { get => content; }
        public Transform PlayerOpenTransform { get => playerOpenTransform; }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player") && !open)
            {
                open = true;
                collision.collider.GetComponent<RPlayerInventory>().OpenChest(this);
                chestAnimator.SetTrigger("open");
            }
        }
    }
}