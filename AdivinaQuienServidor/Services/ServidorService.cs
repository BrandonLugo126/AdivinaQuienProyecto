using AdivinaQuienServidor.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace AdivinaQuienServidor.Services
{
    public class ServidorService
    {
        private TcpListener? Servidor { get; set; }
        public List<Personaje> Personajes { get; set; } = new();
        public List<string> HistorialPyR { get; set; } = new List<string>();


        public string? Personaje1 { get; set; } // Por defecto siempre va a hacer el del servidor
        public string? Personaje2 { get; set; }
        public string? NickServidor { get; set; }  //Nombre del servidor que pordefecto tendra el turno y personaje #1
        public string? NickPersonaje2 { get; set; }
        public string? Turno { get; set; }
        public TcpClient ConexionJ2 { get; set; } = null!;

        int puerto = 5000;
        bool juegoIniciado = false;
        bool Jugador2Conectado = false;
        public bool EnTurno;
        public string Pregunta { get; set; } = null!;
        public string Respuesta { get; set; } = null!;

        public event Action? JugadorConectado; // Evento para notificar que el jugador se ha conectado
        public event Action<string>? ChatActualizado; // Evento para notificar que el chat se ha actualizado
        public event Action<string>? TurnoCambiado; // Evento para notificar que el turno ha cambiado
        public event Action? JuegoListoParaIniciar; // Evento para notificar que el juego está listo para iniciar
        public event Action<string>? PartidaTerminada; // Evento para notificar que la partida ha terminado
        public event Action<string>? LogActualizado;//Solo para pruebas para saber que se estan recibiendo los comandos correctamente

        public void AbrirSala(string nombre)
        {
            if (juegoIniciado == false)
            {
                juegoIniciado = true;
                NickServidor = nombre;
                Thread Hilo = new Thread(RecibirJugador2); //Hilo para recibir al jugador 2 y comenzar el juego
                Hilo.IsBackground = true;
                Hilo.Start();
            }
        }
        public void SeleccionarPersonajeServidor(string personaje)
        {
            if (Jugador2Conectado == true && Personaje1 == null && ConexionJ2 != null)
            {
                Personaje1 = personaje;
                if (NickPersonaje2 != null)
                {
                    var comando = new TerminarTurnoCommando()
                    {
                        Comamando = Orden.TerminarTurno,
                        JugadorTurno = NickPersonaje2

                    };
                    EnviarComando(ConexionJ2, comando);
                    TurnoCambiado?.Invoke(NickPersonaje2);
                }
            }
        }
        private void EnviarPregunta(string Pregunta)
        {
            if (Jugador2Conectado && ConexionJ2 != null)
            {
                if (Turno == NickServidor && !string.IsNullOrWhiteSpace(Pregunta))
                {
                    HistorialPyR.Add($"{NickServidor}: {Pregunta}");
                    var comando = new PreguntaCommando()
                    {
                        Comamando = Orden.Preguntar,
                        Pregunta = Pregunta
                    };
                    EnviarComando(ConexionJ2, comando);
                    ChatActualizado?.Invoke($"{NickServidor}: {Pregunta}");
                }
                else
                {
                    LogActualizado?.Invoke("No es tu turno para hacer una pregunta.");
                }
            }
        }

        public void TerminarTurno()
        {
            if (Turno != NickServidor)
            {
                LogActualizado?.Invoke("No es tu turno para terminarlo.");
                return;
            }

            Turno = NickPersonaje2;
            TurnoCambiado?.Invoke(Turno!);
            var Commando = new TerminarTurnoCommando()
            {
                Comamando = Orden.TerminarTurno,
                JugadorTurno = Turno ?? ""
            };
            EnviarComando(ConexionJ2, Commando);
            EnTurno = false;

        }

        public void IntentarAdivinar(string personaje)
        {
            if (Turno != NickServidor)
            {
                LogActualizado?.Invoke("No es tu turno para intentar adivinar.");
                return;
            }
            else
            {
                if (Personaje2 == personaje)
                {
                    var comando = new TerminarPartidaCommando
                    {
                        Comamando = Orden.TerminarPartida,
                        NombreGanador = NickServidor ?? "",
                        PersonajeJ1 = Personaje1 ?? "",
                        PersonajeJ2 = Personaje2 ?? ""
                    };
                    EnviarComando(ConexionJ2, comando);
                    PartidaTerminada?.Invoke($"¡{NickServidor} ha ganado! El personaje de {NickServidor} era {Personaje1} y el de {NickPersonaje2} era {Personaje2}.");
                }
                else
                {
                    var comando = new TerminarTurnoCommando()
                    {
                        Comamando = Orden.TerminarTurno,
                        JugadorTurno = NickPersonaje2 ?? ""
                    };
                    EnviarComando(ConexionJ2, comando);
                    TurnoCambiado?.Invoke(NickPersonaje2 ?? "");
                    ChatActualizado?.Invoke($"{NickServidor} ha intentado adivinar y ha fallado. El turno pasa a {NickPersonaje2}.");
                }
            }
        }

        public void ProcesarRespuesta(bool respuesta)
        {
            string Respues = respuesta? "Si":"No";
            ChatActualizado?.Invoke($"{Turno}:{Respues}");
            var commando = new RespuestaCommando()
            {
                Comamando = Orden.EsperarRespuesta,
                Respuesta = respuesta
            };
            EnviarComando(ConexionJ2, commando);
        }

       
        private void RecibirJugador2()
        {

            IPEndPoint Ipserver = new(IPAddress.Any, puerto);
            byte[] buffer = new byte[1024];
            Servidor = new(Ipserver);
            Servidor.Start();
            while (true)
            {
                try
                {
                    var clieneNuevo = Servidor.AcceptTcpClient();
                    Thread.Sleep(100);
                    var stream = clieneNuevo.GetStream();                    
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    var json = Encoding.UTF8.GetString(buffer, 0, bytes);

                    var ConectarCommand = JsonSerializer.Deserialize<ConectarCommando>(json);


                    if (ConectarCommand != null)
                    {
                        NickPersonaje2 = ConectarCommand.Nombre;
                        Personaje2 = null;
                        Cliente cliente = new Cliente()
                        {
                            Conexion = clieneNuevo,
                            Nombre = ConectarCommand.Nombre
                        };
                        NickPersonaje2 = ConectarCommand.Nombre;
                        ConexionJ2 = clieneNuevo;
                        JugadorConectado?.Invoke();
                        Thread Hilo = new Thread(EscucharCliente);
                        Hilo.IsBackground = true;
                        Hilo.Start();
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void EscucharCliente(object? c)
        {
            if (c != null)
            {
                TcpClient cliente = (TcpClient)c;

                try
                {
                    while (cliente.Connected)
                    {
                        if (cliente.Available > 0)
                        {
                            var stream = cliente.GetStream();
                            byte[] Buffer = new byte[cliente.Available];
                            stream.ReadExactly(Buffer, 0, Buffer.Length);
                            var json = Encoding.UTF8.GetString(Buffer);

                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }
        public void IniciarJuego()
        {
            juegoIniciado = true;
            if (juegoIniciado == true && Personaje1 == null)
            {
                if (NickServidor != null)
                {
                    Turno = NickServidor;

                }

            }
        }
        public void EnviarComando(TcpClient cliente, object comando)
        {
            var stream = cliente.GetStream();
            var json = JsonSerializer.Serialize(comando);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
