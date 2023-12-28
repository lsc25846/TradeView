using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TradeView
{
    public class SocketDataClient
    {
        private TcpClient tcpClient;
        private Thread receiveThread;
        private readonly Logger logger;
        private bool isConnected = false;

        // 定义事件以便于通知表单数据已经接收
        public event Action<StockQuote[]> DataReceived;
        // 定义一个事件，用于通知新数据
        public event Action<StockQuote[]> NewDataReceived;
        public SocketDataClient(Logger logger)
        {
            this.logger = logger;
        }

        public bool IsConnected
        {
            get { return isConnected; }
        }


        public void Connect(string server, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(server, port);
                isConnected = true;

                // 开始接收数据的线程
                receiveThread = new Thread(ReceiveData);
                receiveThread.IsBackground = true;
                receiveThread.Start();
                logger.LogInfo("連接Server成功");
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                isConnected = false;
            }
        }

        private void ReceiveData()
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] receivedBytes = new byte[32768];
            StringBuilder messageBuffer = new StringBuilder();
            int byteCount;

            while (isConnected)
            {
                byteCount = stream.Read(receivedBytes, 0, receivedBytes.Length);
                if (byteCount > 0)
                {
                    string data = Encoding.UTF8.GetString(receivedBytes, 0, byteCount);
                    messageBuffer.Append(data);
                    ProcessReceivedData(messageBuffer);
                }                                             
            }
        }
        private void ProcessReceivedData(StringBuilder messageBuffer)
        {
            string bufferContent = messageBuffer.ToString();
            int lastNewLineIndex = bufferContent.LastIndexOf('\n');

            if (lastNewLineIndex < 0) return; // 没有完整的消息

            string[] messages = bufferContent.Substring(0, lastNewLineIndex).Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            messageBuffer.Remove(0, lastNewLineIndex + 1); // 移除已处理的消息，保留不完整的消息部分

            foreach (var message in messages)
            {
                try
                {
                    var stockQuotes = JsonSerializer.Deserialize<StockQuote[]>(message);
                    DataReceived?.Invoke(stockQuotes);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex);                                        
                }
            }
        }

        public void ResetConnection()
        {
            // 首先断开现有的连接
            Disconnect();

            // 重置状态
            tcpClient = null;
            receiveThread = null;
            isConnected = false;

            // 可以在这里记录一条日志
            logger.LogInfo("Connection reset.");
        }
        public void Disconnect()
        {
            if (isConnected)
            {
                logger.LogInfo("Disconnecting from server.");

                // 标记为不再连接
                isConnected = false;

                // 关闭TcpClient
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    tcpClient = null;
                }

                // 等待接收线程结束
                if (receiveThread != null && receiveThread.IsAlive)
                {
                    receiveThread.Join();
                    receiveThread = null;
                }
            }
        }
        private string FixJson(string brokenJson)
        {
            // 插入逗号分隔符来分隔JSON对象，并将整个字符串包裹在中括号中            
            string fixedJson = "[" + brokenJson.Replace("}{", "},{") + "]";
            return fixedJson;
        }

    }
}
