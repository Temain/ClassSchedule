﻿$(function () {
    /* Показ модального окна редактирования занятия
    ------------------------------------------------------------*/
    $(".lesson-cell").dblclick(function () {
        viewModel.loadLesson(this);
    });

    ko.applyBindings(viewModel);
});

var viewModel = new MainViewModel();
function MainViewModel() {
    var self = this;

    self.EditLessonViewModel = ko.observable();

    self.loadLesson = function (lessonCell) {
        var weekNumber = $('.week-panel').attr('data-week');
        var groupId = $(lessonCell).attr('data-group');
        var dayNumber = $(lessonCell).attr('data-day');
        var classNumber = $(lessonCell).attr('data-class-number');
        var classDate = $(lessonCell).attr('data-class-date');

        var parameters = {
            weekNumber: weekNumber,
            groupId: groupId,
            dayNumber: dayNumber,
            classNumber: classNumber
        };

        $.ajax({
            type: "POST",
            url: "/Home/GetLesson",
            data: parameters,
            success: function (data) {
                self.EditLessonViewModel(new EditLessonViewModel(data));
                if (self.EditLessonViewModel().Lessons().length === 0) {
                    self.addDiscipline();
                }

                $('#edit-lesson').modal({
                    backdrop: 'static',
                    keyboard: false
                });
            },
            error: function () {
                alert("failure");
            }
        });     
    };

    self.saveLesson = function () {
        var editLessonViewModel = ko.toJS(self.EditLessonViewModel());

        $.ajax({
            type: "POST",
            url: "/Home/Edit",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ viewModel: editLessonViewModel }),
            dataType: 'json',
            success: function (result) {
                $("#edit-lesson").modal('hide');
            },
            error: function () {
                alert("failure");
            }
        });
    };

    self.removeDiscipline = function (discipline) {
        self.EditLessonViewModel().Lessons.remove(discipline);
    };

    self.addDiscipline = function () {
        self.EditLessonViewModel().Lessons.push(new LessonViewModel());
    };

    self.addTeacher = function (lesson) {
        lesson.LessonParts.push(new LessonPartViewModel());
    };

    self.removeTeacher = function (lesson, teacher) {
        lesson.LessonParts.remove(teacher);
    };

    /* Заполнение выпадающих списков
    ------------------------------------------------------------*/
    self.loadTeachers = function (lesson, chairId) {
        $.post('/Dictionary/Teacher', { chairId: chairId }, function (data) {
            ko.mapping.fromJS(data, {}, lesson.ChairTeachers);
        });
    };

    self.loadHousings = function (lesson) {
        $.post('/Dictionary/Housing', {}, function (data) {
            ko.mapping.fromJS(data, {}, lesson.Housings);
        });
    };

    self.housingChanged = function (lessonPart, event) {
        var housingSelect = $(event.target);

        var chairId = housingSelect.closest('.lesson-content').find('.chair-id').val();
        $.post('/Dictionary/Auditorium', { chairId: chairId, housingId: lessonPart.HousingId() }, function (data) {
            ko.mapping.fromJS(data, {}, lessonPart.Auditoriums);
        });
    };

    /* Биндинги
    ------------------------------------------------------------*/
    ko.bindingHandlers.typeahead = {
        init: function(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var $element = $(element);
            // var allBindings = allBindingsAccessor();          
            var elementData = ko.utils.unwrapObservable(valueAccessor());

            // source.DisciplineName.subscribe(function (newValue) {
            //     alert("The person's new name is " + newValue);
            // });

            var disciplines = {};
            var disciplineLabels = [];

            var searchDiscipline = _.debounce(function (query, process) {
                $.post('/Dictionary/Discipline', { query: query }, function (data) {

                    disciplines = {};
                    disciplineLabels = [];

                    // if (query.length > 2) {

                        _.each(data, function (item, ix, list) {
                            if (_.contains(disciplines, item.DisciplineName)) {
                                item.DisciplineName = item.DisciplineName + ' #' + item.DisciplineId;
                            }
                            disciplineLabels.push(item.DisciplineName);
                            disciplines[item.DisciplineName] = {
                                DisciplineId: item.DisciplineId,
                                DisciplineName: item.DisciplineName,
                                ChairId: item.ChairId,
                                ChairName: item.ChairName
                            };
                        });

                        var labelsCount = Object.keys(disciplineLabels).length;
                        if (labelsCount === 0) {
                            $element.closest(".discipline").siblings('.msg-text').slideDown(250);
                            elementData.DisciplineId("-1");
                            elementData.ChairId("");
                            elementData.ChairName("");
                        } else {
                            $element.closest(".discipline").siblings('.msg-text').slideUp(250);
                        }

                        process(disciplineLabels);
                    // }

                    if (query.length === 0) $element.closest(".discipline").siblings('.msg-text').slideUp(250);
                });
            }, 300);

            var options = {
                source: function (query, process) {
                    searchDiscipline(query, process);
                },
                updater: function (item) {
                    elementData.DisciplineId(disciplines[item].DisciplineId);
                    elementData.ChairId(disciplines[item].ChairId);
                    elementData.ChairName("Кафедра " + disciplines[item].ChairName);

                    self.loadTeachers(elementData, disciplines[item].ChairId);
                    self.loadHousings(elementData);

                    return item;
                },
                matcher: function (item) {
                    if (item.toLowerCase().indexOf(this.query.trim().toLowerCase()) !== -1) {
                        elementData.DisciplineId(disciplines[item].DisciplineId);

                        return item;
                    }

                    elementData.DisciplineId("-1");
                    return query;
                },
                highlighter: function (item) {
                    var discipline = disciplines[item];
                    var template = ''
                        + "<div class='typeahead_wrapper'>"
                        + "<div class='typeahead_labels'>"
                        + "<div class='typeahead_primary'>" + discipline.DisciplineName + "</div>"
                        + "<div class='typeahead_secondary'>" + discipline.ChairName + "</div>"
                        + "</div>"
                        + "</div>";
                    return template;
                }
            };

            $element
                .attr('autocomplete', 'off')
                .typeahead(options);
        }
    };
};

