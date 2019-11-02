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

namespace LYMG.Electronics.Handlers
{
    public partial class View : DevExpress.XtraEditors.XtraUserControl
    {
        public View(SeriesContext context)
        {
            InitializeComponent();
            chartControl1.DataSource = context;
        }
    }
}
