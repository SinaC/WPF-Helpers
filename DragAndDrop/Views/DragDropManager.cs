using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DragAndDrop.ViewModels;

namespace DragAndDrop.Views
{
    // Control accepting drag&drop must have a minimal size and a background
    public class DragDropManager<T> where T : class
    {
        #region Singleton
        // singleton
        private DragDropManager()
        {
        }
        private static DragDropManager<T> _instance;
        public static DragDropManager<T> Instance
        {
            get { return _instance ?? (_instance = new DragDropManager<T>()); }
        }
        #endregion

        #region Drag&Drop datas
        private const string DragDataKey = "Item";

        private class DragDropData
        {
            public IDragDroppable<T> Source { get; set; }
            public T Item { get; set; }
        }

        private DragDropData _data;
        private Window _topWindow;
        private Point _initialMousePosition;
        #endregion

        #region Events Handler
        // DragSource
        public void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _topWindow = Window.GetWindow((DependencyObject)sender);
            _initialMousePosition = e.GetPosition(_topWindow);

            _data = GetDragDropData(e.Source, (DependencyObject)e.OriginalSource);
        }

        // Drag = mouse down + move by a certain amount
        public void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_data != null)
            {
                // Only drag when user moved the mouse by a reasonable amount
                Point position = e.GetPosition(_topWindow);
                if (Math.Abs(position.X - _initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    //
                    DataObject dragData = new DataObject(DragDataKey, _data);
                    DragDrop.DoDragDrop((DependencyObject)e.OriginalSource, dragData, DragDropEffects.Move);

                    //
                    _data = null;
                }
            }
        }

        public void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _data = null;
        }

        // Double click
        public void DragSource_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DragDropData data = GetDragDropData(e.Source, (DependencyObject)e.OriginalSource);
            if (data != null)
            {
                IDragDroppable<T> source = data.Source; // get source from args
                T item = data.Item;

                DoubleClickActions action = source.DoubleClickAction(item);
                if (action == DoubleClickActions.DragDrop)
                {
                    IDragDroppable<T> target = source.DoubleClickTarget;
                    PerformDragDrop(source, target, item);
                }
                else if (action == DoubleClickActions.Remove)
                {
                    //source.Remove(item);
                    IDragDropActionManager<T> sourceDragDropActionManager = source.DragDropActionManager;
                    sourceDragDropActionManager.PerformAction(source, null, item, DragDropActions.Remove);
                }
            }
            e.Handled = true;
        }

        // DropTarget
        public void DropTarget_DragEnter(object sender, DragEventArgs e)
        {
            if (_data != null)
            {
                IDragDroppable<T> dragDropObject = GetDragDroppableObject(sender);
                if (dragDropObject != null)
                    if (dragDropObject.IsDroppable(_data.Source, _data.Item) == DropResults.NoDrop)
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
            }
        }

        public void DropTarget_DragOver(object sender, DragEventArgs e)
        {
            if (_data != null)
            {
                IDragDroppable<T> dragDropObject = GetDragDroppableObject(sender);
                if (dragDropObject != null)
                    if (dragDropObject.IsDroppable(_data.Source, _data.Item) == DropResults.NoDrop)
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
            }
        }

        public void DropTarget_DragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        public void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DragDataKey))
            {
                DragDropData data = e.Data.GetData(DragDataKey) as DragDropData;
                FrameworkElement targetElement = e.Source as FrameworkElement;
                if (data != null && targetElement != null)
                {
                    IDragDroppable<T> source = data.Source; // get source from args
                    IDragDroppable<T> target = targetElement.DataContext as IDragDroppable<T>; // get target from args
                    T item = data.Item; // get item from args
                    PerformDragDrop(source, target, item);
                }
            }
            e.Handled = true;
        }

        #endregion

        #region Drag&Drop logic
        public void PerformDragDrop(IDragDroppable<T> source, IDragDroppable<T> target, T item)
        {
            if (source == null || target == null || item == null)
                return;
            if (source == target) // useless drag/drop ==> may cause bug with double click (double click event generates PreviewMouseLeftButtonDown+Drop+MouseDoubleClick)
                return;
            DragResults isDraggable = source.IsDraggable(target, item);
            DropResults isDroppable = target.IsDroppable(source, item);
            if (isDraggable != DragResults.NoDrag && isDroppable != DropResults.NoDrop)
            {
                IDragDropActionManager<T> sourceDragDropActionManager = source.DragDropActionManager;
                IDragDropActionManager<T> targetDragDropActionManager = target.DragDropActionManager;
                //if (isDraggable != DragResults.DragNoRemove)
                //    source.Remove(item);
                //if (isDroppable != DropResults.DropNoAdd)
                //    target.Add(item);
                /*
                                    NoDrop      Drop    DropNoAdd
                    NoDrag          /           /       /     
                    Drag            /           Move    Del
                    DragNoRemove    /           Add     /
                 */
                if (isDroppable == DropResults.Drop)
                {
                    if (isDraggable == DragResults.Drag)
                    {
                        if (sourceDragDropActionManager != targetDragDropActionManager)
                        {
                            sourceDragDropActionManager.PerformAction(source, null, item, DragDropActions.Remove);
                            targetDragDropActionManager.PerformAction(null, target, item, DragDropActions.Add);
                        }
                        else
                        {
                            sourceDragDropActionManager.PerformAction(source, target, item, DragDropActions.Move); // source action manager and target action manager are equals
                        }
                    }
                    else if (isDraggable == DragResults.DragNoRemove)
                        sourceDragDropActionManager.PerformAction(null, target, item, DragDropActions.Add);
                }
                else if (isDroppable == DropResults.DropNoAdd)
                    if (isDraggable == DragResults.Drag)
                        targetDragDropActionManager.PerformAction(source, null, item, DragDropActions.Remove);
            }
        }

        private static DragDropData GetDragDropData(object eventSource, DependencyObject eventOriginalSource)
        {
            IDragDroppable<T> source = null;
            T item = null;

            if (eventSource is DataGrid)
            {
                DataGrid grid = eventSource as DataGrid;
                source = grid.DataContext as IDragDroppable<T>;
                item = FindItemInContainer<DataGrid, DataGridRow>(grid, eventOriginalSource, row => row.DataContext as T);
            }
            else if (eventSource is ListBox) // works with ListView (which inherits from ListBox)
            {
                ListBox lst = eventSource as ListBox;
                source = lst.DataContext as IDragDroppable<T>;
                item = FindItemInContainer<ListBox, ListBoxItem>(lst, eventOriginalSource,
                                                          lbi =>
                                                          lst.ItemContainerGenerator.ItemFromContainer(lbi) as T);
            }
            else if (eventSource is ItemsControl)
            {
                ItemsControl ctrl = eventSource as ItemsControl;
                //source = ctrl.DataContext as IDragDroppable<T> ?? DragDropHandler.GetDataContextDragDrop(ctrl) as IDragDroppable<T>; // DEPRECATED
                source = ctrl.DataContext as IDragDroppable<T>;
                item = FindItemInContainer<ItemsControl, ContentPresenter>(ctrl, eventOriginalSource,
                                                                    presenter => presenter.Content as T);
            }
            if (item != null && source != null && source.IsItemDraggable(item) == ItemDragResults.Allowed)
                return new DragDropData
                {
                    Source = source,
                    Item = item
                };
            return null;
        }
        #endregion

        #region VisualTree helpers
        internal static IDragDroppable<T> GetDragDroppableObject(object o)
        {
            if (o is DataGrid)
            {
                DataGrid grid = o as DataGrid;
                return grid.DataContext as IDragDroppable<T>;
            }
            else if (o is ListBox) // works with ListView (which inherits from ListBox)
            {
                ListBox lst = o as ListBox;
                return lst.DataContext as IDragDroppable<T>;
            }
            else if (o is ItemsControl)
            {
                ItemsControl ctrl = o as ItemsControl;
                return ctrl.DataContext as IDragDroppable<T>;
            }
            return null;
        }

        internal static TAncestor FindAncestor<TAncestor>(DependencyObject current) where TAncestor : DependencyObject
        {
            do
            {
                if (current is TAncestor)
                {
                    return current as TAncestor;
                }
                current = VisualTreeHelper.GetParent(current);
            } while (current != null);
            return null;
        }

        internal static T FindItemInContainer<TContainer, TItem>(TContainer container, DependencyObject current, Func<TItem, T> getDataFromControlFunc) where TItem : DependencyObject
        {
            while (current != null)
            {
                TItem containerItem = FindAncestor<TItem>(current);
                if (containerItem != null)
                {
                    T item = getDataFromControlFunc(containerItem);
                    if (item != null)
                        return item;
                    current = VisualTreeHelper.GetParent(containerItem);
                }
                else
                    break;
            }
            return null;
        }
        #endregion
    }

    public class DragDropOptionsData
    {
        public Type ItemType { get; set; }
        public bool IsDragSource { get; set; }
        public bool IsDropTarget { get; set; }
        public bool DoubleClickable { get; set; }
    }

    [MarkupExtensionReturnType(typeof(DragDropOptionsData))]
    public class DragDropOptionsExtension : MarkupExtension
    {
        public DragDropOptionsExtension()
        {
        }

        public DragDropOptionsExtension(object itemType, object isDragSource, object isDropTarget, object doubleClickable)
        {
            ItemType = itemType;
            IsDragSource = isDragSource;
            IsDropTarget = isDropTarget;
            DoubleClickable = doubleClickable;
        }

        [ConstructorArgument("itemType")]
        public object ItemType { get; set; }

        [ConstructorArgument("isDragSource")]
        public object IsDragSource { get; set; }

        [ConstructorArgument("isDropTarget")]
        public object IsDropTarget { get; set; }

        [ConstructorArgument("doubleClickable")]
        public object DoubleClickable { get; set; }

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new DragDropOptionsData
            {
                ItemType = ItemType as Type,
                IsDragSource = IsDragSource == null || Convert.ToBoolean(IsDragSource), // default value: true
                IsDropTarget = IsDropTarget == null || Convert.ToBoolean(IsDropTarget), // default value: true
                DoubleClickable = DoubleClickable == null || Convert.ToBoolean(DoubleClickable), // default value: true
            };
        }
    }

    public class DragDropHandler
    {
        // DragDrop
        public static readonly DependencyProperty DragDropProperty = DependencyProperty.RegisterAttached(
            "DragDrop", 
            typeof(DragDropOptionsData), 
            typeof(DragDropHandler), 
            new UIPropertyMetadata(null, DragDropChanged));

        public static DragDropOptionsData GetDragDrop(DependencyObject obj)
        {
            return (DragDropOptionsData)obj.GetValue(DragDropProperty);
        }

        public static void SetDragDrop(DependencyObject obj, DragDropOptionsData value)
        {
            obj.SetValue(DragDropProperty, value);
        }

        public static void DragDropChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            // TODO: check if e.OldValue <> e.NewValue, remove old handlers
            Control ctrl = obj as Control;
            DragDropOptionsData options = e.NewValue as DragDropOptionsData;
            if (ctrl != null && options != null) // TODO: check if ctrl is ItemsControl/ListBox/ListView/Datagrid
            {
                if (options.ItemType == null)
                    throw new ArgumentException("ItemType cannot be null");
                if (options.IsDragSource)
                {
                    MouseButtonEventHandler previousMouseLeftButtonDown =
                        (MouseButtonEventHandler)CreateDelegateToDragDropManagerEventHandler(ctrl,
                                                                                                "DragSource_PreviewMouseLeftButtonDown",
                                                                                                "PreviewMouseLeftButtonDown",
                                                                                                options.ItemType);
                    MouseButtonEventHandler previousMouseLeftButtonUp =
                        (MouseButtonEventHandler)CreateDelegateToDragDropManagerEventHandler(ctrl,
                                                                                                "DragSource_PreviewMouseLeftButtonUp",
                                                                                                "PreviewMouseLeftButtonUp",
                                                                                                options.ItemType);
                    MouseEventHandler previewMouseMove =
                        (MouseEventHandler)CreateDelegateToDragDropManagerEventHandler(ctrl,
                                                                                        "DragSource_PreviewMouseMove",
                                                                                        "PreviewMouseMove",
                                                                                        options.ItemType);

                    ctrl.PreviewMouseLeftButtonDown += previousMouseLeftButtonDown;
                    ctrl.PreviewMouseLeftButtonUp += previousMouseLeftButtonUp;
                    ctrl.PreviewMouseMove += previewMouseMove;
                }
                if (options.IsDropTarget)
                {
                    ctrl.AllowDrop = true;
                    DragEventHandler previewDrop =
                        (DragEventHandler)CreateDelegateToDragDropManagerEventHandler(ctrl,
                                                                                        "DropTarget_PreviewDrop",
                                                                                        "PreviewDrop",
                                                                                        options.ItemType);
                    DragEventHandler dragEnter =
                        (DragEventHandler)CreateDelegateToDragDropManagerEventHandler(ctrl,
                                                                                        "DropTarget_DragEnter",
                                                                                        "DragEnter",
                                                                                        options.ItemType);
                    DragEventHandler dragOver =
                        (DragEventHandler)CreateDelegateToDragDropManagerEventHandler(ctrl,
                                                                    "DropTarget_DragOver",
                                                                    "DragOver",
                                                                    options.ItemType);
                    DragEventHandler dragLeave =
                        (DragEventHandler)CreateDelegateToDragDropManagerEventHandler(ctrl,
                                                "DropTarget_DragLeave",
                                                "DragLeave",
                                                options.ItemType);

                    ctrl.PreviewDrop += previewDrop;
                    ctrl.DragEnter += dragEnter;
                    ctrl.DragOver += dragOver;
                    ctrl.DragLeave += dragLeave;
                }
                if (options.DoubleClickable)
                {
                    MouseButtonEventHandler mouseDoubleClick =
                        (MouseButtonEventHandler)CreateDelegateToDragDropManagerEventHandler(ctrl,
                                                                                                "DragSource_MouseDoubleClick",
                                                                                                "MouseDoubleClick",
                                                                                                options.ItemType);
                    ctrl.MouseDoubleClick += mouseDoubleClick;
                }
            }
        }
        //
        private static Delegate CreateDelegateToDragDropManagerEventHandler(Control control, string handlerName, string eventName, Type type)
        {
            // get instance of DragDropManager<typeName>
            Type typeofClassWithGenericStaticMethod = typeof(DragDropManager<>);
            Type genericType = typeofClassWithGenericStaticMethod.MakeGenericType(type);
            PropertyInfo propertyInfo = genericType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            MethodInfo methodInfo = propertyInfo.GetGetMethod();
            object dragDropManagerInstance = methodInfo.Invoke(null, null);

            // get control type
            Type controlType = control.GetType();

            // get event info
            EventInfo eventInfo = controlType.GetEvent(eventName);

            // get method handler in generic type
            MethodInfo handlerInfo = genericType.GetMethod(handlerName, BindingFlags.Public | BindingFlags.Instance);

            // create delegate from handler to event
            return Delegate.CreateDelegate(eventInfo.EventHandlerType, dragDropManagerInstance, handlerInfo);
        }
    }
}
