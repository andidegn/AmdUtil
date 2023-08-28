using AMD.Util.Collections.QueueArray;
using System.Text;
using Xunit;
namespace AMD.Util.Collections.ArrayStack.Tests
{
  public class QueueArrayTests
  {
    IQueue<int> q;

    [Fact()]
    public void QueueArrayTest()
    {
      q = new ArrayQueue<int>();
    }

    [Fact()]
    public void QueueArrayTest1()
    {
      q = new ArrayQueue<int>(100);
      Assert.True(null != q);
    }

    [Fact()]
    public void EnqueueTest()
    {
      q = new ArrayQueue<int>(10);
      for (int i = 0; i < 10; i++)
        q.Enqueue(i);
    }

    [Fact()]
    public void DequeueTest()
    {
      q = new ArrayQueue<int>(10);
      for (int i = 0; i < 10; i++)
        q.Enqueue(i);

      for (int i = 0; i < 10; i++)
        Assert.Equal(i, q.Dequeue());
    }

    [Fact()]
    public void FirstTest()
    {
      q = new ArrayQueue<int>(10);
      for (int i = 0; i < 10; i++)
        q.Enqueue(i);
      Assert.Equal(0, q.First());
      Assert.Equal(0, q.First());
      q.Enqueue(10);
      Assert.Equal(1, q.First());
      for (int i = 11; i < 100; i++)
        q.Enqueue(i);
      Assert.Equal(90, q.First());
    }

    [Fact()]
    public void GetTest()
    {
      q = new ArrayQueue<int>(10);
      for (int i = 0; i < 10; i++)
        q.Enqueue(i);
      for (int i = 0; i < 10; i++)
        Assert.Equal(i, q.Get(i));
    }

    [Fact()]
    public void IndexOfTest()
    {
      q = new ArrayQueue<int>(10);
      for (int i = 0; i < 10; i++)
        q.Enqueue(i);
      for (int i = 0; i < 10; i++)
        Assert.Equal(i, q.IndexOf(i));
    }

    [Fact()]
    public void IsEmptyTest()
    {
      q = new ArrayQueue<int>(10);
      Assert.True(q.IsEmpty);
      for (int i = 0; i < 10; i++)
        q.Enqueue(i);
      Assert.False(q.IsEmpty);
      for (int i = 0; i < 10; i++)
        q.Dequeue();
      Assert.True(q.IsEmpty);
    }

    [Fact()]
    public void SizeTest()
    {
      q = new ArrayQueue<int>(10);
      for (int i = 0; i < 10; i++)
      {
        Assert.Equal(i, q.Count);
        q.Enqueue(i);
      }
      for (int i = 10; i < 100000; i++)
        q.Enqueue(i);
      Assert.Equal(10, q.Count);
    }

    [Fact()]
    public void ToStringTest()
    {
      q = new ArrayQueue<int>(10);
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < 10; i++)
      {
        q.Enqueue(i);
        sb.Append(i);
        sb.Append("\n");
      }
      Assert.Equal(sb.ToString(), q.ToString());
    }
  }
}
