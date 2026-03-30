class SearchViewModel {
    constructor(data) {
        var self = this;
        data = data || {};
        self.SearchString = ko.observable(data.SearchString || "");
        self.SearchType = ko.observable(data.SearchType || 0);
        self.Page = ko.observable(data.page || 1);
        self.SortColumn = ko.observable(data.SortColumn || "Id");
        self.SortDirection = ko.observable(data.SortDirection || "Asc");
        self.RecordsPerPage = ko.observable(data.RecordsPerPage || 10);
        self.TotalRecords = ko.observable(data.TotalRecords || 0);
        self.TotalPages = ko.observable(data.TotalPages || 0);
        self.Paginate = ko.observable(typeof data.Paginate === "boolean" ? data.Paginate : true);
        self.ExcludedIds = ko.observableArray(data.ExcludedIds || []); //Id de los registros agregados a excluir en una búsqueda...
    }
    ResetDefault(except) {
        this.Page(except.includes("Page") ? this.Page() : 1);
        this.SortColumn(except.includes("SortColumn") ? this.SortColumn() : "Id");
        this.SortDirection(except.includes("SortDirection") ? this.SortDirection() : "Asc");
        this.RecordsPerPage(except.includes("RecordsPerPage") ? this.RecordsPerPage() : 10);
        this.TotalRecords(except.includes("TotalRecords") ? this.TotalRecords() : 0);
        this.TotalPages(except.includes("TotalPages") ? this.TotalPages() : 0);
        this.Paginate(except.includes("Paginate") ? this.Paginate() : true);
        this.ExcludedIds(except.includes("ExcludedIds") ? this.ExcludedIds() : []);
    }
};