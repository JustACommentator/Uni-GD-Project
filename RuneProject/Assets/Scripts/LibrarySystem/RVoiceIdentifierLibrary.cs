using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.LibrarySystem
{
    public class RVoiceIdentifierLibrary : MonoBehaviour
    {
        [Header("Voice Clips")]
        public List<AudioClip> autoAttackChargeStartVoiceClips = new List<AudioClip>();
        public List<AudioClip> autoAttackFireVoiceClips = new List<AudioClip>();
        public List<AudioClip> dashClips = new List<AudioClip>();
        public List<AudioClip> deathClips = new List<AudioClip>();
        public List<AudioClip> hitClips = new List<AudioClip>();
        public List<AudioClip> hurtClips = new List<AudioClip>();
        public List<AudioClip> joyClips = new List<AudioClip>();
        public List<AudioClip> laughClips = new List<AudioClip>();

        private static RVoiceIdentifierLibrary singleton = null;

        public static RVoiceIdentifierLibrary Singleton { get { if (singleton == null) singleton = FindObjectOfType<RVoiceIdentifierLibrary>(); return singleton; } }

        public static AudioClip GetRandomOf(List<AudioClip> clips) => clips[Random.Range(0, clips.Count)];
    }
}