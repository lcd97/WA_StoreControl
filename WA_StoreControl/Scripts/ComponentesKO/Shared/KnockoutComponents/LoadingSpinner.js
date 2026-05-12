ko.components.register("loading-spinner", {
    template: document.getElementById("loading-spinner-template").innerHTML,
    viewModel: function (params) {
        const self = this;
        params = params || {};

        self.Loading = ko.isObservable(params.Loading) ? params.Loading : ko.observable(false);

        self.Message = ko.observable(params.Message || " Cargando...");
    }
});