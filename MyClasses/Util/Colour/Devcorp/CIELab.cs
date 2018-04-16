using System;
using System.ComponentModel;

namespace AMD.Util.Colour.Devcorp
{
	/// <summary>
	/// Structure to define CIE L*a*b*.
	/// </summary>
	public struct CIELab
	{
		/// <summary>
		/// Gets an empty CIELab structure.
		/// </summary>
		public static readonly CIELab Empty = new CIELab();

    #region Fields
    public double L { get; set; }
    public double A { get; set; }
    public double B { get; set; }
    #endregion

    #region Operators
    public static bool operator ==(CIELab item1, CIELab item2)
		{
			return (
				item1.L == item2.L 
				&& item1.A == item2.A 
				&& item1.B == item2.B
				);
		}

		public static bool operator !=(CIELab item1, CIELab item2)
		{
			return (
				item1.L != item2.L 
				|| item1.A != item2.A 
				|| item1.B != item2.B
				);
		}
		#endregion

		public CIELab(double l, double a, double b) 
		{
			this.L = l;
			this.A = a;
			this.B = b;
		}

		#region Methods
		public override bool Equals(Object obj) 
		{
			if(obj==null || GetType()!=obj.GetType()) return false;

			return (this == (CIELab)obj);
		}

		public override int GetHashCode() 
		{
			return L.GetHashCode() ^ A.GetHashCode() ^ B.GetHashCode();
		}

		#endregion
	} 
}
