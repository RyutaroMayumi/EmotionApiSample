using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Newtonsoft.Json;

namespace EmotionApiSample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    /// 

    public class Result
    {
        public Rectangle faceRectangle { get; set; }
        public Emotions scores { get; set; }
    }

    public class Rectangle
    {
        public int height { get; set; }
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
    }

    public class Emotions
    {
        public double anger { get; set; }
        public double contempt { get; set; }
        public double disgust { get; set; }
        public double fear { get; set; }
        public double happiness { get; set; }
        public double neutral { get; set; }
        public double sadness { get; set; }
        public double surprise { get; set; }
    }

    public partial class MainWindow : Window
    {
        string proxyUrl = "http://agoodproxy.com/";
        string proxyAccount = "";
        string proxyPassword = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // HTTPクライアント自作の場合
        private async Task<HttpResponseMessage> uploadImage(string file)
        {
            // HttpClientHandlerにProxy情報を設定する
            var httpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy(proxyUrl, false),
                Credentials = new NetworkCredential(proxyAccount, proxyPassword),
                UseProxy = true
            };

            using (var client = new HttpClient())
            //using (var client = new HttpClient(httpClientHandler))
            {
                // HTTPクライアントの作成
                client.BaseAddress = new Uri("https://api.projectoxford.ai/emotion/v1.0/recognize");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "your_key");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                textBox1.AppendText(client.DefaultRequestHeaders.ToString());

                // 送信するデータ（画像）のセット
                HttpContent content = new StreamContent(File.OpenRead(file));
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
                //textBox1.AppendText(client.ToString());

                // リクエストを送信（受信まで待機）
                return await client.PostAsync("https://api.projectoxford.ai/emotion/v1.0/recognize", content);
            }
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            HttpResponseMessage resp = await uploadImage(textBox.Text);
            var json = await resp.Content.ReadAsStringAsync();
            textBox1.AppendText(json + "\n");

            var results = JsonConvert.DeserializeObject<Result[]>(json);
            foreach (Result obj in results)
            {
                textBox1.AppendText("anger: " + obj.scores.anger + "\n");
                textBox1.AppendText("contempt: " + obj.scores.contempt + "\n");
                textBox1.AppendText("disgust: " + obj.scores.disgust + "\n");
                textBox1.AppendText("fear: " + obj.scores.fear + "\n");
                textBox1.AppendText("happiness: " + obj.scores.happiness + "\n");
                textBox1.AppendText("neutral: " + obj.scores.neutral + "\n");
                textBox1.AppendText("sadness: " + obj.scores.sadness + "\n");
                textBox1.AppendText("surprise: " + obj.scores.surprise + "\n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string str = ofd.FileName;
            textBox.Text = str.ToString();
        }
    }
}
