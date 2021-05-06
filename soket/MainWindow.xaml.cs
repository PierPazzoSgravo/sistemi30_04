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
            IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse("10.73.0.17"), 56000);

           

            btnInviaMsg.IsEnabled = true;

            Thread ricezione = new Thread(new ParameterizedThreadStart(soketRecive));

            ricezione.Start(sourceSocket);

        }

        private void btnInviaMsg_Click(object sender, RoutedEventArgs e)
        {
            ///nomi txt invertiti
            string ipAddress = TxtPorta.Text;
            int port =int.Parse( TxtIndirizzo.Text);

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


                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lbaStoricoChat.Content = message;
                        }));
                    }
                }
            });
        }
    }
}
