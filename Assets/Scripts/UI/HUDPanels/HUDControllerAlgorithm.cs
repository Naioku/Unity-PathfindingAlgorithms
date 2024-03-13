using System;
using TMPro;
using UnityEngine;

namespace UI.HUDPanels
{
    public class HUDControllerAlgorithm : BaseHUDController
    {
        [SerializeField] private TextMeshProUGUI currentStateLabel;
        [SerializeField] private Button play;
        [SerializeField] private Button pause;
        [SerializeField] private Button step;
        [SerializeField] private Button stop;
        [SerializeField] private Button back;

        public void Initialize(
            Action playAction,
            Action pauseAction,
            Action stepAction,
            Action stopAction,
            Action backAction)
        {
            play.OnPressAction += playAction;
            pause.OnPressAction += pauseAction;
            step.OnPressAction += stepAction;
            stop.OnPressAction += stopAction;
            back.OnPressAction += backAction;
        }

        public void Deinitialize()
        {
            play.ResetObj();
            pause.ResetObj();
            step.ResetObj();
            stop.ResetObj();
            back.ResetObj();
        }
        
        public override void Show()
        {
            base.Show();
            play.Select();
            UpdateCurrentStateLabel(Enums.AlgorithmState.Initial);
        }

        public void UpdateCurrentStateLabel(Enums.AlgorithmState currentState) => currentStateLabel.text = currentState.ToString();
    }
}