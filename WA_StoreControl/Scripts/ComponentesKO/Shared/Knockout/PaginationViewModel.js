//ViewModel de Paginacion...
class PaginationViewModel {
    constructor(data) {
        var self = this;

        self.TotalPages = ko.isObservable(data.TotalPages) ? data.TotalPages : ko.observable(data.TotalPages); //Numero Total de paginas...

        self.TotalDisplayedPages = ko.observable(data.TotalDisplayedPages || 5); //Numero de links de pagina mostrados a la vez...

        self.CurrentPage = ko.isObservable(data.CurrentPage) ? data.CurrentPage : ko.observable(data.CurrentPage || 1); //Pagina actual...

        if (data.OnCurrentPageChange && typeof(data.OnCurrentPageChange) === "function") { //Callback al paginar...
            self.TriggerOnCurrentPageChange = ko.observable(true); //Bandera: Disparar el callback al paginar...(?)
            
            self.CurrentPage.extend({ notify: "always" }).subscribe(function (page) {
                if (self.TriggerOnCurrentPageChange()) {
                    data.OnCurrentPageChange.call(self);
                }

                self.TriggerOnCurrentPageChange(true); //El valor por defecto es true: Invocar el callback cada vez que se pagina...
            });   
        }

        self.PageLinks = ko.computed(function () { //Links de paginación calculados...
            let totalPages = parseInt(self.TotalPages());
            let totalDisplayedPages = parseInt(self.TotalDisplayedPages());

            let currentPage = parseInt(self.CurrentPage());
            let maxPage = Math.trunc(currentPage + (totalDisplayedPages / 2));
            let minPage = maxPage - totalDisplayedPages + 1;

            if (maxPage > totalPages) {
                minPage -= maxPage - totalPages;
                maxPage = totalPages;
            }

            if (minPage < 1) {
                maxPage = Math.min(maxPage + (1 - minPage), totalPages);
                minPage = 1;
            }

            return ko.utils.range(minPage, maxPage);

        });

        self.GoToFirstPage = function (data, event) {
            if (self.CurrentPage() > 1 && self.CurrentPage() <= self.TotalPages()) {
                self.CurrentPage(1);
            }
        };

        self.GoToNextPage = function (data, event) {
            if (self.CurrentPage() < self.TotalPages()) {
                self.CurrentPage(self.CurrentPage() + 1);
            }
        };

        self.GoToPreviousPage = function (data, event) {
            if (self.CurrentPage() > 1) {
                self.CurrentPage(self.CurrentPage() - 1);
            }
        };

        self.GoToLastPage = function (data, event) {
            if (self.CurrentPage() < self.TotalPages()) {
                self.CurrentPage(self.TotalPages());
            }
        };

    }
};
