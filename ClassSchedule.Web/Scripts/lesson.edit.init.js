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

    /* Подготовка модального окна редактирования занятия
     --------------------------------------------------------------*/
    //var prepareEditLessonModal = function (lesson) {
    //    if (lesson.length === 0) {
    //        $('#second-discipline').hide();

    //    } else {
    //        // Первая дисциплина
    //        $('#first-discipline .discipline-id').val(lesson[0].DisciplineId);
    //        $('#first-discipline .discipline').val(lesson[0].DisciplineName);
    //        $('#first-discipline .chair-id').val(lesson[0].ChairId);
    //        $('#first-discipline .chair').text(lesson[0].ChairName);

    //        var d1Lt1 = $("#first-discipline .lesson-teacher");
    //        var r = d1Lt1.find("[data-order='1'] .lesson-id");
    //        d1Lt1.find("[data-order='1'] .lesson-id").val(lesson[0].LessonParts[0].LessonId);

    //        // Первый преподаватель первой дисциплины           
    //        fillTeacherSelect(lesson[0].ChairTeachers, $("#first-discipline .lesson-teacher[data-order='1'] .teacher"), lesson[0].LessonParts[0].TeacherId);
    //        fillHousingSelect(lesson[0].Housings, $("#first-discipline .lesson-teacher[data-order='1'] .housing"), lesson[0].LessonParts[0].HousingId);
    //        fillAuditoriumSelect(lesson[0].LessonParts[0].Auditoriums, $("#first-discipline .lesson-teacher[data-order='1'] .auditorium"), lesson[0].LessonParts[0].AuditoriumId);

    //        if (lesson[0].LessonParts.length < 2) {
    //            $("#first-discipline .lesson-teacher[data-order='2']").hide();
    //            $("#first-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").show();
    //            $("#first-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").hide();
    //        } else {
    //            $("#first-discipline .lesson-teacher[data-order='2']").show();
    //            $("#first-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").hide();
    //            $("#first-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").show();

    //            $("#first-discipline .lesson-teacher[data-order='2'] .lesson-id").val(lesson[0].LessonParts[1].LessonId);

    //            // Второй преподаватель первой дисциплины               
    //            fillTeacherSelect(lesson[0].ChairTeachers, $("#first-discipline .lesson-teacher[data-order='2'] .teacher"), lesson[0].LessonParts[1].TeacherId);
    //            fillHousingSelect(lesson[0].Housings, $("#first-discipline .lesson-teacher[data-order='2'] .housing"), lesson[0].LessonParts[1].HousingId);
    //            fillAuditoriumSelect(lesson[0].LessonParts[1].Auditoriums, $("#first-discipline .lesson-teacher[data-order='2'] .auditorium"), lesson[0].LessonParts[1].AuditoriumId);
    //        }

    //        if (lesson.length === 1) {
    //            $('#second-discipline').hide();
    //        } else {
    //            $('#second-discipline').show();

    //            // Вторая дисциплина
    //            $('#second-discipline .discipline-id').val(lesson[1].DisciplineId);
    //            $('#second-discipline .discipline').val(lesson[1].DisciplineName);
    //            $('#second-discipline .chair-id').val(lesson[1].ChairId);
    //            $('#second-discipline .chair').text(lesson[1].ChairName);

    //            $("#second-discipline .lesson-teacher[data-order='1'] .lesson-id").val(lesson[1].LessonParts[0].LessonId);

    //            // Первый преподаватель второй дисциплины           
    //            fillTeacherSelect(lesson[1].ChairTeachers, $("#second-discipline .lesson-teacher[data-order='1'] .teacher"), lesson[1].LessonParts[0].TeacherId);
    //            fillHousingSelect(lesson[1].Housings, $("#second-discipline .lesson-teacher[data-order='1'] .housing"), lesson[1].LessonParts[0].HousingId);
    //            fillAuditoriumSelect(lesson[1].LessonParts[0].Auditoriums, $("#second-discipline .lesson-teacher[data-order='1'] .auditorium"), lesson[1].LessonParts[0].AuditoriumId);

    //            if (lesson[1].LessonParts.length < 2) {
    //                $("#second-discipline .lesson-teacher[data-order='2']").hide();
    //                $("#second-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").show();
    //                $("#second-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").hide();
    //            } else {
    //                $("#second-discipline .lesson-teacher[data-order='2']").show();
    //                $("#second-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").hide();
    //                $("#second-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").show();

    //                $("#second-discipline .lesson-teacher[data-order='2'] .lesson-id").val(lesson[1].LessonParts[1].LessonId);

    //                // Второй преподаватель второй дисциплины           
    //                fillTeacherSelect(lesson[1].ChairTeachers, $("#second-discipline .lesson-teacher[data-order='2'] .teacher"), lesson[1].LessonParts[1].TeacherId);
    //                fillHousingSelect(lesson[1].Housings, $("#second-discipline .lesson-teacher[data-order='2'] .housing"), lesson[1].LessonParts[1].HousingId);
    //                fillAuditoriumSelect(lesson[1].LessonParts[1].Auditoriums, $("#second-discipline .lesson-teacher[data-order='2'] .auditorium"), lesson[1].LessonParts[1].AuditoriumId);
    //            }
    //        }
    //    }
    //};

    /* Показ модального окна редактирования занятия
    ------------------------------------------------------------*/
    $(".lesson-cell").dblclick(function () {
        var weekNumber = $('.week-panel').attr('data-week');
        var groupId = $(this).attr('data-group');
        var dayNumber = $(this).attr('data-day');
        var classNumber = $(this).attr('data-class-number');
        var classDate = $(this).attr('data-class-date');

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
            success: function (lesson) {
                $('#edit-lesson .group-id').val(groupId);
                $('#edit-lesson .week-number').val(weekNumber);
                $('#edit-lesson .day-number').val(dayNumber);
                $('#edit-lesson .class-number').val(classNumber);
                $('#edit-lesson .class-date').val(classDate);

                // prepareEditLessonModal(lesson);

                $('#edit-lesson').modal({
                    backdrop: 'static',
                    keyboard: false
                });
            },
            error: function () {
                alert("failure");
            }
        });

    });

    /* Поиск первой дисциплины в модальном окне редактирования занятия
    -----------------------------------------------------------------*/
    var disciplinesFirst = {};
    var disciplineLabelsFirst = [];

    var searchFirstDiscipline = _.debounce(function (query, process) {
        $.post('/Dictionary/Discipline', { query: query }, function (data) {

            disciplinesFirst = {};
            disciplineLabelsFirst = [];

            if (query.length > 2) {

                _.each(data, function (item, ix, list) {
                    if (_.contains(disciplinesFirst, item.DisciplineName)) {
                        item.DisciplineName = item.DisciplineName + ' #' + item.DisciplineId;
                    }
                    disciplineLabelsFirst.push(item.DisciplineName);
                    disciplinesFirst[item.DisciplineName] = {
                        DisciplineId: item.DisciplineId,
                        DisciplineName: item.DisciplineName,
                        ChairId: item.ChairId,
                        ChairName: item.ChairName
                    };
                });

                var labelsCount = Object.keys(disciplineLabelsFirst).length;
                if (labelsCount === 0) {
                    $("#first-discipline .discipline").siblings('.msg-text').slideDown(250);
                    $("#first-discipline .discipline-id").val("-1");
                    $("#first-discipline .chair-id").val("");
                    $("#first-discipline .chair").text("");
                } else {
                    $("#first-discipline .discipline").siblings('.msg-text').slideUp(250);
                }

                process(disciplineLabelsFirst);
            }

            if (query.length === 0) $("#first-discipline .discipline").siblings('.msg-text').slideUp(250);
        });
    }, 300);

    $("#first-discipline .discipline").typeahead({
        source: function (query, process) {
            searchFirstDiscipline(query, process);
        },
        updater: function (item) {
            $("#first-discipline .discipline-id").val(disciplinesFirst[item].DisciplineId);
            $("#first-discipline .chair-id").val(disciplinesFirst[item].ChairId);
            $("#first-discipline .chair").text("Кафедра " + disciplinesFirst[item].ChairName);

            var teacherSelects = $("#first-discipline .teacher");
            //loadTeachers(teacherSelects, disciplinesFirst[item].ChairId);

            var housingSelects = $("#first-discipline .housing");
            //loadHousings(housingSelects);

            return item;
        },
        matcher: function (item) {
            if (item.toLowerCase().indexOf(this.query.trim().toLowerCase()) !== -1) {
                $("#first-discipline .discipline-id").val(disciplinesFirst[item].DisciplineId);

                return item;
            }

            $("#first-discipline .discipline-id").val('-1');
            return query;
        },
        highlighter: function (item) {
            var discipline = disciplinesFirst[item];
            var template = ''
                + "<div class='typeahead_wrapper'>"
                + "<div class='typeahead_labels'>"
                + "<div class='typeahead_primary'>" + discipline.DisciplineName + "</div>"
                + "<div class='typeahead_secondary'>" + discipline.ChairName + "</div>"
                + "</div>"
                + "</div>";
            return template;
        }
    });

    var disciplineNameFirst = $('#first-discipline .discipline').val();
    if (disciplineNameFirst == null || disciplineNameFirst === "") {
        $('#first-discipline .discipline').siblings('.msg-text').slideUp(0);
        $('#first-discipline .discipline-id').val('-1');
    }

    $("#first-discipline .discipline").keyup(function () {
        if (!this.value) {
            $('#first-discipline .discipline').siblings('.msg-text').slideUp(0);
            $('#first-discipline .discipline-id').val('-1');
            $("#first-discipline .chair-id").val("");
            $("#first-discipline .chair").text("");
        }
    });

    /* Поиск второй дисциплины в модальном окне редактирования занятия
    ------------------------------------------------------------------*/
    var disciplinesSecond = {};
    var disciplineLabelsSecond = [];

    var searchSecondDiscipline = _.debounce(function (query, process) {
        $.post('/Dictionary/Discipline', { query: query }, function (data) {

            disciplinesSecond = {};
            disciplineLabelsSecond = [];

            if (query.length > 2) {

                _.each(data, function (item, ix, list) {
                    if (_.contains(disciplinesSecond, item.DisciplineName)) {
                        item.DisciplineName = item.DisciplineName + ' #' + item.DisciplineId;
                    }
                    disciplineLabelsSecond.push(item.DisciplineName);
                    disciplinesSecond[item.DisciplineName] = {
                        DisciplineId: item.DisciplineId,
                        DisciplineName: item.DisciplineName,
                        ChairId: item.ChairId,
                        ChairName: item.ChairName
                    };
                });

                var labelsCount = Object.keys(disciplineLabelsSecond).length;
                if (labelsCount === 0) {
                    $("#second-discipline .discipline").siblings('.msg-text').slideDown(250);
                    $("#second-discipline .discipline-id").val("-1");
                    $("#second-discipline .chair-id").val("");
                    $("#second-discipline .chair").text("");
                } else {
                    $("#second-discipline .discipline").siblings('.msg-text').slideUp(250);
                }

                process(disciplineLabelsSecond);
            }

            if (query.length === 0) $("#second-discipline .discipline").siblings('.msg-text').slideUp(250);
        });
    }, 300);

    $("#second-discipline .discipline").typeahead({
        source: function (query, process) {
            searchSecondDiscipline(query, process);
        },
        updater: function (item) {
            $("#second-discipline .discipline-id").val(disciplinesSecond[item].DisciplineId);
            $("#second-discipline .chair-id").val(disciplinesSecond[item].ChairId);
            $("#second-discipline .chair").text("Кафедра " + disciplinesSecond[item].ChairName);

            var teacherSelects = $("#second-discipline .teacher");
            //loadTeachers(teacherSelects, disciplinesSecond[item].ChairId);

            var housingSelects = $("#second-discipline .housing");
            //loadHousings(housingSelects);

            return item;
        },
        matcher: function (item) {
            if (item.toLowerCase().indexOf(this.query.trim().toLowerCase()) !== -1) {
                $("#second-discipline .discipline-id").val(disciplinesSecond[item].DisciplineId);

                return item;
            }

            $("#second-discipline .discipline-id").val('-1');
            return query;
        },
        highlighter: function (item) {
            var discipline = disciplinesSecond[item];
            var template = ''
                + "<div class='typeahead_wrapper'>"
                + "<div class='typeahead_labels'>"
                + "<div class='typeahead_primary'>" + discipline.DisciplineName + "</div>"
                + "<div class='typeahead_secondary'>" + discipline.ChairName + "</div>"
                + "</div>"
                + "</div>";
            return template;
        }
    });

    var disciplineNameSecond = $('#second-discipline .discipline').val();
    if (disciplineNameSecond == null || disciplineNameSecond === "") {
        $('#second-discipline .discipline').siblings('.msg-text').slideUp(0);
        $('#second-discipline .discipline-id').val('-1');
    }

    $("#second-discipline .discipline").keyup(function () {
        if (!this.value) {
            $('#second-discipline .discipline').siblings('.msg-text').slideUp(0);
            $('#second-discipline .discipline-id').val('-1');
            $("#second-discipline .chair-id").val("");
            $("#second-discipline .chair").text("");
        }
    });

    /* Скрытие/показ второй дисциплины/преподавателей 
     -------------------------------------------------------*/
    //$('#first-discipline .lesson-btn.add').click(function() {
    //    $('#second-discipline').show();
    //    $('#second-discipline .discipline').focus();
    //});

    //$('#second-discipline .lesson-btn.remove').click(function () {
    //    $('#second-discipline').hide();
    //});

    //$("#first-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").click(function () {
    //    $(this).hide();
    //    $("#first-discipline .lesson-teacher[data-order='2']").show();
    //    $("#first-discipline .lesson-teacher[data-order='2'] .teacher").focus();
    //});

    //$("#first-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").click(function () {
    //    $("#first-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").show();
    //    $("#first-discipline .lesson-teacher[data-order='2']").hide();
    //});

    //$("#second-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").click(function () {
    //    $(this).hide();
    //    $("#second-discipline .lesson-teacher[data-order='2']").show();
    //    $("#second-discipline .lesson-teacher[data-order='2'] .teacher").focus();
    //});

    //$("#second-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").click(function () {
    //    $("#second-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").show();
    //    $("#second-discipline .lesson-teacher[data-order='2']").hide();
    //});
});

