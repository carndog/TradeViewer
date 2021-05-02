using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NodaTime;

namespace TradesViewer
{
    public partial class Form1 : Form
    {
        private readonly DataTable _table = new DataTable();

        private readonly string[] _months = new[]
        {
            "January", "February", "March", "April",
            "May", "June", "July", "August", "September",
            "October", "November", "December"
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BuildColumns();

            dgTradeView.DataSource = _table;

            foreach (DataGridViewColumn column in dgTradeView.Columns)
            {
                column.HeaderText = _table.Columns[column.HeaderText].Caption;
            }
        }

        private void BuildColumns()
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

            TimeSpan oneYear = new TimeSpan(365, 0, 0, 0);
            MonthIterator months = new MonthIterator(DateTime.Now.Subtract(oneYear));

            int i = 0;
            foreach (DateTime dateTime in months)
            {
                string heading = _months[dateTime.Month - 1] + " " + dateTime.Year;
                DataColumn currentMonthColumn = new DataColumn("month" + i++)
                {
                    Caption = heading,
                    DataType = typeof(int)
                };
                _table.Columns.Add(currentMonthColumn);
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

                        object[] rowData = {
                            assetSummary.Asset,
                            assetSummary.NumberOfTrades,
                            assetSummary.AverageWinPercentage,
                            assetSummary.NumberOfTradesOverTenPercent,
                            assetSummary.AveragePercentGainOverTenPercent,
                            assetSummary.NumberOfWins,
                            assetSummary.NumberOfLosses,
                            assetSummary.AverageLosingPercent
                        };

                        object[] row = new object[rowData.Length + assetSummary.NumberInMonth.Length];
                        rowData.CopyTo(row, 0);
                        assetSummary.NumberInMonth.CopyTo(row, rowData.Length);

                        _table.Rows.Add(row);
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