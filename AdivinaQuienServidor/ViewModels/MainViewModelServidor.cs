using AdivinaQuienServidor.Models;
using AdivinaQuienServidor.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        Juego,
        SeleccionarPersonaje,
        Derrota,
        Victoria
    }
    public class MainViewModelServidor : INotifyPropertyChanged
    {
        private TipoVista _vistaActual = TipoVista.Inicio;
        ServidorService service = new();
        public string NombreServidor { get; set; }
        public ObservableCollection<Personaje> ListaPersonajes { get; set; } = new();
        public ObservableCollection<string> HistorialChat {  get; set; } = new();
        public TipoVista VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }
        public ICommand IniciarPartidaCommand { get; set; }

        public MainViewModelServidor()
        {
            IniciarPartidaCommand = new RelayCommand(IrASala);
            foreach (var p in service.Personajes)
            {
                ListaPersonajes.Add(p);
            }
        }
        private void VolverAInicio()
        {
            VistaActual = TipoVista.Inicio;
        }
        private void IrAJuego()
        {
            VistaActual = TipoVista.Juego;
        }

        private void IrASala()
        {
            VistaActual = TipoVista.Juego;
            service.AbrirSala(NombreServidor);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
