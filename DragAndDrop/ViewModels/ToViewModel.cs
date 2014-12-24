namespace DragAndDrop.ViewModels
{
    public class ToViewModel : DragDropViewModelBase
    {
        public ToViewModel(IDragDropActionManager<string> manager)
            : base(manager)
        {
            Items.Add("item");
        }

        #region IDragDroppable<string>

        public override ItemDragResults IsItemDraggable(string item)
        {
            return ItemDragResults.Allowed; // needed to allow double click remove
        }

        public override DragResults IsDraggable(IDragDroppable<string> to, string item)
        {
            return DragResults.NoDrag;
        }

        public override DropResults IsDroppable(IDragDroppable<string> from, string item)
        {
            return DropResults.Drop;
        }

        public override DoubleClickActions DoubleClickAction(string item)
        {
            return DoubleClickActions.Remove;
        }

        public override IDragDroppable<string> DoubleClickTarget
        {
            get { return null; }
        }

        public override IDragDropActionManager<string> DragDropActionManager
        {
            get { return Manager; }
        }

        public override string Id
        {
            get { return "To"; }
        }

        #endregion
    }
}