using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Collections.ArrayStack
{
  public class ArrayDropOutStack<T> : IStack<T>
  {
    /**
    * constant to represent the default capacity of the array
    */
    private static readonly int DEFAULT_CAPACITY = 100;
    private int top;
    private T[] stack;

    public int Count { get; private set; }

    public ArrayDropOutStack()
      : this(DEFAULT_CAPACITY)
    {
    }

    public ArrayDropOutStack(int initialCapacity)
    {
      top = 0;
      Count = 0;
      stack = new T[initialCapacity];
    }

    public void Push(T element)
    {
      stack[top] = element;
      top = (top + 1) % stack.Length;

      if (Count < stack.Length)
      {
        Count++;
      }
    }

    public T Pop()
    {
      if (IsEmpty())
      {
        throw new InvalidOperationException("Stack is empty");
      }
      Count--;
      top--;
      if (top < 0)
      {
        top = stack.Length - 1;
      }

      T result = stack[top];
      stack[top] = default(T);

      return result;
    }

    public T Peek()
    {
      if (IsEmpty())
      {
        throw new InvalidOperationException("Stack is empty");
      }
      return top == 0 ? stack[Count - 1] : stack[top - 1];
    }

    public void Clear()
    {
      top = 0;
      Count = 0;
    }

    public bool IsEmpty()
    {
      return (Count == 0);
    }

    public String toString()
    {
      String result = "";

      for (int scan = 0; scan < Count; scan++)
        result = result + stack[scan].ToString() + "\n";

      return result;
    }
  }
}
