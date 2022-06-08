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
        [SerializeField] private List<GameObject> roomResources = new List<GameObject>();

        private bool waitingForUnload = false;
        private float unloadTimer = 0f;
        public event System.EventHandler OnClearRoom;

        private const float MAX_UNLOAD_TIMER = 3f;

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

        public void SetRoomResources(List<GameObject> _roomResources)
        {
            roomResources = new List<GameObject>(_roomResources);
            DisableRoomResources();
        }

        private void RPlayerRoomTrigger_OnDeath(object sender, GameObject e)
        {
            int enemiesLeft = enemies.Count;
            for (int i = 0; i < enemies.Count; i++)
                if (!enemies[i] || !enemies[i].IsAlive)
                    enemiesLeft--;

            if (enemiesLeft == 0)
            {
                UnlockRoom();
                OnClearRoom?.Invoke(this, null);
            }
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

        private void EnableRoomResources()
        {
            for (int i = 0; i < roomResources.Count; i++)
            {
                GameObject resource = roomResources[i];
                if (resource)
                    resource.SetActive(true);
            }
        }

        private void DisableRoomResources()
        {
            for (int i = 0; i < roomResources.Count; i++)
            {
                GameObject resource = roomResources[i];
                if (resource)
                    resource.SetActive(false);
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

                EnableRoomResources();
                waitingForUnload = false;
                unloadTimer = 0f;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                waitingForUnload = true;
                unloadTimer = 0f;
            }
        }

        private void Update()
        {
            if (waitingForUnload)
            {
                if (unloadTimer < MAX_UNLOAD_TIMER)
                    unloadTimer += Time.deltaTime;
                else
                    DisableRoomResources();
            }
        }
    }
}