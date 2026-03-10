using System;
using System.Collections.Generic;
using System.Text;

namespace AdivinaQuienServidor.Models
{
    public class Personaje
    {
        public int Id { get; set; } //Esta es por si acaso
        public string Nombre { get; set; } = null!;
        public string ImagenUrl { get; set; } = null!; // o ruta depende de ti dago
    }
}