function MainViewModel() {
    var self = this;


    //self.Faculties = ko.observableArray([]);
    //self.SelectedFaculty = ko.observable();

    self.loadLesson = function () {
        var weekNumber = $('.week-panel').attr('data-week');
        var groupId = $(this).attr('data-group');
        var dayNumber = $(this).attr('data-day');
        var classNumber = $(this).attr('data-class-number');
        var classDate = $(this).attr('data-class-date');

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
            success: function (lesson) {

                ko.mapping.fromJS(response,
                {
                    key: function (data) {
                        return ko.utils.unwrapObservable(data.LessonId);
                    },
                    create: function (options) {
                        return new LessonViewModel(options.data);
                    }
                },
                self.Students);

                $('#edit-lesson .group-id').val(groupId);
                $('#edit-lesson .week-number').val(weekNumber);
                $('#edit-lesson .day-number').val(dayNumber);
                $('#edit-lesson .class-number').val(classNumber);
                $('#edit-lesson .class-date').val(classDate);

                // prepareEditLessonModal(lesson);

                $('#edit-lesson').modal({
                    backdrop: 'static',
                    keyboard: false
                });
            },
            error: function () {
                alert("failure");
            }
        });


        $.post('/Home/GetLesson', {
            page: self.SelectedPage(),
            pageSize: self.SelectedPageSize(),
            facultyId: self.SelectedFaculty(),
            courseId: self.SelectedCourse(),
            groupId: self.SelectedGroup(),
            educationLevelId: self.SelectedEducationLevel(),
            educationFormId: self.SelectedEducationForm(),
            educationSourceId: self.SelectedEducationSource(),
            countryId: self.SelectedCountry(),
            budgetTypeId: self.SelectedBudgetType(),
            Accommodation: self.SelectedAccommodation(),
            searchLastName: self.SearchLastName(),
            StudentStatusId: self.SelectedStudentStatus(),
            DormitoryNumber: self.SelectedDormitory()
        }, function (response) {
            ko.mapping.fromJS(response,
                {
                    key: function (data) {
                        return ko.utils.unwrapObservable(data.LessonId);
                    },
                    create: function (options) {
                        return new LessonViewModel(options.data);
                    }
                },
                self.Students
            );
            self.PagesCount(response.PagesCount);
            self.StudentsCount(response.StudentsCount);
        });
    };
};

