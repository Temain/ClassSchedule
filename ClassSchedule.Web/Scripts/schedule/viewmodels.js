function WeekViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function DisciplineViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function LessonTypeViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function TeacherViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function HousingViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function AuditoriumViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function DayViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function GroupSetViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function GroupViewModel(data) {
    var self = this;
    if (!data) {
        self.GroupId = ko.observable('');
        self.GroupName = ko.observable('');
    }

    self.Order = ko.observable('');
    self.IsSelected = ko.observable(false);

    ko.mapping.fromJS(data, {}, self);

    return self;
}

function FacultyViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function EducationFormViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}

function EducationLevelViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}