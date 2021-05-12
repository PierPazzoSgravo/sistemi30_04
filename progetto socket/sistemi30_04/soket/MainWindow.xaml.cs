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
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace soket
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreaSoket_Click(object sender, RoutedEventArgs e)
        {

            //reperisce in automatico l'indirizzo della macchina (ho preso spunto da stack overflow perchè non avevo la minima idea di dove mettere le mani, qui si spiega il 5 in informatica)
           string MioIPAdd = Dns.GetHostName();
            IPAddress[] iphostentry = Dns.GetHostAddresses(MioIPAdd);
            foreach (IPAddress ip in iphostentry)
            {
                MioIPAdd = ip.ToString();
            }
            //creo un random per i numeri di porta
            Random rd = new Random();
            int porta = rd.Next(49152, 65536);



            IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse(MioIPAdd),porta );

           

            btnInviaMsg.IsEnabled = true;

            Thread ricezione = new Thread(new ParameterizedThreadStart(soketRecive));

            ricezione.Start(sourceSocket);

            MessageBox.Show("Il tuo indirizzo è:" + MioIPAdd + "\n" + "Il numero di porta è:" + porta.ToString(),"Informazione");

        }

        private void btnInviaMsg_Click(object sender, RoutedEventArgs e)
        {
            
            string ipAddress = TxtIndirizzo.Text;
            int port =int.Parse( TxtPorta.Text);

            SocketSend(IPAddress.Parse(ipAddress), port, txtMsgSend.Text);
        }



        public void SocketSend(IPAddress dest,int destPort,string message)
        {
            Byte[] byteInviati = Encoding.ASCII.GetBytes(message);

            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint remote_endpoint = new IPEndPoint(dest, destPort);

            s.SendTo(byteInviati, remote_endpoint);
        }
        public async void soketRecive(object sockSource)
        {
            IPEndPoint ipendp = (IPEndPoint)sockSource;

            Socket t = new Socket(ipendp.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            t.Bind(ipendp);

            Byte[] bytesRicevuti = new Byte[256];

            string message;
            int contaCaratteri = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    if (t.Available > 0)
                    {
                        message = "";

                        contaCaratteri = t.Receive(bytesRicevuti, bytesRicevuti.Length, 0);
                        message += Encoding.ASCII.GetString(bytesRicevuti, 0, contaCaratteri);

                        //concateno il testo nella textbox 
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            txtbxStorico.AppendText("["+DateTime.Now.Hour+":"+DateTime.Now.Minute+"]"+" " +message+ "\r\n");
                        }));
                    }
                }
            });
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            txtMsgSend.Clear();
            TxtPorta.Clear();
            TxtIndirizzo.Clear();
        }
        //chiudo il programma quando premo la croce rossa
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
