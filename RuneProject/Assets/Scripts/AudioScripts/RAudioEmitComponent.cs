using System.Collections;
using UnityEngine;

namespace RuneProject.AudioSystem
{
    public class RAudioEmitComponent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioSource emittingSource = null;

        private Coroutine currentDelayRoutine = null;

        private const float ADDITIONAL_DESTROY_TIME = 0.25f;

        public AudioSource PlayClip(AudioClip clip, bool newInstance, bool loop = false, float delay = 0f)
        {
            if (!clip) return null;

            AudioSource usedInstance = newInstance ? Instantiate(emittingSource) : emittingSource;

            usedInstance.Stop();
            usedInstance.clip = clip;
            usedInstance.loop = loop;

            if (!newInstance && currentDelayRoutine != null)
                StopCoroutine(currentDelayRoutine);

            currentDelayRoutine = StartCoroutine(IPlayClipDelayed(usedInstance, delay));

            if (newInstance && !loop)
                Destroy(usedInstance.gameObject, clip.length + ADDITIONAL_DESTROY_TIME + delay);

            return usedInstance;
        }

        private IEnumerator IPlayClipDelayed(AudioSource usedInstance, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (usedInstance)
                usedInstance.Play();
        }
    }
}