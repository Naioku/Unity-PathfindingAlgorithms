using System;
using TMPro;
using UnityEngine;

namespace UI.HUDPanels
{
    public class HUDControllerMazeModification : BaseHUDController
    {
        [SerializeField] private TextMeshProUGUI currentNodeLabel;
        [SerializeField] private Button defaultNode;
        [SerializeField] private Button startNode;
        [SerializeField] private Button destinationNode;
        [SerializeField] private Button blockedNode;
        [SerializeField] private Button back;

        public void Initialize(
            Action defaultNodeAction,
            Action startNodeAction,
            Action destinationNodeAction,
            Action blockedNodeAction,
            Action backAction)
        {
            defaultNode.OnPressAction += defaultNodeAction;
            startNode.OnPressAction += startNodeAction;
            destinationNode.OnPressAction += destinationNodeAction;
            blockedNode.OnPressAction += blockedNodeAction;
            back.OnPressAction += backAction;
        }
        
        public void Deinitialize()
        {
            defaultNode.ResetObj();
            startNode.ResetObj();
            destinationNode.ResetObj();
            blockedNode.ResetObj();
            back.ResetObj();
        }

        public override void Show()
        {
            base.Show();
            defaultNode.Select();
            UpdateCurrentNodeLabel(Enums.TileType.Default);
        }

        public void UpdateCurrentNodeLabel(Enums.TileType tileType)
        {
            Color color = AllManagers.Instance.GameManager.GameDataSO.GetPermanentColor(tileType);
            currentNodeLabel.text = tileType.ToString();
            currentNodeLabel.color = color;
        }
    }
}