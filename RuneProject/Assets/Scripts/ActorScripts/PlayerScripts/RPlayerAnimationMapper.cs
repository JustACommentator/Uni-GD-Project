using RuneProject.AudioSystem;
using RuneProject.LibrarySystem;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerAnimationMapper : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RAudioEmitComponent sfxSource = null;
        [SerializeField] private RPlayerHealth playerHealth = null;
        [Space]
        [SerializeField] private ParticleSystem walkDustParticleSystem = null;
        [SerializeField] private ParticleSystem landDustParticleSystem = null;

        private const float INVINCIBLE_TIME = 0.3f;

        public void Anim_PlayStepSound()
        {
            sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.walkClip, true, randomizePitch: true);
        }

        public void Anim_SetInvincible()
        {
            playerHealth.SetInvincible(INVINCIBLE_TIME);
        }

        public void Anim_PlayWalkDustParticles()
        {
            walkDustParticleSystem.Play();
        }

        public void Anim_PlayLandDustParticles()
        {
            landDustParticleSystem.Play();
        }
    }
}