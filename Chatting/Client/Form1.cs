using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworkLibrary;
using Newtonsoft.Json.Linq;
namespace Client
{
    public partial class Form1 : Form
    {
        NetworkLibrary.Client server;
        public Form1()
        {
            InitializeComponent();
            server = new NetworkLibrary.Client("easyrobot.co.kr", 1000);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive;
            server.Exit += Server_Exit;
            server.Start();
        }
        public void AddText(string text)
        {
            this.Invoke(new Action(delegate () {
                textBox2.AppendText(text + "\r\n");
            }));
          
        }
        private void Server_Connect(ESocket socket)
        {
            AddText("연결됨");
        }

        private void Server_Receive(ESocket socket, JObject Message)
        {
            if ((int)Message["type"]==0)
            {
                AddText((string)Message["message"]);
            }
        }

        private void Server_Exit(ESocket socket)
        {
            AddText("종료됨");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            JObject json = new JObject();
            json["type"] = 0;
            json["message"] = textBox3.Text;
            textBox3.Text = "";

            server.Send(json);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Dispose();
        }
    }
}
