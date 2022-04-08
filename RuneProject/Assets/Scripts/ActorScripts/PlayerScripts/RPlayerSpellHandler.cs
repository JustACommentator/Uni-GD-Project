using RuneProject.ItemSystem;
using RuneProject.LibrarySystem;
using RuneProject.SpellSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    /// <summary>
    /// Handles the player's spell using.
    /// </summary>
    public class RPlayerSpellHandler : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private float spellMaxRadius = 7f;
        [SerializeField] private ESpellTargetSelectionType targetSelectionType = ESpellTargetSelectionType.ALL_IN_RADIUS;
        [SerializeField] private Vector3 defaultInstantiateOffset = Vector3.forward;
        [SerializeField] private float instantiateTumbleSpinForce = 150f;
        [SerializeField] private int instantiateLimit = 30;

        [Header("References")]
        [SerializeField] private Transform characterTransform = null;
        [SerializeField] private Camera mainCamera = null;

        [Header("Prefabs")]
        [SerializeField] private GameObject playerTeleportIndicatorPrefab = null;

        private Queue<GameObject> instantiatedObjects = new Queue<GameObject>();

        public void ResolveSpell(RSpell spell)
        {
            StartCoroutine(IResolveSpell(spell));
        }

        private IEnumerator IResolveSpell(RSpell spell)
        {
            yield return null;

            switch (spell.RuneType)
            {
                //Wirf Xg (0) nach Yg (1)
                case ERuneType.THROW_G:

                    break;

                //Wirf Xg (0) nach Yi
                case ERuneType.THROW_I:

                    break;

                //Verwandle Xg (0) in Yg (1)
                case ERuneType.TRANSFORM:

                    break;

                //Erzeuge Xe (0)
                case ERuneType.CREATE_E:
                    
                    break;

                //Erzeuge Xg (0)
                case ERuneType.CREATE_G:
                    {
                        RWorldItem instantiateTarget = RItemIdentifierLibrary.GetItem(spell.RuneArguments[0]);
                        RWorldItem instance = Instantiate(instantiateTarget, transform.position
                            + characterTransform.forward * defaultInstantiateOffset.z
                            + Vector3.up * defaultInstantiateOffset.y
                            + characterTransform.right * defaultInstantiateOffset.x
                            + instantiateTarget.SpawnOffset,
                            instantiateTarget.DontRotateOnInstantiate ? Quaternion.identity : RandomRotation());

                        if (!instantiateTarget.DontTumbleOnInstantiate)
                            instance.Rigidbody.angularVelocity = Random.insideUnitSphere * instantiateTumbleSpinForce;

                        instantiatedObjects.Enqueue(instance.gameObject);
                        if (instantiatedObjects.Count > instantiateLimit)
                            Destroy(instantiatedObjects.Dequeue());
                    }
                    break;

                //Erzeuge Xe (0) an Stelle Yg (1)
                case ERuneType.CREATE_E_AT_G:

                    break;

                //Erzeuge Xe (0) an Yi
                case ERuneType.CREATE_E_AT_I:

                    break;

                //Erzeuge Xg (0) an Yg (1)
                case ERuneType.CREATE_G_AT_G:

                    break;

                //Erzeuge Xg (0) an Yi
                case ERuneType.CREATE_G_AT_I:
                    {
                        RWorldItem instantiateTarget = RItemIdentifierLibrary.GetItem(spell.RuneArguments[0]);
                        bool foundTarget = false;
                        RWorldItem placeholderInstance = Instantiate(instantiateTarget);
                        placeholderInstance.SetAsPreview();
                        while (!foundTarget)
                        {
                            Vector3 placeholderPos = characterTransform.position + defaultInstantiateOffset;
                            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                            if (Physics.Raycast(ray, out RaycastHit hit))                            
                                placeholderPos = hit.point;
                            
                            //OK-Check
                            placeholderInstance.transform.position = placeholderPos;

                            if (Input.GetMouseButtonDown(0))
                            {
                                Destroy(placeholderInstance.gameObject);
                                RWorldItem instance = Instantiate(instantiateTarget, placeholderPos + instantiateTarget.SpawnOffset,
                                    instantiateTarget.DontRotateOnInstantiate ? Quaternion.identity : RandomRotation());

                                if (!instantiateTarget.DontTumbleOnInstantiate)
                                    instance.Rigidbody.angularVelocity = Random.insideUnitSphere * instantiateTumbleSpinForce;

                                instantiatedObjects.Enqueue(instance.gameObject);
                                if (instantiatedObjects.Count > instantiateLimit)
                                    Destroy(instantiatedObjects.Dequeue());

                                foundTarget = true;
                            }

                            yield return null;
                        }
                    }
                    break;

                //Führe Xg (0)
                case ERuneType.WIELD:

                    break;

                //Hetze Xg (0) auf Yg (1)
                case ERuneType.AGGRO:

                    break;

                //Wende Xe (0) auf Yg (1)
                case ERuneType.APPLY_ELEMENT:

                    break;

                //Lasse Xg (0) Yg (1) anziehen
                case ERuneType.PULL_G:

                    break;

                //Lasse Xi Yg (1) anzeigen
                case ERuneType.PULL_I:

                    break;

                //Teleportiere den Spieler zu Yi
                case ERuneType.TELEPORT_I:
                    {
                        GameObject placeholderInstance = Instantiate(playerTeleportIndicatorPrefab, transform.position, characterTransform.rotation);
                        bool foundTarget = false;
                        while (!foundTarget)
                        {
                            Vector3 placeholderPos = characterTransform.position;
                            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                            if (Physics.Raycast(ray, out RaycastHit hit))
                                placeholderPos = hit.point;

                            //OK-Check
                            placeholderInstance.transform.position = placeholderPos;
                            placeholderInstance.transform.rotation = characterTransform.rotation;

                            if (Input.GetMouseButtonDown(0))
                            {
                                transform.position = placeholderInstance.transform.position;
                                Destroy(placeholderInstance.gameObject);

                                foundTarget = true;
                            }

                            yield return null;
                        }
                    }
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (targetSelectionType == ESpellTargetSelectionType.ALL_IN_RADIUS)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, spellMaxRadius);
            }
        }

        private Quaternion RandomRotation()
        {
            return Quaternion.Euler(Random.insideUnitSphere * 360f);
        }
    }

    public enum ESpellTargetSelectionType
    {
        ALL,
        ALL_IN_RADIUS,
        NEAREST
    }
}