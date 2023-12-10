using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Search
{
  public class BSearch<T> where T : IComparable<T>, IComparable
  {
    private IList<T> col;
    public T FoundValue { get; private set; }
    public int FoundValueIndex { get;private set; }

    public BSearch(IList<T> col)
    {
      this.col = col;
    }

    public bool Exists(T val)
    {
      if (EqualityComparer<T>.Default.Equals(val))
      {
        return false; 
      }
      return Search(val, 0, col.Count - 1);
    }

    private bool Search(T val, int l, int r)
    {
      int m = (l + r) / 2;

      T valAtM = col[m];

      if (valAtM.Equals(val))
      {
        //FoundValueIndex = m;
        //FoundValue = val;
        return true;
      }

      if (l >= r)
      {
        //FoundValueIndex = -1;
        //FoundValue = default(T);
        return false;
      }

      if (0 < val.CompareTo(valAtM))
      {
        return Search(val, m + 1, r);
      }
      return Search(val, l, m - 1);
    }

    public bool Exists1(T val)
    {
      if (EqualityComparer<T>.Default.Equals(val))
      {
        return false;
      }
      return Search1(val, 0, col.Count - 1);
    }

    private bool Search1(T val, int l, int r)
    {
      int m = 0;
      while (l <= r)
      {
        m = (r - l) / 2 + l;

        if (0 < val.CompareTo(col[m]))
        {
          l = m + 1;
        }
        else if (0 > val.CompareTo(col[m]))
        {
          r = m - 1;
        }
        else
        {
          //FoundValueIndex = m;
          //FoundValue = val;
          return true;
        }
      }
      //FoundValueIndex = -1;
      //FoundValue = default;
      return false;
    }
  }
}
