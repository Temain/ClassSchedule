$(function () {
    var fd = $('#first-discipline');
    var sd = $('#second-discipline');
    
    /* Заполнение выпадающих списков
    ------------------------------------------------------------*/
    var loadTeachers = function (select, chairId, selectedId) {
        $.post('/Dictionary/Teacher', { chairId: chairId }, function (data) {
            select.html('');
            if (!data.length) return;

            var option = "<option></option>";
            select.append(option);

            _.each(data, function (item, ix, list) {
                option = "<option value='" + item.JobId + "'>" + item.FullName + "</option>";
                select.append(option);
            });

            if (selectedId) select.val(selectedId);
        });
    };

    var loadHousings = function (select) {
        $.post('/Dictionary/Housing', {}, function (data) {
            select.html('');
            if (!data.length) return;

            var option = "<option></option>";
            select.append(option);

            _.each(data, function (item, ix, list) {
                option = "<option value='" + item.HousingId + "'>" + item.HousingName + "</option>";
                select.append(option);
            });
        });
    };

    var loadAuditorium = function (select, chairId, housingId) {
        $.post('/Dictionary/Auditorium', { chairId: chairId, housingId: housingId }, function (data) {
            select.html('');
            if (!data.length) return;

            var option = "<option></option>";
            select.append(option);

            _.each(data, function (item, ix, list) {
                option = "<option value='" + item.AuditoriumId + "'>" + item.AuditoriumNumber + "</option>";
                select.append(option);
            });
        });
    };

    $('.housing').change(function () {
        var lessonContainer = $(this).closest('.lesson-container');

        var housingId = $(this).val();
        var chairId = lessonContainer.find('.chair-id').val();

        var auditoriumSelect = $(this).closest('.lesson-teacher').find('.auditorium');
        loadAuditorium(auditoriumSelect, chairId, housingId);
    });

    /* Подготовка модального окна редактирования занятия
     --------------------------------------------------------------*/
    var prepareEditLessonModal = function (lesson) {
        if (lesson.length === 0) {
            $('#second-discipline').hide();

        } else {
            // Первая дисциплина
            $('#first-discipline .discipline-id').val(lesson[0].DisciplineId);
            $('#first-discipline .discipline').val(lesson[0].DisciplineName);
            $('#first-discipline .chair-id').val(lesson[0].ChairId);
            $('#first-discipline .chair').text(lesson[0].ChairName);

            // Первый преподаватель первой дисциплины
            loadTeachers($("#first-discipline .lesson-teacher[data-order='1'] .teacher"), lesson[0].ChairId, lesson[0].LessonParts[0].TeacherId);

            if (lesson[0].LessonParts.length < 2) {
                $("#first-discipline .lesson-teacher[data-order='2']").hide();
                $("#first-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").show();
                $("#first-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").hide();
            } else {
                $("#first-discipline .lesson-teacher[data-order='2']").show();
                $("#first-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").hide();
                $("#first-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").show();
            }

            if (lesson.length === 1) {
                $('#second-discipline').hide();
            } else {
                $('#second-discipline').show();

                // Вторая дисциплина
                $('#second-discipline .discipline-id').val(lesson[1].DisciplineId);
                $('#second-discipline .discipline').val(lesson[1].DisciplineName);
                $('#second-discipline .chair-id').val(lesson[1].ChairId);
                $('#second-discipline .chair').text(lesson[1].ChairName);

                if (lesson[1].LessonParts.length < 2) {
                    $("#second-discipline .lesson-teacher[data-order='2']").hide();
                    $("#second-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").show();
                    $("#second-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").hide();
                } else {
                    $("#second-discipline .lesson-teacher[data-order='2']").show();
                    $("#second-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").hide();
                    $("#second-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").show();
                }
            }

        }

        /*$('#first-discipline .discipline-id').val();
        $("#first-discipline .lesson-teacher[data-order='1'] .auditorium").val();
        $("#first-discipline .lesson-teacher[data-order='1'] .teacher").val();*/
    };

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

                prepareEditLessonModal(lesson);

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
            loadTeachers(teacherSelects, disciplinesFirst[item].ChairId);

            var housingSelects = $("#first-discipline .housing");
            loadHousings(housingSelects);

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
            loadTeachers(teacherSelects, disciplinesSecond[item].ChairId);

            var housingSelects = $("#second-discipline .housing");
            loadHousings(housingSelects);

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
    $('#first-discipline .lesson-btn.add').click(function() {
        $('#second-discipline').show();
        $('#second-discipline .discipline').focus();
    });

    $('#second-discipline .lesson-btn.remove').click(function () {
        $('#second-discipline').hide();
    });

    $("#first-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").click(function () {
        $(this).hide();
        $("#first-discipline .lesson-teacher[data-order='2']").show();
        $("#first-discipline .lesson-teacher[data-order='2'] .teacher").focus();
    });

    $("#first-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").click(function () {
        $("#first-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").show();
        $("#first-discipline .lesson-teacher[data-order='2']").hide();
    });

    $("#second-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").click(function () {
        $(this).hide();
        $("#second-discipline .lesson-teacher[data-order='2']").show();
        $("#second-discipline .lesson-teacher[data-order='2'] .teacher").focus();
    });

    $("#second-discipline .lesson-teacher[data-order='2'] .teacher-btn.remove").click(function () {
        $("#second-discipline .lesson-teacher[data-order='1'] .teacher-btn.add").show();
        $("#second-discipline .lesson-teacher[data-order='2']").hide();
    });
});