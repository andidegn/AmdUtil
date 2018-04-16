using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Collections
{
  public interface IQueue<T> : IEnumerable<T>
  {
    bool IsEmpty { get; }

    int Count { get; }

    void Enqueue(T element);

    T Dequeue();

    T First();

    T Get(int index);

    int IndexOf(T element);
  }
}
