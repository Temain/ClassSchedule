$(function () {
 
    /* Заполнение выпадающих списков
    ------------------------------------------------------------*/
    //var fillTeacherSelect = function (data, select, selectedId) {
    //    select.html('');
    //    if (!data.length) return;

    //    var option = "<option></option>";
    //    select.append(option);

    //    _.each(data, function (item, ix, list) {
    //        option = "<option value='" + item.TeacherId + "'>" + item.TeacherFullName + "</option>";
    //        select.append(option);                  
    //    });

    //    if (selectedId) select.val(selectedId);
    //};

    //var loadTeachers = function (select, chairId, selectedId) {
    //    $.post('/Dictionary/Teacher', { chairId: chairId }, function (data) {
    //        fillTeacherSelect(data, select, selectedId);
    //    });
    //};

    //var fillHousingSelect = function (data, select, selectedId) {
    //    select.html('');
    //    if (!data.length) return;

    //    var option = "<option></option>";
    //    select.append(option);

    //    _.each(data, function (item, ix, list) {
    //        option = "<option value='" + item.HousingId + "'>" + item.HousingName + "</option>";
    //        select.append(option);
    //    });

    //    if (selectedId) select.val(selectedId);
    //};

    //var loadHousings = function (select, selectedId) {
    //    $.post('/Dictionary/Housing', {}, function (data) {
    //        fillHousingSelect(data, select, selectedId);
    //    });
    //};

    //var fillAuditoriumSelect = function (data, select, selectedId) {
    //    select.html('');
    //    if (!data.length) return;

    //    var option = "<option></option>";
    //    select.append(option);

    //    _.each(data, function (item, ix, list) {
    //        option = "<option value='" + item.AuditoriumId + "'>" + item.AuditoriumName + "</option>";
    //        select.append(option);
    //    });

    //    if (selectedId) select.val(selectedId);
    //};

    //var loadAuditorium = function (select, chairId, housingId, selectedId) {
    //    $.post('/Dictionary/Auditorium', { chairId: chairId, housingId: housingId }, function (data) {
    //        fillAuditoriumSelect(data, select, selectedId);
    //    });
    //};

    //$('.housing').change(function () {
    //    var lessonContainer = $(this).closest('.lesson-container');

    //    var housingId = $(this).val();
    //    var chairId = lessonContainer.find('.chair-id').val();

    //    var auditoriumSelect = $(this).closest('.lesson-teacher').find('.auditorium');
    //    loadAuditorium(auditoriumSelect, chairId, housingId);
    //});

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

                    //var teacherSelects = $("#second-discipline .teacher");
                    //loadTeachers(teacherSelects, disciplinesSecond[item].ChairId);

                    //var housingSelects = $("#second-discipline .housing");
                    //loadHousings(housingSelects);

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

function EditLessonViewModel(data) {
    var editLessonMapping = {
        'Lessons': {
            create: function (options) {
                return new LessonViewModel(options.data);
            }
        }
    };
    ko.mapping.fromJS(data, editLessonMapping, this);
}

function LessonViewModel(data) {
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
        },
        'Housings': {
            create: function (options) {
                return new HousingViewModel(options.data);
            }
        }
    };
    ko.mapping.fromJS(data, lessonMapping, this);
}

function LessonPartViewModel(data) {
    var lessonPartMapping = {
        'Auditoriums': {
            create: function (options) {
                return new AuditoriumViewModel(options.data);
            }
        }
    };
    ko.mapping.fromJS(data, lessonPartMapping, this);
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