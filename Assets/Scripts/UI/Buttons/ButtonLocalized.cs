using System;
using System.Collections.Generic;
using UI.Localization;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace UI.Buttons
{
    public class ButtonLocalized : Button
    {
        [SerializeField] private LocalizedTextMeshPro localizedLabel;
        
        protected override Color LabelColor
        {
            set => localizedLabel.Color = value;
        }
        
        public void Initialize<T>(T localizedTextKey) where T : Enum =>
            localizedLabel.Initialize(localizedTextKey);
        
        public void Initialize<T>(T localizedTextKey, params object[] arguments) where T : Enum =>
            localizedLabel.Initialize(localizedTextKey, arguments);
        
        public void Initialize<T>(T localizedTextKey, params KeyValuePair<string, IVariable>[] persistentArguments) where T : Enum =>
            localizedLabel.Initialize(localizedTextKey, persistentArguments);
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            localizedLabel.Destroy();
        }
    }
}