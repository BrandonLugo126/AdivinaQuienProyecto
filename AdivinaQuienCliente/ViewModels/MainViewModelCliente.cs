using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
