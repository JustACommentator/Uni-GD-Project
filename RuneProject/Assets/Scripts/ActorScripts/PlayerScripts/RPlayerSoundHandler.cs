using RuneProject.AudioSystem;
using RuneProject.ItemSystem;
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

        public void Laugh()
        {
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.laughClips), false);
        }

        private void Start()
        {
            basicAttack.OnBeginCharge += BasicAttack_OnBeginCharge;
            basicAttack.OnEndCharge += BasicAttack_OnEndCharge;
            basicAttack.OnFireAutoAttack += BasicAttack_OnFireAutoAttack;
            basicAttack.OnThrow += BasicAttack_OnThrow;
            basicAttack.OnFireItemAttack += BasicAttack_OnFireItemAttack;
            basicAttack.OnHitWithItemAttack += BasicAttack_OnHitWithItemAttack;
            basicAttack.OnPickUp += BasicAttack_OnPickUp;
            dash.OnDash += Dash_OnDash;
            playerHealth.OnDamageTaken += PlayerHealth_OnDamageTaken;
            playerHealth.OnHealReceived += PlayerHealth_OnHealReceived;
            playerHealth.OnDeath += PlayerHealth_OnDeath;
            movement.OnPushLevelObject += Movement_OnPushLevelObject;
        }

        private void Movement_OnPushLevelObject(object sender, GameObject e)
        {
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.hitClips), false);
        }

        private void BasicAttack_OnPickUp(object sender, RWorldItem e)
        {
            sfxSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RSFXIdentifierLibrary.Singleton.pickUpClips), true);
        }

        private void BasicAttack_OnHitWithItemAttack(object sender, RPlayerHealth e)
        {
            //Fragen, ob physischer oder magischer Angriff
            sfxSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RSFXIdentifierLibrary.Singleton.physicalImpactClips), true);
        }

        private void BasicAttack_OnFireItemAttack(object sender, EPlayerAttackAnimationType e)
        {
            sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.swingClip, true, randomizePitch: true);

            if (e == EPlayerAttackAnimationType.CHARGED)
            {
                voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.autoAttackFireVoiceClips), false);
                sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.swingClip, true, randomizePitch: true, delay:0.2f);
                sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.swingClip, true, randomizePitch: true, delay:0.4f);
                sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.swingClip, true, randomizePitch: true, delay:0.6f);
                sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.swingClip, true, randomizePitch: true, delay:0.8f);
            }
            else
            {
                voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.hitClips), false);
            }            
        }

        private void BasicAttack_OnThrow(object sender, RWorldItem e)
        {
            sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.throwClip, true, randomizePitch: true);
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.hitClips), false);
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

        private void BasicAttack_OnEndCharge(object sender, bool e)
        {
            if (autoAttackChargeSource)            
                Destroy(autoAttackChargeSource.gameObject, e ? ADDITIONAL_FIRE_TIME : 0f);            
        }

        private void BasicAttack_OnFireAutoAttack(object sender, System.EventArgs e)
        {
            sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.autoAttackFireClip, true, delay: ADDITIONAL_FIRE_TIME);           
            voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.autoAttackFireVoiceClips), false);
        }

        private void BasicAttack_OnBeginCharge(object sender, bool e)
        {
            if (e)
            {
                AudioClip startSFX = RSFXIdentifierLibrary.Singleton.autoAttackChargeStartClip;
                sfxSource.PlayClip(startSFX, true);
                voiceSource.PlayClip(RVoiceIdentifierLibrary.GetRandomOf(RVoiceIdentifierLibrary.Singleton.autoAttackChargeStartVoiceClips), false);
                autoAttackChargeSource = sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.autoAttackChargeSustainClip, true, true, startSFX.length);
            }
        }
    }
}