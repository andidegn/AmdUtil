using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.LiniarAlgebra
{
  public class Trendline
  {
    public Trendline(IList<double> yAxisValues, IList<double> xAxisValues)
        : this(yAxisValues.Select((t, i) => (xAxisValues[i], t)))
    { }

    public Trendline(IEnumerable<(double, double)> data)
    {
      List<(double, double)> cachedData = data.ToList();

      int n = cachedData.Count;
      double sumX = cachedData.Sum(x => x.Item1);
      double sumX2 = cachedData.Sum(x => x.Item1 * x.Item1);
      double sumY = cachedData.Sum(x => x.Item2);
      double sumXY = cachedData.Sum(x => x.Item1 * x.Item2);

      Slope = (sumXY - ((sumX * sumY) / n)) / (sumX2 - (sumX * sumX / n));

      Intercept = (sumY / n) - (Slope * (sumX / n));

      Start = GetYValue(cachedData.Min(a => a.Item1));
      End = GetYValue(cachedData.Max(a => a.Item1));
    }

    public double Slope { get; private set; }
    public double Intercept { get; private set; }
    public double Start { get; private set; }
    public double End { get; private set; }

    public double GetYValue(double xValue)
    {
      return Intercept + Slope * xValue;
    }
  }
}
