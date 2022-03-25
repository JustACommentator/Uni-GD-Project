using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GravityProject.DebugSystem
{
    public class DebugCanvasComponent : MonoBehaviour
    {
        [SerializeField] private Text debugText = null;
        [SerializeField] private Rigidbody debugRB = null;

        private float fpsTimer = 0f;
        private int currentFPS = 0;
        private Vector3 currentVel = Vector3.zero;

        private void Update()
        {
            HandleFPSCounter();
            HandleTextUpdate();
        }

        private void HandleFPSCounter()
        {
            if (Time.unscaledTime > fpsTimer)
            {
                currentFPS = Mathf.FloorToInt(1f / Time.unscaledDeltaTime);
                fpsTimer = Time.unscaledTime + 1f;
            }
        }

        private void HandleTextUpdate()
        {
            if (debugRB.velocity != Vector3.zero)
                currentVel = debugRB.velocity;

            debugText.text = $"FPS: {currentFPS}\nVelocity: {currentVel}";
        }
    }
}