using System;
using AMD.Util.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AMDUtilUnitTest
{
  [TestClass]
  public class ExtensionDouble
  {
    [TestMethod]
    public void DoubleIsFiniteNumber()
    {
      double d = 1.234;

      Assert.AreEqual(true, d.IsFinite());
    }

    [TestMethod]
    public void DoubleIsFiniteNaN()
    {
      double d = double.NaN;

      Assert.AreEqual(false, d.IsFinite());
    }

    [TestMethod]
    public void DoubleIsFiniteNegInf()
    {
      double d = double.NegativeInfinity;

      Assert.AreEqual(false, d.IsFinite());
    }

    [TestMethod]
    public void DoubleIsFinitePosInf()
    {
      double d = double.PositiveInfinity;

      Assert.AreEqual(false, d.IsFinite());
    }
  }
}
