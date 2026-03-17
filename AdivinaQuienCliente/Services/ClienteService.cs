using AdivinaQuienServidor.Models;

using GalaSoft.MvvmLight.Command;
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
        int puerto = 5000;
        public string? Nick { get; set; }
        public string? Personaje { get; set; }
        public IPAddress? IP { get; set; }

        public event Action? JugadorConectado;
        public bool Enturno;
        public List<Personaje> Personajes = new List<Personaje>();

        public void ConectarAlServidor(IPAddress IP, string nombre)
        
        {

            if (cliente == null)
            {
                cliente = new();
                IPEndPoint endPoint = new(IP, puerto);
                cliente.Connect(endPoint);

                if (cliente.Connected)
                {
                    Nick = nombre;
                    var comando = new ConectarCommando
                    {
                        Comamando = Orden.Conectar,
                        Nombre = nombre
                    };
                    Thread hilo = new Thread(EscucharServidor);
                    hilo.IsBackground = true;
                    hilo.Start();
                    EnviarCommando(comando, cliente);

                }
            }
        }
        public void SeleccionarPersonaje(string personaje)
        {
            if (Enturno==true)
            {

            }
        }
        private void EscucharServidor(object? obj)
        {
            if (cliente!=null)
            {
                try
                {
                    while (cliente.Connected)
                    {
                        if (cliente.Available>0)
                        {
                            var stream = cliente.GetStream();
                            var buffer = new byte[cliente.Available];
                            stream.ReadExactly(buffer, 0, buffer.Length);
                            var json = Encoding.UTF8.GetString(buffer);
                            var comando = JsonSerializer.Deserialize<Comandos>(json);
                            if (comando != null) 
                            {
                                switch (comando.Comamando)
                                {
                                    case Orden.Conectar:
                                        JugadorConectado?.Invoke();
                                        break;
                                    case Orden.SeleccionarPersonaje:
                                        
                                        break;
                                    case Orden.EsperarRespuesta:
                                        break;
                                    case Orden.Preguntar:
                                        break;
                                    case Orden.TerminarPartida:
                                        break;
                                    case Orden.AdivinarPersonaje:
                                        break;
                                    case Orden.TerminarTurno:
                                        break;
                                    default:
                                        break;
                                }
                            }

                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
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
