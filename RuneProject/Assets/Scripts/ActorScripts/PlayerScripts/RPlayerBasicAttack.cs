using RuneProject.ActorSystem;
using UnityEngine;

public class RPlayerBasicAttack : MonoBehaviour
{
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float radius = 1;
    [SerializeField] private float range = 5;
    [SerializeField] private LayerMask playerMask = new LayerMask();
    [SerializeField] private int damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position + offset, radius, transform.forward, range, ~playerMask);
            RPlayerHealth nearestTarget = null;
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.TryGetComponent<RPlayerHealth>(out RPlayerHealth current))
                {
                    if (hits[i].distance < nearestDistance) ;
                    {
                        nearestDistance = hits[i].distance;
                        nearestTarget = hits[i].collider.GetComponent<RPlayerHealth>();
                    }
                }
            }
            if (nearestTarget != null)
            { 
                nearestTarget.TakeDamage(gameObject, damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color  = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, radius);
    }
}
