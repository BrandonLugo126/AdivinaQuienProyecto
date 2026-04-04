using AdivinaQuienServidor.Models;
using AdivinaQuienServidor.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Media;
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
        public string Error { get; set; } = "";
        public string MensajeElegir { get; set; } = "Tu turno de elegir";
        public object Modo { get; set; }
        public Personaje? PersonajeElegido { get; set; } = new Personaje();
        public bool Enturno { get; set; }
        public bool TurnoPreguntar { get; set; }
        public bool TurnoResponder { get; set; }
        public bool PuedesAdivinar { get; set; } = true;
        public string Mensaje { get; set; }
        public string Turno { get; set; }
        public ObservableCollection<Personaje> ListaPersonajes { get; set; } = new();
        public ObservableCollection<string> HistorialChat { get; set; } = new();
        Dispatcher HiloUi;
        public bool ConPersonaje { get; set; } = true;
        public TipoVista VistaActual
        {
            get => _vistaActual;
            set { _vistaActual = value; OnPropertyChanged(); }
        }

        SoundPlayer Ganar = new SoundPlayer("C:\\Users\\kaipr\\source\\repos\\AdivinaQuienProyecto\\AdivinaQuienServidor\\Assets\\Sounds\\Cheer Sound.wav");
        SoundPlayer Perder = new SoundPlayer("C:\\Users\\kaipr\\source\\repos\\AdivinaQuienProyecto\\AdivinaQuienServidor\\Assets\\Sounds\\buzzer.wav");
        SoundPlayer Click = new SoundPlayer("C:\\Users\\kaipr\\source\\repos\\AdivinaQuienProyecto\\AdivinaQuienServidor\\Assets\\Sounds\\CLICK.wav");

        public ICommand IniciarPartidaCommand { get; set; }

        public ICommand SeleccionarPersonajeCommand { get; }
        public ICommand AdivininarPersonajeCommand { get; }
        public ICommand ResponderCommand { get; }
        public ICommand PreguntarCommand { get; }
        public ICommand VistaGanadaCommand { get; }
        public ICommand VistaPerdidaCommand { get; }
        public ICommand CambiarDeModoCommand { get; }
        public ICommand VoltearCartaCommand { get; }
        public ICommand VolverAJugarCommand { get; }


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
            service.PartidaTerminada += Service_PartidaTerminada;
            service.TurnoCambiado += Service_TurnoCambiado;
            service.Ganador += Service_Ganador;
            service.ClientePregunto += Service_ClientePregunto;
            service.ClienteRespondio += Service_ClienteRespondio;
            service.ClienteIntentoAdivinar += Service_ClienteIntentoAdivinar;
            AdivininarPersonajeCommand = new RelayCommand<string>(AdivinarPersonaje);
            ResponderCommand = new RelayCommand<string>(Responder);
            PreguntarCommand = new RelayCommand(Preguntar);
            SeleccionarPersonajeCommand = new RelayCommand<string>(SeleccionarPersonaje);
            VistaGanadaCommand = new RelayCommand(VistaGanada);
            VistaPerdidaCommand = new RelayCommand(VistaPerdida);
            VoltearCartaCommand = new RelayCommand<object>(VoltearCarta);
            CambiarDeModoCommand = new RelayCommand(CambiarModo);
            VolverAJugarCommand = new RelayCommand(VolverAjugar);
            HiloUi = Dispatcher.CurrentDispatcher;

        }

        private void VolverAjugar()
        {
            Pregunta = "";
            Error = "";
            MensajeElegir = "Tu turno de elegir";
            Modo = null;
            PersonajeElegido = new Personaje();
            Enturno = false;
            TurnoPreguntar = false;
            TurnoResponder = false;
            PuedesAdivinar = true;
            Mensaje = "";
            Turno = "";
            HistorialChat.Clear();
            ConPersonaje = true;
            service.TerminarPatida();
            VistaActual = TipoVista.SeleccionarPersonaje;
            OnPropertyChanged(nameof(Pregunta));
            OnPropertyChanged(nameof(Error));
            OnPropertyChanged(nameof(MensajeElegir));
            OnPropertyChanged(nameof(Modo));
            OnPropertyChanged(nameof(PersonajeElegido));
            OnPropertyChanged(nameof(Enturno));
            OnPropertyChanged(nameof(TurnoPreguntar));
            OnPropertyChanged(nameof(TurnoResponder));
            OnPropertyChanged(nameof(PuedesAdivinar));
            OnPropertyChanged(nameof(Mensaje));
            OnPropertyChanged(nameof(Turno));
            OnPropertyChanged(nameof(ConPersonaje));
            Click.Play();
        }

        private void Service_ClienteIntentoAdivinar()
        {
            HiloUi.BeginInvoke(() =>
            {
                TurnoPreguntar = true;
                PuedesAdivinar = true;
                OnPropertyChanged(nameof(TurnoPreguntar));
                OnPropertyChanged(nameof(PuedesAdivinar));

            });
        }

        private void Service_ClienteRespondio()
        {
            HiloUi.BeginInvoke(() =>
            {
                Enturno = false;
                TurnoPreguntar = false;
                TurnoResponder = false;
                PuedesAdivinar = false;              
                Turno = $"Turno de {service.Turno}";
                OnPropertyChanged(nameof(Turno));
                OnPropertyChanged(nameof(Enturno));
                OnPropertyChanged(nameof(TurnoPreguntar));
                OnPropertyChanged(nameof(TurnoResponder));
                OnPropertyChanged(nameof(PuedesAdivinar));
            });
        }

        private void Service_ClientePregunto()
        {
            HiloUi.BeginInvoke(() =>
            {
                Enturno = true;
                TurnoResponder = true;
                TurnoPreguntar = false;
                PuedesAdivinar = false;

                OnPropertyChanged(nameof(TurnoResponder));
                OnPropertyChanged(nameof(PuedesAdivinar));
                OnPropertyChanged(nameof(TurnoPreguntar));
            });
        }

        private void Service_TurnoCambiado(string obj)
        {
            HiloUi.BeginInvoke(() =>
            {

                Turno = $"Turno de {obj}";
               
                if (NombreServidor==obj)
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
            Click.Play();

        }

        private void Service_Ganador(string obj)
        {
            if (obj != null)
            {
                if (NombreServidor == obj)
                {
                    VistaActual = TipoVista.Victoria;
                    Ganar.Play();
                }
                else
                {
                    VistaActual = TipoVista.Derrota;
                    Perder.Play();
                }
            }
        }

        private void Service_PartidaTerminada(string obj)
        {

            PersonajeElegido = null;
            if (obj != null)
            {
                Mensaje = obj;
            }
            OnPropertyChanged(nameof(PersonajeElegido));
            OnPropertyChanged(nameof(Mensaje));
        }

        private void VoltearCarta(object param)
        {

            if (param is System.Windows.Controls.Button btn)
            {

                bool estadoActual = bool.Parse(btn.Tag.ToString());
                btn.Tag = (!estadoActual).ToString();
                Click.Play();

            }
        }
        private void Service_ChatActualizado(string obj)
        {
            HiloUi.BeginInvoke(() =>
            {
                if (obj != "")
                {
                    HistorialChat.Add(obj);

                }

            });

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
            if (Pregunta!="")
            {
                service.EnviarPregunta(Pregunta);
                Enturno = false;
                TurnoPreguntar = false;
                PuedesAdivinar = false;
                Pregunta = "";
                OnPropertyChanged(nameof(Enturno));
                OnPropertyChanged(nameof(TurnoPreguntar));
                OnPropertyChanged(nameof(Pregunta));
                OnPropertyChanged(nameof(PuedesAdivinar));
            }
            else
            {
                Error = "Manda una pregunta valida";
                OnPropertyChanged(nameof(Error));
            }
            Click.Play();

        }

        private void Responder(string obj)
        {
            bool respuesta = false;
            if (obj == "Si")
            {
                respuesta = true;
            }
            else
            {
                respuesta = false;
            }
            service.ProcesarRespuesta(respuesta);
            service.CambiarDeTurno();
            TurnoResponder = false;
            TurnoPreguntar = true;
            PuedesAdivinar = true;
            OnPropertyChanged(nameof(TurnoResponder));
            OnPropertyChanged(nameof(TurnoPreguntar));
            OnPropertyChanged(nameof(PuedesAdivinar));
            Turno = $"Turno de {NombreServidor}";
            OnPropertyChanged(nameof(Turno));
            Click.Play();

        }

        private void AdivinarPersonaje(string obj)
        {
            service.IntentarAdivinar(obj);
            service.CambiarDeTurno();
            Turno = $"Turno de {service.Turno}";
            Modo = null;
            OnPropertyChanged(nameof(Modo));
            Click.Play();

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
            PersonajeElegido = ListaPersonajes.Where(x => x.Nombre == personaje).First();
            OnPropertyChanged(nameof(PersonajeElegido));
            MensajeElegir = "Turno del cliente";
            OnPropertyChanged(Error);
            Click.Play();

        }

        private void Service_JugadorConectado()
        {
            VistaActual = TipoVista.SeleccionarPersonaje;

        }



        private void IrASala()
        {
            Error = "";
            if (string.IsNullOrWhiteSpace(NombreServidor))
            {
                Error = "Utiliza un nombre valido";
                OnPropertyChanged(nameof(Error));
            }
            if (Error=="")
            {
                VistaActual = TipoVista.SalaEspera;
                service.AbrirSala(NombreServidor);
                service.NickServidor = NombreServidor;
            }
            Click.Play();

        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
