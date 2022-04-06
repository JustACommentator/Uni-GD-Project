using RuneProject.SpellSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RuneProject.UserInterfaceSystem
{
    /// <summary>
    /// Handles the developer console to test functions at runtime.
    /// Open the console with ^.
    /// </summary>
    public class RUI_DeveloperConsole : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private bool disableOnBuild = false;
        [SerializeField] private KeyCode openConsoleKeyCode = KeyCode.Comma;

        [Header("References")]
        [SerializeField] private GameObject consoleParent = null;
        [SerializeField] private TMP_InputField consoleInputField = null;
        [SerializeField] private TMP_Text consoleResolveText = null;

        private float consoleTimerLeft = 0f;
        private bool consoleIsOpen = false;
        private bool isInInputField = false;

        private const float CONSOLE_TIMER = 5f;
        private const KeyCode ENTER_KEYCODE = KeyCode.Return;
        private const KeyCode ESCAPE_KEYCODE = KeyCode.Escape;

        private void Update()
        {
            HandleConsole();
            HandleConsoleCooldowns();
        }

        private void HandleConsole()
        {
            if (Input.GetKeyDown(openConsoleKeyCode) && !isInInputField && (Application.isEditor || disableOnBuild))
            {
                OpenConsole();
            }
            else if (Input.GetKeyDown(ENTER_KEYCODE) && isInInputField && (Application.isEditor || disableOnBuild))
            {
                ResolveInput();
            }
            else if (Input.GetKeyDown(ESCAPE_KEYCODE) && isInInputField && (Application.isEditor || disableOnBuild))
            {
                CloseConsole();
            }
        }

        private void HandleConsoleCooldowns()
        {
            if (consoleIsOpen && !isInInputField)
            {
                if (consoleTimerLeft >= 0f)
                    consoleTimerLeft -= Time.deltaTime;
                else
                    CloseConsole();
            }
        }

        private void ResolveInput()
        {
            consoleTimerLeft = CONSOLE_TIMER;
            isInInputField = false;
            consoleInputField.ReleaseSelection();
            consoleInputField.DeactivateInputField();

            string[] parts = consoleInputField.text.Split(' ');

            if (!parts[0].Equals("s"))
            {
                consoleResolveText.text = GetErrorMessage(0);
                return;
            }

            if (Enum.TryParse<ERuneType>(parts[1], out ERuneType runeType) && Enum.IsDefined(typeof(ERuneType), runeType))
            {
                int parameterCount = parts.Length - 2;
                if (parameterCount <= 0)
                {
                    consoleResolveText.text = GetErrorMessage(2);
                    return;
                }

                List<int> args = new List<int>();
                for (int i = 2; i < parts.Length; i++)
                {
                    if (int.TryParse(parts[i], out int current))                    
                        args.Add(current);
                    else
                    {
                        consoleResolveText.text = GetErrorMessage(-1);
                        return;
                    }
                }
                
                RSpell spell = new RSpell(runeType, args);                

                if (spell == null)
                {
                    consoleResolveText.text = GetErrorMessage(0);
                    return;
                }
                else
                {
                    spell.Resolve();
                }
            }
            else
            {
                consoleResolveText.text = GetErrorMessage(1);
                return;
            }

            consoleInputField.text = string.Empty;
        }

        private void OpenConsole()
        {
            consoleIsOpen = true;
            isInInputField = true;
            consoleParent.SetActive(true);
            consoleInputField.Select();
            consoleInputField.ActivateInputField();
        }

        private void CloseConsole()
        {
            consoleParent.SetActive(false);
            consoleInputField.text = string.Empty;
        }

        private string GetErrorMessage(int id)
        {
            switch (id)
            {
                case 0:
                    return "<color=red>InvalidCommandException</color>";
                case 1:
                    return "<color=red>RuneNotFoundException</color>";
                case 2: 
                    return "<color=red>MissingParameterException</color>";
            }

            return "<color=red>Unknown Error.</color>";
        }
    }
}