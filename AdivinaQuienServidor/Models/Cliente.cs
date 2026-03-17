using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AdivinaQuienServidor.Models
{
    public class Cliente
    {
        public TcpClient Conexion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}
