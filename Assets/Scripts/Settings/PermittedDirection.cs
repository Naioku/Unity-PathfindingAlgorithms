using System;
using UI.PopupPanels.EntryPanels;

namespace Settings
{
    [Serializable]
    public struct PermittedDirection : IEntrySetting<Enums.PermittedDirection>
    {
        public Enums.PermittedDirection direction;
        
        public bool Enabled;
        public Enums.PermittedDirection Enum => direction;

        public PermittedDirection(Enums.PermittedDirection permittedDirection, bool enabled)
        {
            direction = permittedDirection;
            Enabled = enabled;
        }
    }
}