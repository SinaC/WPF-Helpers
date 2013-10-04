namespace DragAndDrop.ViewModels
{
    public class ToViewModel : DragDropViewModelBase, IDragDroppable<string>
    {
        public ToViewModel(IDragDropActionManager<string> manager)
            : base(manager)
        {
            Items.Add("item");
        }

        #region IDragDroppable<string>

        public ItemDragResults IsItemDraggable(string item)
        {
            return ItemDragResults.Denied;
        }

        public DragResults IsDraggable(IDragDroppable<string> to, string item)
        {
            return DragResults.NoDrag;
        }

        public DropResults IsDroppable(IDragDroppable<string> @from, string item)
        {
            return DropResults.Drop;
        }

        public DoubleClickActions DoubleClickAction(string item)
        {
            return DoubleClickActions.Nothing;
        }

        public IDragDroppable<string> DoubleClickTarget
        {
            get { return null; }
        }

        public IDragDropActionManager<string> DragDropActionManager
        {
            get { return Manager; }
        }

        public string Id
        {
            get { return "To"; }
        }

        #endregion
    }
}