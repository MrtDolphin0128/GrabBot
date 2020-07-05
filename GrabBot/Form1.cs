using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using HtmlAgilityPack;

namespace GrabBot
{
    public partial class Form1 : Form
    {
        bool browserState = false;
        string fileName = "";
        string outsideImage = "";
        string stringSubHandle = "";
        string stringSubTitle = "";
        float curPercent = 0;
        //Thread interfThread;

        public List<string> ImageList = new List<string>(); // Creating a list array
        string DescriptionList = "";
        public int nId = 0;
        public List<string> RealList = new List<string>();
        public List<string> ColorList = new List<string>();
        public List<string> SizeList= new List<string>();
        public List<string> TypeList = new List<string>();
        public List<string> UrlList = new List<string>();


        public Form1()
        {
            InitializeComponent();
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = 100;

            // interfThread = new Thread(ShowProgress);
        }

        public void ShowProgress(object data)
        {
            do
            {
                this.Refresh();
                Thread.Sleep(100);
            } while (true);

        }

        public void TextParse()
        {
            string[] colors = textBox2.Text.Split(',');
            string[] types = textBox3.Text.Split(',');
            string[] sizes = textBox4.Text.Split(',');
            int rowCounts = sizes.Count() * colors.Count();
            for (int i = 0; i < rowCounts; i++)
            {
                for (int j = 0; j < types.Count(); j++)
                {
                    TypeList.Add(types[j]);
                }
            } 
            for (int i = 0; i< sizes.Count(); i++)
            {
                for (int x = 0; x < colors.Count(); x++)
                {
                    for (int j = 0; j < types.Count(); j++)
                    {
                        ColorList.Add(colors[x]);
                    }
                }
            }
            

            for (int i = 0; i < sizes.Count(); i++)
            {
                for (int j = 0; j < types.Count() * colors.Count(); j++)
                {
                    SizeList.Add(sizes[i]);
                }
            }
        }


