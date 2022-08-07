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
        [SerializeField] private List<GameObject> minimapAdditionalMarkers = new List<GameObject>();
        [SerializeField] private GameObject minimapBase = null;
        [SerializeField] private SpriteRenderer minimapBaseSpriteRenderer = null;
        [SerializeField] private SpriteRenderer minimapNorthSpriteRenderer = null;
        [SerializeField] private SpriteRenderer minimapSouthSpriteRenderer = null;
        [SerializeField] private SpriteRenderer minimapEastSpriteRenderer = null;
        [SerializeField] private SpriteRenderer minimapWestSpriteRenderer = null;
        [Space]
        [SerializeField] private float cameraFOVMultiplier = 1f;
        [SerializeField] private AudioClip overrideMusic = null;

        private Color playerInRoomColor = new Color(0.96f, 0.43f, 0.19f);
        private Color playerOutRoomColor = Color.white;
        private bool waitingForUnload = false;
        private bool solved = false;
        private float unloadTimer = 0f;

        public event System.EventHandler OnClearRoom;

        public float CameraFOVMultiplier => cameraFOVMultiplier;
        public AudioClip OverrideMusic => overrideMusic;

        private const float MAX_UNLOAD_TIMER = 3f;

        public void SetMinimapBase(GameObject _minimapBase, bool _north, bool _south, bool _east, bool _west, List<GameObject> _additionalMarkers)
        {
            minimapBase = _minimapBase;
            minimapBase.transform.localPosition = Vector3.zero;
            minimapBase.SetActive(false);

            minimapBaseSpriteRenderer = minimapBase.transform.GetChild(0).GetComponent<SpriteRenderer>();
            minimapNorthSpriteRenderer = minimapBase.transform.GetChild(1).GetComponent<SpriteRenderer>();
            minimapSouthSpriteRenderer = minimapBase.transform.GetChild(2).GetComponent<SpriteRenderer>();
            minimapEastSpriteRenderer = minimapBase.transform.GetChild(3).GetComponent<SpriteRenderer>();
            minimapWestSpriteRenderer = minimapBase.transform.GetChild(4).GetComponent<SpriteRenderer>();

            minimapNorthSpriteRenderer.gameObject.SetActive(_north);
            minimapSouthSpriteRenderer.gameObject.SetActive(_south);
            minimapEastSpriteRenderer.gameObject.SetActive(_east);
            minimapWestSpriteRenderer.gameObject.SetActive(_west);

            minimapAdditionalMarkers = new List<GameObject>(_additionalMarkers);

            for (int i = 0; i < minimapAdditionalMarkers.Count; i++)            
                minimapAdditionalMarkers[i].transform.SetParent(minimapBase.transform);            
        }

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
                solved = true;
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

        private void EnableMinimap()
        {
            minimapBase.SetActive(true);

            for (int i = 0; i < minimapAdditionalMarkers.Count; i++)
                minimapAdditionalMarkers[i].SetActive(true);
        }

        private void SetMinimapEnter()
        {
            minimapBaseSpriteRenderer.color = playerInRoomColor;
            minimapNorthSpriteRenderer.color = playerInRoomColor;
            minimapSouthSpriteRenderer.color = playerInRoomColor;
            minimapEastSpriteRenderer.color = playerInRoomColor;
            minimapWestSpriteRenderer.color = playerInRoomColor;

            minimapNorthSpriteRenderer.sortingOrder = 2;
            minimapSouthSpriteRenderer.sortingOrder = 2;
            minimapEastSpriteRenderer.sortingOrder = 2;
            minimapWestSpriteRenderer.sortingOrder = 2;
        }

        private void SetMinimapExit()
        {
            minimapBaseSpriteRenderer.color = playerOutRoomColor;
            minimapNorthSpriteRenderer.color = playerOutRoomColor;
            minimapSouthSpriteRenderer.color = playerOutRoomColor;
            minimapEastSpriteRenderer.color = playerOutRoomColor;
            minimapWestSpriteRenderer.color = playerOutRoomColor; 

            minimapNorthSpriteRenderer.sortingOrder = 1;
            minimapSouthSpriteRenderer.sortingOrder = 1;
            minimapEastSpriteRenderer.sortingOrder = 1;
            minimapWestSpriteRenderer.sortingOrder = 1;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                RPlayerCameraComponent.Singleton.EnterRoom(this);

                if (overrideMusic != null)
                {
                    AudioSource source = GameObject.FindGameObjectWithTag("Manager").transform.GetChild(0).GetComponent<AudioSource>();
                    source.Stop();
                    source.clip = overrideMusic;
                    source.Play();
                }

                EnableEnemies();
                EnableMinimap();
                SetMinimapEnter();

                if (enemies.Count > 0 && !solved)
                    LockRoom();

                EnableRoomResources();
                waitingForUnload = false;
                unloadTimer = 0f;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                waitingForUnload = true;
                unloadTimer = 0f;
                SetMinimapExit();
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