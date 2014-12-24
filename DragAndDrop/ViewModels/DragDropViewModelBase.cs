using System.Collections.ObjectModel;

namespace DragAndDrop.ViewModels
{
    public abstract class DragDropViewModelBase: IDragDroppable<string>
    {
        public IDragDropActionManager<string> Manager { get; private set; }
        public ObservableCollection<string> Items { get; set; }

        protected DragDropViewModelBase(IDragDropActionManager<string> manager)
        {
            Manager = manager;

            Items = new ObservableCollection<string>();

            DragDropMapper<string>.Register(this);
        }

        #region IDragDroppable<string>

        public abstract ItemDragResults IsItemDraggable(string item);
        public abstract DragResults IsDraggable(IDragDroppable<string> to, string item);
        public abstract DropResults IsDroppable(IDragDroppable<string> from, string item);
        public abstract DoubleClickActions DoubleClickAction(string item);

        public abstract IDragDroppable<string> DoubleClickTarget { get; }

        public abstract IDragDropActionManager<string> DragDropActionManager { get; }

        public abstract string Id { get; }

        #endregion
    }
}
