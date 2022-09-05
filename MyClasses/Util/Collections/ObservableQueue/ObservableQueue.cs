using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Collections.ObservableQueue
{
  public enum QueueChangedEventType
  {
    Enqueued,
    Dequeued,
    Cleared
  }

  public class QueueChangedEventArgs<T> : EventArgs
  {
    public QueueChangedEventType EventType { get; set; }
    public T ChangedItem { get; set; }
  }
  public class QueueEnqueuedEventArgs<T> : EventArgs
  {
    public T EnqueuedItem { get; set; }
  }
  public class QueueDequeuedEventArgs<T> : EventArgs
  {
    public T DequeuedItem { get; set; }
  }

  public class ObservableQueue<T> : Queue<T>
  {
    public delegate void QueueChangedEvent(object s, QueueChangedEventArgs<T> e);
    public delegate void QueueEnqueuedEvent(object s, QueueEnqueuedEventArgs<T> e);
    public delegate void QueueDequeuedEvent(object s, QueueDequeuedEventArgs<T> e);
    public event QueueChangedEvent Changed;
    public event QueueEnqueuedEvent Enqueued;
    public event QueueDequeuedEvent Dequeued;

    private void OnChanged(QueueChangedEventType type, T Item)
    {
      Changed?.Invoke(this, new QueueChangedEventArgs<T>() { EventType = type, ChangedItem = Item });
    }

    private void OnEnqueued(T item)
    {
      OnChanged(QueueChangedEventType.Enqueued, item);
      Enqueued?.Invoke(this, new QueueEnqueuedEventArgs<T>() { EnqueuedItem = item });
    }

    private void OnDequeued(T item)
    {
      OnChanged(QueueChangedEventType.Dequeued, item);
      Dequeued?.Invoke(this, new QueueDequeuedEventArgs<T>() { DequeuedItem = item });
    }

    object lockObj = new object();
    public new void Enqueue(T item)
    {
      lock (lockObj)
      {
        base.Enqueue(item);
        OnEnqueued(item);
      }
    }

    public new T Dequeue()
    {
      lock (lockObj)
      {
        T item = base.Dequeue();
        OnDequeued(item);
        return item;
      }
    }

    public new void Clear()
    {
      lock (lockObj)
      {
        base.Clear();
        OnChanged(QueueChangedEventType.Cleared, default(T));
      }
    }
  }
}
