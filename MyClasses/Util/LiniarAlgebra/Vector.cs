using System;
using System.Text;

namespace AMD.Util.LinearAlgebra
{
  public class Vector
  {
    public const double EqualityTolerence = Double.Epsilon;
    public double[] components;

    /// <summary>
    /// Indexer
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public double this[int index]
    {
      set
      {
        components[index] = value;
      }
      get
      {
        return components[index];
      }
    }

    /// <summary>
    /// Constructor 1
    /// </summary>
    /// <param name="size"></param>
    public Vector(int size)
    {
      this.components = new double[size];
    }

    /// <summary>
    /// Constructor 2
    /// </summary>
    /// <param name="components"></param>
    public Vector(double[] components)
    {
      this.components = components;
    }

    /// <summary>
    /// Constructor 3
    /// </summary>
    /// <param name="v1"></param>
    public Vector(Vector v1)
    {
      this.components = new double[v1.Size()];
      Array.Copy(v1.components, this.components, v1.Size());
    }

    #region Functions
    /// <summary>
    /// Returns the length of the vector
    /// </summary>
    /// <returns></returns>
    public double Length()
    {
      double sum = 0;
      foreach (double p in components)
      {
        sum += Math.Pow(p, 2);
      }
      return Math.Sqrt(sum);
    }

    /// <summary>
    /// returns the R (dimension) of the vector
    /// </summary>
    /// <returns></returns>
    public int Size()
    {
      return components.Length;
    }

    /// <summary>
    /// Returns the DOT (scalar) product of two vectors
    /// a DOT b = a1*b1 + a2*b2 + ... an*bn
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static double DotProduct(Vector v1, Vector v2)
    {
      double sum = 0;
      int size = v1.Size();
      if (size != v2.Size())
      {
        throw new Exception("Not equal sizes!");
      }
      for (int i = 0; i < size; i++)
      {
        sum += v1.components[i] * v2.components[i];
      }
      return sum;
    }

    /// <summary>
    /// Returns the DOT (scalar) product of two vectors
    /// a DOT b = a1*b1 + a2*b2 + ... an*bn
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double DotProduct(Vector other)
    {
      return DotProduct(this, other);
    }

    /// <summary>
    /// Returns the angle in radians between two vectors
    /// Note. The angle is always the smallest possible angle between the two,
    /// i.e. it can never surpass PI and the angle is always possitive
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static double AngleR(Vector v1, Vector v2)
    {
      double res = DotProduct(v1, v2) / (v1.Length() * v2.Length());
      if (res > 1)
      {
        res = 1;
      }
      else if (res < -1)
      {
        res = -1;
      }
      return Math.Acos(res);
    }

    /// <summary>
    /// Returns the angle in radians between two vectors
    /// Note. The angle is always the smallest possible angle between the two,
    /// i.e. it can never surpass PI the angle is always possitive
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double AngleR(Vector other)
    {
      return AngleR(this, other);
    }

    /// <summary>
    /// Returns the angle in degrees between two vectors
    /// Note. The angle is always the smallest possible angle between the two,
    /// i.e. it can never surpass 180 degrees the angle is always possitive
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static double AngleD(Vector v1, Vector v2)
    {
      double res = DotProduct(v1, v2) / (v1.Length() * v2.Length());
      if (res > 1)
      {
        res = 1;
      }
      else if (res < -1)
      {
        res = -1;
      }
      return Math.Acos(res) * 180 / Math.PI;
    }

    /// <summary>
    /// Returns the angle in degrees between two vectors
    /// Note. The angle is always the smallest possible angle between the two,
    /// i.e. it can never surpass 180 degrees the angle is always possitive
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double AngleD(Vector other)
    {
      return AngleD(this, other);
    }

    /// <summary>
    /// Returns the projected vector when projecting v1 onto v2
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vector Projection(Vector v1, Vector v2)
    {
      return (double)(DotProduct(v1, v2) / Math.Pow(v2.Length(), 2)) * v2;
    }

    /// <summary>
    /// Returns the projected vector when projecting the vector onto v2
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Vector Projection(Vector v)
    {
      return Projection(this, v);
    }

    /// <summary>
    /// Returns the length of the projected vector v1 onto v2
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static double ProjectionLenght(Vector v1, Vector v2)
    {
      return Math.Abs(DotProduct(v1, v2)) / v2.Length();
    }

    /// <summary>
    /// Checks if two vectors are equal
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      return this == obj as Vector;
    }

    /// <summary>
    /// Returns the hashcode of the vector
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Returns a String with the component values
    /// </summary>
    /// <returns></returns>
    public override String ToString()
    {
      StringBuilder sb = new StringBuilder();
      int i = 1;
      foreach (double d in components)
      {
        sb.Append(String.Format("x{0}: {1}\n", i++, d));
      }
      return sb.ToString(); ;
    }
    #endregion

    #region Operators
    public static Vector operator +(Vector v1, Vector v2)
    {
      int size = v1.Size();
      if (size != v2.Size())
      {
        throw new Exception("Not equal sizes!");
      }
      Vector v = new Vector(size);
      for (int i = 0; i < size; i++)
      {
        v.components[i] = v1.components[i] + v2.components[i];
      }
      return v;
    }

    public static Vector operator -(Vector v1, Vector v2)
    {
      int size = v1.Size();
      if (size != v2.Size())
      {
        throw new Exception("Not equal sizes!");
      }
      Vector v = new Vector(size);
      for (int i = 0; i < size; i++)
      {
        v.components[i] = v1.components[i] - v2.components[i];
      }
      return v;
    }

    public static Vector operator *(Vector v1, double k)
    {
      int size = v1.Size();
      Vector v = new Vector(size);
      for (int i = 0; i < size; i++)
      {
        v.components[i] = v1.components[i] * k;
      }
      return v;
    }

    public static Vector operator *(double k, Vector v1)
    {
      return v1 * k;
    }

    public static double operator *(Vector v1, Vector v2)
    {
      return Vector.DotProduct(v1, v2);
    }

    public static Vector operator /(Vector v1, double k)
    {
      int size = v1.Size();
      Vector v = new Vector(size);
      for (int i = 0; i < size; i++)
      {
        v.components[i] = v1.components[i] / k;
      }
      return v;
    }

    public static Vector operator +(Vector v1)
    {
      int size = v1.Size();
      Vector v = new Vector(size);
      for (int i = 0; i < size; i++)
      {
        v.components[i] = +v1.components[i];
      }
      return v;
    }

    public static Vector operator -(Vector v1)
    {
      int size = v1.Size();
      Vector v = new Vector(size);
      for (int i = 0; i < size; i++)
      {
        v.components[i] = -v1.components[i];
      }
      return v;
    }

    public static bool operator ==(Vector v1, Vector v2)
    {
      for (int i = 0; i < v1.Size(); i++)
      {
        if (Math.Abs(v1.components[i] - v2.components[i]) > EqualityTolerence)
        {
          return false;
        }
      }
      return true;
    }

    public static bool operator !=(Vector v1, Vector v2)
    {
      return !(v1 == v2);
    }
    #endregion
  }
}
