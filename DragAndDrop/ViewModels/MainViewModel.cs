namespace DragAndDrop.ViewModels
{
    public class MainViewModel : IDragDropActionManager<string>
    {
        public FromViewModel FromViewModel { get; private set; }
        public ToViewModel ToViewModel { get; private set; }

        public MainViewModel()
        {
            FromViewModel = new FromViewModel(this);
            ToViewModel = new ToViewModel(this);
        }

        public void PerformAction(IDragDroppable<string> from, IDragDroppable<string> to, string item, DragDropActions action)
        {
            DragDropViewModelBase source = from as DragDropViewModelBase;
            DragDropViewModelBase target = to as DragDropViewModelBase;

            switch (action)
            {
                case DragDropActions.Add:
                    if (target != null)
                        target.Items.Add(item);
                    break;
                case DragDropActions.Remove:
                    if (source != null)
                        source.Items.Remove(item);
                    break;
                case DragDropActions.Move:
                    if (source != null && target != null)
                    {
                        source.Items.Remove(item);
                        target.Items.Add(item);
                    }
                    break;
            }
        }
    }
}
