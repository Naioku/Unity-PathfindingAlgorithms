
namespace UI.HUDPanels
{
    public class HUDControllerAlgorithm : BaseHUDController<Enums.AlgorithmAction>
    {
        public Enums.MainMenuPanelButtonTag Header
        {
            set
            {
                if (header.Initialized)
                {
                    header.SetLocalizedTextKey(value);
                }
                else
                {
                    header.Initialize(value);
                }
            }
        }
        
        protected override void Initialize()
        {
            staticLabel.Initialize(Enums.GeneralText.HUDCurrentNodeLabel);
            dynamicLabel.Initialize(Enums.AlgorithmState.Initial);
        }

        public void UpdateCurrentStateLabel(Enums.AlgorithmState state) => dynamicLabel.SetLocalizedTextKey(state);
    }
}