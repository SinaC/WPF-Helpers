namespace DragAndDrop.ViewModels
{
    public class FromViewModel : DragDropViewModelBase
    {
        public FromViewModel(IDragDropActionManager<string> manager)
            : base(manager)
        {
            Items.Add("first item");
            Items.Add("second item");
            Items.Add("third item");
            Items.Add("fourth item");
        }

        #region IDragDroppable<string>

        public override ItemDragResults IsItemDraggable(string item)
        {
            return ItemDragResults.Allowed;
        }

        public override DragResults IsDraggable(IDragDroppable<string> to, string item)
        {
            return DragResults.DragNoRemove;
        }

        public override DropResults IsDroppable(IDragDroppable<string> from, string item)
        {
            return DropResults.NoDrop;
        }

        public override DoubleClickActions DoubleClickAction(string item)
        {
            return DoubleClickActions.DragDrop;
        }

        public override IDragDroppable<string> DoubleClickTarget
        {
            get { return DragDropMapper<string>.GetFromId("To"); }
        }

        public override IDragDropActionManager<string> DragDropActionManager
        {
            get { return Manager; }
        }

        public override string Id
        {
            get { return "From"; }
        }

        #endregion
    }
}