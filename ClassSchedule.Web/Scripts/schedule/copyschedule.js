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

    self.submit = function (onComplete) {
        self.InProgress(true);
        var postData = ko.toJSON(self);

        $.ajax({
            method: 'post',
            url: '/Home/CopySchedule',
            data: postData,
            contentType: "application/json; charset=utf-8",
            beforeSend: function() {
                var message = 'Подождите, идёт копирование расписания';
                console.log(message);
                var noty = new Noty(notyOptions);
                noty.options.type = 'information';
                noty.options.text = message;
                noty.show();
            },
            complete: function () {
                self.InProgress(false);
                if (onComplete) onComplete();
            },
            error: function (response) {
                var message = 'При копировании расписания произошла ошибка';
                console.log(message);
                var noty = new Noty(notyOptions);
                noty.options.type = 'error';
                noty.options.text = message;
                noty.show();
            },
            success: function (response) {
                var message = 'Расписание успешно копировано';
                console.log(message);
                var noty = new Noty(notyOptions);
                noty.options.type = 'success';
                noty.options.text = message;
                noty.show();
            }
        });
    };

    ko.mapping.fromJS(data, changeWeekMapping, self);

    self.SelectedWeeks = ko.observableArray([]);
    self.InProgress = ko.observable(false);
}

CopyScheduleViewModel.prototype.toJSON = function () {
    var copy = ko.toJS(this);
    delete copy.Weeks;
    delete copy.Days;
    delete copy.Groups;
    return copy;
}