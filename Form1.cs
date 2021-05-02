using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace TradesViewer
{
    public partial class Form1 : Form
    {
        private readonly DataTable _table = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataColumn assetColumn = _table.Columns.Add("Asset", typeof(string));
            assetColumn.Caption = "Asset";
            DataColumn tradesColumn = _table.Columns.Add("Trades", typeof(int));
            tradesColumn.Caption = "# Trades";
            DataColumn averagePercentageGainColumn = _table.Columns.Add("AveragePercentGain", typeof(decimal));
            averagePercentageGainColumn.Caption = "Avg Percent Gain";
            DataColumn numberOfGainsOverTenPercent = _table.Columns.Add("NumberGainsOverTenPercent", typeof(int));
            numberOfGainsOverTenPercent.Caption = "# Gains Over 10%";
            DataColumn averageGainOverTenPercent =
                _table.Columns.Add("AveragePercentGainOverTenPercent", typeof(decimal));
            averageGainOverTenPercent.Caption = "Avg Percent Gain Over 10%";
            DataColumn tradeWinsCountColumn = _table.Columns.Add("TradeWinsCount", typeof(int));
            tradeWinsCountColumn.Caption = "# Trade Wins";
            DataColumn tradeLossesCountColumn = _table.Columns.Add("TradeLossesCount", typeof(int));
            tradeLossesCountColumn.Caption = "# Trade Losses";
            DataColumn averagePercentLossesColumn = _table.Columns.Add("AveragePercentLosses", typeof(decimal));
            averagePercentLossesColumn.Caption = "Avg Percent Losses";

            dgTradeView.DataSource = _table;

            foreach (DataGridViewColumn column in dgTradeView.Columns)
            {
                column.HeaderText = _table.Columns[column.HeaderText].Caption;
            }
        }

        private void loadFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _table.Clear();
            
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    string[] filePaths = Directory.GetFiles(folderBrowserDialog.SelectedPath);

                    foreach (string path in filePaths)
                    {
                        TradeHistory tradeHistory = ProcessFile(path);
                        
                        AssetSummary assetSummary = tradeHistory.GetSummary();
                        
                        _table.Rows.Add(
                            assetSummary.Asset, 
                            assetSummary.NumberOfTrades, 
                            assetSummary.AverageWinPercentage, 
                            assetSummary.NumberOfTradesOverTenPercent, 
                            assetSummary.AveragePercentGainOverTenPercent, 
                            assetSummary.NumberOfWins, 
                            assetSummary.NumberOfLosses, 
                            assetSummary.AverageLosingPercent);
                    }
                }
            }
        }

        private static TradeHistory ProcessFile(string path)
        {
            if (!GetAssetNameFromPath(path, out string asset))
            {
                return null;
            }

            TradeHistory history = new TradeHistory(asset);

            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    history.ParseTrade(line);
                }
            }

            return history;
        }

        private static bool GetAssetNameFromPath(string path, out string asset)
        {
            int lastIndexOf = path.LastIndexOf("\\", StringComparison.Ordinal);
            if (lastIndexOf == -1)
            {
                asset = null;
                return false;
            }

            int lastIndexOfDot = path.LastIndexOf(".", StringComparison.Ordinal);

            if (lastIndexOfDot == -1)
            {
                asset = null;
                return false;
            }

            asset = path.Substring(lastIndexOf + 1, lastIndexOfDot - lastIndexOf - 1);
            return true;
        }
    }
}