using AdivinaQuienServidor.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace AdivinaQuienCliente.Services
{
    public class ClienteService
    {

        TcpClient? cliente;
        int puerto = 5000;
        public string? Nick { get; set; }
        public string? Personaje { get; set; }

        public void ConectarAlServidor(IPAddress ipServidor,string nickJugador)
        {
            try
            {
                if (cliente== null)
                {
                    cliente = new();
                    IPEndPoint endPoint= new IPEndPoint(ipServidor, puerto);
                    cliente.Connect(endPoint);
                    Nick = nickJugador;
                    if (cliente.Connected)
                    {
                        Nick = nickJugador;
                        var comando = new ConectarCommando
                        {
                            Comamando = Orden.Conectar,
                            Nombre = Nick ?? ""
                        };
                        var json = JsonSerializer.Serialize(comando);
                        var buffer = Encoding.UTF8.GetBytes(json);
                        cliente.GetStream().Write(buffer, 0, buffer.Length);
                    }
                }
               
            }
            catch 
            {
                
            }
        }
    }
}
