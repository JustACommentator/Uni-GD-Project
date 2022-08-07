using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;

namespace RuneProject.SaveGameSystem
{
    public class RSaveGameContainer : MonoBehaviour
    {
        private RSaveData saveData = RSaveData.zero;

        private static RSaveGameContainer instance = null;

        public static RSaveGameContainer Instance { get { if (!instance) instance = FindObjectOfType<RSaveGameContainer>(); return instance; } }

        public RSaveData SaveData { get => saveData; set => saveData = value; }

        private void Awake()
        {
            Load();
        }

        public static void SetRoll(bool value)
        {
            instance.saveData.rollInWalkingDirection = value;
            Save();
        }

        public static void Save()
        {
            SaveGame.Save<RSaveData>(Application.persistentDataPath + "/saveData.sav", Instance.SaveData, true);
        }

        public static void Load()
        {
            Instance.SaveData = SaveGame.Exists(Application.persistentDataPath + "/saveData.sav") ?
                SaveGame.Load<RSaveData>(Application.persistentDataPath + "/saveData.sav", RSaveData.zero, true) : RSaveData.zero;
        }
    }

    [System.Serializable]
    public struct RSaveData
    {
        [Header("Options")]
        public bool rollInWalkingDirection;

        public static RSaveData zero => new RSaveData();
    }
}