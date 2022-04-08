using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.SpellSystem
{
    /// <summary>
    /// Handles the Spells that can be casted by the player.
    /// </summary>
    [System.Serializable]
    public class RSpell
    {
        private ERuneType runeType = ERuneType.THROW_G;
        private List<int> runeArguments = new List<int>();

        public ERuneType RuneType { get => runeType; }
        public List<int> RuneArguments { get => runeArguments; }

        /// <summary>
        /// Creates a new Spell with set data.
        /// </summary>
        public RSpell(ERuneType _runeType, List<int> _args)
        {
            runeType = _runeType;
            runeArguments = _args;
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
        PULL_I,
        TELEPORT_I
    }
}