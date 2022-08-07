using RuneProject.ActorSystem;
using RuneProject.HitboxSystem;
using RuneProject.UtilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnemySystem
{
    public class RGaiusBossAI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerHealth health = null;
        [SerializeField] private Animator animator = null;
        [Space]
        [SerializeField] private CanvasGroup endGameCanvas = null;
        [SerializeField] private Transform leftHand = null;
        [SerializeField] private Transform rightHand = null;
        [SerializeField] private Transform fireballSpawnPoint = null;
        [SerializeField] private List<Transform> additionalFireballSpawnPoints = new List<Transform>();
        [SerializeField] private List<Transform> potentialResidencePoints = new List<Transform>();
        [SerializeField] private GameObject dashHitbox = null;        

        [Header("Prefabs")]
        [SerializeField] private RProjectileComponent fireballPrefab = null;

        private EGaiusBossState currentState = EGaiusBossState.WAIT;
        private EGaiusBossState lastState = EGaiusBossState.WAIT;
        private EGaiusBossState lastNonWaitState = EGaiusBossState.WAIT;
        private float currentRemainingUpdateTimer = 0f;
        private Coroutine currentStateExecutionRoutine = null;        

        private Vector2 updateTimeRange = new Vector2(3f, 5f);
        private Vector2 percentageUpdateTimeDecreaseOnHit = new Vector2(0.1f, 0.25f);

        private Transform playerTransform = null;
        private RPlayerHealth playerHealth = null;
        private int currentResidenceIndex = 0;

        private const float ENRAGE_HP_PERCENTAGE = 0.5f;
        private const float SPIN_RANGE = 0.8f;
        private const float TRANSITION_TIME = 1f;
        private const float TURNAROUND_TIME = 0.35f;
        private const float END_GAME_CANVAS_ALPHA_DELTA = 1f;
        private const bool ALLOW_SAME_STATE_DEFAULT = true;
        private const bool ALLOW_SAME_STATE_ENRAGED = false;

        public event System.EventHandler<EGaiusBossState> OnStateChange;

        private void Start()
        {
            health.OnDamageTaken += Health_OnDamageTaken;
            health.OnDeath += Health_OnDeath;

            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            playerHealth = playerTransform.GetComponent<RPlayerHealth>();
        }

        private void OnDestroy()
        {
            health.OnDamageTaken -= Health_OnDamageTaken;
            health.OnDeath -= Health_OnDeath;
        }

        private void Update()
        {
            HandleStateChange();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, SPIN_RANGE);
        }

        private void Health_OnDeath(object sender, GameObject e)
        {
            animator.SetTrigger("die");
            playerTransform.gameObject.SetActive(false);
            StartCoroutine(ISetEndGameCanvasActiveAfter(3f));
        }

        private void Health_OnDamageTaken(object sender, int e)
        {
            currentRemainingUpdateTimer -= currentRemainingUpdateTimer * Random.Range(percentageUpdateTimeDecreaseOnHit.x, percentageUpdateTimeDecreaseOnHit.y);
        }

        private void HandleStateChange()
        {
            if (health.IsAlive && playerHealth.IsAlive)
            {
                if (currentRemainingUpdateTimer > 0f)
                    currentRemainingUpdateTimer -= Time.deltaTime;
                else
                {
                    currentRemainingUpdateTimer = Random.Range(updateTimeRange.x, updateTimeRange.y);

                    if (currentState != EGaiusBossState.WAIT)
                        lastNonWaitState = currentState;

                    lastState = currentState;
                    currentState = GetRandomFittingBossState();
                    OnStateChange?.Invoke(this, currentState);
                    ExecuteState(currentState);
                }
            }
        }

        private void ExecuteState(EGaiusBossState targetState)
        {
            if (currentStateExecutionRoutine != null)
                StopCoroutine(currentStateExecutionRoutine);

            currentStateExecutionRoutine = StartCoroutine(IExecuteState(targetState));
        }

        private EGaiusBossState GetRandomFittingBossState()
        {
            if (RVectorUtility.GetFlatDistance(transform.position, playerTransform.position) < SPIN_RANGE && lastState != EGaiusBossState.SPIN)
                return EGaiusBossState.SPIN;

            if (lastState != EGaiusBossState.WAIT)
                return EGaiusBossState.WAIT;

            EGaiusBossState selectedState = EGaiusBossState.WAIT;

            do
            {
                int randomIndex = Random.Range(0, 2);
                selectedState = randomIndex == 0 ? (health.CurrentHealth <= health.MaxHealth/2f ? EGaiusBossState.FIREBALL_ENHANCED : EGaiusBossState.FIREBALL) :
                                randomIndex == 1 ? EGaiusBossState.CHANGE_POSITION :
                                EGaiusBossState.WAIT;
            } while (selectedState == EGaiusBossState.WAIT || selectedState == lastState || (selectedState == lastNonWaitState && ALLOW_SAME_STATE_DEFAULT));
            
            return selectedState;
        }

        private IEnumerator IExecuteState(EGaiusBossState targetState)
        {
            switch (targetState)
            {
                case EGaiusBossState.CHANGE_POSITION:
                    {
                        int randomIndex = 0;
                        if (potentialResidencePoints.Count > 1)
                        {
                            randomIndex = currentResidenceIndex;
                            do
                            {
                                randomIndex = Random.Range(0, potentialResidencePoints.Count);
                            } while (randomIndex == currentResidenceIndex);
                        }

                        currentResidenceIndex = randomIndex;
                        Vector3 startPos = transform.position;

                        Quaternion targetRot = RVectorUtility.GetFlatLookAt(transform, potentialResidencePoints[currentResidenceIndex].position);

                        yield return StartCoroutine(IExecuteTurnaround(targetRot));

                        dashHitbox.SetActive(true);

                        //Traverse to selected point
                        float currentTimer = 0f;
                        while (currentTimer <= TRANSITION_TIME)
                        {
                            currentTimer += Time.deltaTime;
                            transform.position = Vector3.Lerp(startPos, potentialResidencePoints[currentResidenceIndex].position, currentTimer / TRANSITION_TIME);
                            yield return null;
                        }

                        dashHitbox.SetActive(false);

                        targetRot = RVectorUtility.GetFlatLookAt(transform, playerTransform.position);

                        yield return StartCoroutine(IExecuteTurnaround(targetRot));

                        break;
                    }
                case EGaiusBossState.FIREBALL:
                    {
                        Quaternion targetRot = RVectorUtility.GetFlatLookAt(transform, playerTransform.position);

                        yield return StartCoroutine(IExecuteTurnaround(targetRot));

                        RProjectileComponent instance = Instantiate(fireballPrefab, fireballSpawnPoint.position,
                            Quaternion.LookRotation(fireballSpawnPoint.forward));

                        instance.Owner = gameObject;

                        break;
                    }
                case EGaiusBossState.FIREBALL_ENHANCED:
                    {
                        //Instantiate Fireball from spawners
                        Quaternion targetRot = RVectorUtility.GetFlatLookAt(transform, playerTransform.position);

                        yield return StartCoroutine(IExecuteTurnaround(targetRot));

                        RProjectileComponent instance = Instantiate(fireballPrefab, fireballSpawnPoint.position,
                            Quaternion.LookRotation(fireballSpawnPoint.forward));

                        instance.Owner = gameObject;

                        for (int i=0; i<additionalFireballSpawnPoints.Count; i++)
                        {
                            instance = Instantiate(fireballPrefab, additionalFireballSpawnPoints[i].position,
                            Quaternion.LookRotation(additionalFireballSpawnPoints[i].forward));

                            instance.Owner = gameObject;
                        }

                        break;
                    }
                case EGaiusBossState.SPIN:
                    {
                        //AoE Explosion

                        break;
                    }
                case EGaiusBossState.HANDS_PLUS:
                    {
                        //Hands Plus

                        break;
                    }
                case EGaiusBossState.HANDS_CROSS:
                    {
                        //Hands Cross

                        break;
                    }
                case EGaiusBossState.HANDS_CATCH:
                    {
                        //Hands Catch

                        break;
                    }
            }

            yield return null;
        }

        private IEnumerator IExecuteTurnaround(Quaternion targetRot)
        {
            if (transform.rotation != targetRot)
            {
                float currentTimer = 0f;
                while (currentTimer <= TURNAROUND_TIME)
                {
                    currentTimer += Time.deltaTime;
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, currentTimer / TURNAROUND_TIME);
                    yield return null;
                }
            }
        }

        private IEnumerator ISetEndGameCanvasActiveAfter(float time)
        {
            yield return new WaitForSeconds(time);

            endGameCanvas.gameObject.SetActive(true);
            endGameCanvas.alpha = 0f;

            float timer = 0f; 
            while (timer <= END_GAME_CANVAS_ALPHA_DELTA)
            {
                timer += Time.deltaTime;
                endGameCanvas.alpha = Mathf.Lerp(0f, 1f, timer / END_GAME_CANVAS_ALPHA_DELTA);
                yield return null;
            }
        }
    }

    public enum EGaiusBossState
    {
        WAIT,               //Warte
        CHANGE_POSITION,    //Teleportiert auf eine andere Position und l�sst eine zeitverz�gerte Explosion zur�ck
        FIREBALL,           //Wirf einen Feuerball
        FIREBALL_ENHANCED,  //Wirft 3 Feuerb�lle (45�, 0�, -45�), die abprallen
        SPIN,               //Um die eigene Achse drehen
        HANDS_PLUS,         //Eine Hand fliegt von oben nach unten, eine von rechts nach links (nachdem sie f�r einige Sekunden der Position des Spielers folgen)
        HANDS_CROSS,        //Die H�nde fliegen sofort diagonal auf die Spielerposition zu
        HANDS_CATCH         //Die H�nde fliegen dem Spieler hinterher
    }
}