namespace DragAndDrop.ViewModels
{
    public enum ItemDragResults
    {
        Allowed,
        Denied
    }

    public enum DragResults
    {
        NoDrag,
        Drag,
        DragNoRemove,
    }

    public enum DropResults
    {
        NoDrop,
        Drop,
        DropNoAdd
    }

    public enum DoubleClickActions
    {
        DragDrop,
        Remove,
        Nothing
    }

    public interface IDragDroppable<T> where T : class
    {
        ItemDragResults IsItemDraggable(T item);
        DragResults IsDraggable(IDragDroppable<T> to, T item);
        DropResults IsDroppable(IDragDroppable<T> from, T item);
        DoubleClickActions DoubleClickAction(T item);

        IDragDroppable<T> DoubleClickTarget { get; }

        IDragDropActionManager<T> DragDropActionManager { get; }

        string Id { get; }
    }
}
