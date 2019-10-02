using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Server
{
    class MessageView : INotifyPropertyChanged
    {
        public ObservableCollection<Message> Messages { get; set; }
        public bool isRunning;
        
        public bool IsRunning
        {
            get
            {
                return IsRunning;
            }
            set
            {
                isRunning = value;
            }
        }

        public void AddMsg(string data, DateTime time)
        {
            Messages.Insert(0, new Message { Data = data, Time = time });
        }

        public MessageView()
        {
            Messages = new ObservableCollection<Message>();
            dispatcher = Dispatcher.CurrentDispatcher;
            isRunning = true;
            this.ServerWork();
        }
        
        Dispatcher dispatcher;
        public void ServerWork()
        {
            int port = 8005;
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);
                AddMsg("Сервер запущен. Ожидание подключений", DateTime.Now);

                Thread secondThread = new Thread(new ParameterizedThreadStart(Receiver));
                secondThread.IsBackground = true;
                secondThread.Start(listenSocket);
                   
            }
            catch (Exception ex)
            {

                AddMsg(ex.Message, DateTime.Now);
            }
        }

        void Receiver (object ls)
        {
                Socket listenSocket = ls as Socket;
                while (isRunning)
                {

                Socket handler = listenSocket.Accept();
               
                // получаем сообщение
                StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                
                
                    dispatcher.Invoke(new Action(() =>
                        {
                            AddMsg(builder.ToString(), DateTime.Now);
                        }));
                
               
            
                    // отправляем ответ
                    string message = "ваше сообщение доставлено";
                    data = Encoding.Unicode.GetBytes(message);
                    handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Thread.Sleep(300);             
            }
        }

        public void StopServer()
        {
            IsRunning = false;
         
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
