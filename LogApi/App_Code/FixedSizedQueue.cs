using System.Collections.Concurrent;

namespace LogApi.App_Code
{
    /// <summary>
    /// Fixed sized queue
    /// </summary>
    /// <typeparam name="T">Object of any type</typeparam>
    public class FixedSizedQueue<T>
    {
        private ConcurrentQueue<T> Queue = new ConcurrentQueue<T>();
        private int Limit { get; set; }
        private T Last;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Limit">
        /// Maximum count of queue elements. 
        /// If you add an element over the limit, the first element in the queue will be deleted
        /// </param>
        public FixedSizedQueue(int Limit)
        {
            this.Limit = Limit;
        }

        /// <summary>
        /// Returns last added element
        /// </summary>
        /// <returns>Element</returns>
        public T GetLast() {
            return Last;
        }

        /// <summary>
        /// Adds element to queue
        /// </summary>
        /// <param name="obj">Element</param>
        /// <returns>The element removed from the queue if the limit was exceeded</returns>
        public T Enqueue(T obj)
        {
            Queue.Enqueue(obj);
            T overflow;
            lock (this)
            {
                overflow = default(T);
                while (Queue.Count > Limit && Queue.TryDequeue(out overflow));
            }
            Last = obj;
            return overflow;
        }
    }
}