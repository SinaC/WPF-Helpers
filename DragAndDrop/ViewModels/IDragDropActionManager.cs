namespace DragAndDrop.ViewModels
{
    public enum DragDropActions
    {
        Add,
        Remove,
        Move
    }

    public interface IDragDropActionManager<T> where T : class
    {
        void PerformAction(IDragDroppable<T> from, IDragDroppable<T> to, T item, DragDropActions action);
    }
}
