using AdivinaQuienCliente.Services;
using AdivinaQuienServidor.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

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
        public string Pregunta { get; set; } = "";
       
        public bool Enturno { get; set; }
        public bool TurnoPreguntar { get; set; }
        public bool TurnoResponder { get; set; }
        public bool ConPersonaje { get; set; }
        public ObservableCollection<Personaje> ListaPersonajes { get; set; } = new();
        public ObservableCollection<string> HistorialChat { get; set; } = new();

        public TipoVista VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }

        // Nuevo comando para el modo normal (voltear carta)
        public ICommand VoltearCartaCommand { get; }

        public ICommand IrASalaCommand { get; }
        public ICommand VolverAConexionCommand { get; }
        public ICommand SeleccionarPersonajeCommand { get; }
        public ICommand AdivininarPersonajeCommand { get; }
        public ICommand ResponderCommand { get; }
        public ICommand PreguntarCommand { get; }
        public ICommand VistaJuegoCommand { get; }
        public ICommand VistaGanadaCommand { get; }
        public ICommand VistaPerdidaCommand { get; }
        Dispatcher HiloUi;

        public MainViewModelCliente()
        {
            IrASalaCommand = new RelayCommand(IrASala);
            VolverAConexionCommand = new RelayCommand(VolverAConexion);
            SeleccionarPersonajeCommand = new RelayCommand<string>(SeleccionarPersonaje);
            VistaGanadaCommand = new RelayCommand(VistaGanada);
            VistaPerdidaCommand = new RelayCommand(VistaPerdida);
            VistaJuegoCommand = new RelayCommand(VistaJuego);
            AdivininarPersonajeCommand = new RelayCommand<string>(AdivinarPersonaje);
            ResponderCommand = new RelayCommand<string>(Responder);
            PreguntarCommand = new RelayCommand(Preguntar);
            Service.JugadorConectado += Service_JugadorConectado;
            Service.PersonajeServidorElegido += Service_PersonajeServidorElegido;
            Service.ChatActualizado += Service_ChatActualizado;
            Service.ServidorPregunto += Service_ServidorPregunto;
            HiloUi = Dispatcher.CurrentDispatcher;
            foreach (var p in Service.Personajes)
            {
                ListaPersonajes.Add(p);
            }

            VoltearCartaCommand = new RelayCommand<object>(VoltearCarta);
        }

        private void Service_ServidorPregunto()
        {
            TurnoResponder = true;
            TurnoPreguntar = false;
            OnPropertyChanged(nameof(TurnoResponder));
            OnPropertyChanged(nameof(TurnoPreguntar));
        }

        private void Service_ChatActualizado(string obj)
        {
            HiloUi.BeginInvoke(() =>
            {
                HistorialChat.Add(obj);           
            });

        }

        private void Preguntar()
        {
            Service.EnviarPregunta(Pregunta);
            TurnoPreguntar = false;
            Pregunta = "";
            OnPropertyChanged(Pregunta);
            OnPropertyChanged(nameof(TurnoPreguntar));
        }

        private void Responder(string obj)
        {
            var respuesta = false;
            if (obj == "Si")
            {
                respuesta= true;
            }
            else
            {
                respuesta = false;
            }
            Service.ProcesarRespuesta(respuesta);
        }

        private void AdivinarPersonaje(string obj)
        {


        }

        private void VoltearCarta(object param)
        {
            // El parámetro es el botón que lanzó el evento
            if (param is System.Windows.Controls.Button btn)
            {
                // Invertimos el estado guardado en el Tag
                bool estadoActual = bool.Parse(btn.Tag.ToString());
                btn.Tag = (!estadoActual).ToString();
            }
        }
        private void Service_PersonajeServidorElegido()
        {

            ConPersonaje = true;
            OnPropertyChanged(nameof(ConPersonaje));
        }

        private void Service_JugadorConectado()
        {
            VistaActual = TipoVista.SeleccionarPersonaje;
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

        private void SeleccionarPersonaje(string nombre)
        {
            Service.SeleccionarPersonaje(nombre);
            VistaActual = TipoVista.Juego;
        }

        private void VolverAConexion()
        {
            VistaActual = TipoVista.Conexion;
        }

        private void IrASala()
        {
            VistaActual = TipoVista.SeleccionarPersonaje;
            Service.ConectarAlServidor(IPAddress.Parse(Ip), Nombre);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
