namespace TradeView
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            dgvStockQuotes = new DataGridView();
            trackBarRefreshRate = new TrackBar();
            labelRefreshRate = new Label();
            refreshTimer = new System.Windows.Forms.Timer(components);
            txtBoxServer = new TextBox();
            txtBoxPort = new TextBox();
            btnConnect = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvStockQuotes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarRefreshRate).BeginInit();
            SuspendLayout();
            // 
            // dgvStockQuotes
            // 
            dgvStockQuotes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStockQuotes.Location = new Point(0, 34);
            dgvStockQuotes.Name = "dgvStockQuotes";
            dgvStockQuotes.Size = new Size(800, 367);
            dgvStockQuotes.TabIndex = 0;
            // 
            // trackBarRefreshRate
            // 
            trackBarRefreshRate.Location = new Point(684, 427);
            trackBarRefreshRate.Maximum = 3000;
            trackBarRefreshRate.Minimum = 10;
            trackBarRefreshRate.Name = "trackBarRefreshRate";
            trackBarRefreshRate.Size = new Size(104, 45);
            trackBarRefreshRate.TabIndex = 1;
            trackBarRefreshRate.Value = 10;
            // 
            // labelRefreshRate
            // 
            labelRefreshRate.AutoSize = true;
            labelRefreshRate.Location = new Point(684, 409);
            labelRefreshRate.Name = "labelRefreshRate";
            labelRefreshRate.Size = new Size(42, 15);
            labelRefreshRate.TabIndex = 2;
            labelRefreshRate.Text = "label1";
            // 
            // txtBoxServer
            // 
            txtBoxServer.Location = new Point(12, 497);
            txtBoxServer.Name = "txtBoxServer";
            txtBoxServer.Size = new Size(100, 23);
            txtBoxServer.TabIndex = 3;
            txtBoxServer.Text = "127.0.0.1";
            // 
            // txtBoxPort
            // 
            txtBoxPort.Location = new Point(141, 497);
            txtBoxPort.Name = "txtBoxPort";
            txtBoxPort.Size = new Size(100, 23);
            txtBoxPort.TabIndex = 4;
            txtBoxPort.Text = "5566";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(256, 497);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 5;
            btnConnect.Text = "連接";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 532);
            Controls.Add(btnConnect);
            Controls.Add(txtBoxPort);
            Controls.Add(txtBoxServer);
            Controls.Add(labelRefreshRate);
            Controls.Add(trackBarRefreshRate);
            Controls.Add(dgvStockQuotes);
            Name = "Form1";
            Text = "Form1";
            FormClosing += StockQuoteForm_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvStockQuotes).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarRefreshRate).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvStockQuotes;
        private TrackBar trackBarRefreshRate;
        private Label labelRefreshRate;
        private System.Windows.Forms.Timer refreshTimer;
        private TextBox txtBoxServer;
        private TextBox txtBoxPort;
        private Button btnConnect;
    }
}
