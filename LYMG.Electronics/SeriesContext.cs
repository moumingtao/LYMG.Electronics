using DevExpress.XtraCharts;
using LYMG.Electronics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using View = LYMG.Electronics.View;

namespace LYMG.Electronics
{
    public interface ISeriesContext
    {
        void ReciveData(SerialPort serialPort);
    }
    public abstract class SeriesContext<TData> : ISeriesContext
        where TData : CHS24B
    {
        public SeriesContext()
        {
            outBuffer[0] = 0x24;
            InputStartCH = 1;
            InputEndCH = 8;
            outBuffer[15] = 15;
        }
        public BindDataSource<TData> DataSource = new BindDataSource<TData>();

        protected byte[] buffer = new byte[34];
        int bufferPosition;
        public CHS10B CHS10B;
        public virtual void ReciveData(SerialPort serialPort)
        {
            while (true)
            {
                var data = ReciveOne(serialPort);
                if (data == null)
                {
                    Application.DoEvents();
                    DataSource.NotifyAdd();
                    return;
                }
                DataSource.Add(data);
            }
        }
        TData ReciveOne(SerialPort serialPort)
        {
            var frameSize = (InputEndCH - InputStartCH + 1) * 4 + 1;

            //do
            //{
            #region 查找帧起始字
            if (bufferPosition == 0)
            {
                while (true)
                {
                    var b = serialPort.ReadByte();
                    if (b < 0) return null;// 没找到帧开始字节
                    if (b == 0x24) break;
                }
            }
            #endregion

            #region 读取帧后面的内容
            var needCount = frameSize - bufferPosition;
            var count = serialPort.Read(buffer, bufferPosition, needCount);
            if (count < needCount)
            {
                bufferPosition += count;
                return null;
            }
            bufferPosition = 0;
            #endregion
            //} while (buffer[frameSize - 1] != frameSize);// 检查帧长度，模块返回的帧长度有时不对，不检查

            #region 读取24位模拟电压
            var input = CreateDataItem();
            input.Time = DateTime.Now;
            int readOffset = 0;
            for (int ch = InputStartCH; ch <= InputEndCH; ch++)
            {
                // C++的字节顺序是反的
                //var b = buffer[readOffset];
                //buffer[readOffset] = buffer[readOffset + 3];
                //buffer[readOffset + 3] = b;
                //b = buffer[readOffset + 1];
                //buffer[readOffset + 1] = buffer[readOffset + 2];
                //buffer[readOffset + 2] = b;

                switch (ch)
                {
                    case 1: input.IN1 = BitConverter.ToSingle(buffer, readOffset); break;
                    case 2: input.IN2 = BitConverter.ToSingle(buffer, readOffset); break;
                    case 3: input.IN3 = BitConverter.ToSingle(buffer, readOffset); break;
                    case 4: input.IN4 = BitConverter.ToSingle(buffer, readOffset); break;
                    case 5: input.IN5 = BitConverter.ToSingle(buffer, readOffset); break;
                    case 6: input.IN6 = BitConverter.ToSingle(buffer, readOffset); break;
                    case 7: input.IN7 = BitConverter.ToSingle(buffer, readOffset); break;
                    case 8: input.IN8 = BitConverter.ToSingle(buffer, readOffset); break;
                }
                readOffset += 4;
            }
            #endregion

            FpsCounter++;
            InitDataItem(input);
            return input;
        }
        #region 输出
        byte[] outBuffer = new byte[16];
        /// <summary>
        /// 设置输入从多少通道开始
        /// </summary>
        public byte InputStartCH
        {
            get => outBuffer[1];
            set => outBuffer[1] = value;
        }
        /// <summary>
        /// 设置输入到多少通道结束
        /// </summary>
        public byte InputEndCH
        {
            get => outBuffer[2];
            set => outBuffer[2] = value;
        }
        public void SetPWM(int ch, ushort value)
        {
            var p = ch << 1;
            outBuffer[1 + p] = (byte)value;
            outBuffer[2 + p] = (byte)(value >> 8);
        }
        public ushort GetPWM(int ch)
        {
            var p = ch << 1;
            return (ushort)((outBuffer[2 + p] << 8) + outBuffer[1 + p]);
        }
        public void SetDevice(SerialPort serialPort)
        {
            serialPort.Write(outBuffer, 0, outBuffer.Length);
        }
        #endregion
        public int FpsCounter;
        public int LastFps;

        protected abstract TData CreateDataItem();
        protected virtual void InitDataItem(TData data) { }
    }
}