function EditLessonViewModel(editLessonViewModel) {
    var self = this;
    self.GroupId = ko.observable(editLessonViewModel.GroupId || '');
    self.WeekNumber = ko.observable(editLessonViewModel.WeekNumber || '');
    self.DayNumber = ko.observable(editLessonViewModel.DayNumber || '');
    self.ClassNumber = ko.observable(editLessonViewModel.ClassNumber || '');
    self.LessonParts = ko.observableArray(editLessonViewModel.LessonParts || []);
}

function LessonViewModel(lessonViewModel) {
    var self = this;
    self.DisciplineId = ko.observable(lessonViewModel.DisciplineId || '');
    self.DisciplineName = ko.observable(lessonViewModel.DisciplineName || '');
    self.ChairId = ko.observable(lessonViewModel.ChairId || '');
    self.ChairName = ko.observable(lessonViewModel.ChairName || '');
    self.ChairTeachers = ko.observableArray(lessonViewModel.ChairTeachers || []);
    self.DayNumber = ko.observable(lessonViewModel.DayNumber || '');
    self.ClassNumber = ko.observable(lessonViewModel.ClassNumber || '');
    self.LessonTypeId = ko.observable(lessonViewModel.LessonTypeId || '');
    self.Housings = ko.observableArray(lessonViewModel.Housings || []);
    self.LessonParts = ko.observableArray(lessonViewModel.LessonParts || []);
}

