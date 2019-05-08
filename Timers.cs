using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarioBros
{
    public class Timer
    {
        float time;
        float maxTime;
        public Timer(float timeDelay)
        {
            this.time = 0;
            this.maxTime = timeDelay;
        }
        public bool AddTimeCheckIfEnded(float delta)
        {
            time += delta;
            if (time > maxTime) return true;
            else return false;
        }
        public float MaxTime { get { return maxTime; } }
    }

    public class CircularList<T> : IEnumerable<T>
    {
        public Node First { get; set; }

        public CircularList() { }

        public CircularList(T val)
        {
            First = new Node(val);
            First.Next = First;
            First.Prev = First;
        }

        public void Add(T val)
        {
            if (First == null)
            {
                First = new Node(val);
                First.Next = First;
                First.Prev = First;
            }
            else
            {
                Node x = new Node(val);
                x.Next = First;
                x.Prev = First.Prev;
                First = x;
                x.Next.Prev = x;
                x.Prev.Next = x;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node current = First;
            while (true)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        
        public class Node
        {
            public T Value { get; set; }
            public Node Next { get; set; }
            public Node Prev { get; set; }
            public Node(T val)
            {
                Value = val;
            }
        }
    }
}
