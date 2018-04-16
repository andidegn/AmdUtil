using System;
using AMD.Util.Collections;
using AMD.Util.Collections.ArrayStack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMDUtilUnitTest
{
  [TestClass]
  public class ArrayDropOutStackTest
  {
    IStack<int> stack;

    private void InitForTest()
    {
      InitStack();
      SetUp();
    }

    [TestMethod]
    public void InitStack()
    {
      stack = new ArrayDropOutStack<int>(5);
    }

    [TestMethod]
    public void SetUp()
    {
      stack = new ArrayDropOutStack<int>(5);
      stack.Push(1);
      stack.Push(2);
      stack.Push(3);
      stack.Push(4);
      stack.Push(5);
    }

    [TestMethod]
    public void TestPush()
    {
      InitForTest();

      Assert.AreEqual(5, stack.Count);
      Assert.AreEqual(5, (int)stack.Peek());

      stack.Push(6);
      Assert.AreEqual(6, (int)stack.Peek());

      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();

      stack.Push(1);
      Assert.AreEqual(1, (int)stack.Pop());
    }
    
    [TestMethod]
    public void TestPop()
    {
      InitForTest();

      Assert.AreEqual(5, (int)stack.Pop());
      Assert.AreEqual(4, (int)stack.Pop());
      Assert.AreEqual(3, (int)stack.Pop());
      Assert.AreEqual(2, (int)stack.Pop());
      Assert.AreEqual(1, (int)stack.Pop());
      Assert.AreEqual(0, stack.Count);
    }

    [TestMethod]
    public void TestPeek()
    {
      InitForTest();

      Assert.AreEqual(5, (int)stack.Peek());
      stack.Pop();
      Assert.AreEqual(4, (int)stack.Peek());
      stack.Push(5);
      Assert.AreEqual(5, (int)stack.Peek());
    }

    [TestMethod]
    public void TestIsEmpty()
    {
      InitForTest();

      Assert.IsFalse(stack.IsEmpty());

      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();

      Assert.IsTrue(stack.IsEmpty());

      stack = new ArrayDropOutStack<int>(5);

      Assert.AreEqual(true, stack.IsEmpty());
    }

    [TestMethod]
    public void TestSize()
    {
      InitForTest();

      Assert.AreEqual(5, stack.Count);
      stack.Push(6);
      Assert.AreEqual(5, stack.Count);
      stack.Pop();
      Assert.AreEqual(4, stack.Count);
      stack.Pop();
      stack.Pop();
      stack.Pop();
      stack.Pop();
      Assert.AreEqual(0, stack.Count);
    }

    [TestMethod]
    public void TestClear()
    {
      InitForTest();

      Assert.AreEqual(5, stack.Count);
      stack.Clear();
      Assert.AreEqual(0, stack.Count);
      Assert.ThrowsException<InvalidOperationException>(() => stack.Pop());
      Assert.ThrowsException<InvalidOperationException>(() => stack.Peek());
      stack.Push(5);
      Assert.AreEqual(5, stack.Pop());
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
