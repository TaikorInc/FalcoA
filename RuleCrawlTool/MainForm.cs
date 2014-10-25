using FalcoA.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace RuleCrawlTool
{
    public partial class MainForm : Form
    {
        List<Object> results = new List<object>();

        public MainForm()
        {
            InitializeComponent();
            btnOutputResult.Enabled = false;
        }

        private void btnStartJob_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtRule.Text))
                {
                    MessageBox.Show("The rule can't be empty!");
                    return;
                }

                btnOutputResult.Enabled = false;
                results.Clear();
                ITemplateFactory factory = new GeneralTemplateFactory();
                IDataProvider pp = BasicDataProvider.CreateFromXml(
                                                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                                                <ParameterProvider></ParameterProvider>"
                                            );

                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.LoadXml(txtRule.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The rule is wrong! please check it again." + Environment.NewLine + ex.ToString());
                    return;
                }

                ITemplate template = factory.GetCrawlTemplate(doc);
                var result = template.Run(pp);

                foreach (string item in result.JsonResult)
                    results.Add(JsonConvert.DeserializeObject(item));

                dgvResult.DataSource = null;
                dgvResult.DataSource = results;
                dgvResult.Refresh();

                btnOutputResult.Enabled = true;

                MessageBox.Show("Crawl successful!");
            }
            catch (Exception ex2)
            {
                MessageBox.Show("This is something wrong about this program." + Environment.NewLine + ex2.ToString());
                return;
            }
        }

        private void btnOutputResult_Click(object sender, EventArgs e)
        {
            if (results != null && results.Count > 0)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "JSON|*.json";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (fileDialog.FileName.ToLower().EndsWith("json"))
                    {
                        using (StreamWriter write = new StreamWriter(fileDialog.FileName))
                        {
                            write.Write(JsonConvert.SerializeObject(results));
                        }
                    }
                    MessageBox.Show("Output the result successful!");
                }
            }
            else
            {
                MessageBox.Show("The result is null!");
            }
        }
    }
}
