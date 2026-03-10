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

        public string Personaje1 { get; set; } = null!; // Por defecto siempre va a hacer el del servidor
        public string Personaje2 { get; set; } = null!;
        public string NickPersonaje1 { get; set; } = null!; //Nombre del servidor que pordefecto tendra el turno y personaje #1
        public string NickPersonaje2 { get; set; } = null!;

        int puerto = 5000;
        bool juegoIniciado = false;
        public string Pregunta { get; set; } = null!;
        public string Respuesta { get; set; } = null!;

        public void AbrirSala()
        {
            if (juegoIniciado == false)
            {
                juegoIniciado = true;
                Thread Hilo = new Thread(RecibirJugador2); //Hilo para recibir al jugador 2 y comenzar el juego
                Hilo.IsBackground = true;
                Hilo.Start();
            }
        }

        private void RecibirJugador2(object? obj)
        {
            IPEndPoint Ipserver = new(IPAddress.Any, puerto);
            Servidor = new(Ipserver);
            try
            {
                var clieneNuevo = Servidor.AcceptTcpClient();
                var stream = clieneNuevo.GetStream();

                byte[] buffer = new byte[clieneNuevo.Available];
                var json = Encoding.UTF8.GetString(buffer);
                var ConectarCommand = JsonSerializer.Deserialize<ConectarCommando>(json);
                if (ConectarCommand != null)
                {
                    Personaje2 = ConectarCommand.Personaje;
                    NickPersonaje2 = ConectarCommand.Nombre;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
