using System;
using UI.PopupPanels.EntryPanels;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public struct Language : IEntrySetting<Enums.Language>
    {
        [SerializeField] private Enums.Language language;

        public Enums.Language Enum => language;

        public Language(Enums.Language language) => this.language = language;
    }
}