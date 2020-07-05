using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace GrabBot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        public static string GetHtml(string sUrl)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sUrl);

            //Create response-object
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            //Take response stream
            StreamReader sr = new StreamReader(response.GetResponseStream());

            //Read response stream (html code)
            string html = sr.ReadToEnd();

            //Close streamreader and response
            sr.Close();
            response.Close();

            //return source
            return html;
        }
    }
}
