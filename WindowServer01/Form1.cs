using System;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace WindowServer01
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private TcpListener tcpListener = null;
        //private BinaryReader br = null;
        //private BinaryWriter bw = null;
        //private NetworkStream ns;

        //int intValue;
        //float floatValue;
        //string strValue;

        private void Form1_Load(object sender, EventArgs e)
        {
            tcpListener = new TcpListener(3000); //리스너 생성
            tcpListener.Start();    //리스너 시작
            //현재의 ip 표시
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            for(int i = 0; i < host.AddressList.Length; i++)
            {
                if(host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ipBox.Text = host.AddressList[i].ToString();
                    break;
                }
            }
        }

        private void AcceptClient()
        {
            while (true)    //클라이언트 접속 수만큼 무한 생성
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();    //스레드 수만큼 AcceptTcpClient 요청 생성

                if (tcpClient.Connected)
                {
                    string str = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                    int pid = Thread.CurrentThread.GetHashCode();
                    string strTemp = " ip: "+str + "  pid:" +pid.ToString() ;
                    listBox1.Items.Add(strTemp);
                }

                EchoServer echoServer = new EchoServer(tcpClient);
                Thread th = new Thread(new ThreadStart(echoServer.Process));
                th.IsBackground = true;
                th.Start();
            }
        }

        /// <summary>
        /// 접속 시작 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartBtn_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(new ThreadStart(AcceptClient));  //스레드 th 생성 및  AcceptClient 요청
            th.IsBackground = true;
            th.Start();

            //tcpClient = tcpListener.AcceptTcpClient();
            //if (tcpClient.Connected)
            //{
            //    countBox.Text = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
            //}
            //ns = tcpClient.GetStream();
            //bw = new BinaryWriter(ns);
            //br = new BinaryReader(ns);
        }

        //private void srBtn_Click(object sender, EventArgs e)
        //{
        //    while (true)
        //    {
        //        if (tcpClient.Connected)
        //        {
        //            if (DataReceive() == -1)
        //                break;
        //            DataSend();
        //        }
        //        else
        //        {
        //            AllClose();
        //            break;
        //        }
        //    }
        //    AllClose();
        //}

        //private int DataReceive()
        //{
        //    intValue = br.ReadInt32();
        //    if (intValue == -1)
        //        return -1;

        //    floatValue = br.ReadSingle();
        //    strValue = br.ReadString();
        //    string str = intValue + "/" + floatValue + "/" + strValue;
        //    MessageBox.Show(str);
        //    return 0;
        //}

        

        //private void DataSend()
        //{
        //    bw.Write(intValue);
        //    bw.Write(floatValue);
        //    bw.Write(strValue);

        //    MessageBox.Show("보냈습니다.");
        //}

        //private void AllClose()
        //{
        //    if (bw != null)
        //    {  bw.Close(); bw = null;   }
        //    if (br != null)
        //    { br.Close(); br = null; }
        //    if (ns != null)
        //    { ns.Close(); ns = null; }
        //    if (tcpClient != null)
        //    { tcpClient.Close(); tcpClient = null; }
        //}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //AllClose();
            //tcpListener.Stop();
            if(tcpListener != null)
            {
                tcpListener.Stop();
                tcpListener = null;
            }
        }
    }

    class EchoServer
    {
        TcpClient RefClient;
        private BinaryReader br = null;
        private BinaryWriter bw = null;
        int intValue;
        float floatValue;
        string strValue;

        public EchoServer(TcpClient Client)
        {
            RefClient = Client;
        }

        public void Process()
        {
            NetworkStream ns = RefClient.GetStream();
            try
            {
                br = new BinaryReader(ns);
                bw = new BinaryWriter(ns);

                while (true)
                {
                    intValue = br.ReadInt32();
                    floatValue = br.ReadSingle();
                    strValue = br.ReadString();

                    MessageBox.Show("가져온값:" + intValue.ToString() + "/" + floatValue.ToString() + "/" + strValue);

                    bw.Write(intValue);
                    bw.Write(floatValue);
                    bw.Write(strValue);
                }

            }catch(SocketException se)
            {
                br.Close();
                bw.Close();
                ns.Close();
                ns = null;
                RefClient.Close();
                MessageBox.Show(se.Message);
                Thread.CurrentThread.Abort();
            }catch(IOException ex)
            {
                br.Close();
                bw.Close();
                ns.Close();
                ns = null;
                RefClient.Close();
                MessageBox.Show(ex.Message);
                Thread.CurrentThread.Abort();
            }
        }
    }
}
