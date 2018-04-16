using AMD.Util.LinearAlgebra;
using System;

namespace AMD.Util.LinearAlgebra
{

  public class Vector3D : Vector
  {

    public double X
    {
      get { return components[0]; }
      set { components[0] = value; }
    }

    public double Y
    {
      get { return components[1]; }
      set { components[1] = value; }
    }

    public double Z
    {
      get { return components[2]; }
      set { components[2] = value; }
    }

    /// <summary>
    /// Property for the magnitude (aka. length or absolute value) of the Vector3
    /// </summary>
    public double Length
    {
      get
      {
        return this.Length();
      }
      set
      {
        if (value < 0) { throw new ArgumentOutOfRangeException("value", value, "Out of range, can't be zero"); }

        if (this == new Vector3D(0, 0, 0)) { throw new ArgumentException("Invalid vector to scale (0, 0, 0)", "this"); }

        double scalar = value / Length;
        this.X *= scalar;
        this.Y *= scalar;
        this.Z *= scalar;
      }
    }

    public Vector3D()
        : base(3)
    {
    }

    public Vector3D(double x, double y, double z)
        : base(new double[] { x, y, z })
    {
    }

    public Vector3D(Vector v1)
        : base(v1)
    {
    }

    #region Functions
    /// <summary>
    /// Returns the Cross (X) product of two vectors
    ///         |a2  b2|
    ///         |      |
    ///         |a3  b3|
    /// 
    ///         |a3  b3|
    /// a X b = |      |
    ///         |a1  b1|
    ///         
    ///         |a1  b1|
    ///         |      |
    ///         |a2  b2|
    ///         
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vector3D CrossProduct(Vector3D v1, Vector3D v2)
    {
      Vector3D v = new Vector3D();
      v.X = (v1.Y * v2.Z) - (v1.Z * v2.Y);
      v.Y = (v1.Z * v2.X) - (v1.X * v2.Z);
      v.Z = (v1.X * v2.Y) - (v1.Y * v2.X);
      return v;
    }

    /// <summary>
    /// Returns the Cross (X) product of two vectors
    ///         |a2  b2|
    ///         |      |
    ///         |a3  b3|
    /// 
    ///         |a3  b3|
    /// a X b = |      |
    ///         |a1  b1|
    ///         
    ///         |a1  b1|
    ///         |      |
    ///         |a2  b2|
    ///         
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Vector3D CrossProduct(Vector3D v)
    {
      return CrossProduct(this, v);
    }

    /// <summary>
    /// Returns a vector projected onto a plane where 'normal' is the normal to the plane
    /// projectionOnPlane = v1 - (v1 DOT normal) * normal
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static Vector3D ProjectionOnPlane(Vector3D v1, Vector3D normal)
    {
      return v1 - (v1.DotProduct(normal) * normal);
    }

    /// <summary>
    /// Returns a vector projected onto a plane where 'normal' is the normal to the plane
    /// projectionOnPlane = vector - (vector DOT normal) * normal
    /// </summary>
    /// <param name="normal"></param>
    /// <returns></returns>
    public Vector3D ProjectionOnPlane(Vector3D normal)
    {
      return ProjectionOnPlane(this, normal);
    }

    /// <summary>
    /// Returns a normalized vector of v
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vector3D Normalize(Vector3D v)
    {
      //return v / v.Length();

      // Check for divide by zero errors
      if (v.Length() == 0)
      {
        throw new DivideByZeroException("Can't divide by zero");
      }
      else
      {
        // find the inverse of the vectors magnitude
        double inverse = 1 / v.Length;
        return new Vector3D(v.X * inverse, v.Y * inverse, v.Z * inverse);
      }
    }

    /// <summary>
    /// Returns the vector normalized
    /// </summary>
    /// <returns></returns>
    public Vector3D Normalize()
    {
      return Normalize(this);
    }

    /// <summary>
    /// Returns a vector rotated 'angle' degrees around the 'y' axis
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="degree"></param>
    /// <returns></returns>
    public static Vector3D RotateY(Vector3D v1, double degree)
    {
      Vector3D v = new Vector3D();

      v.X = (v1.Z * Math.Sin(degree)) + (v1.X * Math.Cos(degree));
      v.Y = v1.Y;
      v.Z = (v1.Z * Math.Cos(degree)) - (v1.X * Math.Sin(degree));
      return v;
    }

    /// <summary>
    /// Returns a vector rotated 'angle' degrees around the 'y'
    /// </summary>
    /// <param name="degree"></param>
    /// <returns></returns>
    public void RotateY(double degree)
    {
      Vector3D v = RotateY(this, degree);
      X = v.X;
      Y = v.Y;
      Z = v.Z;
    }


    /// <summary>
    /// Returns a vector rotated 'angle' degrees around the 'x' axis
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="degree"></param>
    /// <returns></returns>
    public static Vector3D RotateX(Vector3D v1, double degree)
    {
      Vector3D v = new Vector3D();

      v.X = v1.X;
      v.Y = (v1.Y * Math.Cos(degree)) - (v1.Z * Math.Sin(degree));
      v.Z = (v1.Y * Math.Sin(degree)) + (v1.Z * Math.Cos(degree));
      return v;
    }


    /// <summary>
    /// Returns a vector rotated 'angle' degrees around the 'x' axis
    /// </summary>
    /// <param name="degree"></param>
    /// <returns></returns>
    public void RotateX(double degree)
    {
      Vector3D v = RotateX(this, degree);
      X = v.X;
      Y = v.Y;
      Z = v.Z;
    }


