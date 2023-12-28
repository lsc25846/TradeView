using System;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;

namespace TradeView
{
    public partial class Form1 : Form
    {
        private SocketDataClient dataClient;
        private StockQuote[] latestStockQuotes = null;
        private Logger logger;
        public Form1()
        {
            InitializeComponents();

            // 加载股票名称
            var stockNames = LoadStockNames("StockName.json"); // 替换为JSON文件的实际路径

            // 用股票名称填充DataGridView
            foreach (var stockName in stockNames)
            {
                dgvStockQuotes.Rows.Add(stockName, null, null, null);
            }
            //初始化紀錄log路徑
            logger = new Logger("logfile.txt");

            // 初始化数据客户端并订阅事件
            dataClient = new SocketDataClient(logger);
            dataClient.DataReceived += DataClient_NewDataReceived;
            // 初始化 Timer
            refreshTimer = new System.Windows.Forms.Timer();
            //refreshTimer.Interval = 1000; // 預設更新間隔設為10毫秒
            refreshTimer.Tick += new EventHandler(RefreshTimer_Tick);
            refreshTimer.Start();
        }
        private void InitializeComponents()
        {
            // 窗口基本设置
            this.Text = "股票報價列表";
            this.Size = new Size(800, 800);

            // 创建 DataGridView
            dgvStockQuotes = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false, // 不允许用户手动添加行
            };

            // 添加列
            dgvStockQuotes.Columns.Add("stockName", "股票名稱");
            dgvStockQuotes.Columns.Add("transactionPrice", "成交價");
            dgvStockQuotes.Columns.Add("change", "漲跌幅");
            dgvStockQuotes.Columns.Add("volume", "成交量");

            // 添加到窗口中
            this.Controls.Add(dgvStockQuotes);

            // 创建 TrackBar 来控制更新频率
            trackBarRefreshRate = new TrackBar
            {
                Dock = DockStyle.Bottom,
                Minimum = 10,
                Maximum = 3000,
                Value = 10,
                TickFrequency = 10,
                SmallChange = 10,
                LargeChange = 50
            };

            // 更新频率标签
            labelRefreshRate = new Label
            {
                Dock = DockStyle.Bottom,
                Text = "更新頻率: 100ms"
            };

            // 添加事件处理
            trackBarRefreshRate.Scroll += TrackBarRefreshRate_Scroll;

            // 添加控件到窗口中
            this.Controls.Add(labelRefreshRate);
            this.Controls.Add(trackBarRefreshRate);
            // 创建包含 TextBox 和 Button 的 FlowLayoutPanel
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.LeftToRight
            };

            // 创建服务器地址 TextBox
            txtBoxServer = new TextBox
            {
                Width = 200, // 设置适当的宽度
                PlaceholderText = "127.0.0.1"
            };

            // 创建端口号 TextBox
            txtBoxPort = new TextBox
            {
                Width = 100, // 设置适当的宽度
                PlaceholderText = "5566"
            };

            // 创建连接按钮
            btnConnect = new Button
            {
                Width = 100, // 设置适当的宽度
                Text = "連接"
            };
            btnConnect.Click += btnConnect_Click;
            // 将 TextBox 和 Button 控件添加到 FlowLayoutPanel 中
            panel.Controls.Add(txtBoxServer);
            panel.Controls.Add(txtBoxPort);
            panel.Controls.Add(btnConnect);