/* Модели
------------------------------------------------------------*/
function EditLessonViewModel(data) {
    var self = this;
    self.foo = function() {
        alert('!!!');
    };

    var editLessonMapping = {
        'Lessons': {
            create: function (options) {
                return new LessonViewModel(options.data);
            }
        },
        'LessonTypes': {
            create: function (options) {
                return new LessonTypeViewModel(options.data);
            }
        },
        'Housings': {
            create: function (options) {
                return new HousingViewModel(options.data);
            }
        }
    };
    ko.mapping.fromJS(data, editLessonMapping, this);
}

function LessonViewModel(data) {
    var self = this;
    if (!data) {
        self.DisciplineId = ko.observable('');
        self.DisciplineName = ko.observable('');
        self.ChairId = ko.observable('');
        self.ChairName = ko.observable('');
        // self.DayNumber = ko.observable('');
        // self.ClassNumber = ko.observable('');
        self.LessonTypeId = ko.observable('');
        self.ChairTeachers = ko.observableArray([]);
        self.LessonParts = ko.observableArray([new LessonPartViewModel()]);

        return self;
    }

    var lessonMapping = {
        'LessonParts': {
            create: function (options) {
                return new LessonPartViewModel(options.data);
            }
        },
        'ChairTeachers': {
            create: function (options) {
                return new TeacherViewModel(options.data);
            }
        }
    };
    ko.mapping.fromJS(data, lessonMapping, this);
}

function LessonPartViewModel(data) {
    var self = this;
    if (!data) {
        self.LessonId = ko.observable('');
        //self.DayNumber = ko.observable('');
        //self.ClassNumber = ko.observable('');
        //self.LessonTypeId = ko.observable('');
        //self.LessonTypeName = ko.observable('');
        //self.ChairId = ko.observable('');
        //self.ChairName = ko.observable('');
        //self.DisciplineId = ko.observable('');
        //self.DisciplineName = ko.observable('');
        self.TeacherId = ko.observable('');
        self.TeacherLastName = ko.observable('');
        self.TeacherFirstName = ko.observable('');
        self.TeacherMiddleName = ko.observable('');
        self.HousingId = ko.observable('');
        self.AuditoriumId = ko.observable('');
        self.AuditoriumName = ko.observable('');
        self.Auditoriums = ko.observableArray([]);
        self.IsNotActive = ko.observable('');

        return self;
    }

    var lessonPartMapping = {
        'Auditoriums': {
            create: function (options) {
                return new AuditoriumViewModel(options.data);
            }
        }
    };
    ko.mapping.fromJS(data, lessonPartMapping, this);
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