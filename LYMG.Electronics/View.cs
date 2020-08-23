using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Reflection;
using DevExpress.XtraCharts;

namespace LYMG.Electronics
{
    public partial class View : XtraUserControl
    {
        public View()
        {
            InitializeComponent();
            chartControl1.UseDirectXPaint = true;
        }

        public void SetDataSource(ISeriesContext context)
        {
            var ss = new List<Series>();

            for (var type = context.GetType().BaseType; type != null; type = type.BaseType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(SeriesContext<>))
                {
                    var tdata = type.GetGenericArguments()[0];

                    foreach (var prop in tdata.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (prop.Name == nameof(CHS24B.Time)) continue;
                        var series1 = new Series();
                        series1.ArgumentDataMember = nameof(CHS24B.Time);
                        series1.ArgumentScaleType = ScaleType.DateTime;
                        series1.Name = prop.Name;
                        series1.ValueDataMembersSerializable = prop.Name;
                        ss.Add(series1);
                    }
                }
            }

            chartControl1.DataSource = null;
            chartControl1.SeriesSerializable = ss.ToArray();
            chartControl1.DataSource = ((dynamic)context).DataSource;
        }
    }
}
