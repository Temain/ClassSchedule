function CopyScheduleViewModel(data) {
    var self = this;
    if (!data) {
        self.EditedWeek = ko.observable('');
        self.EditedWeekStartDate = ko.observable('');
        self.EditedWeekEndDate = ko.observable('');
        self.CurrentWeek = ko.observable('');
        self.CurrentWeekStartDate = ko.observable('');
        self.CurrentWeekEndDate = ko.observable('');
        self.Today = ko.observable('');
        self.Weeks = ko.observableArray([]);
        self.SelectedWeeks = ko.observableArray([]);
        self.Days = ko.observableArray([]);
        self.SelectedDays = ko.observableArray([]);
        self.Groups = ko.observableArray([]);
        self.SelectedGroups = ko.observableArray([]);
    }

    var changeWeekMapping = {
        'Weeks': {
            create: function (options) {
                return new WeekViewModel(options.data);
            }
        },
        'Days': {
            create: function (options) {
                return new DayViewModel(options.data);
            }
        },
        'Groups': {
            create: function (options) {
                return new GroupViewModel(options.data);
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

    self.setDaysOptionContent = function (option, item) {
        if (!item) return;

        $(option).text(item.DayShortName());

        var subtext = "<span class='description'> " + item.DayName() + "</span>";
        $(option).attr('data-subtext', subtext);

        $(option).attr('title', item.DayShortName());

        ko.applyBindingsToNode(option, {}, item);
    };

    ko.mapping.fromJS(data, changeWeekMapping, self);
}

CopyScheduleViewModel.prototype.toJSON = function () {
    var copy = ko.toJS(this);
    delete copy.Weeks;
    delete copy.Days;
    delete copy.Groups;
    return copy;
}