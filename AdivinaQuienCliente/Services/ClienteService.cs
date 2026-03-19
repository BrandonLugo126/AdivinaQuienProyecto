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
        public IPAddress? IP { get; set; }
        TcpClient? cliente;
        int puerto = 5000;
        public string? Nick { get; set; }
        public string? NickServer { get; set; }
        public string Personaje { get; set; } = "";
        public string? Turno { get; set; }
        public string MensajeError { get; private set; } = "";

        public event Action? JugadorConectado;
        public event Action? PersonajeServidorElegido;
        public event Action<string>? ChatActualizado;
        public event Action? ServidorPregunto;
        public event Action? PartidaTerminada;
        public bool Enturno;



        public List<string> HistorialPyR { get; set; } = new();
        public List<Personaje> Personajes { get; set; } = new() {
            new Personaje { Nombre = "Alejandro", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Alejandro.png" },
            new Personaje { Nombre = "Camila", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Camila.png" },
            new Personaje { Nombre = "Daniel", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Daniel.png" },
            new Personaje { Nombre = "Daniela", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Daniela.png" },
            new Personaje { Nombre = "Diego", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Diego.png" },
            new Personaje { Nombre = "Emiliano", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Emiliano.png" },
            new Personaje { Nombre = "Fernanda", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Fernanda.png" },
            new Personaje { Nombre = "Gael", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Gael.png" },
            new Personaje { Nombre = "Isabela", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Isabela.png" },
            new Personaje { Nombre = "Jesús", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Jesús.png" },
            new Personaje { Nombre = "Leonardo", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Leonardo.png" },
            new Personaje { Nombre = "María", ImagenUrl = "/AdivinaQuienCliente;component/Assets/María.png" },
            new Personaje { Nombre = "Mateo", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Mateo.png" },
            new Personaje { Nombre = "Matías", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Matías.png" },
            new Personaje { Nombre = "Miguel", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Miguel.png" },
            new Personaje { Nombre = "Natalia", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Natalia.png" },
            new Personaje { Nombre = "Regina", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Regina.png" },
            new Personaje { Nombre = "Renata", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Renata.png" },
            new Personaje { Nombre = "Santiago", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Santiago.png" },
            new Personaje { Nombre = "Sebastián", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Sebastián.png" },
            new Personaje { Nombre = "Sofía", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Sofía.png" },
            new Personaje { Nombre = "Valentina", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Valentina.png" },
            new Personaje { Nombre = "Valeria", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Valeria.png" },
            new Personaje { Nombre = "Ximena", ImagenUrl = "/AdivinaQuienCliente;component/Assets/Ximena.png" }
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

        private void EscucharServidor(object? obj)
        {
            if (cliente != null)
            {
                try
                {
                    while (cliente.Connected)
                    {
                        if (cliente.Available > 0)
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
                                        var personajeSeleccionado = JsonSerializer.Deserialize<SeleccionarPersonajeCommando>(json);
                                        if (personajeSeleccionado != null)
                                        {
                                            PersonajeServidorElegido?.Invoke();
                                        }

                                        break;
                                    case Orden.EsperarRespuesta:

                                        break;
                                    case Orden.Preguntar:
                                        var pregunta = JsonSerializer.Deserialize<PreguntaCommando>(json);
                                        if (pregunta != null)
                                        {
                                            HistorialPyR.Add($"{pregunta.Quien}: {pregunta.Pregunta}");
                                            ChatActualizado?.Invoke($"{pregunta.Quien}: {pregunta.Pregunta}");
                                            ServidorPregunto?.Invoke();
                                        }
                                        break;
                                    case Orden.TerminarPartida:

                                        break;
                                    case Orden.AdivinarPersonaje:                                                                                                                       
                                        break;
                                    case Orden.TerminarTurno:
                                        var terminarTurno = JsonSerializer.Deserialize<TerminarTurnoCommando>(json);
                                        if (terminarTurno != null)
                                        {
                                            Turno = terminarTurno.JugadorTurno;
                                            if (Nick == Turno)
                                            {
                                                Enturno = true;
                                            }
                                            else
                                            {
                                                Enturno = false;
                                            }
                                        }
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
        public void ProcesarRespuesta(bool respuesta)
        {
            if (cliente != null)
            {
                string Respues = respuesta ? "Si" : "No";
                ChatActualizado?.Invoke($"{Turno}:{Respues}");
                var commando = new RespuestaCommando()
                {
                    Quien = Nick,
                    Comamando = Orden.EsperarRespuesta,
                    Respuesta = respuesta
                };
                EnviarCommando(commando, cliente);
            }
        }
        public void EnviarPregunta(string Pregunta)
        {
            if (cliente != null)
            {
                if (Turno == Nick && !string.IsNullOrWhiteSpace(Pregunta))
                {
                    HistorialPyR.Add($"{Nick}: {Pregunta}");
                    var comando = new PreguntaCommando()
                    {
                        Quien = Nick,
                        Comamando = Orden.Preguntar,
                        Pregunta = Pregunta
                    };
                    EnviarCommando(comando, cliente);
                    ChatActualizado?.Invoke($"{Nick}: {Pregunta}");
                }
                else
                {
                    MensajeError = "No es tu turno para hacer una pregunta.";
                }
            }
        }
        public void SeleccionarPersonaje(string personaje)
        {
            if (cliente != null && Enturno == true && Personaje == "")
            {
                var commando = new SeleccionarPersonajeCommando()
                {
                    Comamando = Orden.SeleccionarPersonaje,
                    NombrePersonaje = personaje,
                };
                EnviarCommando(commando, cliente);
            }
        }
        public void IntentarAdivinar(string personaje)
        {
            if (cliente != null && personaje != "")
            {
                var commando = new AdivinarPersonajeCommando()
                {
                    Comamando = Orden.AdivinarPersonaje,
                    PersonajeAdivinado = personaje,
                };
                EnviarCommando(commando, cliente);
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
