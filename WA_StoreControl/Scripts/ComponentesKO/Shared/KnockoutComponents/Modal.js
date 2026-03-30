//Registrar custom binding para inicializar el componente de bootstrap...
ko.bindingHandlers.bsModal = {
    init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
       let options = ko.unwrap(valueAccessor());
       let modal = new bootstrap.Modal(element, options); //Crear la instancia del modal...
       bindingContext.$data.BootstrapInstance(modal); //Actualizar la instancia del viewModel a la actual instancia creada...
       let nameItemFocusId = bindingContext.$data.NameItemFocusId();

       if(ko.unwrap(nameItemFocusId) != ""){
            element.addEventListener('shown.bs.modal', function (event) {
                $("#" + ko.unwrap(nameItemFocusId)).trigger("focus");
            })
       }
    }
};


class ModalHeaderViewModel {
    constructor(data) {
        data = data || {};
        const self = this;
        self.ModalTitle = ko.observable(data.ModalTitle || "");
        self.BackgroundColorClass = ko.observable(data.BackgroundColorClass || "");
        self.ModalHeaderClass = ko.computed(function () {
            return "modal-header " + self.BackgroundColorClass();
        });
    };
};

class ModalBodyViewModel {
    constructor(data) {
        data = data || {};
        const self = this;
        self.ModalBodyTemplate = ko.observable(data.ModalBodyTemplate);
    };
};

class ModalViewModel {
    constructor(data) {
        data = data || {};
        const self = this;
        self.ComponentOptions = ko.observable(data.ComponentOptions || {});
        self.NameItemFocusId = ko.observable(data.NameItemFocusId || "");
        self.ModalId = ko.observable(data.ModalId || "DefaultModalId");
        self.BootstrapInstance = ko.observable();
        self.ModalHeaderViewModel = ko.observable(data.ModalHeaderViewModel);
        self.ModalBodyViewModel = ko.observable(data.ModalBodyViewModel);
        self.ModalSizeClass = ko.observable(data.ModalSizeClass);   
    };
};


ko.components.register("modal-header", {
    template: { element: "modal-header-template" },
    viewModel: function (params) {
        return params.ViewModel;
    }
});

ko.components.register("modal-body", {
    template: { element: "modal-body-template" },
    viewModel: function (params) {
        return params.ViewModel;
    }
});

ko.components.register("modal", {
    template: { element: "modal-template" },
    viewModel: function (params) {
        return params.ViewModel;
    }
});     