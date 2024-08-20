using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System.Drawing.Imaging;

namespace EmployeeDataVisualizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code={vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==}";
            var htmlFilePath = "employees.html";
            var pieChartFilePath = "piechart.png";

            
            var employees = await FetchEmployeeData(apiUrl);


            GenerateHtmlTable(employees, htmlFilePath);


            GeneratePieChart(employees, pieChartFilePath);

            Console.WriteLine("HTML table and Pie chart generated successfully.");
        }

        private static async Task<List<Employee>> FetchEmployeeData(string url)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);
            var employees = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Employee>>(response);
            return employees;
        }

        private static void GenerateHtmlTable(List<Employee> employees, string filePath)
        {
            employees = employees.OrderByDescending(e => e.TotalTimeWorked).ToList();

            var htmlDoc = new HtmlDocument();
            var html = "<html><body><table border='1'><tr><th>Name</th><th>Total Time Worked</th></tr>";

            foreach (var employee in employees)
            {
                var color = employee.TotalTimeWorked < 100 ? "red" : "white";
                html += $"<tr style='background-color: {color};'><td>{employee.Name}</td><td>{employee.TotalTimeWorked}</td></tr>";
            }

            html += "</table></body></html>";
            htmlDoc.LoadHtml(html);
            htmlDoc.Save(filePath);
        }

        private static void GeneratePieChart(List<Employee> employees, string filePath)
        {
            var totalWorkedHours = employees.Sum(e => e.TotalTimeWorked);
            var pieSeries = new PieSeries();

            foreach (var employee in employees)
            {
                var percentage = (employee.TotalTimeWorked / (double)totalWorkedHours) * 100;
                pieSeries.Slices.Add(new PieSlice(employee.Name, percentage));
            }

            var plotModel = new PlotModel { Title = "Employee Time Worked" };
            plotModel.Series.Add(pieSeries);

            using var plotView = new PlotView { Model = plotModel };
            using var bitmap = new System.Drawing.Bitmap(600, 400);
            using var graphics = System.Drawing.Graphics.FromImage(bitmap);
            plotView.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, 600, 400));
            bitmap.Save(filePath, ImageFormat.Png);
        }
    }

    public class Employee
    {
        public string Name { get; set; }
        public int TotalTimeWorked { get; set; }
    }
}
