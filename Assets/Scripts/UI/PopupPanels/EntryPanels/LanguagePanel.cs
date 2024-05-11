using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace UI.PopupPanels.EntryPanels
{
    public class LanguagePanel : InputPanel<Language>
    {
        [SerializeField] private RectTransform content;
        
        private List<Enums.Language> entriesValues;
        private EntryLanguage currentEntry;
        
        public override GameObject SelectableOnOpen { get; }
        
        protected override void SetInitialValue(Language initialValue)
        {
            foreach (Enums.Language language in Enum.GetValues(initialValue.Enum.GetType()))
            {
                EntryLanguage entry = AllManagers.Instance.UIManager.UISpawner.CreateObject<EntryLanguage>(Enums.UISpawned.EntryLanguages, content);
                entry.Initialize(new Language(language), SetCurrentEntry);
                if (Equals(entry.Value, initialValue))
                {
                    SetCurrentEntry(entry);
                }
            }
        }

        protected override void Confirm() => onConfirm.Invoke(currentEntry.Value);

        private void SetCurrentEntry(Entry<Language, Enums.Language> entry)
        {
            // Todo: Is this if statement necessary?
            if (currentEntry != null)
            {
                currentEntry.Selected = false;
            }
            
            currentEntry = (EntryLanguage)entry;
            entry.Selected = true;
        }
    }
}