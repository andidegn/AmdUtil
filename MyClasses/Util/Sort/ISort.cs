using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMD.Util.Sort
{
  interface ISort<T> where T : IComparable
  {
    bool IsSorted(T[] array);

    void Sort(T[] array);
  }
}