    /// <summary>
    /// Returns a vector rotated 'angle' degrees around the 'z' axis
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="degree"></param>
    /// <returns></returns>
    public static Vector3D RotateZ(Vector3D v1, double degree)
    {
      Vector3D v = new Vector3D();

      v.X = (v1.X * Math.Cos(degree)) - (v1.Y * Math.Sin(degree));
      v.Y = (v1.X * Math.Sin(degree)) + (v1.Y * Math.Cos(degree));
      v.Z = v1.Z;
      return v;
    }


    /// <summary>
    /// Returns a vector rotated 'angle' degrees around the 'z' axis
    /// </summary>
    /// <param name="degree"></param>
    /// <returns></returns>
    public void RotateZ(double degree)
    {
      Vector3D v = RotateZ(this, degree);
      X = v.X;
      Y = v.Y;
      Z = v.Z;
    }

    /// <summary>
    /// Returns a vector rotated 'angle' degrees around 'axis'
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public void Rotate(Vector3D axis, double angle)
    {
      Vector3D v = Rotate(this, axis, angle);
      X = v.X;
      Y = v.Y;
      Z = v.Z;
    }

    /// <summary>
    /// Returns a vector rotated 'angle' degrees around 'axis'
    /// </summary>
    /// <param name="v"></param>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3D Rotate(Vector3D v, Vector3D axis, double angle)
    {
      Vector3D result = new Vector3D();

      double tr = 1 - Math.Cos(angle);
      double cos = Math.Cos(angle);
      double sin = Math.Sin(angle);

      result.X = (tr * axis.X * axis.X) + cos * v.X + (tr * axis.X * axis.Y) - (sin * axis.Z) * v.Y + (tr * axis.X * axis.Z) + (sin * axis.Y) * v.Z;
      result.Y = (tr * axis.X * axis.Y) + (sin * axis.Z) * v.X + (tr * axis.Y * axis.Y) + cos * v.Y + (tr * axis.Y * axis.Z) - (sin * axis.X) * v.Z;
      result.Z = (tr * axis.X * axis.Z) - (sin * axis.Y) * v.X + (tr * axis.Y * axis.Z) + (sin * axis.X) * v.Y + (tr * axis.Z * axis.Z) + cos * v.Z;

      //result.x = a1(angle, axis, tr, cos) * v.x + a2(angle, axis, tr, sin) * v.y + a3(angle, axis, tr, sin) * v.z;
      //result.y = b1(angle, axis, tr, sin) * v.x + b2(angle, axis, tr, cos) * v.y + b3(angle, axis, tr, sin) * v.z;
      //result.z = c1(angle, axis, tr, sin) * v.x + c2(angle, axis, tr, sin) * v.y + c3(angle, axis, tr, cos) * v.z;

      return result;
    }

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /*
    #region Private functions for the 'Rotate()' function
    private static double a1(double angle, Vector3D axis, double tr, double cos) {
        return (tr * axis.x * axis.x) + cos;
    }

    private static double a2(double angle, Vector3D axis, double tr, double sin) {
        return (tr * axis.x * axis.y) - (sin * axis.z);
    }

    private static double a3(double angle, Vector3D axis, double tr, double sin) {
        return (tr * axis.x * axis.z) + (sin * axis.y);
    }

    private static double b1(double angle, Vector3D axis, double tr, double sin) {
        return (tr * axis.x * axis.y) + (sin * axis.z);
    }

    private static double b2(double angle, Vector3D axis, double tr, double cos) {
        return (tr * axis.y * axis.y) + cos;
    }

    private static double b3(double angle, Vector3D axis, double tr, double sin) {
        return (tr * axis.y * axis.z) - (sin * axis.x);
    }

    private static double c1(double angle, Vector3D axis, double tr, double sin) {
        return (tr * axis.x * axis.z) - (sin * axis.y);
    }

    private static double c2(double angle, Vector3D axis, double tr, double sin) {
        return (tr * axis.y * axis.z) + (sin * axis.x);
    }

    private static double c3(double angle, Vector3D axis, double tr, double cos) {
        return (tr * axis.z * axis.z) + cos;
    }
    #endregion
     */

    public override String ToString()
    {
      return String.Format("x: {0}, y: {1}, z: {2}", components[0], components[1], components[2]);
    }
    #endregion

    #region Operators
    public static Vector3D operator +(Vector3D v1, Vector3D v2)
    {
      return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }

    public static Vector3D operator -(Vector3D v1, Vector3D v2)
    {
      return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }

    public static Vector3D operator *(Vector3D v1, double k)
    {
      return new Vector3D(k * v1.X, k * v1.Y, k * v1.Z);
    }

    public static Vector3D operator *(double k, Vector3D v1)
    {
      return new Vector3D(k * v1.X, k * v1.Y, k * v1.Z);
    }

    public static double operator *(Vector3D v1, Vector3D v2)
    {
      return Vector.DotProduct(v1, v2);
    }

    public static Vector3D operator /(Vector3D v1, double k)
    {
      return new Vector3D(v1.X / k, v1.Y / k, v1.Z / k);
    }

    public static bool operator ==(Vector3D v1, Vector3D v2)
    {
      return Math.Round(v1.X, 15) == Math.Round(v2.X, 15) && Math.Round(v1.Y, 15) == Math.Round(v2.Y, 15) && Math.Round(v1.Z, 15) == Math.Round(v2.Z, 15);
    }

    public static bool operator !=(Vector3D v1, Vector3D v2)
    {
      return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
    }

    public static bool operator <(Vector3D v1, Vector3D v2)
    {
      return v1.Length() < v2.Length();
    }

    public static bool operator >(Vector3D v1, Vector3D v2)
    {
      return v1.Length() > v2.Length();
    }
    #endregion
  }
}
