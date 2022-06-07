using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.LibrarySystem
{
    public class RSFXIdentifierLibrary : MonoBehaviour
    {
        [Header("SFX")]
        public AudioClip autoAttackChargeStartClip = null;
        public AudioClip autoAttackChargeSustainClip = null;
        public AudioClip autoAttackFireClip = null;
        [Space]
        public AudioClip walkClip = null;
        public AudioClip dashClip = null;
        [Space]
        public AudioClip swingClip = null;
        public AudioClip throwClip = null;
        public List<AudioClip> pickUpClips = new List<AudioClip>();
        [Space]
        public List<AudioClip> physicalImpactClips = new List<AudioClip>();
        public List<AudioClip> magicImpactClips = new List<AudioClip>();

        private static RSFXIdentifierLibrary singleton = null;

        public static RSFXIdentifierLibrary Singleton { get { if (singleton == null) singleton = FindObjectOfType<RSFXIdentifierLibrary>(); return singleton; } }
    }
}