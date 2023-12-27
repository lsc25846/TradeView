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
        private bool isConnected = false;

        // 定义事件以便于通知表单数据已经接收
        public event Action<StockQuote[]> DataReceived;
        // 定义一个事件，用于通知新数据
        public event Action<StockQuote[]> NewDataReceived;

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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to server: " + ex.Message);
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
                    Console.WriteLine("JSON Parsing Error: " + ex.Message);
                    // 可能需要进一步处理错误或部分消息
                }
            }
        }


        public void Disconnect()
        {
            isConnected = false;
            tcpClient?.Close();
        }
        private string FixJson(string brokenJson)
        {
            // 插入逗号分隔符来分隔JSON对象，并将整个字符串包裹在中括号中
            string fixedJson = "[" + brokenJson.Replace("}{", "},{") + "]";
            return fixedJson;
        }

    }
}
