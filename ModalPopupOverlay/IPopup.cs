using System;

namespace ModalPopupOverlay
{
    public interface IPopup
    {
        Guid Guid { get; }
        void Close();
    }
}
