//Registrar componente...
ko.components.register("loading-spinner", {
    template: { element: "loading-spinner-template" },
    viewModel: function (params) {
        const self = this;
        params = params || {};
        self.Loading = ko.isObservable(params.Loading) ? params.Loading : ko.observable(false); //Bandera: Se está cargando (?)...
        self.Message = ko.observable(params.Message || "Cargando...");  //Mensaje a mostrar cuando se está cargando algo...
        
        // return params.LoadingSpinnerViewModel
    }
})