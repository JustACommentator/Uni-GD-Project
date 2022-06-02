using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerAnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator playerAnimator = null;
        [SerializeField] private RPlayerMovement movement = null;
        [SerializeField] private RPlayerDash dash = null;
        [SerializeField] private RPlayerBasicAttack basicAttack = null;
        [SerializeField] private RPlayerHealth playerHealth = null;

        private float lastTimeSinceMove = 0f;

        private void Start()
        {
            movement.OnMove += Movement_OnMove;
            dash.OnDash += Dash_OnDash;
            basicAttack.OnBeginCharge += BasicAttack_OnBeginCharge;
            basicAttack.OnEndCharge += BasicAttack_OnEndCharge;
            basicAttack.OnFireAutoAttack += BasicAttack_OnFireAutoAttack;
            playerHealth.OnDeath += PlayerHealth_OnDeath;
        }

        private void PlayerHealth_OnDeath(object sender, GameObject e)
        {
            playerAnimator.Play("Death");
            //Timescale-Sp��chen
        }

        private void BasicAttack_OnFireAutoAttack(object sender, System.EventArgs e)
        {
            playerAnimator.Play("CastSpell");
        }

        private void BasicAttack_OnEndCharge(object sender, System.EventArgs e)
        {
            playerAnimator.SetBool("charging", false);
        }

        private void BasicAttack_OnBeginCharge(object sender, System.EventArgs e)
        {
            playerAnimator.SetBool("charging", true);
        }

        private void Dash_OnDash(object sender, System.EventArgs e)
        {
            playerAnimator.Play("Roll");
        }

        private void Update()
        {
            lastTimeSinceMove += Time.deltaTime;

            if (lastTimeSinceMove > 0.01f)
                playerAnimator.SetFloat("inputMagnitude", 0f);
        }

        private void Movement_OnMove(object sender, Vector2 e)
        {
            playerAnimator.SetFloat("inputMagnitude", e.magnitude);
            lastTimeSinceMove = 0f;
        }
    }
}