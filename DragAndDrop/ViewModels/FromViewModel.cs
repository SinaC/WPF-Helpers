namespace DragAndDrop.ViewModels
{
    public class FromViewModel : DragDropViewModelBase, IDragDroppable<string>
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

        public ItemDragResults IsItemDraggable(string item)
        {
            return ItemDragResults.Allowed;
        }

        public DragResults IsDraggable(IDragDroppable<string> to, string item)
        {
            return DragResults.DragNoRemove;
        }

        public DropResults IsDroppable(IDragDroppable<string> @from, string item)
        {
            return DropResults.NoDrop;
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
            get { return "From"; }
        }

        #endregion
    }
}