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
        public List<Personaje> Personajes { get; set; } = new() {
            new Personaje { Nombre = "Alejandro", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Alejandro.png" },
            new Personaje { Nombre = "Camila", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Camila.png" },
            new Personaje { Nombre = "Daniel", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Daniel.png" },
            new Personaje { Nombre = "Daniela", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Daniela.png" },
            new Personaje { Nombre = "Diego", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Diego.png" },
            new Personaje { Nombre = "Emiliano", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Emiliano.png" },
            new Personaje { Nombre = "Fernanda", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Fernanda.png" },
            new Personaje { Nombre = "Gael", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Gael.png" },
            new Personaje { Nombre = "Isabela", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Isabela.png" },
            new Personaje { Nombre = "Jesús", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Jesús.png" },
            new Personaje { Nombre = "Leonardo", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Leonardo.png" },
            new Personaje { Nombre = "María", ImagenUrl = "/AdivinaQuienServidor;component/Assets/María.png" },
            new Personaje { Nombre = "Mateo", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Mateo.png" },
            new Personaje { Nombre = "Matías", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Matías.png" },
            new Personaje { Nombre = "Miguel", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Miguel.png" },
            new Personaje { Nombre = "Natalia", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Natalia.png" },
            new Personaje { Nombre = "Regina", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Regina.png" },
            new Personaje { Nombre = "Renata", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Renata.png" },
            new Personaje { Nombre = "Santiago", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Santiago.png" },
            new Personaje { Nombre = "Sebastián", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Sebastián.png" },
            new Personaje { Nombre = "Sofía", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Sofía.png" },
            new Personaje { Nombre = "Valentina", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Valentina.png" },
            new Personaje { Nombre = "Valeria", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Valeria.png" },
            new Personaje { Nombre = "Ximena", ImagenUrl = "/AdivinaQuienServidor;component/Assets/Ximena.png" }
        };
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
