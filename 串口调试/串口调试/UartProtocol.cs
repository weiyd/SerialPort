using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 串口调试
{
    public class UartProtocol
    {
        /// <summary>
        /// 接收新的串口数据保存地址
        /// </summary>
        private byte[] m_data = null;
        /// <summary>
        /// 新的串口数据长度
        /// </summary>
        private int m_Length = 0;
        /// <summary>
        /// 检测数据包头标志位
        /// </summary>
        private bool findHeadFlag = false;
        /// <summary>
        /// 当前数据包的总长度(包含协议头)
        /// </summary>
        private int packetLen = 0;
        /// <summary>
        /// 整个数据包的保存地址
        /// </summary>
        static public byte[] revdata = null;
        /// <summary>
        /// 整个数据包的索引
        /// </summary>
        static private int revdataIndex = 0;
        /// <summary>
        /// 数据包接收完毕标志位
        /// </summary>
        static private bool revFinished = false;



        /// <summary>
        /// 接收新的数据
        /// </summary>
        public void revNewData(byte[] data)
        {
            m_Length = data.Length;
            m_data = new byte[m_Length];
            for (int m_dataIndex = 0; m_dataIndex < m_Length; m_dataIndex++)
                m_data[m_dataIndex] = data[m_dataIndex];
            PackageAnalysis();
        }

        /// <summary>
        /// 检查接收到的数据是否含有协议头(当前没有协议头的情况)
        /// </summary>
        private bool isfindHead()
        {
            if (findHeadFlag == false)
            {
                if ((m_data[0] & 0xFF) == 0XA7 && (m_data[1] & 0xFF) == 0XB8 && (m_data[2] & 0xFF) == 0X00)
                {
                    findHeadFlag = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 对接收到的数据进行协议分析
        /// 0xA7 0xB8 0x00 包头
        /// 0x00 数据类型
        /// 0x00 0x00 0x00 0x10 数据包长度(当前数据包大小是16)
        /// 可以在此处添加更多的协议头
        /// </summary>
        private void PackageAnalysis()
        {
            if (findHeadFlag == false)
            {
                if (isfindHead())
                {
                    // 读取4-7个数据作为数据包长度
                    Array.Reverse(m_data, 4, 4);
                    packetLen = BitConverter.ToInt32(m_data, 4);
                    // 数据包长度小于当前传进的数据长度对数据进行截断处理
                    if (packetLen < m_Length)
                        m_Length = packetLen;
                    revdata = new byte[packetLen];
                    System.Array.Copy(m_data, 0, revdata, 0, m_Length);
                    revdataIndex += m_Length;
                    if (revdataIndex >= packetLen)
                    {
                        findHeadFlag = false;
                        revFinished = true;
                        revdataIndex = 0;
                    }
                }
            }
            else
            {
                if(m_Length>packetLen-revdataIndex)
                {
                    m_Length = packetLen - revdataIndex;
                    System.Array.Copy(m_data, 0, revdata, revdataIndex, m_Length);  
                }
                else
                {
                    System.Array.Copy(m_data, 0, revdata, revdataIndex, m_Length);
                }

                revdataIndex += m_Length;
                if (revdataIndex >= packetLen)
                {
                    findHeadFlag = false;
                    revFinished = true;
                    revdataIndex = 0;
                }
            }
        }

        /// <summary>
        /// 回传数据包中的数据
        /// </summary>
        /// <returns></returns>
        public byte[] packageData()
        {
            byte[] temp = null;
            temp = new byte[packetLen - 8];
            // 去掉数据包头的8个字节
            Array.Copy(revdata, 8, temp, 0, packetLen - 8);
            return temp;
        }

        /// <summary>
        /// 判断当前数据包是否接收完毕
        /// </summary>
        /// <returns></returns>
        public bool isFinished()
        {
            return revFinished;
        }
    }
    public class test
    {
        static public int a = 1;
    }
}
