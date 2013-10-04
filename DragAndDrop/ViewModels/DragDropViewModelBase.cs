using System.Collections.ObjectModel;

namespace DragAndDrop.ViewModels
{
    public class DragDropViewModelBase
    {
        public IDragDropActionManager<string> Manager { get; private set; }
        public ObservableCollection<string> Items { get; set; }

        public DragDropViewModelBase(IDragDropActionManager<string> manager)
        {
            Manager = manager;

            Items = new ObservableCollection<string>();
        }
    }
}
