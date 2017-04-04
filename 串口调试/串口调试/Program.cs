using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 串口调试
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            // 模拟数据包
        #if false
            byte[] a1 = { 0xA7, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0xd0, 0x01, 0x01 };
            byte[] a2 = { 0x01, 0x02, 0x03, 0x04 };
            byte[] a3 = { 0xA7, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0d, 0x01, 0x01 };
            byte[] a4 = { 0x00, 0x00, 0x05, 0x01, 0x01 };
            UartProtocol up = new UartProtocol();

            byte[] b = null;

            up.revNewData(a1);

            while (!up.isFinished())
            {
                up.revNewData(a2);
            }
            b = up.packageData();
        #endif
        }
    }
}
