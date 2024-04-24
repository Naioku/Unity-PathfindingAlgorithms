using System;
using Settings;

namespace UI.MenuPanels.Settings.Logic
{
    public class UILogicSettingNumeric<T> : UILogicSetting<T> where T : IComparable
    {
        private T minValue;
        private T maxValue;

        public override void SetValue(ISetting setting, Enums.SettingLoadingParam param)
        {
            base.SetValue(setting, param);
            SettingNumeric<T> settingNumeric = (SettingNumeric<T>)setting;

            minValue = settingNumeric.MinValue;
            maxValue = settingNumeric.MaxValue;
        }
        
        protected override void OnButtonPress()
        {
            AllManagers.Instance.UIManager.OpenPopupInput
            (
                $"{staticTextManager.GetValue(groupNameStaticKey)}: {staticTextManager.GetValue(Name)}",
                value,
                OnClosePanel,
                minValue,
                maxValue
            );
        }
    }
}