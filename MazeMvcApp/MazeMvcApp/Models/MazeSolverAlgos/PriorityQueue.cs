namespace MazeMvcApp.Models.MazeSolverAlgos
{
    // This class is for Dijkstra's and possibly Greedy Best First Search
    public class PriorityQueue<T>
    {
        private SortedDictionary<int, Queue<T>> dictionary;

        public int Count { get; private set; }

        public PriorityQueue()
        {
            dictionary = new SortedDictionary<int, Queue<T>>();
            Count = 0;
        }

        public void Enqueue(T item, int priority)
        {
            if (!dictionary.ContainsKey(priority))
            {
                dictionary[priority] = new Queue<T>();
            }

            dictionary[priority].Enqueue(item);
            Count++;
        }

        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }
                
            var queue = dictionary.First().Value;
            T item = queue.Dequeue();
            if (queue.Count == 0)
            {
                dictionary.Remove(dictionary.First().Key);
            }
            Count--;

            return item;
        }
    }
}
