using RuneProject.AudioSystem;
using RuneProject.LibrarySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerSoundHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RAudioEmitComponent voiceSource = null;
        [SerializeField] private RAudioEmitComponent sfxSource = null;
        [Space]
        [SerializeField] private RPlayerBasicAttack basicAttack = null;
        [SerializeField] private RPlayerMovement movement = null;
        [SerializeField] private RPlayerDash dash = null;
        [SerializeField] private RPlayerHealth playerHealth = null;

        private AudioSource autoAttackChargeSource = null;

        private const float ADDITIONAL_FIRE_TIME = 0.15f;

        private void Start()
        {
            basicAttack.OnBeginCharge += BasicAttack_OnBeginCharge;
            basicAttack.OnEndCharge += BasicAttack_OnEndCharge;
            basicAttack.OnFireAutoAttack += BasicAttack_OnFireAutoAttack;
            dash.OnDash += Dash_OnDash;
            playerHealth.OnDamageTaken += PlayerHealth_OnDamageTaken;
            playerHealth.OnHealReceived += PlayerHealth_OnHealReceived;
            playerHealth.OnDeath += PlayerHealth_OnDeath;
        }

        private void PlayerHealth_OnDeath(object sender, GameObject e)
        {
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.deathClips), false, delay: 0.4f);
        }

        private void PlayerHealth_OnHealReceived(object sender, int e)
        {
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.laughClips), false);
        }

        private void PlayerHealth_OnDamageTaken(object sender, int e)
        {
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.hurtClips), false);
        }

        private void Dash_OnDash(object sender, System.EventArgs e)
        {
            sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.dashClip, true, randomizePitch: true);
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.dashClips), false);
        }

        private void BasicAttack_OnEndCharge(object sender, System.EventArgs e)
        {
            if (autoAttackChargeSource)            
                Destroy(autoAttackChargeSource.gameObject, ADDITIONAL_FIRE_TIME);            
        }

        private void BasicAttack_OnFireAutoAttack(object sender, System.EventArgs e)
        {
            sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.autoAttackFireClip, true, delay: ADDITIONAL_FIRE_TIME);           
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.autoAttackFireVoiceClips), false);
        }

        private void BasicAttack_OnBeginCharge(object sender, System.EventArgs e)
        {
            AudioClip startSFX = RSFXIdentifierLibrary.Singleton.autoAttackChargeStartClip;
            sfxSource.PlayClip(startSFX, true);
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.autoAttackChargeStartVoiceClips), false);
            autoAttackChargeSource = sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.autoAttackChargeSustainClip, true, true, startSFX.length);
        }
    }
}