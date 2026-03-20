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
        public object Modo { get; set; }
        public string Turno { get; set; }
        public string Mensaje { get;  set; }


        public bool Enturno { get; set; }
        public bool TurnoPreguntar { get; set; }
        public bool TurnoResponder { get; set; }
        public bool ConPersonaje { get; set; }
        public bool PuedesAdivinar { get; set; }
        public Personaje? PersonajeElegido { get; set; } = new Personaje();

        public ObservableCollection<Personaje> ListaPersonajes { get; set; } = new();
        public ObservableCollection<string> HistorialChat { get; set; } = new();

        public TipoVista VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }


        public ICommand VoltearCartaCommand { get; }

        public ICommand IrASalaCommand { get; }
        public ICommand VolverAConexionCommand { get; }
        public ICommand SeleccionarPersonajeCommand { get; }
        public ICommand AdivininarPersonajeCommand { get; }
        public ICommand ResponderCommand { get; }
        public ICommand PreguntarCommand { get; set; }
        public ICommand VistaJuegoCommand { get; }
        public ICommand VistaGanadaCommand { get; }
        public ICommand VistaPerdidaCommand { get; }
        public ICommand CambiarDeModoCommand { get; }

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
            CambiarDeModoCommand = new RelayCommand(CambiarModo);


            Service.JugadorConectado += Service_JugadorConectado;
            Service.PersonajeServidorElegido += Service_PersonajeServidorElegido;
            Service.ChatActualizado += Service_ChatActualizado;
            Service.ServidorPregunto += Service_ServidorPregunto;
            Service.TurnoCambiado += Service_TurnoCambiado;
            Service.ServidorRespondio += Service_ServidorRespondio;
            Service.ServidorGano += Service_ServidorGano ;
            Service.ClientePerdio += Service_ClientePerdio;
            Service.ClienteGano += Service_ClienteGano;
            Service.ServidorFallo += Service_ServidorFallo;
            HiloUi = Dispatcher.CurrentDispatcher;
            foreach (var p in Service.Personajes)
            {
                ListaPersonajes.Add(p);
            }
            VoltearCartaCommand = new RelayCommand<object>(VoltearCarta);
        }

        private void Service_ServidorFallo()
        {
            HiloUi.BeginInvoke(() =>
            {
                TurnoPreguntar = true;
                PuedesAdivinar = true;
                OnPropertyChanged(nameof(TurnoPreguntar));
                OnPropertyChanged(nameof(PuedesAdivinar));

            });
        }

        private void Service_ClientePerdio(string obj)
        {
            HiloUi.BeginInvoke(() =>
            {
                Mensaje = obj;
                VistaActual = TipoVista.Derrota;
                OnPropertyChanged(Mensaje);
            });
        }

        private void Service_ClienteGano(string obj)
        {
            HiloUi.BeginInvoke(() =>
            {
                Mensaje = obj;
                VistaActual = TipoVista.Victoria;
                OnPropertyChanged(Mensaje);
            });
        }

        private void Service_ServidorGano(string servidorGano)
        {
            HiloUi.BeginInvoke(() =>
            {
                VistaActual = TipoVista.Derrota;
                Mensaje = servidorGano;
                OnPropertyChanged(Mensaje);

            });
        }
        private void Service_ServidorRespondio()
        {
            HiloUi.BeginInvoke(() =>
            {
                Enturno = true;
                TurnoPreguntar = false;
                TurnoResponder = false;
                PuedesAdivinar = false;
                OnPropertyChanged(nameof(Enturno));
                OnPropertyChanged(nameof(TurnoPreguntar));
                OnPropertyChanged(nameof(TurnoResponder));
                OnPropertyChanged(nameof(PuedesAdivinar));
            });
        }

        private void Service_TurnoCambiado(string obj)
        {
            HiloUi.BeginInvoke(() =>
            {
                Turno = $"Turno de {obj}";
                OnPropertyChanged(nameof(Turno));
               
                if (Nombre == obj)
                {
                    PuedesAdivinar = true;
                    TurnoPreguntar = true;
                    TurnoResponder = false;
                }
                else
                {
                    PuedesAdivinar = false;
                    TurnoPreguntar = false;
                    TurnoResponder = false;
                }
                OnPropertyChanged();
                

            });

        }

        public void CambiarModo()
        {
            Modo = "Seleccion";
            OnPropertyChanged(nameof(Modo));
            TurnoResponder = false;
            TurnoPreguntar = false;
            PuedesAdivinar = false;
            Turno = "¡Selecciona un personaje!";
            OnPropertyChanged(nameof(Turno));
            OnPropertyChanged(nameof(PuedesAdivinar));
            OnPropertyChanged(nameof(TurnoResponder));
            OnPropertyChanged(nameof(TurnoPreguntar));
        }

        private void Service_ServidorPregunto()
        {
            Enturno = true;
            TurnoResponder = true;
            TurnoPreguntar = false;
            PuedesAdivinar = false;

            OnPropertyChanged(nameof(PuedesAdivinar));
            OnPropertyChanged(nameof(Enturno));
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
            PuedesAdivinar=false;
            Pregunta = "";
            OnPropertyChanged(Pregunta);
            OnPropertyChanged(nameof(TurnoPreguntar));
            OnPropertyChanged(nameof(PuedesAdivinar));
        }

        private void Responder(string obj)
        {
            var respuesta = false;
            if (obj == "Si")
            {
                respuesta = true;
            }
            else
            {
                respuesta = false;
            }
            Service.ProcesarRespuesta(respuesta);
            Service.CambiarDeTurno();
            TurnoResponder = false;
            TurnoPreguntar = true;
            PuedesAdivinar = true;
            OnPropertyChanged(nameof(TurnoResponder));
            OnPropertyChanged(nameof(TurnoPreguntar));
            OnPropertyChanged(nameof(PuedesAdivinar));
            Turno = $"Turno de {Nombre}";
            OnPropertyChanged(nameof(Turno));
        }

        private void AdivinarPersonaje(string obj)
        {
            Service.IntentarAdivinar(obj);
            Service.CambiarDeTurno();
        }

        private void VoltearCarta(object param)
        {
           
            if (param is System.Windows.Controls.Button btn)
            {
              
                bool estadoActual = bool.Parse(btn.Tag.ToString());
                btn.Tag = (!estadoActual).ToString();
            }
        }
        private void Service_PersonajeServidorElegido()
        {

            ConPersonaje = true;
            Turno = $"Turno de {Service.Turno}";
            OnPropertyChanged(nameof(ConPersonaje));
            OnPropertyChanged(nameof(Turno));
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
            PersonajeElegido = ListaPersonajes.Where(x => x.Nombre == nombre).First();
            Service.Personaje = PersonajeElegido.Nombre;
            OnPropertyChanged(nameof(PersonajeElegido));
            Turno = $"Turno de {Service.Turno}";
            OnPropertyChanged(nameof(Turno));
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
