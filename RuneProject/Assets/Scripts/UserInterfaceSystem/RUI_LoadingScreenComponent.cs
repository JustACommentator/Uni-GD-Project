using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuneProject.UserInterfaceSystem
{
    public class RUI_LoadingScreenComponent : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private bool loadFromCSV = false;

        [Header("References")]
        [SerializeField] private Image loadingDetailsImage = null;
        [SerializeField] private TMP_Text loadingHintTitleText = null;
        [SerializeField] private TMP_Text loadingHintDescriptionText = null;

        private const string LOADING_SCREEN_CSV_PATH = "CSVs/loadingScreenHints";
        private const string LOADING_IMAGE_PATH_PREFIX = "Images/loadingScreenImage";

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }

        private void Initialize()
        {
            if (loadFromCSV)
                StartCoroutine(IExecuteInitialization());            
        }

        private IEnumerator IExecuteInitialization()
        {
            ResourceRequest rr = Resources.LoadAsync<TextAsset>(LOADING_SCREEN_CSV_PATH);
            yield return rr;

            string[] lines = ((TextAsset)rr.asset).text.Split('\n');

            string[] parts = lines[Random.Range(0, lines.Length)].Split(';');
            loadingHintTitleText.text = parts[1];
            loadingHintDescriptionText.text = parts[2];

            //Aktuell: 1 festes Bild
            //loadingDetailsImage.sprite = Resources.Load<Sprite>($"{LOADING_IMAGE_PATH_PREFIX}{parts[0]}");
        }
    }
}