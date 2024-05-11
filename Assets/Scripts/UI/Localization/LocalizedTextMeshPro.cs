using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace UI.Localization
{
    [Serializable]
    public class LocalizedTextMeshPro
    {
        [SerializeField] private TextMeshProUGUI label;
    
        private LocalizedString localizedString;
        private LocalizedTextManager localizedTextManager;

        public bool Initialized => localizedString != null;

        public Color Color
        {
            set => label.color = value;
        }
        
        public void SetLocalizedTextKey<T>(T localizedTextKey) where T : Enum
        {
            if (localizedString == null)
            {
                localizedString = localizedTextManager.GetLocalizedString(localizedTextKey);
            }
            else
            {
                localizedTextManager.ChangeReference(localizedString, localizedTextKey);
            }
        }
        
        public void Initialize<T>(T localizedTextKey) where T : Enum
        {
            localizedTextManager = AllManagers.Instance.LocalizedTextManager;
            localizedString = localizedTextManager.GetLocalizedString(localizedTextKey);
            localizedString.StringChanged += OnStringChanged;
        }
    
        public void Initialize<T>(T localizedTextKey, object[] arguments) where T : Enum
        {
            Initialize(localizedTextKey);
            localizedString.Arguments = arguments;
        }
        
        public void Initialize<T>(T localizedTextKey, KeyValuePair<string, IVariable>[] persistentArguments) where T : Enum
        {
            Initialize(localizedTextKey);
            foreach (KeyValuePair<string, IVariable> argument in persistentArguments)
            {
                localizedString.Add(argument);
            }
        }

        public void Destroy()
        {
            if (localizedString == null) return;
            
            localizedString.StringChanged -= OnStringChanged;
        }

        private void OnStringChanged(string value)
        {
            if (label == null) return;
        
            label.text = value;
        }
    }
}