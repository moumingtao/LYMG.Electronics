﻿using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LYMG.Electronics
{
    public partial class FrmMain : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        #region 串口开关
        private void tcSerialPort_Toggled(object sender, EventArgs e)
        {
            if (tcSerialPort.IsOn)
            {
                try
                {
                    serialPort1.Open();
                    accordionControlElement2.Text = serialPort1.PortName + "已打开";
                    accordionControlElement2.Appearance.Normal.ForeColor = Color.Green;
                    propertyGridControl1.OptionsBehavior.Editable = false;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                    tcSerialPort.IsOn = false;
                }
            }
            else
            {
                serialPort1.Close();
                accordionControlElement2.Text = "设备已关闭";
                accordionControlElement2.Appearance.Normal.ForeColor = Color.Empty;
                propertyGridControl1.OptionsBehavior.Editable = true;
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
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
        }
        private void serialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            tcSerialPort.IsOn = false;
            XtraMessageBox.Show("接收数据时发生错误：" + e.EventType);
        }
        #endregion

    }
}
