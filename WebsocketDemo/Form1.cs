using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Websocket.Client;

namespace WebsocketDemo
{
    public partial class Form1 : Form
    {
        WebsocketClient _ws = null;
        public Form1()
        {
            InitializeComponent();
        }

        void Log(string msg)
        {
            this.BeginInvoke(new Action<string>(m => this.listBoxLog.Items.Insert(0, m)), $"{DateTime.Now.ToShortTimeString()}:{msg}");
        }



        private void UpdateButtonText()
        {
            buttonConnect.Text = _ws?.IsRunning == true ? "断开" : "连接";
        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            if (_ws?.IsRunning == true)
            {
                _ws?.Dispose();
                UpdateButtonText();
            }
            else
            {
                var url = new Uri($"ws://{textBoxIp.Text}:8000");
                _ws = new WebsocketClient(url);
                _ws.ReconnectionHappened.Subscribe(info => this.Log($"重新连接：{info.Type}"));
                _ws.MessageReceived.Subscribe(msg => this.HandleMsg(msg));
                buttonConnect.Enabled = false;
                try
                {
                    await _ws.StartOrFail();
                    UpdateButtonText();
                }
                catch (Exception ex)
                {
                    this.Log($"无法连接{ex.Message}");
                }
                finally
                {
                    buttonConnect.Enabled = true;
                }

            }

        }

        private void HandleMsg(ResponseMessage msg)
        {
            if (msg.MessageType == System.Net.WebSockets.WebSocketMessageType.Binary)//二进制数据流
            {
                if (Helper.GetImageFormat(msg.Binary) == ImageFormat.jpeg)//jpeg流
                {
                    var img = Image.FromStream(new MemoryStream(msg.Binary));
                    pictureBoxVideoStream.Image = img;
                }
            }
            else
            {
                var faceEvt = JsonConvert.DeserializeObject<FaceCaptureEvent>(msg.Text);
                this.Log($"{faceEvt.cmd}");
                if (faceEvt.cmd == "face") //人脸抓拍
                {
                    if (faceEvt.closeup_pic_flag)
                    {
                        var img = Image.FromStream(new MemoryStream(Convert.FromBase64String(faceEvt.closeup_pic.data)));
                        pictureBox1.Image = img;
                    }

                    if (faceEvt.match != null && faceEvt.match.image != null)
                    {
                        var img = Image.FromStream(new MemoryStream(Convert.FromBase64String(faceEvt.match.image)));
                        pictureBox2.Image = img;
                    }
                }
            }

        }
    }
}
