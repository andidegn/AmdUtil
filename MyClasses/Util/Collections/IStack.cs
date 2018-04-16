using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Collections
{
  /// <summary>
  /// Interface defining Stack
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IStack<T>
  {
    /// <summary>
    /// The number of elements in this stack. 
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Adds one element to the top of this stack. 
    /// </summary>
    /// <param name="element">element to be pushed onto stack</param>
    void Push(T element);

    /// <summary>
    /// Removes and returns the top element from this stack. 
    /// </summary>
    /// <returns>T element removed from the top of the stack</returns>
    T Pop();

    /// <summary>
    /// Returns without removing the top element of this stack. 
    /// </summary>
    /// <returns>T element on top of the stack</returns>
    T Peek();

    /// <summary>
    /// Clears the stack
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Returns true if this stack contains no elements. 
    /// </summary>
    /// <returns>boolean whether or not this stack is empty</returns>
    bool IsEmpty();
  }
}
