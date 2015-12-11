using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVVM.Tests
{
    [TestClass]
    public class PagedCollectionTests
    {
        [TestMethod]
        public void TestSort()
        {
            const int itemsPerPage = 4;
            const int min = 1;
            const int max = 20;
            List<int> values = Enumerable.Range(min,max-min+1).ToList();
            PagedCollection<int> collection = new PagedCollection<int>(values, itemsPerPage);

            collection.Sort((i, i1) => i < i1 ? 1 : (i > i1 ? -1 : 0)); // reverse order
            int first = collection.FirstOrDefault();
            int last = collection.LastOrDefault();

            Assert.AreEqual(first, max);
            Assert.AreEqual(last, max-itemsPerPage+1);
        }
    }
}
