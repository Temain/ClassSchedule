function ChangeWeekViewModel(data) {
    var self = this;
    if (!data) {
        self.EditedWeek = ko.observable('');
        self.EditedWeekStartDate = ko.observable('');
        self.EditedWeekEndDate = ko.observable('');
        self.CurrentWeek = ko.observable('');
        self.CurrentWeekStartDate = ko.observable('');
        self.CurrentWeekEndDate = ko.observable('');
        self.SelectedWeek = ko.observable(''),
        self.Today = ko.observable('');
        self.Weeks = ko.observableArray([]);
    }

    var changeWeekMapping = {
        'Weeks': {
            create: function (options) {
                return new WeekViewModel(options.data);
            }
        }
    };

    function pad(str, max) {
        str = str.toString();
        return str.length < max ? pad(str + " ", max) : str;
    }

    self.setWeekOptionContent = function (option, item) {
        if (!item) return;

        var weekNumber = pad(item.WeekNumber(), 2);

        $(option).text(weekNumber + ' неделя');

        var subtext = "<span class='description'> (" + item.WeekStartDate() + " - " + item.WeekEndDate() + ")     ";
        if (item.ScheduleTypeName() != null) {
            subtext += "<span class='schedule-type' style='color:" + item.ScheduleTypeColor() + "'>" + item.ScheduleTypeName() + "</span>";
        }
        subtext += "</span>";
        $(option).attr('data-subtext', subtext);

        $(option).attr('title', item.WeekNumber() + ' неделя');

        ko.applyBindingsToNode(option, {}, item);
    };

    ko.mapping.fromJS(data, changeWeekMapping, self);
    self.SelectedWeek = ko.observable(self.EditedWeek());
}

function WeekViewModel(data) {
    ko.mapping.fromJS(data, {}, this);
}