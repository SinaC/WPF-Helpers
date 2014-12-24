using System.Collections.Generic;

namespace DragAndDrop.ViewModels
{
    public static class DragDropMapper<T> where T : class
    {
        private static readonly Dictionary<string, IDragDroppable<T>> Mapper = new Dictionary<string, IDragDroppable<T>>();

        public static bool Register(IDragDroppable<T> dragDrop)
        {
            // Overwrite existing
            Mapper[dragDrop.Id] = dragDrop;
            return true;
        }

        public static IDragDroppable<T> GetFromId(string id)
        {
            IDragDroppable<T> outValue;
            if (!Mapper.TryGetValue(id, out outValue))
                return null;
            return outValue;
        }
    }
}
