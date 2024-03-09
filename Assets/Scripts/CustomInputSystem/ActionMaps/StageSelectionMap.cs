using UnityEngine.InputSystem;

namespace CustomInputSystem.ActionMaps
{
    public class StageSelectionMap : ActionMap, Controls.IStageSelectionActions
    {
        public ActionData OnMazeModificationData { get; } = new ActionData();
        public ActionData OnBFSData { get; } = new ActionData();
        public ActionData OnAStarData { get; } = new ActionData();
        
        public StageSelectionMap(Controls.StageSelectionActions actionMap)
        {
            this.actionMap = actionMap.Get();
            actionMap.SetCallbacks(this);
        }

        public void OnMazeModification(InputAction.CallbackContext context) => OnMazeModificationData.Invoke(context.phase);
        public void OnBFS(InputAction.CallbackContext context) => OnBFSData.Invoke(context.phase);
        public void OnAStar(InputAction.CallbackContext context) => OnAStarData.Invoke(context.phase);
    }
}