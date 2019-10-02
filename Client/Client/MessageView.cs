using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class MessageView : INotifyPropertyChanged
    {
        public ObservableCollection<Message> Messages { get; set; }

        public void AddMsg(string data, DateTime time)
        {
            Messages.Insert(0, new Message { Data = data, Time = time });
        }

        public MessageView()
        {
            Messages = new ObservableCollection<Message>();
        }
        
        private RelayCommand workCommand;
        public RelayCommand WorkCommand
        {
            get
            {
                return workCommand ??
                  (workCommand = new RelayCommand(obj =>
                  {
                      string message = obj as string;
                      if (message != null)
                      {
                          ClientWork(message);
                      }
                      else
                      {
                          ClientWork(" ");
                      }
                  }));
            }
        }

        
        public void ClientWork(string message)
        {
            // адрес и порт сервера, к которому будем подключаться
            int port = 8005; // порт сервера
            string address = "127.0.0.1"; // адрес сервера
            
            
                try
                {
                    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // подключаемся к удаленному хосту
                    socket.Connect(ipPoint);
                //Console.Write("Введите сообщение:");
                //string message = Console.ReadLine();
                this.Messages.Insert(0, new Message { Data = message, Time = DateTime.Now });
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    socket.Send(data);

                    // получаем ответ
                    data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                this.Messages.Insert(0, new Message { Data = "Ответ сервера: "+builder.ToString(), Time = DateTime.Now });
                //Console.WriteLine("ответ сервера: " + builder.ToString());

                    // закрываем сокет
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception ex)
                {
                // Console.WriteLine(ex.Message);
                this.Messages.Insert(0, new Message { Data = ex.Message, Time = DateTime.Now });
            }
                Console.Read();
            
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
