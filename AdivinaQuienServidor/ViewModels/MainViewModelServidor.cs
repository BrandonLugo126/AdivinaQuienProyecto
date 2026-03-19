using AdivinaQuienServidor.Models;
using AdivinaQuienServidor.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
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
        public string Pregunta { get; set; } = "";
        public string Modo { get; set; } = "Normal";
        public bool Enturno { get; set; }
        public bool TurnoPreguntar { get; set; }
        public bool TurnoResponder { get; set; }
        public ObservableCollection<Personaje> ListaPersonajes { get; set; } = new();
        public ObservableCollection<string> HistorialChat { get; set; } = new();
        Dispatcher dispatcher;
        public bool ConPersonaje { get; set; } = true;
        public TipoVista VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }
        public ICommand IniciarPartidaCommand { get; set; }

        public ICommand SeleccionarPersonajeCommand { get; }
        public ICommand AdivininarPersonajeCommand { get; }
        public ICommand ResponderCommand { get; }
        public ICommand PreguntarCommand { get; }
        public ICommand VistaGanadaCommand { get; }
        public ICommand VistaPerdidaCommand { get; }

        public ICommand VoltearCartaCommand { get; }

        public MainViewModelServidor()
        {
            IniciarPartidaCommand = new RelayCommand(IrASala);
            foreach (var p in service.Personajes)
            {
                ListaPersonajes.Add(p);
            }
            service.JugadorConectado += Service_JugadorConectado;
            service.JuegoListoParaIniciar += Service_JuegoListoParaIniciar;
            service.ChatActualizado += Service_ChatActualizado;
            AdivininarPersonajeCommand = new RelayCommand<string>(AdivinarPersonaje);
            ResponderCommand = new RelayCommand<string>(Responder);
            PreguntarCommand = new RelayCommand(Preguntar);
            SeleccionarPersonajeCommand = new RelayCommand<string>(SeleccionarPersonaje);
            VistaGanadaCommand = new RelayCommand(VistaGanada);
            VistaPerdidaCommand = new RelayCommand(VistaPerdida);
            VoltearCartaCommand = new RelayCommand<object>(VoltearCarta);

        }

        private void VoltearCarta(object param)
        {
            
            if (param is System.Windows.Controls.Button btn)
            {
              
                bool estadoActual = bool.Parse(btn.Tag.ToString());
                btn.Tag = (!estadoActual).ToString();
            }
        }
        private void Service_ChatActualizado(string obj)
        {

            if (obj != "")
            {
                HistorialChat.Add(obj);
            }

        }

        private void Service_JuegoListoParaIniciar()
        {
            VistaActual = TipoVista.Juego;
            Enturno = true;
            TurnoPreguntar = true;
            OnPropertyChanged(nameof(Enturno));
            OnPropertyChanged(nameof(TurnoPreguntar));

        }

        private void Preguntar()
        {
            service.EnviarPregunta(Pregunta);
            TurnoPreguntar = false;
            Pregunta = "";
            OnPropertyChanged(nameof(Enturno));
            OnPropertyChanged(nameof(Pregunta));
        }

        private void Responder(string obj)
        {
            bool respuesta;
            if (obj=="Si")
            {
                respuesta = true;
            }
            else
            {
                respuesta= false;
            }
            service.ProcesarRespuesta(respuesta);
            TurnoResponder = false;
            OnPropertyChanged(nameof(TurnoResponder));

        }

        private void AdivinarPersonaje(string obj)
        {
            service.IntentarAdivinar(obj);

        }
        private void VistaPerdida()
        {
            VistaActual = TipoVista.Derrota;
        }

        private void VistaGanada()
        {
            VistaActual = TipoVista.Victoria;
        }

        private void SeleccionarPersonaje(string personaje)
        {
            VistaActual = TipoVista.SeleccionarPersonaje;
            service.SeleccionarPersonajeServidor(personaje);
            ConPersonaje = false;
            OnPropertyChanged(nameof(ConPersonaje));

        }

        private void Service_JugadorConectado()
        {
            VistaActual = TipoVista.SeleccionarPersonaje;

        }



        private void IrASala()
        {
            VistaActual = TipoVista.SalaEspera;
            service.AbrirSala(NombreServidor);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
