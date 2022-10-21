using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using WebTask.Entities;
using WebTask.MnbServiceReference;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WebTask
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        BindingList<string> Currency = new BindingList<string>();

        public Form1()
        {
            InitializeComponent();

            
            Fuggveny2();
            RefreshData();

            comboBox1.DataSource = Currency;
            dataGridView1.DataSource = Rates;
            
        
        var mnbService = new MNBArfolyamServiceSoapClient();
        var request = new GetCurrenciesRequestBody();

        var response = mnbService.GetCurrencies(request);
        var result = response.GetCurrenciesResult;

        var xml = new XmlDocument();

        xml.LoadXml(result);

            foreach (XmlElement item in xml.DocumentElement.ChildNodes[0])
            {
                string c;

        var childElement = (XmlElement)item;
                if (childElement == null)
                    continue;

                c = childElement.InnerText;
                Currency.Add(c);
            }

    RefreshData();
}

    string GetExchangeRates()
    {
        var mnbService = new MNBArfolyamServiceSoapClient();
        var request = new GetExchangeRatesRequestBody()
        {
            currencyNames = comboBox1.SelectedItem.ToString(),
            startDate = dateTimePicker1.Value.ToString(),
            endDate = dateTimePicker2.Value.ToString()
        };

        var response = mnbService.GetExchangeRates(request);
        var result = response.GetExchangeRatesResult;

        return result;
    }

        void Xml(string result)
        {
            var xml = new XmlDocument();

            xml.LoadXml(result);

            foreach (XmlElement item in xml.DocumentElement)
            {
                RateData r = new RateData();

                Rates.Add(r);

                var childElement = (XmlElement)item.ChildNodes[0];
                if (childElement == null)
                    continue;

                r.Date = DateTime.Parse(item.GetAttribute("date"));
                r.Currency = childElement.GetAttribute("curr");
                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                {
                    r.Value = value / unit;
                }
            }
        }
        private void Fuggveny2()
                {

                    chartRateData.DataSource = Rates;

                    var series = chartRateData.Series[0];
                    series.ChartType = SeriesChartType.Line;
                    series.XValueMember = "Date";
                    series.YValueMembers = "Value";
                    series.BorderWidth = 2;

                    var legend = chartRateData.Legends[0];
                    legend.Enabled = false;

                    var chartArea = chartRateData.ChartAreas[0];
                    chartArea.AxisX.MajorGrid.Enabled = false;
                    chartArea.AxisY.MajorGrid.Enabled = false;
                    chartArea.AxisY.IsStartedFromZero = false;

                }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            Rates.Clear();

            var result = GetExchangeRates();
            Xml(result);
            Fuggveny2();
        }
    }
}
