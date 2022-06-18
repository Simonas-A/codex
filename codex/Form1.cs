using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

/* most of the code is written for demonstration purposes only
 * there are a lot of things to keep in mind when doing real project
 * 
 * Real Smart Id api seems to be available for businesses only
 * therefore this sample project uses demo version of smart id
 * 
 */


namespace codex
{
    public partial class Form1 : Form 
    {
        public Form1()
        {
            InitializeComponent();
        }

        string responseString;

        string result;

        private void Form1_Load(object sender, EventArgs e)
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            this.FormBorderStyle = FormBorderStyle.None;
            this.Left = 0;
            this.Top = 0;
            this.Width = resolution.Width;
            this.Height = resolution.Height;

            textBox2.Left = resolution.Width / 2 - textBox2.Width / 2;
            textBox2.Top = resolution.Height / 4 - textBox2.Height / 2;

            label1.Parent = pictureBox1;
            label1.BackColor = Color.FromArgb(100, 0, 0, 0);
            SetText("Personal code:");
            

            button1.Left = resolution.Width / 2 - button1.Width / 2;
            button1.Top = resolution.Height / 2 + button1.Height / 2 + textBox2.Height;

            pictureBox1.Top = 0;
            pictureBox1.Height = resolution.Height;
            pictureBox1.Width = resolution.Width;

            pictureBox1.Left = (resolution.Width - pictureBox1.Width) / 2;

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;


            pictureBox1.Image = codex.Properties.Resources.safeidle;
        }

        private void SetText(string v)
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            label1.Text = v;
            label1.Left = resolution.Width / 2 - label1.Width / 2;
            label1.Top = resolution.Height / 4 - label1.Height / 2 - textBox2.Height;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var task = SendRequestAsync();
            await task;

            textBox2.Visible = false;
            button1.Visible = false;

            var data = (JObject)JsonConvert.DeserializeObject(responseString);

            if (data.SelectToken("sessionId") == null)
            {
                label1.Text = "User not found";
                return;
            }

            var address = data.SelectToken("sessionId").Value<string>();
            var code = data.SelectToken("verificationCode").Value<string>();

            SetText("Verification code: " + code + "\nCheck your phone");



            var resultTask = WaitResponseAsync(address);
            await resultTask;

            var data1 = (JObject)JsonConvert.DeserializeObject(result);
            var address1 = data1.SelectToken("result.endResult").Value<string>();

            if (address1 == "OK")
            {
                Correct();
            }
            else
            {
                Incorrect();
            }
        }

        private void Incorrect()
        {
            textBox2.Visible = false;
            button1.Visible = false;
            SetText("Authentication failed!");
            label1.ForeColor = Color.Red;
            pictureBox1.Image = codex.Properties.Resources.safeAD;
        }

        private void Correct()
        {
            textBox2.Visible = false;
            button1.Visible = false;
            label1.ForeColor = Color.Lime;
            SetText("Authentication successful!");
            pictureBox1.Image = codex.Properties.Resources.safe;
        }

        private async Task WaitResponseAsync(string sessionId)
        {
            HttpClient client = new HttpClient();

            string json = "{\"channel\":\"demo\",\"sessionId\":\"" + sessionId + "\"}";
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            client.DefaultRequestHeaders.TryAddWithoutValidation("authority", "smid.demo.sk.ee");
            client.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9,lt;q=0.8,ru;q=0.7,pl;q=0.6,zh-TW;q=0.5,zh;q=0.4");
            client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("cookie", "io=KwBOqh7MODth3i0fAADs");
            client.DefaultRequestHeaders.TryAddWithoutValidation("dnt", "1");
            client.DefaultRequestHeaders.TryAddWithoutValidation("origin", "https://smid.demo.sk.ee");
            client.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://smid.demo.sk.ee/");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"102\", \"Google Chrome\";v=\"102\"");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-dest", "empty");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-mode", "cors");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-site", "same-origin");
            client.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36");

            var response = await client.PostAsync("https://smid.demo.sk.ee/debug/api/getTransactionResult", httpContent);

            result = await response.Content.ReadAsStringAsync();
        }

        private async Task SendRequestAsync()
        {
            HttpClient client = new HttpClient();


            string json = "{\"channel\":\"demo\",\"vcChoice\":false,\"method\":\"authenticateUser\",\"country\":\"EE\",\"identityType\":\"PNO\",\"idCode\":\"" + textBox2.Text + "\",\"displayText\":\"Safe deposit box\"}";
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

            client.DefaultRequestHeaders.TryAddWithoutValidation("authority", "smid.demo.sk.ee");
            client.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9,lt;q=0.8,ru;q=0.7,pl;q=0.6,zh-TW;q=0.5,zh;q=0.4");
            client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("cookie", "io=KwBOqh7MODth3i0fAADs");
            client.DefaultRequestHeaders.TryAddWithoutValidation("dnt", "1");
            client.DefaultRequestHeaders.TryAddWithoutValidation("origin", "https://smid.demo.sk.ee");
            client.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://smid.demo.sk.ee/");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"102\", \"Google Chrome\";v=\"102\"");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-dest", "empty");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-mode", "cors");
            client.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-site", "same-origin");
            client.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36");

            var response = await client.PostAsync("https://smid.demo.sk.ee/debug/api/createTransaction", httpContent);

            responseString = await response.Content.ReadAsStringAsync();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
