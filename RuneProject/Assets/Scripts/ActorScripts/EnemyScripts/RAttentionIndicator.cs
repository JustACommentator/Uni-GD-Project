using System.Collections;
using System.Collections.Generic;
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

        private Vector3 origin;
        private Transform vcam;

        private void Start()
        {
            origin = transform.localPosition;
        }

        void Update()
        {
            if (!vcam) vcam = CameraSystem.RPlayerCameraComponent.Singleton.VirtualCam.transform;
            transform.forward = vcam.forward;
            transform.localPosition = origin;
            transform.position += offset * vcam.up.normalized;
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
    }
}