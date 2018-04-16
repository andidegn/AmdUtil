using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AMD.Util
{
	/// <summary>
	/// Small timing class designed for measure how long time an operation takes
	/// </summary>
	public static class Diagnostics
	{
		/// <summary>
		/// Measures how long time an action takes to perform
		/// </summary>
		/// <param name="repetitions">The number of times to run the action</param>
		/// <param name="action">the action to be run</param>
		/// <returns>The result of the measurement</returns>
		public static Measurement Measure(int repetitions, Action action)
		{
			TimeSpan[] results = new TimeSpan[repetitions];

      Stopwatch sw = new Stopwatch();
      while (repetitions-- > 0)
			{
        GC.Collect();
        sw.Start();
				action();
				results[repetitions] = sw.Elapsed;
        sw.Reset();
			}
			return new Measurement(results);
		}

		/// <summary>
		/// Measures an action and prints it to console
		/// </summary>
		/// <param name="what">A descriptive string of what is being measured</param>
		/// <param name="repetitions">The number of times to run the action</param>
		/// <param name="action">the action to be run</param>
		public static void MeasureAndPrintToConsole(String what, int repetitions, Action action)
		{
			Measurement m = Measure(repetitions, action);
			Console.WriteLine("{0} - Avg: {1}, Min: {2}, Max: {3}", what, m.Avg, m.Min, m.Max);
		}
	}

	/// <summary>
	/// Wrapper class for measurement
	/// </summary>
	public class Measurement
	{
		/// <summary>
		/// Minimum value
		/// </summary>
		public TimeSpan Min { get; private set; }
		/// <summary>
		/// Maximum value
		/// </summary>
		public TimeSpan Max { get; private set; }
		/// <summary>
		/// Average value
		/// </summary>
		public TimeSpan Avg { get; private set; }

		/// <summary>
		/// Storing the min, max and average values from double array
		/// </summary>
		/// <param name="measurements"></param>
		public Measurement(TimeSpan[] measurements)
		{
			this.Min = measurements.Min();
			this.Max = measurements.Max();
			long avgTicks = Convert.ToInt64(measurements.Average(x => x.Ticks));
			this.Avg = new TimeSpan(avgTicks);
		}
	}
}
