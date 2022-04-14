using System.Collections;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    /// <summary>
    /// Script for autoattacks
    /// on attack, attack nearest enemy and draw indicator line
    /// </summary>
    public class RPlayerBasicAttack : MonoBehaviour
    {
        [Header("Values")] 
        [SerializeField] private Vector3 offsetHitSphere = Vector3.zero;
        [SerializeField] private float range = 1.5f;
        [SerializeField] private LayerMask playerMask = new LayerMask();
        [SerializeField] private int damage = 1;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Vector3 offsetLROrigin = Vector3.zero;
        [SerializeField] private Vector3 offsetLRTarget = Vector3.zero;
        [SerializeField] private float attackCooldown = 0.5f;

        private float currentAttackCooldown = 0f;

        void Update()
        {
            if (currentAttackCooldown > 0f)
                currentAttackCooldown -= Time.deltaTime;
            else if (Input.GetMouseButtonDown(1))
            {
                RaycastHit[] hits = Physics.SphereCastAll(transform.position + offsetHitSphere, range, transform.forward, float.MaxValue, ~playerMask);
                RPlayerHealth nearestTarget = null;
                float nearestDistance = float.MaxValue;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.TryGetComponent<RPlayerHealth>(out RPlayerHealth current))
                    {
                        if (hits[i].distance < nearestDistance)
                        {
                            nearestDistance = hits[i].distance;
                            nearestTarget = hits[i].collider.GetComponent<RPlayerHealth>();
                        }
                    }
                }

                if (nearestTarget != null)
                {
                    StartCoroutine(IDrawLine(transform.position, nearestTarget.transform.position));
                    nearestTarget.TakeDamage(gameObject, damage);
                    currentAttackCooldown = attackCooldown;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + offsetHitSphere, range);
        }

        private IEnumerator IDrawLine(Vector3 a, Vector3 b)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, a + offsetLROrigin);
            lineRenderer.SetPosition(1, b + offsetLRTarget);
            yield return new WaitForSeconds(0.25f);
            lineRenderer.enabled = false;
        }
    }
}