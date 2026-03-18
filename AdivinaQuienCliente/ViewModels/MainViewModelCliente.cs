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
        SeleccionarPersonaje,
        Victoria,
        Juego,
        Derrota
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
        public ICommand SeleccionarPersonajeCommand { get; }
        public ICommand VistaJuegoCommand { get; }
        public ICommand VistaGanadaCommand { get; }
        public ICommand VistaPerdidaCommand { get; }

        public MainViewModelCliente()
        {
            IrASalaCommand = new RelayCommand(IrASala);
            VolverAConexionCommand = new RelayCommand(VolverAConexion);
            SeleccionarPersonajeCommand = new RelayCommand(SeleccionarPersonaje);
            VistaGanadaCommand = new RelayCommand(VistaGanada);
            VistaPerdidaCommand = new RelayCommand(VistaPerdida);
            VistaJuegoCommand = new RelayCommand(VistaJuego);
        }

        private void VistaJuego()
        {
            VistaActual = TipoVista.Juego;
        }

        private void VistaPerdida()
        {
            VistaActual = TipoVista.Derrota;
        }

        private void VistaGanada()
        {
            VistaActual = TipoVista.Victoria;
        }

        private void SeleccionarPersonaje()
        {
            VistaActual = TipoVista.SeleccionarPersonaje;
;
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
