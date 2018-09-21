using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMD.Util.Collections;
using AMD.Util.Collections.ArrayStack;
using AMD.Util.Collections.QueueArray;
using Xunit;
namespace AMD.Util.Collections.QueueArray.Tests
{
  public class ArrayDropOutStackTest
  {
    IStack<int> stack;

    [Fact()]
    public void InitStack()
    {
      stack = new ArrayDropOutStack<int>(5);
    }

    [Fact()]
    public void setUp()
    {
      stack.Push(1);
      stack.Push(2);
      stack.Push(3);
      stack.Push(4);
      stack.Push(5);
    }

    [Fact()]
    public void testPush()
    {
      Assert.Equal(5, stack.Count);
      Assert.Equal(5, (int)stack.Peek());

      stack.Push(6);
      Assert.Equal(6, (int)stack.Peek());

      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();

      stack.Push(1);
      Assert.Equal(1, (int)stack.Pop());
    }

    [Fact()]
    public void testPop()
    {
      Assert.Equal(5, (int)stack.Pop());
      Assert.Equal(4, (int)stack.Pop());
      Assert.Equal(3, (int)stack.Pop());
      Assert.Equal(2, (int)stack.Pop());
      Assert.Equal(1, (int)stack.Pop());
      Assert.Equal(0, stack.Count);
    }

    [Fact()]
    public void testPeek()
    {
      Assert.Equal(5, (int)stack.Peek());
      stack.Pop();
      Assert.Equal(4, (int)stack.Peek());
      stack.Push(5);
      Assert.Equal(5, (int)stack.Peek());
    }

    [Fact()]
    public void testIsEmpty()
    {
      Assert.False(stack.IsEmpty());

      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();

      Assert.True(stack.IsEmpty());

      stack = new ArrayDropOutStack<int>(5);

      Assert.Equal(true, stack.IsEmpty());
    }

    [Fact()]
    public void testSize()
    {
      Assert.Equal(5, stack.Count);
      stack.Push(6);
      Assert.Equal(5, stack.Count);
      stack.Pop();
      Assert.Equal(4, stack.Count);
      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();
      Assert.Equal(0, stack.Count);
    }

    //public void testEmptyCollectionException()
    //{
    //  try
    //  {
    //    stack.Peek();
    //  }
    //  catch (EmptyCollectionException e)
    //  {
    //    fail("Should not throw exception here \"Peek\"");
    //  }

    //  try
    //  {
    //    stack.Pop();
    //  }
    //  catch (EmptyCollectionException e)
    //  {
    //    fail("Should not throw exception here \"Pop\"");
    //  }

    //  stack.Pop();
    //  stack.Pop();
    //  stack.Pop();
    //  stack.Pop();

    //  try
    //  {
    //    stack.Peek();
    //    fail("Should throw exception here \"Peek\"");
    //  }
    //  catch (EmptyCollectionException e)
    //  {
    //  }

    //  try
    //  {
    //    stack.Pop();
    //    fail("Should throw exception here \"Pop\"");
    //  }
    //  catch (EmptyCollectionException e)
    //  {
    //  }
    //}
  }
}
