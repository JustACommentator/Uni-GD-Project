using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ItemSystem
{
    public class RSpinItemPreview : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform rotationParent = null;

        [Header("Values")]
        [SerializeField] private float rotationSpeed = 0f;

        private void Update()
        {
            rotationParent.Rotate(rotationSpeed * Time.deltaTime * Vector3.up, Space.Self);
        }
    }
}