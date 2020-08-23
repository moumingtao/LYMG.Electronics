using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYMG.Electronics
{
    public class CHS24B
    {
        public DateTime Time { get; set; }
        public float IN1 { get; set; }
        public float IN2 { get; set; }
        public float IN3 { get; set; }
        public float IN4 { get; set; }
        public float IN5 { get; set; }
        public float IN6 { get; set; }
        public float IN7 { get; set; }
        public float IN8 { get; set; }
    }
    public class CHS10B
    {
        public DateTime Time { get; set; }
        public short IO1 { get; set; }
        public short IO2 { get; set; }
        public short IO3 { get; set; }
        public short IO4 { get; set; }
        public short IO5 { get; set; }
        public short IO6 { get; set; }
        public short IO7 { get; set; }
        public short IO8 { get; set; }

        internal CHS10B Clone() => (CHS10B)MemberwiseClone();
    }
}