            // 将 FlowLayoutPanel 添加到窗口中
            this.Controls.Add(panel);
            
        }
        #region "Client 接收"
        private void btnConnect_Click(object sender, EventArgs e)
        {
            // 从UI获取服务器和端口

            string server = txtBoxServer.Text;
            string portText = txtBoxPort.Text;
            // 检查服Server地址和port是否有效
            if (!IsValidIPAddress(server))
            {
                MessageBox.Show("無效的伺服器IP。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(portText, out int port) || port < 0 || port > 65535)
            {
                MessageBox.Show("無效的port號碼。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 连接服务器
            dataClient.Connect(server, port);
            if (dataClient.IsConnected) 
            {
                //連線成功後，按鈕改為綠色
                btnConnect.BackColor = Color.LightSeaGreen;
                //禁用按鈕避免重複點擊
                btnConnect.Enabled = false;
                btnConnect.Text = "連線中";
            }
            else
            {
                // 如果没有连接到，重置dataClient并准备下一次连接
                dataClient.ResetConnection();
                // 显示错误消息
                MessageBox.Show("無法連接到Server，請檢察IP與Port是否正確。", "連接失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.LogInfo("無法連接到Server，請檢察IP與Port是否正確。");
            }

        }

        // 验证IP地址格式是否正确
        private bool IsValidIPAddress(string ipAddress)
        {
            return IPAddress.TryParse(ipAddress, out _);
        }
        private void DataClient_NewDataReceived(StockQuote[] stockQuotes)
        {
            // 保存最新数据，等待Timer更新界面
            latestStockQuotes = stockQuotes;
        }

        private void DataClient_DataReceived(StockQuote[] stockQuotes)
        {
            // 需要使用Invoke来确保跨线程操作UI是安全的
            Invoke(new System.Windows.Forms.MethodInvoker(() =>
            {
                UpdateStockQuotes(stockQuotes);
            }));
        }

        private void StockQuoteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataClient?.Disconnect();
        }
        #endregion "Client 接收"


        

        private List<string> LoadStockNames(string filePath)
        {
            try
            {
                string jsonContent = ReadEmbeddedResource("TradeView.StockName.json");                
                //string jsonContent = File.ReadAllText(filePath);
                List<string> stockNames = JsonSerializer.Deserialize<List<string>>(jsonContent);
                return stockNames ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading stock names: " + ex.Message);
                return new List<string>();
            }
        }
        public static string ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void TrackBarRefreshRate_Scroll(object sender, EventArgs e)
        {
            // 更新标签以反映新的更新频率
            labelRefreshRate.Text = $"更新頻率: {trackBarRefreshRate.Value}ms";
            // 这里你可以添加代码来改变数据更新的定时器的间隔
            // 获取TrackBar的当前值
            int interval = trackBarRefreshRate.Value;
            // 设置Timer的Interval
            refreshTimer.Interval = interval;
        }

        // 你可以添加一个方法来更新DataGridView的数据        
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // 使用最新数据更新界面
            if (latestStockQuotes != null)
            {
                UpdateStockQuotes(latestStockQuotes);
                latestStockQuotes = null;
            }
        }

        //更新DataGridView的数据
        private void UpdateStockQuotes(StockQuote[] stockQuotes)
        {
            // 假设股票数据的顺序与DataGridView中的行顺序相对应
            for (int i = 0; i < stockQuotes.Length; i++)
            {
                var quote = stockQuotes[i];

                // 检查是否有足够的行，如果没有，则添加新行
                if (dgvStockQuotes.Rows.Count <= i)
                {
                    dgvStockQuotes.Rows.Add();
                }

                // 更新成交价、涨跌幅和成交量
                dgvStockQuotes.Rows[i].Cells["transactionPrice"].Value = quote.TransactionPrice;
                dgvStockQuotes.Rows[i].Cells["change"].Value = $"{quote.Change:P2}"; // 格式化为百分比
                dgvStockQuotes.Rows[i].Cells["volume"].Value = quote.Volume;

                // 根据涨跌幅设置行的颜色
                dgvStockQuotes.Rows[i].DefaultCellStyle.ForeColor = quote.Change >= 0 ? Color.Green : Color.Red;
            }

            // 移除多余的行（如果有的话）
            while (dgvStockQuotes.Rows.Count > stockQuotes.Length)
            {
                dgvStockQuotes.Rows.RemoveAt(dgvStockQuotes.Rows.Count - 1);
            }
        }
    
    }
}
