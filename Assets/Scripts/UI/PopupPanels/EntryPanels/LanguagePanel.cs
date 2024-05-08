using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.PopupPanels.EntryPanels
{
    public class LanguagePanel : InputPanel<Enums.Language>
    {
        [SerializeField] private RectTransform content;
        
        private List<Enums.Language> entriesValues;
        private Entry<Enums.Language> currentEntry;
        
        public override GameObject SelectableOnOpen { get; }
        
        protected override void SetInitialValue(Enums.Language initialValue)
        {
            foreach (Enums.Language language in Enum.GetValues(initialValue.GetType()))
            {
                Entry<Enums.Language> entry = AllManagers.Instance.UIManager.UISpawner.CreateObject<Entry<Enums.Language>>(Enums.UISpawned.EntryLanguages, content);
                entry.Initialize(language, SetCurrentEntry);
                if (Equals(entry.Value, initialValue))
                {
                    SetCurrentEntry(entry);
                }
            }
        }

        protected override void Confirm() => onConfirm.Invoke(currentEntry.Value);

        private void SetCurrentEntry(Entry<Enums.Language> entry)
        {
            if (currentEntry != null)
            {
                currentEntry.Selected = false;
            }
            currentEntry = entry;
            entry.Selected = true;
        }
    }
}