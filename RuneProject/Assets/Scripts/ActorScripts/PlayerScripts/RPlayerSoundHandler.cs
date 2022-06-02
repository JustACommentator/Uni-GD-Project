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

        private AudioSource autoAttackChargeSource = null;

        private const float ADDITIONAL_FIRE_TIME = 0.15f;

        private void Start()
        {
            basicAttack.OnBeginCharge += BasicAttack_OnBeginCharge;
            basicAttack.OnEndCharge += BasicAttack_OnEndCharge;
            basicAttack.OnFireAutoAttack += BasicAttack_OnFireAutoAttack;

        }

        private void BasicAttack_OnEndCharge(object sender, System.EventArgs e)
        {
            if (autoAttackChargeSource)
            {
                Destroy(autoAttackChargeSource.gameObject, ADDITIONAL_FIRE_TIME);
            }
        }

        private void BasicAttack_OnFireAutoAttack(object sender, System.EventArgs e)
        {
            sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.autoAttackFireClip, true, delay: ADDITIONAL_FIRE_TIME);           
            voiceSource.PlayClip(RVoiceIdentifierLibrary.Singleton.autoAttackFireVoiceClip, true);
        }

        private void BasicAttack_OnBeginCharge(object sender, System.EventArgs e)
        {
            AudioClip startSFX = RSFXIdentifierLibrary.Singleton.autoAttackChargeStartClip;
            sfxSource.PlayClip(startSFX, true);
            voiceSource.PlayClip(RVoiceIdentifierLibrary.Singleton.autoAttackChargeStartVoiceClip, true);
            autoAttackChargeSource = sfxSource.PlayClip(RSFXIdentifierLibrary.Singleton.autoAttackChargeSustainClip, true, true, startSFX.length);
        }
    }
}