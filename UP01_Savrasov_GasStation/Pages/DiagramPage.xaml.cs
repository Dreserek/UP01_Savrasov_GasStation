using System;
using System.Data.Entity; // <--- Обязательно добавить
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using MessageBox = System.Windows.MessageBox;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class DiagramPage : Page
    {
        public DiagramPage()
        {
            InitializeComponent();
            ChartPayments.ChartAreas.Add(new ChartArea("Main"));
            ChartPayments.Series.Add(new Series("Платежи") { IsValueShownAsLabel = true, ChartType = SeriesChartType.Column });

            using (var db = new DB_Savrasov_GasStationEntities1())
            {
                CbUsers.ItemsSource = db.Users.ToList();
                CbUsers.SelectedIndex = 0;
            }
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CbUsers.SelectedItem == null) return;
            int userId = (CbUsers.SelectedItem as Users).ID;

            try
            {
                using (var db = new DB_Savrasov_GasStationEntities1())
                {
                    // Добавлен Include для PaymentMethods
                    var data = db.Sales
                                 .Include(s => s.PaymentMethods)
                                 .Where(p => p.UserID == userId)
                                 .GroupBy(p => p.PaymentMethods.Name)
                                 .Select(g => new { Category = g.Key, Total = g.Sum(p => p.TotalAmount) })
                                 .ToList();

                    ChartPayments.Series[0].Points.Clear();
                    foreach (var item in data)
                    {
                        ChartPayments.Series[0].Points.AddXY(item.Category, item.Total);
                    }
                }
            }
            catch { }
        }

        private void BtnExportWord_Click(object sender, RoutedEventArgs e)
        {
            if (CbUsers.SelectedItem == null) return;
            var user = CbUsers.SelectedItem as Users;

            try
            {
                var app = new Word.Application();
                Word.Document doc = app.Documents.Add();
                Word.Paragraph p = doc.Paragraphs.Add();
                p.Range.Text = $"Отчет по платежам: {user.FIO}";
                p.Range.Font.Bold = 1;
                p.Range.Font.Size = 16;
                p.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                Word.Paragraph tableP = doc.Paragraphs.Add();
                Word.Table table = doc.Tables.Add(tableP.Range, ChartPayments.Series[0].Points.Count + 1, 2);
                table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

                table.Cell(1, 1).Range.Text = "Категория";
                table.Cell(1, 2).Range.Text = "Сумма";
                table.Rows[1].Range.Font.Bold = 1;

                for (int i = 0; i < ChartPayments.Series[0].Points.Count; i++)
                {
                    table.Cell(i + 2, 1).Range.Text = ChartPayments.Series[0].Points[i].AxisLabel;
                    table.Cell(i + 2, 2).Range.Text = ChartPayments.Series[0].Points[i].YValues[0].ToString("N2") + " руб.";
                }

                app.Visible = true;
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                doc.SaveAs2(System.IO.Path.Combine(desktopPath, "Report_AZS.docx"));
                doc.SaveAs2(System.IO.Path.Combine(desktopPath, "Report_AZS.pdf"), Word.WdExportFormat.wdExportFormatPDF);
                MessageBox.Show("Отчет Word и PDF сохранен на рабочий стол!");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка Word: " + ex.Message); }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (CbUsers.SelectedItem == null) return;

            try
            {
                var app = new Excel.Application();
                Excel.Workbook wb = app.Workbooks.Add();
                Excel.Worksheet ws = wb.Sheets[1];

                ws.Cells[1, 1] = "Категория";
                ws.Cells[1, 2] = "Сумма";

                for (int i = 0; i < ChartPayments.Series[0].Points.Count; i++)
                {
                    ws.Cells[i + 2, 1] = ChartPayments.Series[0].Points[i].AxisLabel;
                    ws.Cells[i + 2, 2] = ChartPayments.Series[0].Points[i].YValues[0];
                }

                app.Visible = true;
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                wb.SaveAs(System.IO.Path.Combine(desktopPath, "Report_AZS.xlsx"));
                MessageBox.Show("Отчет Excel сохранен на рабочий стол!");
            }
            catch (Exception ex) { MessageBox.Show("Ошибка Excel: " + ex.Message); }
        }
    }
}