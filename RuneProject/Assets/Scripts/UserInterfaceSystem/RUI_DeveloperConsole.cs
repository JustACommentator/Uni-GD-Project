using RuneProject.ActorSystem;
using RuneProject.LibrarySystem;
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
        [SerializeField] private RPlayerInventory playerInventory = null;
        [SerializeField] private RPlayerSpellHandler spellHandler = null;
        [SerializeField] private RPlayerMovement playerMovement = null;
        [SerializeField] private RPlayerHealth playerHealth = null;
        [SerializeField] private GameObject consoleParent = null;
        [SerializeField] private TMP_InputField consoleInputField = null;
        [SerializeField] private TMP_Text consoleResolveText = null;

        private float consoleTimerLeft = 0f;
        private string lastInput = string.Empty;
        private bool consoleIsOpen = false;
        private bool isInInputField = false;
        private bool blockedMovement = false;

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
            if (Input.GetKeyDown(openConsoleKeyCode) && !isInInputField && (Application.isEditor || !disableOnBuild))
            {
                OpenConsole();
            }
            else if (Input.GetKeyDown(ENTER_KEYCODE) && isInInputField && (Application.isEditor || !disableOnBuild))
            {
                ResolveInput();
            }
            else if (Input.GetKeyDown(ESCAPE_KEYCODE) && isInInputField && (Application.isEditor || !disableOnBuild))
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

            if (parts[0].Equals("s")) 
            {
                if (Enum.TryParse<ERuneType>(parts[1], out ERuneType runeType) && Enum.IsDefined(typeof(ERuneType), runeType))
                {
                    int parameterCount = parts.Length - 2;
                    if (parameterCount <= 0 && runeType != (ERuneType.TELEPORT_I))
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
                        spellHandler.ResolveSpell(spell);
                        consoleResolveText.text = $"Resolving spell {spell.RuneType} with arguments ";

                        foreach (int i in spell.RuneArguments)
                            consoleResolveText.text += $"{i} ";

                        lastInput = consoleInputField.text;
                    }
                }
                else
                {
                    consoleResolveText.text = GetErrorMessage(1);
                    return;
                }
            }
            else if (parts[0].Equals("a"))
            {
                if (parts.Length <= 1)
                {
                    consoleResolveText.text = GetErrorMessage(2);
                    return;
                }
                else if (int.TryParse(parts[1], out int index))
                {
                    playerInventory.AddPowerUp(RItemIdentifierLibrary.GetPowerUpItem(index));
                    consoleResolveText.text = $"Adding Powerup of ID {index}";
                    lastInput = consoleInputField.text;
                }
                else
                {
                    consoleResolveText.text = GetErrorMessage(2);
                    return;
                }
            }
            else if (consoleInputField.text.Equals("l"))
            {
                if (string.IsNullOrEmpty(lastInput))
                    consoleResolveText.text = GetErrorMessage(3);
                else
                {
                    consoleInputField.text = lastInput;
                    ResolveInput();
                    return;
                }
            }
            else if (consoleInputField.text.Equals("k"))
            {
                playerHealth.ForceKill();
                consoleResolveText.text = $"Killing Player";
                return;
            }
            else
            {
                consoleResolveText.text = GetErrorMessage(0);
                return;
            }

            consoleInputField.text = string.Empty;

            if (blockedMovement)
            {
                blockedMovement = false;
                playerMovement.UnBlockMovementInput();
            }
        }

        private void OpenConsole()
        {
            consoleIsOpen = true;
            isInInputField = true;
            consoleParent.SetActive(true);
            consoleInputField.Select();
            consoleInputField.ActivateInputField();

            if (!blockedMovement)
            {
                blockedMovement = true;
                playerMovement.BlockMovementInput();
            }
        }

        private void CloseConsole()
        {
            consoleParent.SetActive(false);
            consoleInputField.text = string.Empty;

            if (blockedMovement)
            {
                blockedMovement = false;
                playerMovement.UnBlockMovementInput();
            }
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
                case 3:
                    return "<color=red>NoValidLastInputException</color>";
            }

            return "<color=red>Unknown Error.</color>";
        }
    }
}