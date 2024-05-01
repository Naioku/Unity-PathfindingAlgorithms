
namespace UI.HUDPanels
{
    public class HUDControllerAlgorithm : BaseHUDController<Enums.AlgorithmAction>
    {
        protected override void Initialize()
        {
            staticLabel.Initialize(Enums.GeneralText.HUDCurrentNodeLabel);
            dynamicLabel.Initialize(Enums.AlgorithmState.Initial);
        }

        public void UpdateCurrentStateLabel(Enums.AlgorithmState state) => dynamicLabel.SetLocalizedTextKey(state);
    }
}