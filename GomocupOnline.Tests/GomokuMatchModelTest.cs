using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GomocupOnline.Models;

namespace GomocupOnline.Tests
{
    [TestClass]
    [DeploymentItem(@"..\..\TestData\1x0-16(2).psq")]
    [DeploymentItem(@"..\..\TestData\0x1-19(1).psq")]
    public class GomokuMatchModelTest
    {
        [TestMethod]
        public void TestGomokuMatchModeParse()
        {
            GomokuMatchModel match = new GomokuMatchModel(@"1x0-16(2).psq");

            Assert.AreEqual("1x0-16(2).psq", match.FileName);
            Assert.AreEqual(20, match.Width);
            Assert.AreEqual(20, match.Height);
            Assert.AreEqual(0, match.Result);
            Assert.AreEqual("pisq7", match.Player1);
            Assert.AreEqual("renjusolver", match.Player2);

            Assert.AreEqual(16, match.Moves.Length);

            Assert.AreEqual(10, match.Moves[0].X);
            Assert.AreEqual(10, match.Moves[0].Y);
            Assert.AreEqual(456, match.Moves[0].DurationMS);

            Assert.AreEqual(11, match.Moves[1].X);
            Assert.AreEqual(12, match.Moves[1].Y);
            Assert.AreEqual(4500, match.Moves[1].DurationMS);

            Assert.AreEqual(15, match.Moves[15].X);
            Assert.AreEqual(12, match.Moves[15].Y);
            Assert.AreEqual(32, match.Moves[15].DurationMS);
        }

         [TestMethod]
        public void TestGomokuMatchModeParse2()
        {
            GomokuMatchModel match = new GomokuMatchModel(@"0x1-19(1).psq");

            Assert.AreEqual("sWINe13", match.Player1);
            Assert.AreEqual("pela", match.Player2);
        }

        
    }
}
