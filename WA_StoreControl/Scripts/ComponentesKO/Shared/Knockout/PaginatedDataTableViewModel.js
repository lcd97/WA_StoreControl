//ViewModel de tabla de datos paginada...
function PaginatedDataTableViewModel(data) {
    var self = this;
    self.Items = ko.isObservable(data.Items) ? data.Items : ko.observableArray(data.Items || []);

    self.CurrentPage = ko.observable(1);

    self.PageSize = ko.observable(data.PageSize || 10);

    self.TotalPages = ko.computed(function () {
        if (self.Items()) { //Si hay algo en data...
            let totalItems = ko.unwrap(self.Items).length;
            let pageSize = parseInt(self.PageSize());
            return Math.ceil(ko.unwrap(self.Items).length / pageSize);
        }
    });

    self.ItemsOnCurrentPage = ko.computed(function () {
        var items = [];
        if (self.Items()) { //Si hay algo en data...

            let pageSize = parseInt(self.PageSize());
            let currentPage = parseInt(self.CurrentPage());
            let startIndex = pageSize * (currentPage - 1);
            items = ko.unwrap(self.Items).slice(startIndex, startIndex + pageSize);
        }

        return items;
    });

    self.PaginationViewModel = new PaginationViewModel({ //La paginacion se maneja aparte de forma aislada para abstraer su comportamiento...
        TotalPages: self.TotalPages,
        CurrentPage: self.CurrentPage,
        TotalDisplayedPages: data.TotalDisplayedPages || 10
    });

};
