using System.Collections;
using System.Collections.Generic;
using RuneProject.ActorSystem;
using UnityEngine;
using UnityEngine.UI;

namespace RuneProject.EnemySystem
{
    public class RAttentionIndicator : MonoBehaviour
    {
        [SerializeField] private float offset = 0.0f;
        [SerializeField] private Image img;
        [SerializeField] private Sprite ai_sus;
        [SerializeField] private Sprite ai_following;
        [SerializeField] private Sprite ai_dead;

        private Vector3 origin;
        private Transform vcam;
        private RPlayerHealth player = null;

        private void Start()
        {
            origin = transform.localPosition;
            player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<RPlayerHealth>();
        }

        void Update()
        {
            if (!vcam) vcam = CameraSystem.RPlayerCameraComponent.Singleton.VirtualCam.transform;

            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<RPlayerHealth>();
            }

            transform.forward = vcam.forward;
            transform.localPosition = origin;
            transform.position += offset * vcam.up.normalized;

            if (!player.IsAlive) switchState(REnemyAI.EAlertState.IDLE);
        }

        public void switchState(REnemyAI.EAlertState state)
        {
            switch (state)
            {
                case REnemyAI.EAlertState.SUSPICIOUS:
                    img.color = Color.white;
                    img.sprite = ai_sus;
                    img.enabled = true;
                    break;
                case REnemyAI.EAlertState.AGGRESSIVE:
                    img.color = Color.white;
                    img.sprite = ai_following;
                    img.enabled = true;
                    break;
                default:
                    img.enabled = false;
                    break;
            }
        }

        public void setDead()
        {
            img.color = Color.white;
            img.sprite = ai_dead;
            img.enabled = true;
        }
    }
}