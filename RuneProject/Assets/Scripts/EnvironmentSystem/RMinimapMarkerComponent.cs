using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class RMinimapMarkerComponent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject minimapMarker = null;

        public GameObject MinimapMarker { get => minimapMarker; }
    }
}