        public void GetAllImages(string sNewUrl)
        {
            RealList.Clear();
            ImageList.Clear();
            // Declaring 'x' as a new WebClient() method
            WebClient x = new WebClient();
            // Setting the URL, then downloading the data from the URL.
            x.Headers.Add("upgrade-insecure-requests", "1");
            x.Headers.Set("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");

            HtmlAgilityPack.HtmlDocument document1 = new HtmlAgilityPack.HtmlDocument();
            List<HtmlNode> itemnode_list = new List<HtmlNode>();

            try
            {
                string source = x.DownloadString(@sNewUrl);
                // Loading document's source via HtmlAgilityPack
                document1.LoadHtml(source);
                var findclasses = document1.DocumentNode
                    .Descendants("h2")
                    .Where(d =>
                       d.Attributes.Contains("class")
                       &&
                       d.Attributes["class"].Value.Contains("headline secondary")
                    );

                foreach (var input in findclasses)
                {
                    stringSubTitle = input.InnerText;
                    //stringSubHandle = input.InnerText.Replace("   ", "-").Replace(" - ", "-");
                    //stringSubHandle = stringSubHandle.Replace(" ", "-");
                    //stringSubHandle = stringSubHandle.Replace(",", "-");
                }

                HtmlNode itemnode0 = document1.DocumentNode.SelectSingleNode("//*[@id=\"detail-inline-product-views-carousel-view-composition\"]");
                HtmlNode itemnode1 = document1.DocumentNode.SelectSingleNode("//*[@id=\"detail-inline-product-views-carousel-view-id-1\"]");
                HtmlNode itemnode2 = document1.DocumentNode.SelectSingleNode("//*[@id=\"detail-inline-product-views-carousel-view-id-2\"]");

                itemnode_list.Add(itemnode0);
                itemnode_list.Add(itemnode1);
                itemnode_list.Add(itemnode2);


                foreach (HtmlNode itemnode in itemnode_list)
                {
                    foreach (HtmlAgilityPack.HtmlAttribute link in itemnode
                                         .Descendants("img")
                                         .Select(a => a.Attributes["src"]))
                    {
                        // Storing all links found in an array. You can declare this however you want.
                        if (link != null)
                        {
                            if (link.Value.ToString().IndexOf("https://") < 0)
                            {
                                //if (link.Value.ToString().IndexOf(",") > 0)
                                //    RealList.Add("\"" + "https:" + link.Value.ToString() + "\"");
                                //else
                                RealList.Add("https:" + link.Value.ToString());
                            }
                            else continue;
                        }

                    }
                }
                for (int i = 0; i < RealList.Count(); i++)
                {
                    RealList[i] = RealList[i].Replace("100", "600");
                    RealList[i] = RealList[i].Replace("spreadshirtmedia", "spreadshirt");
                    int nbegin = RealList[i].IndexOf("1,");
                    if(nbegin < 0)
                    {
                        continue;
                    }
                    int nEnd = RealList[i].IndexOf("/", nbegin);
                    string substring = string.Empty;
                    if (nEnd < 0)
                    {
                        substring = RealList[i].Substring(nbegin + 1, RealList[i].Length - nbegin - 1);
                        RealList[i] = RealList[i].Replace(substring, "");
                        RealList[i] = RealList[i] + ".jpg?width=600";
                    }
                    else
                    {
                        substring = RealList[i].Substring(nbegin + 1, nEnd - nbegin - 1);
                        RealList[i] = RealList[i].Replace(substring, "");
                        RealList[i] = RealList[i] + "?width=600";
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }

            DescriptionList = document1.DocumentNode.SelectSingleNode("//body").SelectNodes("//div").Single(n => n.Attributes.Any(a => a.Name == "class" && a.Value == "description textMuted")).InnerText;
            if (DescriptionList.IndexOf("\"") > 0)
            {
                DescriptionList = DescriptionList.Replace("\"", "'");
            }
//            if (ImageList.Count == 11)
//            {
//                RealList.Add(ImageList[0].Replace("100", "600"));
//                RealList.Add(ImageList[2].Replace("100","600"));
//                RealList.Add(ImageList[3].Replace("100","600"));
////                RealList.Insert(2, outsideImage);
//            }
//            if (ImageList.Count > 11)
//            {
//                RealList.Add(ImageList[9].Replace("100", "600"));
//                RealList.Add(ImageList[8].Replace("100", "600"));
//                RealList.Add(ImageList[10].Replace("100", "600"));
//                //                RealList.Insert(2, outsideImage);
//            }
            WriteCSV();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //var code = Program.GetHtml(textBox1.Text);
            //string content = code.ToString();
            var appName = Process.GetCurrentProcess().ProcessName + ".exe";
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "csv files (*.csv)|*.csv";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = Path.GetFullPath(saveFileDialog1.FileName);
            }
            else
            {
                MessageBox.Show("Please select your target filepath!");
                return;
            }
            TextParse();
            if (int.Parse(txt_pagenumber.Text) > 10)
            {
                MessageBox.Show("Max 10 Pages are available");
                Application.Exit();
            }
            for (int i = 1; i <= int.Parse(txt_pagenumber.Text); i++)
            {
                UrlList.Add(textBox1.Text + "?page=" + i.ToString());
            }
            
            backgroundWorker1.RunWorkerAsync();
            button1.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            txt_pagenumber.Enabled = false;
            txt_regular.Enabled = false;
            txt_sale.Enabled = false;
            //Parsing();// + i.ToString());
            
        }

        void Parsing(BackgroundWorker bgWorker)
        {
            //bgWorker.ReportProgress(0);
            HtmlAgilityPack.HtmlDocument document1 = new HtmlAgilityPack.HtmlDocument();
            
            for(int i = 0; i < UrlList.Count; i++)
            {
                int nCurIndex = 0;
                string source = string.Empty;
                WebClient x = new WebClient();
                // Setting the URL, then downloading the data from the URL.
                x.Headers.Add("upgrade-insecure-requests", "1");
                x.Headers.Set("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
                try
                {
                    source = x.DownloadString(UrlList[i]);
                    // Loading document's source via HtmlAgilityPack
                    document1.LoadHtml(source);
                    var findclasses = document1.DocumentNode
                    .Descendants("div")
                    .Where(d =>
                       d.Attributes.Contains("class")
                       &&
                       d.Attributes["class"].Value.Contains("ile article zoomed")
                    );
                    foreach (HtmlAgilityPack.HtmlNode node in findclasses)
                    {
                        nCurIndex++;
                        bgWorker.ReportProgress(nCurIndex * 100 / findclasses.Count());
                        string strTemp = string.Empty;
                        string sBasicUrl = string.Empty;
                        HtmlAgilityPack.HtmlNode h1node = node.Descendants("a").ElementAt(0);
                        HtmlAgilityPack.HtmlNode h2node = node.Descendants("img").ElementAt(0);
                        sBasicUrl = "https://www.spreadshirt.com" + h1node.Attributes["href"].Value;
                        //string sBasicUrl = h1node.GetAttributeValue("href","");
                        outsideImage = ("https:" + h2node.Attributes["src"].Value).Replace("300", "600");
                        outsideImage = outsideImage.Replace("spreadshirtmedia", "spreadshirt");
                        int nbegin = outsideImage.IndexOf("1,");
                        if (nbegin < 0)
                        {
                            GetAllImages(sBasicUrl);
                            continue;
                        }
                        int nEnd = outsideImage.IndexOf("/", nbegin);
                        strTemp = outsideImage.Substring(nbegin + 1, nEnd - nbegin - 1);
                        outsideImage = outsideImage.Replace(strTemp, "");
                        outsideImage = outsideImage + "?width=600";
                        GetAllImages(sBasicUrl);
                        }
                }
                catch
                {

                }

            }
            MessageBox.Show("Successfully saved to " + fileName);
        }
        

        void WriteCSV()
        {
            // Write sample data to CSV file
            nId++;
            int secondID = 0;
           
            string TotalUrl = string.Empty;
            if (!File.Exists(fileName))
            {   
                //"ID,Type,SKU,Name,Published," + "\"" + "Is featured?" + "\"" + "," + "\"" + "Visibility in catalog" + "\"" + "," + "\"" + "Short description" + "\"" + ",Description," + "\"" + "Date sale price starts" + "\"" + "," + "\"" + "Date sale price ends" + "\"" + "," + "\"" + "Tax status" + "\"" + "," + "\"" + "Tax class" + "\"" + "," + "\"" + "In stock?" + "\"" + ",Stock," + "\"" + "Backorders allowed?" + "\"" + "," + "\"" + "Sold individually?" + "\"" + "," + "\"" + "Weight (kg)" + "\"" + "," + "\"" + "Length (cm)" + "\"" + "," + "\"" + "Width (cm)" + "\"" + "," + "\"" + "Height (cm)" + "\"" + "," + "\"" + "Allow customer reviews?" + "\"" + "," + "\"" + "Purchase note" + "\"" + "," + "\"" + "Sale price" + "\"" + "," + "\"" + "Regular price" + "\"" + ",Categories,Tags," + "\"" + "Shipping class" + "\"" + ",Images," + "\"" + "Download limit" + "\"" + "," + "\"" + "Download expiry days" + "\"" + ",Parent," + "\"" + "Grouped products" + "\"" + ",Upsells,Cross - sells," + "\"" + "External URL" + "\"" + "," + "\"" + "Button text" + "\"" + "," + "\"" + "Attribute 1 name" + "\"" + "," + "\"" + "Attribute 1 value(s)" + "\"" + "," + "\"" + "Attribute 1 visible" + "\"" + "," + "\"" + "Attribute 1 global" + "\"" + "," + "\"" + "Attribute 2 name" + "\"" + "," + "\"" + "Attribute 2 value(s)" + "\"" + "," + "\"" + "Attribute 2 visible" + "\"" + "," + "\"" + "Attribute 2 global" + "\"" + "," + "\"" + "Attribute 3 name" + "\"" + "," + "\"" + "Attribute 3 value(s)" + "\"" + "," + "\"" + "Attribute 3 visible" + "\"" + "," + "\"" + "Attribute 3 global" + "\""
                string clientHeader = "ID,Type,SKU,Name,Published," + "\"" + "Is featured?" + "\"" + "," + "\"" + "Visibility in catalog" + "\"" + "," + "\"" + "Short description" + "\"" + ",Description," + "\"" + "Date sale price starts" + "\"" + "," + "\"" + "Date sale price ends" + "\"" + "," + "\"" + "Tax status" + "\"" + "," + "\"" + "Tax class" + "\"" + "," + "\"" + "In stock?" + "\"" + ",Stock," + "\"" + "Backorders allowed?" + "\"" + "," + "\"" + "Sold individually?" + "\"" + "," + "\"" + "Weight (kg)" + "\"" + "," + "\"" + "Length (cm)" + "\"" + "," + "\"" + "Width (cm)" + "\"" + "," + "\"" + "Height (cm)" + "\"" + "," + "\"" + "Allow customer reviews?" + "\"" + "," + "\"" + "Purchase note" + "\"" + "," + "\"" + "Sale price" + "\"" + "," + "\"" + "Regular price" + "\"" + ",Categories,Tags," + "\"" + "Shipping class" + "\"" + ",Images," + "\"" + "Download limit" + "\"" + "," + "\"" + "Download expiry days" + "\"" + ",Parent," + "\"" + "Grouped products" + "\"" + ",Upsells,Cross - sells," + "\"" + "External URL" + "\"" + "," + "\"" + "Button text" + "\"" + "," + "\"" + "Attribute 1 name" + "\"" + "," + "\"" + "Attribute 1 value(s)" + "\"" + "," + "\"" + "Attribute 1 visible" + "\"" + "," + "\"" + "Attribute 1 global" + "\"" + "," + "\"" + "Attribute 2 name" + "\"" + "," + "\"" + "Attribute 2 value(s)" + "\"" + "," + "\"" + "Attribute 2 visible" + "\"" + "," + "\"" + "Attribute 2 global" + "\"" + "," + "\"" + "Attribute 3 name" + "\"" + "," + "\"" + "Attribute 3 value(s)" + "\"" + "," + "\"" + "Attribute 3 visible" + "\"" + "," + "\"" + "Attribute 3 global" + "\"";
                File.WriteAllText(fileName, clientHeader + Environment.NewLine);
            }

            //1014,variable,,"Sample Product",1,0,visible,,"Sample description&nbsp;",,,taxable,,1,,0,0,,,,,1,,,,"sample category",,,https://45.76.11.39/wp-content/uploads/2017/09/71z47UXoscL._UL1500-4.jpg,,,,,,,,,Color,"Black, White",1,1,"Fit Type","men, women",1,1,Size,"L, M, S",1,1
            string SecondHeader = string.Empty;
            if (RealList.Count > 1)
                   SecondHeader = nId.ToString() + "," + "variable" + ",," + "\"" + stringSubTitle + "\"" + ",1,0,visible,," + "\"" + DescriptionList + "\"" + ",,,taxable,,1,,0,0,,,,,1,," + txt_sale.Text + "," + txt_regular.Text + "," + "\"" + "sample category" + "\"" + ",,,"  + "\"" + RealList[1] +"\"" + ",,,,,,,,,Color," + "\"" + textBox2.Text + "\"" + ",1,1," + "\"" + "Fit Type" + "\"" + "," + "\"" + textBox3.Text + "\"" + ",1,1,Size," + "\"" + textBox4.Text + "\"" + ",1,1";
            else
                   SecondHeader = nId.ToString() + "," + "variable" + ",," + "\"" + stringSubTitle + "\"" +",1,0,visible,," + "\"" + DescriptionList + "\"" + ",,,taxable,,1,,0,0,,,,,1,," + txt_sale.Text + "," + txt_regular.Text + "," + "\"" + "sample category" + "\"" + ",,," + "\"" + outsideImage + "\"" + ",,,,,,,,,Color," + "\"" + textBox2.Text + "\"" + ",1,1," + "\"" + "Fit Type" + "\"" + "," + "\"" + textBox3.Text + "\"" + ",1,1,Size," + "\"" + textBox4.Text + "\"" + ",1,1";
            File.AppendAllText(fileName, SecondHeader + Environment.NewLine);
            secondID = nId;
            for (int j = 1; j < ColorList.Count; j++)
            {
                nId++;
                if (j > RealList.Count)
                {
                    string clientDetails = nId.ToString() + ",variation,," + "\"" + stringSubTitle + "\"" +",1,0,visible,,,,,taxable,,1,,0,0,,,,,0,," + txt_sale.Text + "," + txt_regular.Text + ",,,,,,,id:" + secondID.ToString() + ",,,,,,Color," + "\"" + ColorList[j] + "\"" + ",,1," + "\"" + "Fit Type" + "\"" + "," + "\"" + TypeList[j] + "\"" + ",,1,Size," + "\"" + SizeList[j] + "\"" + ",,1";
                        //stringSubHandle + ",,,,,,,," + "\"" + SizeList[j] + "\"" + ",," + "\"" + ColorList[j] + "\"" + ",," + "\"" + TypeList[j] + "\"" + ",,0,shopify,999,continue,manual,21.95,,TRUE,TRUE,,,,,,,,,,,,,,,,,,,,,,kg,";
                    File.AppendAllText(fileName, clientDetails + Environment.NewLine);
                }
                else
                {
                    int num = j;
                    if (num == 2)
                        num = 0;
                    string clientDetails = nId.ToString() + ",variation,," + "\"" + stringSubTitle + "\"" + ",1,0,visible,,,,,taxable,,1,,0,0,,,,,0,," + txt_sale.Text + "," + txt_regular.Text + ",,,,,,,id:" + secondID.ToString() + ",,,,,,Color," + "\"" + ColorList[j] + "\"" + ",,1," + "\"" + "Fit Type" + "\"" + "," + "\"" + TypeList[j] + "\"" + ",,1,Size," + "\"" + SizeList[j] + "\"" + ",,1";
                        //stringSubHandle + ",,,,,,,," + "\"" + SizeList[j] + "\"" + ",," + "\"" + ColorList[j] + "\"" + ",," + "\"" + TypeList[j] + "\"" + ",,0,shopify,999,continue,manual,21.95,,TRUE,TRUE,," + RealList[j-1].ToString() + "," + (num + 1).ToString() + ",,,,,,,,,,,,,,,,,,,kg,";
                    File.AppendAllText(fileName, clientDetails + Environment.NewLine);
                }
            }
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(0);
            Parsing(backgroundWorker1);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //backgroundWorker1.ReportProgress(100);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
    }
}
