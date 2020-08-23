using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYMG.Electronics.Tables
{
    public class CHS24B
    {
        [SugarColumn(IsPrimaryKey = true)]
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
    public class HistoryInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string MapType { get; set; }// 他决定了表名称，他输出的数据类型决定表结构
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
    }
    public class MapType
    {
        public Type Type;
        public string Name => Type.FullName;
    }
}
