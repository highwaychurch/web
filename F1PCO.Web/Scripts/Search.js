// Class to represent a person from F1
function F1Person(data) {
    var self = this;
    self.f1id = data.F1ID;
    self.firstName = data.FirstName;
    self.lastName = data.LastName;
}

// Class to represent a person from PCO
function PCOPerson(data) {
    var self = this;
    self.pcoid = data.PCOID;
    self.firstName = data.FirstName;
    self.lastName = data.LastName;
}

// Overall viewmodel for this screen, along with initial state
function SearchViewModel() {
    var self = this;

    self.searchTerm = ko.observable();
    self.f1People = ko.observableArray();
    self.isF1SearchInProgress = ko.observable(false);
    self.pcoPeople = ko.observableArray();
    self.isPCOSearchInProgress = ko.observable(false);

    self.isSearchInProgress = ko.computed(function() {
        return self.isF1SearchInProgress() || self.isPCOSearchInProgress();
    });
    
    self.startSearch = function () {
        // Don't allow double searching
        if (self.isSearchInProgress() == true) return;
        
        // Clear the current results
        self.f1People.removeAll();
        self.pcoPeople.removeAll();

        self.isF1SearchInProgress(true);
        $.getJSON("/api/f1search?searchTerm=" + self.searchTerm(), function (people) {
            self.isF1SearchInProgress(false);
            var mappedPeople = $.map(people, function (person) { return new F1Person(person); });
            self.f1People(mappedPeople);
        });

        self.isPCOSearchInProgress(true);
        $.getJSON("/api/pcosearch?searchTerm=" + self.searchTerm(), function (people) {
            self.isPCOSearchInProgress(false);
            var mappedPeople = $.map(people, function (person) { return new PCOPerson(person); });
            self.pcoPeople(mappedPeople);
        });
    };
}

ko.applyBindings(new SearchViewModel());