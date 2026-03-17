using AdivinaQuienServidor.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdivinaQuienServidor.ViewModels
{
    public enum TipoVista
    {
        Inicio,
        SalaEspera,
    }
    public class MainViewModelServidor : INotifyPropertyChanged
    {
        private TipoVista _vistaActual = TipoVista.Inicio;
        ServidorService service = new();
        public TipoVista VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }
        public ICommand IniciarPartidaCommand { get; set; }

        public MainViewModelServidor()
        {
            IniciarPartidaCommand = new RelayCommand(IrASala);
        }
        private void VolverAInicio()
        {
            VistaActual = TipoVista.Inicio;
        }

        private void IrASala()
        {
            VistaActual = TipoVista.SalaEspera;
            service.AbrirSala();
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
