using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LogApi.App_Code.Tests
{
    [TestClass()]
    public class FixedSizedQueueTests
    {

        [TestMethod()]
        public void GetLastTest()
        {
            var queue = new FixedSizedQueue<int>(5);
            var array = Enumerable.Range(1, 5).Select(x => queue.Enqueue(x)).ToArray();
            Assert.IsTrue(queue.GetLast() == 5);
        }

        [TestMethod()]
        public void EnqueueTest()
        {
            var queue = new FixedSizedQueue<int>(5);
            var array = Enumerable.Range(1, 5).Select(x => queue.Enqueue(x)).ToArray();
            Assert.IsTrue(queue.Enqueue(6) == 1);
        }
    }
}