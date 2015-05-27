using System.Windows;
using System.Windows.Input;

namespace SampleWPF.Views.Popups
{
    public interface ISaveNavigationAndFocusPopup
    {
        KeyboardNavigationMode SavedTabNavigationMode { get; set; }
        KeyboardNavigationMode SavedDirectionalNavigationMode { get; set; }
        IInputElement SavedFocusedElement { get; set; }
    }
}
