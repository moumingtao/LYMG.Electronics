using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LYMG.Electronics
{
    public partial class FrmMain : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        public FrmMain()
        {
            InitializeComponent();
            ReadStorage();
            InitContextTypes();
        }

        #region 串口开关
        void ReadStorage()
        {
            serialPort1.PortName = Program.Storage.Others.Value<string>("PortName");
            serialPort1.BaudRate = Program.Storage.Others.Value<int>("BaudRate");
            serialPort1.Parity = (Parity)Program.Storage.Others.Value<int>("Parity");
            serialPort1.DataBits = Program.Storage.Others.Value<int>("DataBits");
            serialPort1.StopBits = (StopBits)Program.Storage.Others.Value<int>("StopBits");
        }
        void WriteStorage()
        {
            Program.Storage.Others["PortName"] = serialPort1.PortName;
            Program.Storage.Others["BaudRate"] = serialPort1.BaudRate;
            Program.Storage.Others["Parity"] = (int)serialPort1.Parity;
            Program.Storage.Others["DataBits"] = serialPort1.DataBits;
            Program.Storage.Others["StopBits"] = (int)serialPort1.StopBits;
        }
        async void tcSerialPort_Toggled(object sender, EventArgs e)
        {
            serialPort1.DataReceived -= serialPort1_DataReceived;// 停止数据的接收
            if (tcSerialPort.IsOn)
            {
                try
                {
                    serialPort1.Open();
                    accordionControlElement2.Text = serialPort1.PortName + "已打开";
                    accordionControlElement2.Appearance.Normal.ForeColor = Color.Green;
                    propertyGridControl1.OptionsBehavior.Editable = false;
                    serialPort1.DataReceived += serialPort1_DataReceived;// 开始数据接收
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                    tcSerialPort.IsOn = false;
                }
            }
            else
            {
                tcSerialPort.Enabled = false;
                await Task.Delay(500);// 关闭的时候要等一下，等数据停止接收
                serialPort1.Close();
                accordionControlElement2.Text = "设备已关闭";
                accordionControlElement2.Appearance.Normal.ForeColor = Color.Empty;
                propertyGridControl1.OptionsBehavior.Editable = true;
                tcSerialPort.Enabled = true;
            }
        }
        private void cmbPortNames_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
        }

        private void propertyGridControl1_CustomRecordCellEdit(object sender, DevExpress.XtraVerticalGrid.Events.GetCustomRowCellEditEventArgs e)
        {
            switch (e.Row.Properties.FieldName)
            {
                case nameof(SerialPort.PortName):
                    e.RepositoryItem = cmbPortNames;
                    break;
            }
        }
        private void propertyGridControl1_ShowingEditor(object sender, CancelEventArgs e)
        {
            cmbPortNames.Items.Clear();
            var ports = SerialPort.GetPortNames();
            cmbPortNames.Items.AddRange(ports);
        }
        #endregion

        #region 收发数据
        void InitContextTypes()
        {
            cmbContextTypes.Properties.Items.Add(new SeriesContext());
            foreach (var item in cmbContextTypes.Properties.Items)
            {
                if (item.ToString() == Program.Storage.SelectedContextType)
                    cmbContextTypes.SelectedItem = item;
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (cmbContextTypes.EditValue is SeriesContext context)
                context.ReciveData(serialPort1);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (cmbContextTypes.EditValue is SeriesContext context)
            {
                context.LastFps = context.FpsCounter;
                context.FpsCounter = 0;
                lblFps.Text = context.LastFps + "sps";

                propertyGridControl2.SelectedObject = context.LastFrame;
                propertyGridControl2.ExpandAllRows();

                if(serialPort1.IsOpen)
                    context.SetDevice(serialPort1);
            }
        }
        private void serialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            tcSerialPort.IsOn = false;
            XtraMessageBox.Show("接收数据时发生错误：" + e.EventType);
        }

        private void cmbContextTypes_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbContextTypes.EditValue is SeriesContext context)
            {
                fluentDesignFormContainer1.Controls.Clear();
                context.Control.Dock = DockStyle.Fill;
                fluentDesignFormContainer1.Controls.Add(context.Control);
            }
        }
        #endregion

        async void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Storage.SelectedContextType = cmbContextTypes.Text;
            serialPort1.DataReceived -= serialPort1_DataReceived;
            e.Cancel = true;
            this.FormClosing -= FrmMain_FormClosing;

            await Task.Delay(500);
            WriteStorage();
            serialPort1.Close();
            e.Cancel = false;
            this.Close();
        }

    }
}
