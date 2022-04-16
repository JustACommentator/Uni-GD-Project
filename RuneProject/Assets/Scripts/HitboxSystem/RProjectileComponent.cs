using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.HitboxSystem
{
    public class RProjectileComponent : RHitboxComponent
    {
        [Header("Projectile References")]
        [SerializeField] private Rigidbody projectileRigidbody = null;

        [Header("Projectile Values")]
        [SerializeField] private Vector3 flightDirection = Vector3.forward;
        [SerializeField] private float flightSpeed = 3f;
        [SerializeField] private bool isPiercing = false;
        [SerializeField] private bool applyForceContinually = false;

        public Vector3 FlightDirection { get => flightDirection; set => flightDirection = value; }
        public float FlightSpeed { get => flightSpeed; set => flightSpeed = value; }
        public bool IsPiercing { get => isPiercing; set => isPiercing = value; }
        public bool ApplyForceContinually { get => applyForceContinually; set => applyForceContinually = value; }

        private void Start()
        {
            flightDirection.Normalize();
            projectileRigidbody.AddForce(transform.right * flightDirection.x + Vector3.up * flightDirection.y + transform.forward * flightDirection.z, 
                applyForceContinually ? ForceMode.Acceleration : ForceMode.VelocityChange);
        }

        private void Update()
        {
            if (applyForceContinually)
                projectileRigidbody.AddForce(transform.right * flightDirection.x + Vector3.up * flightDirection.y + transform.forward * flightDirection.z, ForceMode.Acceleration);
        }

        protected override void OnTriggerStay(Collider other)
        {
            base.OnTriggerStay(other);

            if (other.gameObject != owner && !isPiercing)
                Destroy(gameObject);
        }
    }
}