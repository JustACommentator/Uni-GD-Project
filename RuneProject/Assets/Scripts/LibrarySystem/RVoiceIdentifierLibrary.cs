using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.LibrarySystem
{
    public class RVoiceIdentifierLibrary : MonoBehaviour
    {
        [Header("Voice Clips")]
        public AudioClip autoAttackChargeStartVoiceClip = null;
        public AudioClip autoAttackFireVoiceClip = null;

        private static RVoiceIdentifierLibrary singleton = null;

        public static RVoiceIdentifierLibrary Singleton { get { if (singleton == null) singleton = FindObjectOfType<RVoiceIdentifierLibrary>(); return singleton; } }
    }
}