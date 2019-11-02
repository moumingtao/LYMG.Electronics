using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LYMG.Electronics
{
    public class Program
    {
        #region 数据
        public static Program Storage { get; private set; }
        [JsonProperty]
        public JObject Others;
        #endregion

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            const string file = "Storage.json";
            string json;
            if (File.Exists(file))
            {
                json = File.ReadAllText(file);
                Storage = JsonConvert.DeserializeObject<Program>(json);
            }
            else
            {
                Storage = new Program
                {
                    Others = new JObject(),
                };
            }
            Application.Run(new FrmMain());

            json = JsonConvert.SerializeObject(Storage);
            File.WriteAllText(file, json);
        }
    }
}
