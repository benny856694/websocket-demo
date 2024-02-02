using MiscUtil.Conversion;
using MiscUtil.IO;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WebSocketSharp;

namespace WebsocketDemo
{
    public partial class Form1 : Form
    {
        WebSocket _ws = null;
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
            this.BeginInvoke(new Action(() => buttonConnect.Text = _ws?.ReadyState == WebSocketState.Open ? "断开" : "连接"));

        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            if (_ws?.ReadyState == WebSocketState.Open)
            {
                _ws.OnMessage -= _ws_OnMessage;
                _ws.CloseAsync();
                UpdateButtonText();
            }
            else
            {
                var url = $"ws://{textBoxIp.Text}:8000";
                _ws = new WebSocket(url);
                _ws.OnError += _ws_OnError;
                _ws.OnMessage += _ws_OnMessage;
                _ws.OnOpen += _ws_OnOpen;
                _ws.OnClose += _ws_OnClose;
                _ws.ConnectAsync();
            }

        }

        private void _ws_OnClose(object sender, CloseEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                this.Log("连接断开");
                UpdateButtonText();
            }));
        }

        private void _ws_OnOpen(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                this.Log("连接成功");
                UpdateButtonText();

            }));
        }

        private void _ws_OnMessage(object sender, MessageEventArgs e)
        {
            this.HandleMsg(e);
        }

        private void _ws_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            this.Log($"异常：{e.Message}");
        }

        private void HandleMsg(MessageEventArgs msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<MessageEventArgs>(this.HandleMsg), msg);
                return;
            }

            if (msg.IsBinary)//二进制数据流
            {
                HandleBinaryStream(msg.RawData);
            }
            else
            {
                var faceEvt = JsonConvert.DeserializeObject<FaceCaptureEvent>(msg.Data);
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

        private void HandleBinaryStream(byte[] rawData)
        {
            var reader = new EndianBinaryReader(EndianBitConverter.Little, new MemoryStream(rawData));
            var imageType = reader.ReadInt32() & 0x3ff; //IR image or normal image
            reader.Seek(8, SeekOrigin.Begin);//skip 4 bytes
            var dataType = reader.ReadInt32(); //if it's image stream
            if (dataType == 1) //jpg
            {
                var dataLen = rawData.Length - 20;
                var buf = new byte[dataLen];
                reader.Seek(20, SeekOrigin.Begin);
                reader.Read(buf, 0, buf.Length);

                //if(Helper.GetImageFormat(buf) == ImageFormat.jpeg)
                {
                    var img = Image.FromStream(new MemoryStream(buf));
                    if (imageType == 103) //normal image
                    {
                        pictureBoxColorImage.Image = img;
                    }
                    else//IR
                    {
                        pictureBoxIRImg.Image = img;
                    }
                }

            }

        }
    }
}
