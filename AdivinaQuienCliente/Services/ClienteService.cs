using AdivinaQuienServidor.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace AdivinaQuienCliente.Services
{
    public class ClienteService
    {

        TcpClient? cliente;
        public string puerto { get; set; } = "5000";
        public string? Nick { get; set; }
        public string? Personaje { get; set; }
        public string IP { get; set; } = "127.0.0.1";
        public ICommand ConnectarCommand { get; set; }

        public ClienteService()
        {
            ConnectarCommand = new RelayCommand(ConectarAlServidor);
        }

        public void ConectarAlServidor()
        {
            try
            {
                if (cliente == null)
                {
                    cliente = new();
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(IP),int.Parse(puerto));
                    cliente.Connect(endPoint);
                    if (cliente.Connected)
                    {
                        Nick = "Brandon";
                        var comando = new ConectarCommando
                        {
                            Comamando = Orden.Conectar,
                            Nombre = Nick ?? "Brandon"
                        };
                        EnviarCommando(comando, cliente);

                    }   
                }

            }
            catch
            {

            }

        }
        private void EnviarCommando(object comando, TcpClient Cliente)
        {
            var stream = Cliente.GetStream();
            var json = JsonSerializer.Serialize(comando);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            stream.Write(buffer, 0, buffer.Length);

        }
    }
}
