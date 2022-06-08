using RuneProject.ActorSystem;
using RuneProject.CameraSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class RPlayerRoomTrigger : MonoBehaviour
    {
        [SerializeField] private List<RPlayerHealth> enemies = new List<RPlayerHealth>();
        [SerializeField] private List<GameObject> doorBlockers = new List<GameObject>();

        public void SetEnemies(List<RPlayerHealth> _enemies)
        {
            enemies = new List<RPlayerHealth>(_enemies);            

            DisableEnemies();
        }

        public void SetDoorBlockers(List<GameObject> _doorBlockers)
        {
            doorBlockers = new List<GameObject>(_doorBlockers);
            UnlockRoom();
        }

        private void RPlayerRoomTrigger_OnDeath(object sender, GameObject e)
        {
            int enemiesLeft = enemies.Count;
            for (int i = 0; i < enemies.Count; i++)
                if (!enemies[i] || !enemies[i].IsAlive)
                    enemiesLeft--;

            if (enemiesLeft == 0)
                UnlockRoom();
        }

        private void LockRoom()
        {
            for (int i = 0; i < doorBlockers.Count; i++)
            {
                GameObject doorBlocker = doorBlockers[i];
                if (doorBlocker)
                    doorBlocker.SetActive(true);
            }            
        }

        private void UnlockRoom()
        {
            for (int i = 0; i < doorBlockers.Count; i++)
            {
                GameObject doorBlocker = doorBlockers[i];
                if (doorBlocker)
                    doorBlocker.SetActive(false);
            }
        }

        private void EnableEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;

                GameObject enemy = enemies[i].gameObject;
                if (enemy)
                    enemy.SetActive(true);

                enemies[i].OnDeath += RPlayerRoomTrigger_OnDeath;
            }            
        }

        private void DisableEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null) continue;

                GameObject enemy = enemies[i].gameObject;
                if (enemy)
                    enemy.SetActive(false);

                enemies[i].OnDeath -= RPlayerRoomTrigger_OnDeath;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                RPlayerCameraComponent.Singleton.EnterRoom(gameObject);
                
                EnableEnemies();

                if (enemies.Count > 0)
                    LockRoom();                             
            }
        }
    }
}