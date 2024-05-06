using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.PopupPanels
{
    public class DirectionPanel : InputPanel<Enums.PermittedDirection[]>
    {
        [SerializeField] private RectTransform content;
        
        private List<Enums.PermittedDirection> directions;
        private bool inChangingState;
        private DirectionEntry entryToMove;
        
        public override GameObject SelectableOnOpen { get; }
        
        protected override void SetInitialValue(Enums.PermittedDirection[] initialValue)
        {
            directions = initialValue.ToList();
            foreach (Enums.PermittedDirection direction in directions)
            {
                DirectionEntry directionEntry = AllManagers.Instance.UIManager.UISpawner.CreateObject<DirectionEntry>(Enums.UISpawned.DirectionEntry, content);
                directionEntry.Initialize(direction, HandleOnPress);
            }
        }

        protected override void Confirm() => onConfirm.Invoke(directions.ToArray());

        private void HandleOnPress(DirectionEntry entry)
        {
            inChangingState = !inChangingState;
            if (inChangingState)
            {
                RemoveInput();
                confirmationButton.interactable = false;
                closeButton.interactable = false;
                entry.Selected = true;
                entryToMove = entry;
            }
            else
            {
                if (entry != entryToMove)
                {
                    int newIndex = directions.IndexOf(entry.Direction);
                    directions.Remove(entryToMove.Direction);
                    directions.Insert(newIndex, entryToMove.Direction);
                    entryToMove.transform.SetSiblingIndex(newIndex);
                }
                
                entryToMove.Selected = false;
                entryToMove = null;
                
                AddInput();
                confirmationButton.interactable = true;
                closeButton.interactable = true;
            }
        }
    }
}