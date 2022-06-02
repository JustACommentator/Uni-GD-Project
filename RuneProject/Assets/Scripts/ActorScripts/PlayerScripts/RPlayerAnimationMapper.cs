using RuneProject.AudioSystem;
using RuneProject.LibrarySystem;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerAnimationMapper : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RAudioEmitComponent sfxSource = null;

        public void Anim_PlayStepSound()
        {
            sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.walkClip, true, randomizePitch: true);
        }
    }
}