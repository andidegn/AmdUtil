using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Collections.QueueArray
{
  public class ArrayQueue<T> : IQueue<T>
  {
    private const int default_capacity = 6;
    private int front;
    private int count;
    private T[] queue;

    public bool IsEmpty
    {
      get
      {
        return count == 0;
      }
    }

    public int Count
    {
      get
      {
        return count;
      }
    }

    public ArrayQueue()
    {
      queue = new T[default_capacity];
      front = 0;
      count = 0;
    }

    public ArrayQueue(int initialCapacity)
    {
      queue = new T[initialCapacity];
      front = 0;
      count = 0;
    }

    public ArrayQueue(int initialCapacity, bool bounded)
    {
      queue = new T[initialCapacity];
      front = 0;
      count = 0;
    }

    public void Enqueue(T element)
    {
      if (element == null)
        throw new Exception("Element cannot be \"null\"");
      queue[(count + front) % queue.Length] = element;
      if (count < queue.Length)
        count++;
      else
        front = (front + 1) % queue.Length;
    }

    public T Dequeue()
    {
      if (count == 0)
        throw new Exception("Queue is empty.");
      T temp = queue[front];
      queue[front] = default(T);
      count--;
      front = (front + 1) % queue.Length;
      return temp;
    }

    public T First()
    {
      if (count == 0)
        throw new Exception("Queue is empty.");
      return queue[front];
    }

    public T Get(int index)
    {
      if (count == 0)
        throw new Exception("Queue is empty.");
      return queue[(front + index) % queue.Length];
    }

    public int IndexOf(T element)
    {
      for (int i = front; i < queue.Length; i++)
      {
        if (element.Equals(queue[i]))
          return i - front;
      }
      return -1;
    }

    public IEnumerator<T> GetEnumerator()
    {
      for (int i = 0; i < count; i++)
        yield return queue[(front + i) % queue.Length];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      // call the generic version of the method
      return this.GetEnumerator();
    }

    public override String ToString()
    {
      String s = "";
      for (int i = 0; i < count; i++)
        s += queue[(front + i) % queue.Length] + "\n";
      return s;
    }
  }
}
