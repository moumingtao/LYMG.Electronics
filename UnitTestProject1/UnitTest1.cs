using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //var bytes = new byte[] { 0x40, 0xbc, 0x7a, 0xe1 };
            //bytes = new byte[] { 0xe1 ,0x7a,0xbc, 0x40  };
            //var value = BitConverter.ToSingle(bytes, 0);
            //Assert.AreEqual(value, 23.56);
            var bytes = BitConverter.GetBytes(23.56F);
            for (int i = 0; i < bytes.Length; i++)
            {
                //Console.WriteLine(Convert.ToString(bytes[i], 2).PadLeft(8, '0'));
                Console.WriteLine(bytes[i].ToString("X"));
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            int data1 = 0xe1;
            int data2 = 0x7a;
            int data3 = 0xbc;
            int data4 = 0x40;

            int data = data1 << 24 | data2 << 16 | data3 << 8 | data4;

            int nSign;
            if ((data & 0x80000000) > 0)
            {
                nSign = -1;
            }
            else
            {
                nSign = 1;
            }
            int nExp = data & (0x7F800000);
            nExp = nExp >> 23;
            float nMantissa = data & (0x7FFFFF);

            if (nMantissa != 0)
                nMantissa = 1 + nMantissa / 8388608;

            float value = nSign * nMantissa * (2 << (nExp - 128));
        }
    }
}
