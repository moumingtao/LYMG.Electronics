using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYMG.Electronics
{
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
