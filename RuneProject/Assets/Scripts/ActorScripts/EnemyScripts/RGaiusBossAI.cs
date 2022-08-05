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
        [Space]
        [SerializeField] private Transform leftHand = null;
        [SerializeField] private Transform rightHand = null;
        [SerializeField] private Transform fireballSpawnPoint = null;
        [SerializeField] private List<Transform> additionalFireballSpawnPoints = new List<Transform>();
        [SerializeField] private List<Transform> potentialResidencePoints = new List<Transform>();

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
        private int currentResidenceIndex = 0;

        private const float ENRAGE_HP_PERCENTAGE = 0.5f;
        private const float SPIN_RANGE = 0.8f;
        private const float TRANSITION_TIME = 1f;
        private const float TURNAROUND_TIME = 0.35f;
        private const bool ALLOW_SAME_STATE_DEFAULT = true;
        private const bool ALLOW_SAME_STATE_ENRAGED = false;

        public event System.EventHandler<EGaiusBossState> OnStateChange;

        private void Start()
        {
            health.OnDamageTaken += Health_OnDamageTaken;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Health_OnDamageTaken(object sender, int e)
        {
            currentRemainingUpdateTimer -= currentRemainingUpdateTimer * Random.Range(percentageUpdateTimeDecreaseOnHit.x, percentageUpdateTimeDecreaseOnHit.y);
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

        private void HandleStateChange()
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

            if (health.CurrentHealth <= health.MaxHealth * ENRAGE_HP_PERCENTAGE)
            {                                
                do
                {
                    int randomIndex = Random.Range(0, 5);
                    selectedState = randomIndex == 0 ? EGaiusBossState.FIREBALL_ENHANCED :
                                    randomIndex == 1 ? EGaiusBossState.HANDS_PLUS :
                                    randomIndex == 2 ? EGaiusBossState.HANDS_CROSS :
                                    randomIndex == 3 ? EGaiusBossState.HANDS_CATCH :
                                    randomIndex == 4 ? EGaiusBossState.CHANGE_POSITION : 
                                    EGaiusBossState.WAIT;
                } while(selectedState == EGaiusBossState.WAIT || selectedState == lastState || (selectedState == lastNonWaitState && ALLOW_SAME_STATE_ENRAGED));
            }
            else
            {
                do
                {
                    int randomIndex = Random.Range(0, 2);
                    selectedState = randomIndex == 0 ? EGaiusBossState.FIREBALL :
                                    randomIndex == 1 ? EGaiusBossState.CHANGE_POSITION :
                                    EGaiusBossState.WAIT;
                } while (selectedState == EGaiusBossState.WAIT || selectedState == lastState || (selectedState == lastNonWaitState && ALLOW_SAME_STATE_DEFAULT));
            }

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

                        //Traverse to selected point
                        float currentTimer = 0f;
                        while (currentTimer <= TRANSITION_TIME)
                        {
                            currentTimer += Time.deltaTime;
                            transform.position = Vector3.Lerp(startPos, potentialResidencePoints[currentResidenceIndex].position, currentTimer / TRANSITION_TIME);
                            yield return null;
                        }

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
    }

    public enum EGaiusBossState
    {
        WAIT,               //Warte
        CHANGE_POSITION,    //Teleportiert auf eine andere Position und lässt eine zeitverzögerte Explosion zurück
        FIREBALL,           //Wirf einen Feuerball
        FIREBALL_ENHANCED,  //Wirft 3 Feuerbälle (45°, 0°, -45°), die abprallen
        SPIN,               //Um die eigene Achse drehen
        HANDS_PLUS,         //Eine Hand fliegt von oben nach unten, eine von rechts nach links (nachdem sie für einige Sekunden der Position des Spielers folgen)
        HANDS_CROSS,        //Die Hände fliegen sofort diagonal auf die Spielerposition zu
        HANDS_CATCH         //Die Hände fliegen dem Spieler hinterher
    }
}