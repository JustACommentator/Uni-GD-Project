using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class RSpikesComponent : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private float spikeCooldown = 0f;
        [SerializeField] private float spikeUptime = 0f;

        [Header("References")]
        [SerializeField] private Transform spikeTransform = null;
        [SerializeField] private GameObject spikeHitbox = null;
        [SerializeField] private ParticleSystem extendParticleSystem = null;
        [SerializeField] private ParticleSystem retractParticleSystem = null;
        [SerializeField] private AudioSource extendAudioSource = null;
        [SerializeField] private AudioSource retractAudioSource = null;

        private float currentSpikeCooldown = 0f;
        private Coroutine currentSpikeRoutine = null;

        private const float SPIKE_ROTATION_TIME = 0.2f;
        private const float MAX_SPIKE_SCALE = 100f;

        private void Update()
        {
            HandleSpikeRising();
        }

        private void OnDisable()
        {
            if (currentSpikeRoutine != null)
                StopCoroutine(currentSpikeRoutine);

            spikeHitbox.SetActive(false);
            spikeTransform.localScale = new Vector3(spikeTransform.localScale.x, spikeTransform.localScale.y, 0f);

            if (currentSpikeCooldown <= 0f)
                currentSpikeCooldown += spikeCooldown;
        }

        private void HandleSpikeRising()
        {
            if (currentSpikeCooldown > 0f)
                currentSpikeCooldown -= Time.deltaTime;
            else
            {
                if (currentSpikeRoutine == null)                    
                    currentSpikeRoutine = StartCoroutine(IExecuteSpikeRotation());
            }
        }

        private IEnumerator IExecuteSpikeRotation()
        {
            extendParticleSystem.Play();
            extendAudioSource.Play();

            float timer = 0f;
            spikeHitbox.SetActive(true);
            while (spikeTransform.localScale.z < MAX_SPIKE_SCALE)
            {
                timer += Time.deltaTime;
                spikeTransform.localScale = new Vector3(spikeTransform.localScale.x, spikeTransform.localScale.y, Mathf.Lerp(0f, MAX_SPIKE_SCALE, timer / SPIKE_ROTATION_TIME));
                yield return null;
            }

            yield return new WaitForSeconds(spikeUptime);

            timer = 0f;
            while (spikeTransform.localScale.z > 0f)
            {
                timer += Time.deltaTime;
                spikeTransform.localScale = new Vector3(spikeTransform.localScale.x, spikeTransform.localScale.y, Mathf.Lerp(MAX_SPIKE_SCALE, 0f, timer / SPIKE_ROTATION_TIME));
                yield return null;
            }

            spikeHitbox.SetActive(false);

            currentSpikeCooldown += spikeCooldown;
            currentSpikeRoutine = null;
        }
    }
}