
ko.components.register('fecha-tiempo-regresiva', {
    viewModel: {
        createViewModel: function (params, componentInfo) {

            var self;

            //#region === VIEWMODELS ===

            function FechaTiempoRegresivaVM()
            {
                self = this;

                self.Intervalo = params.Intervalo;
                self.SepararFechaInicio = ko.observable();
                self.SepararFechaFinal = ko.observable();
                self.SepararHoraInicio = ko.observable();
                self.SepararHoraFinal = ko.observable();
                self.FechaCalculada = ko.observable("");

                self.DiaCalculo = ko.observable();
                self.HoraCalculo = ko.observable();
                self.MinutoCalculo = ko.observable();
                self.SegundoCalculo = ko.observable();
            }

            function CalcularFechaTiempo(Fecha_str)
            {
                var Fecha = new Date(Fecha_str);
                var FechaActual = new Date();

                var DiferenciaMilisegundos = Fecha.getTime() - FechaActual.getTime();
                var Segundos = DiferenciaMilisegundos / 1000;
                var Dias = Math.floor(Segundos / (3600 * 24));
                var Horas = Math.floor((Segundos - (Dias * 3600 * 24)) / (3600));
                var Minutos = Math.floor((Segundos - ((Dias * 3600 * 24) + (Horas * 3600))) / (60));

                self.DiaCalculo(Dias);
                self.HoraCalculo(Horas);
                self.MinutoCalculo(Minutos);
                self.SegundoCalculo(59 - FechaActual.getSeconds());

                self.FechaCalculada("FALTAN " + self.DiaCalculo() + " DÍAS " + " CON " + self.HoraCalculo() + " HORAS, " + self.MinutoCalculo() + " MINUTOS Y " + self.SegundoCalculo() + " SEGUNDOS");

                //console.log(self.FechaCalculada());
            }

            //#endregion

            var vm = new FechaTiempoRegresivaVM();

            vm.Intervalo(setInterval(function ()
            {
                if (params.FechaInicio() != null && params.FechaInicio() != undefined)
                {
                    self.SepararFechaInicio(params.FechaInicio().toString().split("/"));
                    self.SepararHoraInicio(params.HoraInicio().toString().split(":"));

                    if (params.FechaFinal != undefined)
                        self.SepararFechaFinal(params.FechaFinal().toString().split("/"));

                    if (params.HoraFinal != undefined)
                        self.SepararHoraFinal(params.HoraFinal().toString().split(":"));

                    if (new Date(parseInt(self.SepararFechaInicio()[2]), parseInt(self.SepararFechaInicio()[1]) - 1, parseInt(self.SepararFechaInicio()[0]), parseInt(self.SepararHoraInicio()[0]), parseInt(self.SepararHoraInicio()[1])) < new Date())
                        clearInterval(vm.Intervalo());

                    if (new Date(parseInt(self.SepararFechaInicio()[2]), parseInt(self.SepararFechaInicio()[1]) - 1, parseInt(self.SepararFechaInicio()[0]), parseInt(self.SepararHoraInicio()[0]), parseInt(self.SepararHoraInicio()[1])) > new Date())
                        CalcularFechaTiempo(self.SepararFechaInicio()[2] + "/" + self.SepararFechaInicio()[1] + "/" + self.SepararFechaInicio()[0] + " " + params.HoraInicio());
                    else if (self.SepararFechaFinal() != undefined && self.SepararHoraFinal() != undefined && new Date() >= new Date(parseInt(self.SepararFechaInicio()[2]), parseInt(self.SepararFechaInicio()[1]) - 1, parseInt(self.SepararFechaInicio()[0]), parseInt(self.SepararHoraInicio()[0]), parseInt(self.SepararHoraInicio()[1])) && new Date() <= new Date(parseInt(self.SepararFechaFinal()[2]), parseInt(self.SepararFechaFinal()[1]) - 1, parseInt(self.SepararFechaFinal()[0]), parseInt(self.SepararHoraFinal()[0]), parseInt(self.SepararHoraFinal()[1])))
                        self.FechaCalculada("EL EVENTO ESTA EN CURSO");
                    else if (self.SepararFechaFinal() != undefined && self.SepararHoraFinal() != undefined && new Date() > new Date(parseInt(self.SepararFechaFinal()[2]), parseInt(self.SepararFechaFinal()[1]) - 1, parseInt(self.SepararFechaFinal()[0]), parseInt(self.SepararHoraFinal()[0]), parseInt(self.SepararHoraFinal()[1])))
                        self.FechaCalculada("EL EVENTO HA FINALIZADO");

                    if (new Date(parseInt(self.SepararFechaInicio()[2]), parseInt(self.SepararFechaInicio()[1]) - 1, parseInt(self.SepararFechaInicio()[0]), parseInt(self.SepararHoraInicio()[0]), parseInt(self.SepararHoraInicio()[1])) < new Date())
                        clearInterval(vm.Intervalo());
                }
            }, 1000));

            return vm;
        }
    },
    template: { cargadorVista: "ComponentesKo/_FechaTiempoRegresiva", esParcial: true }
});