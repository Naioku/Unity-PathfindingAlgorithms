using TMPro;
using UnityEngine;

namespace UI.HUDPanels
{
    public class HUDControllerAlgorithm : BaseHUDController<Enums.AlgorithmAction>
    {
        [SerializeField] protected TextMeshProUGUI currentStateLabel;

        public override void Show()
        {
            base.Show();
            UpdateCurrentStateLabel(Enums.AlgorithmState.Initial);
        }

        public void UpdateCurrentStateLabel(Enums.AlgorithmState currentState) => currentStateLabel.text = currentState.ToString();
    }
}