function LessonPartViewModel(lessonPartViewModel) {
    var self = this;
    self.LessonId = ko.observableArray(lessonPartViewModel.LessonId || []);
    self.DayNumber = ko.observable(lessonPartViewModel.DayNumber || '');
    self.ClassNumber = ko.observable(lessonPartViewModel.ClassNumber || '');
    self.LessonTypeId = ko.observable(lessonPartViewModel.LessonTypeId || '');
    self.LessonTypeName = ko.observable(lessonPartViewModel.LessonTypeName || '');
    self.ChairId = ko.observable(lessonPartViewModel.ChairId || '');
    self.ChairName = ko.observable(lessonPartViewModel.ChairName || '');
    self.DisciplineId = ko.observable(lessonPartViewModel.DisciplineId || '');
    self.DisciplineName = ko.observable(lessonPartViewModel.DisciplineName || '');
    self.TeacherId = ko.observable(lessonPartViewModel.TeacherId || '');
    self.TeacherLastName = ko.observable(lessonPartViewModel.TeacherLastName || '');
    self.TeacherFirstName = ko.observable(lessonPartViewModel.TeacherFirstName || '');
    self.TeacherMiddleName = ko.observable(lessonPartViewModel.TeacherMiddleName || '');
    self.HousingId = ko.observable(lessonPartViewModel.HousingId || '');
    self.AuditoriumId = ko.observable(lessonPartViewModel.AuditoriumId || '');
    self.AuditoriumName = ko.observable(lessonPartViewModel.AuditoriumName || '');
    self.Auditoriums = ko.observableArray(lessonPartViewModel.Auditoriums || []);
    self.IsNotActive = ko.observable(lessonPartViewModel.IsNotActive || '');
}



