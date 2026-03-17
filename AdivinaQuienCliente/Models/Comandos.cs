using System;
using System.Collections.Generic;
using System.Text;

namespace AdivinaQuienServidor.Models
{
    public enum Orden { Conectar, SeleccionarPersonaje, EsperarRespuesta, Preguntar, TerminarPartida, AdivinarPersonaje,TerminarTurno }
    public class Comandos
    {
        public Orden Comamando { get; set; }
    }
    public class ConectarCommando : Comandos
    {
        public string Nombre { get; set; } = null!;
    }
    public class SeleccionarPersonajeCommando : Comandos
    {
        public string NombrePersonaje { get; set; } = null!;
    }
    public class PreguntaCommando : Comandos
    {
        public string Pregunta { get; set; } = null!;
    }
    public class RespuestaCommando : Comandos
    {
        public bool Respuesta { get; set; }
    }
    public class AdivinarPersonajeCommando : Comandos
    {
        public string PersonajeAdivinado { get; set; } = null!;
    }
    public class TerminarPartidaCommando : Comandos
    {
        public string NombreGanador { get; set; } = null!;
        public string PersonajeJ1 { get; set; } = null!;
        public string PersonajeJ2 { get; set; } = null!;

    }
    public class TerminarTurnoCommando : Comandos
    {
        public string JugadorTurno { get; set; } = null!;
    }

}
