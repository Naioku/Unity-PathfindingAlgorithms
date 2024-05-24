using UI.Localization;
using UnityEngine;

namespace UI
{
    public class VersionInfoPanel : UIStaticPanel
    {
        [SerializeField] private LocalizedTextMeshPro versionLabel;

        private void Awake()
        {
            versionLabel.Initialize(Enums.GeneralText.Version, new object[]{ Application.version });
        }

        protected override void SelectDefaultButton(){}
    }
}