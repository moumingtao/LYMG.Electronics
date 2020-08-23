using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using System.IO.Ports;

namespace LYMG.Electronics
{
    public partial class FrmMain : DevExpress.XtraEditors.XtraForm
    {
        public FrmMain()
        {
            InitializeComponent();
            var maps = new List<MapType>();
            foreach (var type in this.GetType().Assembly.GetTypes())
            {
                if (typeof(ISeriesContext).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    maps.Add(new MapType { Type = type });
            }
            lookUpEdit1.Properties.DataSource = maps;
            serialPort1.PortName = "COM5";
            lookUpEdit1.EditValue = maps[0];
        }

        #region 设备开关
        ISeriesContext SeriesContext;
        private void 打开设备(object sender, EventArgs e)
        {
            if (lookUpEdit1.GetSelectedDataRow() is MapType map)
            {
                SeriesContext = (ISeriesContext)Activator.CreateInstance(map.Type);
                view1.SetDataSource(SeriesContext);
            }
            else return;
            btnOpenClose.Click -= 打开设备;
            btnOpenClose.Click += 关闭设备;
            serialPort1.DataReceived += serialPort1_DataReceived;
            btnOpenClose.Text = "关闭";
            try
            {
                serialPort1.Open();
                propertyGridControl1.OptionsBehavior.Editable = false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
        private async void 关闭设备(object sender, EventArgs e)
        {
            btnOpenClose.Click -= 关闭设备;
            btnOpenClose.Click += 打开设备;
            btnOpenClose.Text = "启动";
            serialPort1.DataReceived -= serialPort1_DataReceived;
            btnOpenClose.Enabled = false;
            await Task.Delay(1000);// 关闭的时候要等一下，等数据停止接收
            serialPort1.Close();
            propertyGridControl1.OptionsBehavior.Editable = true;
            btnOpenClose.Enabled = true;
        }
        #endregion

        #region 设备属性
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

        #region 接收数据
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            SeriesContext?.ReciveData(serialPort1);
        }

        private void serialPort1_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            XtraMessageBox.Show("接收数据时发生错误：" + e.EventType);
        }
        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.DataReceived -= serialPort1_DataReceived;
                serialPort1.Close();
            }
            base.OnClosing(e);
        }
    }
}