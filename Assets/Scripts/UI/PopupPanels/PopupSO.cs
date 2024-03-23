using SpawningSystem;
using UnityEngine;

namespace UI.PopupPanels
{
    [CreateAssetMenu(fileName = "Popup", menuName = "Spawnable objects/Create popup", order = 1)]
    public class PopupSO : Spawnable<Enums.UIPopupType>
    {
        [SerializeField] private Enums.UIPopupType uiName;
        [SerializeField] private GameObject prefab;

        public override Enums.UIPopupType Name => uiName;
        public override GameObject GetPrefab(Enums.EmptyEnum type) => prefab;
    }
}