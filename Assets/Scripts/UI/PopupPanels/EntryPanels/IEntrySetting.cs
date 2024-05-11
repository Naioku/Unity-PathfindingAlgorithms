using System;

namespace UI.PopupPanels.EntryPanels
{
    public interface IEntrySetting<T> where T : Enum
    {
        T Enum { get; }
    }
}