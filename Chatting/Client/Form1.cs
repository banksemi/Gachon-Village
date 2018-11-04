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
            server = new NetworkLibrary.Client("127.0.0.1",1000);
            server.Connect += Server_Connect;
            server.Receive += Server_Receive;
            server.Exit +=  Server_Exit;
            server.Start();
            InitializeComponent();
        }

        private void Server_Connect(ESocket socket)
        {
            textBox2.AppendText("연결됨\r\n");
        }

        private void Server_Receive(ESocket socket, JObject Message)
        {
            if ((int)Message["type"]==0)
            {
                textBox2.AppendText(Message["message"]+"\r\n");
            }
        }

        private void Server_Exit(ESocket socket)
        {
            textBox2.AppendText("종료됨\r\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            JObject json = new JObject();
            json["type"] = 0;
            json["message"] = textBox3.Text;
            textBox3.Text = "";

            server.Send(json);
        }
    }
}
