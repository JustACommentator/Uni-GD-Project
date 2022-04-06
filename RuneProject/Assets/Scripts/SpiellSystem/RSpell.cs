using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.SpellSystem
{
    /// <summary>
    /// Handles the Spells that can be casted by the player.
    /// </summary>
    public class RSpell
    {
        private ERuneType runeType = ERuneType.THROW_G;
        private List<int> runeArguments = new List<int>();

        /// <summary>
        /// Creates a new Spell with set data.
        /// </summary>
        public RSpell(ERuneType _runeType, List<int> _args)
        {
            runeType = _runeType;
            runeArguments = _args;
        }

        /// <summary>
        /// Resolve the spell.
        /// </summary>
        public void Resolve()
        {
            switch (runeType)
            {
                //Wirf Xg (0) nach Yg (1)
                case ERuneType.THROW_G:
                    break;

                //Wirf Xg (0) nach Yi
                case ERuneType.THROW_I:
                    break;

                //Verwandle Xg (0) in Yg (1)
                case ERuneType.TRANSFORM:
                    break;

                //Erzeuge Xe (0)
                case ERuneType.CREATE_E:
                    break;

                //Erzeuge Xg (0)
                case ERuneType.CREATE_G:
                    break;

                //Erzeuge Xe (0) an Stelle Yg (1)
                case ERuneType.CREATE_E_AT_G:
                    break;

                //Erzeuge Xe (0) an Yi
                case ERuneType.CREATE_E_AT_I:
                    break;

                //Erzeuge Xg (0) an Yg (1)
                case ERuneType.CREATE_G_AT_G:
                    break;

                //Erzeuge Xg (0) an Yi
                case ERuneType.CREATE_G_AT_I:
                    break;

                //Führe Xg (0)
                case ERuneType.WIELD:
                    break;

                //Hetze Xg (0) auf Yg (1)
                case ERuneType.AGGRO:
                    break;

                //Wende Xe (0) auf Yg (1)
                case ERuneType.APPLY_ELEMENT:
                    break;

                //Lasse Xg (0) Yg (1) anziehen
                case ERuneType.PULL_G:
                    break;

                //Lasse Xi Yg (1) anzeigen
                case ERuneType.PULL_I:
                    break;
            }
        }
    }

    public enum ERuneType
    {
        THROW_G,
        THROW_I,
        TRANSFORM,
        CREATE_E,
        CREATE_G,
        CREATE_E_AT_G,
        CREATE_E_AT_I,
        CREATE_G_AT_G,
        CREATE_G_AT_I,
        WIELD,
        AGGRO,
        APPLY_ELEMENT,
        PULL_G,
        PULL_I
    }
}