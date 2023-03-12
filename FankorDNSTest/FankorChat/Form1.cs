﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace FankorChat
{
    public partial class Form1 : Form
    {
        Socket socket;

        private void SendToServer(string command)
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            socket.Send(data);
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(textBoxHost.Text, Int32.Parse(textBoxPort.Text));
                SendToServer("name " + textBoxName.Text);
                timer1.Enabled= true;
                textBoxLog.AppendText("Подключено к " + textBoxHost.Text + ":" + textBoxPort.Text + "\n");
                buttonConnect.Enabled= false;
                buttonDisconnect.Enabled= true;
            }
            catch (Exception exc)
            {
                textBoxLog.AppendText(exc.Message + "\n");
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                SendToServer("quit");
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                timer1.Enabled = false;
                buttonConnect.Enabled = true;
                buttonDisconnect.Enabled = false;
                textBoxLog.AppendText("Отключено \n");
            }
            catch (Exception exc)
            {
                textBoxLog.AppendText(exc.Message + "\n");
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                SendToServer("message " + textBoxMessage.Text);
                textBoxLog.AppendText(textBoxName.Text + ":" + textBoxMessage.Text + "\n");
            }
            catch (Exception exc)
            {
                textBoxLog.AppendText(exc.Message + "\n");
            }
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (socket.Available > 0)
                {
                    byte[] data = new byte[socket.Available];
                    int data_size = socket.Receive(data);
                    string text_data = Encoding.UTF8.GetString(data, 0, data_size);



                    if (text_data.StartsWith("new "))
                    {
                        textBoxLog.AppendText(text_data.Substring(4) + " вошел в чат \n");

                    }

                    if (text_data.StartsWith("exit"))
                    {
                        textBoxLog.AppendText(text_data.Substring(5) + " покинул чат \n");
                    }

                    if (text_data.StartsWith("message"))
                    {
                        textBoxLog.AppendText(text_data.Substring(8) + "\n");
                    }
                }
            }

            catch (Exception exc)
            {
                textBoxLog.AppendText(exc.Message + "\n");
            }
        }

    }
}
