function EditLessonViewModel(data) {
    var self = this;
    if (!data) {
        self.ScheduleId = ko.observable(0);
        self.GroupId = ko.observable('');
        self.WeekNumber = ko.observable('');
        self.DayNumber = ko.observable('');
        self.ClassNumber = ko.observable('');
        self.Housings = ko.observableArray([]);

        self.Disciplines = ko.observableArray([]);
        self.LessonTypes = ko.observableArray([]);
        self.Lessons = ko.observableArray([]);
    }
   
    var editLessonMapping = {
        'Lessons': {
            create: function (options) {
                return new LessonViewModel(options.data);
            }
        },
        'Disciplines': {
            create: function (options) {
                return new DisciplineViewModel(options.data);
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
   
    ko.mapping.fromJS(data, editLessonMapping, self);

    self.saveLesson = function () {
        var editLessonViewModel = ko.toJS(self);

        // Избавляемся от избыточных данных
        var postData = {
            ScheduleId: editLessonViewModel.ScheduleId,
            ClassNumber: editLessonViewModel.ClassNumber,
            DayNumber: editLessonViewModel.DayNumber,
            GroupId: editLessonViewModel.GroupId,
            Lessons: _.map(editLessonViewModel.Lessons, function (lessonDiscipline) {
                return {
                    LessonId: lessonDiscipline.LessonId,
                    ScheduleId: lessonDiscipline.ScheduleId,
                    ChairId: lessonDiscipline.ChairId,
                    DisciplineId: lessonDiscipline.DisciplineId,
                    LessonDetails: _.map(lessonDiscipline.LessonDetails, function (lessonDetail) {
                        return {
                            LessonDetailId: lessonDetail.LessonDetailId,
                            LessonId: lessonDiscipline.LessonId,
                            AuditoriumId : lessonDetail.AuditoriumId,
                            HousingId: lessonDetail.HousingId,
                            LessonId: lessonDetail.LessonId,
                            PlannedChairJobId: lessonDetail.PlannedChairJobId
                        };
                    }),
                    LessonTypeId: lessonDiscipline.LessonTypeId
                };
            }),
            WeekNumber: editLessonViewModel.WeekNumber
        };

        $.ajax({
            type: "POST",
            url: "/Home/EditLesson",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ viewModel: postData }),
            dataType: "html",
            success: function (result) {
                if (result) {
                    // Обновление происходит до и после из-за того что нужно обновить расписание
                    // старого преподавателя и нового преподавателя (если преподаватель был изменён)
                    // Временное решение
                    viewModel.refreshLesson();

                    $(viewModel.SelectedLessonCell()).find('.lesson-cell-content').replaceWith(result);

                    viewModel.refreshLesson();
                }

                $("#edit-lesson").modal('hide');

                viewModel.cellMark.check();            
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    };

    self.validateAndSave = function () {
        self.isValidationEnabled(true);
        var valid = self.validationObject.isValid();
               
        if (valid) {
            self.saveLesson();
        }
    };

    self.removeDiscipline = function (discipline) {
        self.Lessons.remove(discipline);
    };

    self.addDiscipline = function () {
        self.Lessons.push(new LessonViewModel());
    };

    self.addTeacher = function (lesson) {
        lesson.LessonDetails.push(new LessonDetailViewModel());
    };

    self.removeTeacher = function (lesson, teacher) {
        lesson.LessonDetails.remove(teacher);
    };

    /* Заполнение выпадающих списков
    ------------------------------------------------------------*/
    self.setDisciplineOptionContent = function (option, item) {
        if (!item) return;

        var subtext = "<span class='description' style='float: right;'>" + item.EducationSemesterNumber() + " семестр</span><br><span class='description'>Кафедра " + item.ChairName() + "</span>";
        $(option).attr('data-subtext', subtext);
        // $(option).text(item.DisciplineName() + " [" + item.EducationSemesterName() + "]");
        $(option).attr('title', item.DisciplineName() + " [" + item.EducationSemesterNumber() + " семестр]");

        ko.applyBindingsToNode(option, {}, item);
    };

    self.disciplineChanged = function (lesson, event) {
        var disciplineSelect = $(event.target);

        var discipline = ko.utils.arrayFirst(self.Disciplines(), function (item) {
            return lesson.DisciplineId() === item.DisciplineId();
        });

        if (discipline) {
            lesson.ChairId(discipline.ChairId())
        }

        disciplineSelect.closest('.lesson-content').find('.chair-id').val(discipline.ChairId());

        self.loadTeachers(lesson);
    };
    
    self.loadTeachers = function (lesson) {
        var parameters = {
            disciplineId: lesson.DisciplineId(),
            weekNumber: self.WeekNumber(),
            dayNumber: self.DayNumber(),
            classNumber: self.ClassNumber(),
            groupId: self.GroupId()
        };

        $.post('/Dictionary/TeacherWithEmployment', parameters, function (data) {
            ko.mapping.fromJS(data, {}, lesson.ChairTeachers);

            $('select.teacher').selectpicker('refresh');
        });
    };

    self.setTeacherOptionContent = function (option, item) {
        if (!item) return;

        if (item.Employment()) {
            var groupsCount = item.Employment().split(',').length - 1;
            var inPlural = groupsCount > 1 ? 'групп' : 'группы';

            $(option).attr('data-subtext', "<br><span class='description'>Уже ведёт занятие у " + inPlural + ": " + item.Employment() + "</span>");
            $(option).addClass('red-gradient');
        }
       
        ko.applyBindingsToNode(option, {}, item);
    };

    self.loadHousings = function (lesson) {
        $.post('/Dictionary/HousingEqualLength', {}, function (data) {
            ko.mapping.fromJS(data, {}, lesson.Housings);
        });
    };

    self.setHousingOptionContent = function (option, item) {
        if (!item) return;

        $(option).text(item.HousingName());
        $(option).attr('data-subtext', "<span class='description'>" + item.Abbreviation() + "</span>");
        $(option).attr('title', item.Abbreviation());

        ko.applyBindingsToNode(option, {}, item);
    };

    self.loadAuditoriums = function (lessonDetail, chairId) {
        if (!chairId) return;

        var housingId = lessonDetail.HousingId();
        if (!housingId) {
            lessonDetail.Auditoriums([]);
            return;
        }

        var parameters = {
            chairId: chairId,
            housingId: lessonDetail.HousingId(),
            weekNumber: self.WeekNumber(),
            dayNumber: self.DayNumber(),
            classNumber: self.ClassNumber(),
            groupId: self.GroupId()
        };

        $.post('/Dictionary/AuditoriumWithEmployment', parameters, function (data) {
            ko.mapping.fromJS(data, {}, lessonDetail.Auditoriums);

            $('select.auditorium').selectpicker('refresh');
        });
    };

    self.setAuditoriumOptionContent = function (option, item) {
        if (!item) return;

        var optionContent = "<span class='description'>";
        optionContent += "<span>" + item.AuditoriumTypeName() + "</span>";

        if (item.Places() !== 0) {
            optionContent += "<span>Мест : " + item.Places() + "</span>";
        }

        if (item.Employment()) {
            var groupsCount = item.Employment().split(',').length - 1;
            var inPlural = groupsCount > 1 ? 'групп' : 'группы';

            optionContent += "<br><span>Занятие у " + inPlural + ": " + item.Employment() + "</span>";
            $(option).addClass('red-gradient');
        }
        
        optionContent += "</span>";
        $(option).attr('data-subtext', optionContent);
       
        // $(option).attr('title', item.Abbreviation());

        ko.applyBindingsToNode(option, {}, item);
    };

    self.housingChanged = function (lessonDetail, event) {
        var housingSelect = $(event.target);
        var chairId = housingSelect.closest('.lesson-content').find('.chair-id').val();
        self.loadAuditoriums(lessonDetail, chairId);
    };

    /* Биндинги
    ------------------------------------------------------------*/
    //ko.bindingHandlers.typeahead = {
    //    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
    //        var $element = $(element);
    //        // var allBindings = allBindingsAccessor();          
    //        var elementData = ko.utils.unwrapObservable(valueAccessor());

    //        // source.DisciplineName.subscribe(function (newValue) {
    //        //     alert("The person's new name is " + newValue);
    //        // });

    //        var disciplines = {};
    //        var disciplineLabels = [];

    //        var searchDiscipline = _.debounce(function (query, process) {
    //            $.post('/Dictionary/Discipline', { query: query }, function (data) {

    //                disciplines = {};
    //                disciplineLabels = [];

    //                // if (query.length > 2) {

    //                _.each(data, function (item, ix, list) {
    //                    if (_.contains(disciplines, item.DisciplineName)) {
    //                        item.DisciplineName = item.DisciplineName + ' #' + item.DisciplineId;
    //                    }
    //                    disciplineLabels.push(item.DisciplineName);
    //                    disciplines[item.DisciplineName] = {
    //                        DisciplineId: item.DisciplineId,
    //                        DisciplineName: item.DisciplineName,
    //                        ChairId: item.ChairId,
    //                        ChairName: item.ChairName
    //                    };
    //                });

    //                var labelsCount = Object.keys(disciplineLabels).length;
    //                if (labelsCount === 0) {
    //                    $element.closest(".discipline").siblings('.msg-text').slideDown(250);
    //                    elementData.DisciplineId("");
    //                    elementData.ChairId("");
    //                    elementData.ChairName("");
    //                } else {
    //                    $element.closest(".discipline").siblings('.msg-text').slideUp(250);
    //                }

    //                process(disciplineLabels);
    //                // }

    //                if (query.length === 0) $element.closest(".discipline").siblings('.msg-text').slideUp(250);
    //            });
    //        }, 300);

    //        var options = {
    //            source: function (query, process) {
    //                searchDiscipline(query, process);
    //            },
    //            updater: function (item) {
    //                elementData.DisciplineId(disciplines[item].DisciplineId);
    //                elementData.ChairId(disciplines[item].ChairId);
    //                elementData.ChairName("Кафедра " + disciplines[item].ChairName);

    //                self.loadTeachers(elementData, disciplines[item].ChairId);

    //                $.each(elementData.LessonDetails(), function (index, lessonDetail) {
    //                    lessonDetail.HousingId('');
    //                });

    //                return item;
    //            },
    //            matcher: function (item) {
    //                if (item.toLowerCase().indexOf(this.query.trim().toLowerCase()) !== -1) {
    //                    elementData.DisciplineId(disciplines[item].DisciplineId);

    //                    return item;
    //                }

    //                elementData.DisciplineId("");
    //                return this.query;
    //            },
    //            highlighter: function (item) {
    //                var discipline = disciplines[item];
    //                var template = ''
    //                    + "<div class='typeahead_wrapper'>"
    //                    + "<div class='typeahead_labels'>"
    //                    + "<div class='typeahead_primary'>" + discipline.DisciplineName + "</div>"
    //                    + "<div class='typeahead_secondary'>" + discipline.ChairName + "</div>"
    //                    + "</div>"
    //                    + "</div>";
    //                return template;
    //            }
    //        };

    //        $element
    //            .attr('autocomplete', 'off')
    //            .typeahead(options);
    //    }
    //};

    // Валидация
    self.isValidationEnabled = ko.observable(false);
    self.validationObject = ko.validatedObservable({
        Lessons: self.Lessons
    });
}

function LessonViewModel(data) {
    var self = this;
    if (!data) {
        self.LessonId = ko.observable(0);
        self.ScheduleId = ko.observable(0);
        self.DisciplineId = ko.observable('');
        self.DisciplineName = ko.observable('');
        self.ChairId = ko.observable('');
        self.ChairName = ko.observable('');
        self.LessonTypeId = ko.observable('');
        self.ChairTeachers = ko.observableArray([]);
        self.LessonDetails = ko.observableArray([new LessonDetailViewModel()]);
    }
    
    var lessonMapping = {
        'LessonDetails': {
            create: function (options) {
                return new LessonDetailViewModel(options.data);
            }
        },
        'ChairTeachers': {
            create: function (options) {
                return new TeacherViewModel(options.data);
            }
        }
    };

    ko.mapping.fromJS(data, lessonMapping, self);

    // Валидация
    self.validationObject = ko.validatedObservable({
        LessonDetails: self.LessonDetails,
        DisciplineId: self.DisciplineId.extend({
            required: {
                params: true,
                message: " ",
                onlyIf: function () {
                    return viewModel.EditLessonViewModel().isValidationEnabled();
                }
            }
        })
    });

    return self;
}

function LessonDetailViewModel(data) {
    var self = this;
    if (!data) {
        self.LessonDetailId = ko.observable(0);
        self.LessonId = ko.observable(0);
        self.PlannedChairJobId = ko.observable('');
        self.TeacherLastName = ko.observable('');
        self.TeacherFirstName = ko.observable('');
        self.TeacherMiddleName = ko.observable('');
        self.HousingId = ko.observable('');
        self.AuditoriumId = ko.observable('');
        self.AuditoriumName = ko.observable('');
        self.Auditoriums = ko.observableArray([]);
        self.IsNotActive = ko.observable('');

        //return self;
    }

    var lessonDetailMapping = {
        'Auditoriums': {
            create: function (options) {
                return new AuditoriumViewModel(options.data);
            }
        }
    };
   
    ko.mapping.fromJS(data, lessonDetailMapping, self);

    // Валидация
    self.validationObject = ko.validatedObservable({
        PlannedChairJobId: self.PlannedChairJobId.extend({
            required: {
                params: true,
                message: " ",
                onlyIf: function () {
                    return viewModel.EditLessonViewModel().isValidationEnabled();
                }
            }
        }),
        HousingId: self.HousingId.extend({
            required: {
                params: true,
                message: " ",
                onlyIf: function () {
                    return viewModel.EditLessonViewModel().isValidationEnabled();
                }
            }
        }),
        AuditoriumId: self.AuditoriumId.extend({
            required: {
                params: true,
                message: " ",
                onlyIf: function () {
                    return viewModel.EditLessonViewModel().isValidationEnabled();
                }
            }
        })
    });

    return self;
}