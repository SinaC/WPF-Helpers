using System.Windows;
using System.Windows.Media;

namespace ModalPopupOverlay
{
    public static class VisualHelper
    {
        public static T FindParent<T>(DependencyObject child)
            where T : class
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null)
                return default(T);

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }

    }
}
