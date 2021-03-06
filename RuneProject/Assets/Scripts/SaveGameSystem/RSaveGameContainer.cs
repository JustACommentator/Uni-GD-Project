using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;

namespace RuneProject.SaveGameSystem
{
    public class RSaveGameContainer : MonoBehaviour
    {
        private RSaveData saveData;

        private static RSaveGameContainer instance = null;

        public static RSaveGameContainer Instance { get { if (!instance) instance = FindObjectOfType<RSaveGameContainer>(); return instance; } }

        public RSaveData SaveData { get => saveData; set => saveData = value; }

        private void Awake()
        {
            Load();
        }

        public static void Save()
        {
            SaveGame.Save<RSaveData>(Application.persistentDataPath + "/saveData.sav", Instance.SaveData, true);
        }

        public static void Load()
        {
            Instance.SaveData = SaveGame.Load<RSaveData>(Application.persistentDataPath + "/saveData.sav", RSaveData.zero, true);
        }
    }

    [System.Serializable]
    public struct RSaveData
    {
        [Header("Options")]
        public bool rollInWalkingDirection;

        [Header("Progress")]
        public int currentLevel;
        public int currentXP;

        public static RSaveData zero => new RSaveData();
    }
}