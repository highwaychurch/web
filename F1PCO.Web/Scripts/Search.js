// Class to represent a person from F1
function F1Person(data) {
    var self = this;
    self.f1id = data.F1ID;
    self.firstName = data.FirstName;
    self.lastName = data.LastName;
    self.email = data.Email;
    self.mobilePhone = data.MobilePhone;
    self.homePhone = data.HomePhone;
    self.lastUpdatedAt = data.LastUpdatedAtUtc;
}

// Class to represent a person from PCO
function PCOPerson(data) {
    var self = this;
    self.pcoid = data.PCOID;
    self.firstName = data.FirstName;
    self.lastName = data.LastName;
    self.email = data.Email;
    self.mobilePhone = data.MobilePhone;
    self.homePhone = data.HomePhone;
    self.lastUpdatedAt = data.LastUpdatedAtUtc;
}

// Overall viewmodel for this screen, along with initial state
function SearchViewModel() {
    var self = this;

    self.searchTerm = ko.observable();
    self.hasSearched = ko.observable(false);

    self.f1People = ko.observableArray();
    self.isF1SearchInProgress = ko.observable(false);
    self.hasF1SearchFailed = ko.observable(false);
    self.hasNoF1Results = ko.computed(function () {
        return self.isF1SearchInProgress() == false && self.hasF1SearchFailed() == false && self.f1People().length == 0;
    });

    self.pcoPeople = ko.observableArray();
    self.isPCOSearchInProgress = ko.observable(false);
    self.hasPCOSearchFailed = ko.observable(false);
    self.hasNoPCOResults = ko.computed(function () {
        return self.isPCOSearchInProgress() == false && self.hasPCOSearchFailed() == false && self.pcoPeople().length == 0;
    });

    self.isSearchInProgress = ko.computed(function() {
        return self.isF1SearchInProgress() || self.isPCOSearchInProgress();
    });

    self.isSearchAllowed = ko.computed(function() {
        return self.isF1SearchInProgress() == false && self.searchTerm() != null && self.searchTerm().length >= 2;
    });
    
    self.startSearch = function () {
        if (self.isSearchAllowed() == false) return;

        self.hasSearched(true);
        
        // Clear the current results
        self.f1People.removeAll();
        self.pcoPeople.removeAll();

        self.hasF1SearchFailed(false);
        self.isF1SearchInProgress(true);
        $.getJSON("/api/f1search?searchTerm=" + self.searchTerm())
            .success(function (people) {
                var mappedPeople = $.map(people, function (person) { return new F1Person(person); });
                self.f1People(mappedPeople);
                $("abbr").timeago();
            })
            .error(function () {
                self.hasF1SearchFailed(true);
            })
            .complete(function () {
                self.isF1SearchInProgress(false);
            });

        self.hasPCOSearchFailed(false);
        self.isPCOSearchInProgress(true);
        $.getJSON("/api/pcosearch?searchTerm=" + self.searchTerm())
            .success(function (people) {
                var mappedPeople = $.map(people, function (person) { return new PCOPerson(person); });
                self.pcoPeople(mappedPeople);
                $("abbr").timeago();
            })
            .error(function () {
                self.hasPCOSearchFailed(true);
            })
            .complete(function () {
                self.isPCOSearchInProgress(false);
            });
    };
}

ko.applyBindings(new SearchViewModel());