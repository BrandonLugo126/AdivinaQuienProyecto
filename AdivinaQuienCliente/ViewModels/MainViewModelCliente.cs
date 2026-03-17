using AdivinaQuienCliente.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace AdivinaQuienCliente.ViewModels
{
    public enum TipoVista
    {
        Conexion,
        SalaEspera,
    }
    public class MainViewModelCliente : INotifyPropertyChanged
    {
        private TipoVista _vistaActual = TipoVista.Conexion;
        ClienteService Service = new();

        public string Nombre { get; set; }
        public string Ip { get; set; } = "127.0.0.1";
        public TipoVista VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }


        public ICommand IrASalaCommand { get; }
        public ICommand VolverAConexionCommand { get; }

        public MainViewModelCliente()
        {
            IrASalaCommand = new RelayCommand(IrASala);
            VolverAConexionCommand = new RelayCommand(VolverAConexion);
        }

        private void VolverAConexion()
        {
            VistaActual = TipoVista.Conexion;
        }

        private void IrASala()
        {
            VistaActual = TipoVista.SalaEspera;
            Service.ConectarAlServidor(IPAddress.Parse(Ip), Nombre);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