//function EditLessonViewModel(editLessonViewModel) {
//    var self = this;
//    self.GroupId = editLessonViewModel.GroupId || '';
//    self.WeekNumber = editLessonViewModel.WeekNumber || '';
//    self.DayNumber = editLessonViewModel.DayNumber || '';
//    self.ClassNumber = editLessonViewModel.ClassNumber || '';
//    self.Lessons = editLessonViewModel.Lessons || [];
//}

//function LessonPartViewModel(lessonPartViewModel) {
//    var self = this;
//    self.LessonId = lessonPartViewModel.LessonId || '';
//    self.LessonTypeId = lessonPartViewModel.LessonTypeId || '';
//    self.LessonTypeName = lessonPartViewModel.LessonTypeName || '';
//    self.DisciplineId = lessonPartViewModel.DisciplineId || '';
//    self.DisciplineName = lessonPartViewModel.DisciplineName || '';
//    self.TeacherId = lessonPartViewModel.TeacherId || '';
//    self.TeacherLastName = lessonPartViewModel.TeacherLastName || '';
//    self.TeacherFistName = lessonPartViewModel.TeacherFirstName || '';
//    self.TeacherMiddleName = lessonPartViewModel.TeacherMiddleName || '';
//    self.AuditoriumId = lessonPartViewModel.AuditoriumId || '';
//    self.AuditoriumName = lessonPartViewModel.AuditoriumName || '';
//    self.IsNotActive = lessonPartViewModel.IsNotActive || '';
//}