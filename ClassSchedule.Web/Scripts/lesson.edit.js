function EditLessonViewModel(data) {
    var self = this;
    if (!data) {
        self.GroupId = ko.observable('');
        self.WeekNumber = ko.observable('');
        self.DayNumber = ko.observable('');
        self.ClassNumber = ko.observable('');
        self.Housings = ko.observableArray([]);
        self.LessonTypes = ko.observableArray([]);
        self.Lessons = ko.observableArray([]);
    }

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

    ko.mapping.fromJS(data, editLessonMapping, self);

    self.saveLesson = function () {
        var editLessonViewModel = ko.toJS(self);

        // Избавляемся от избыточных данных
        var postData = {
            ClassNumber: editLessonViewModel.ClassNumber,
            DayNumber: editLessonViewModel.DayNumber,
            GroupId: editLessonViewModel.GroupId,
            Lessons: _.map(editLessonViewModel.Lessons, function (lessonDiscipline) {
                return {
                    ChairId: lessonDiscipline.ChairId,
                    DisciplineId: lessonDiscipline.DisciplineId,
                    LessonParts: _.map(lessonDiscipline.LessonParts, function (lessonPart) {
                        return {
                            AuditoriumId : lessonPart.AuditoriumId,
                            HousingId: lessonPart.HousingId,
                            LessonId: lessonPart.LessonId,
                            TeacherId: lessonPart.TeacherId
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
                    $(viewModel.SelectedLessonCell()).html(result);
                }

                $("#edit-lesson").modal('hide');

                var flash = $(viewModel.SelectedLessonCell()).find(".flash");
                flash.addClass("bcg-success");
                flash.find("span").addClass("fa fa-check");
                flash.show(500);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    };

    self.removeDiscipline = function (discipline) {
        self.Lessons.remove(discipline);
    };

    self.addDiscipline = function () {
        self.Lessons.push(new LessonViewModel());
    };

    self.addTeacher = function (lesson) {
        lesson.LessonParts.push(new LessonPartViewModel());
    };

    self.removeTeacher = function (lesson, teacher) {
        lesson.LessonParts.remove(teacher);
    };

    /* Заполнение выпадающих списков
    ------------------------------------------------------------*/
    self.loadTeachers = function (lesson, chairId, teacherSelects) {
        var parameters = {
            chairId: chairId,
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
            $(option).attr('data-subtext', "<br><span class='description'>Преподаватель уже ведет занятия у групп: " + item.Employment() + "</span>");
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

    self.loadAuditoriums = function (lessonPart, chairId) {
        if (!chairId) return;

        var housingId = lessonPart.HousingId();
        if (!housingId) {
            lessonPart.Auditoriums([]);
            return;
        }

        var parameters = {
            chairId: chairId,
            housingId: lessonPart.HousingId(),
            weekNumber: self.WeekNumber(),
            dayNumber: self.DayNumber(),
            classNumber: self.ClassNumber(),
            groupId: self.GroupId()
        };

        $.post('/Dictionary/AuditoriumWithEmployment', parameters, function (data) {
            ko.mapping.fromJS(data, {}, lessonPart.Auditoriums);

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
            optionContent += "<br><span>Занятие у групп: " + item.Employment() + "</span>";
            $(option).addClass('red-gradient');
        }
        
        optionContent += "</span>";
        $(option).attr('data-subtext', optionContent);

        

        // $(option).attr('title', item.Abbreviation());

        ko.applyBindingsToNode(option, {}, item);
    };

    self.housingChanged = function (lessonPart, event) {
        var housingSelect = $(event.target);
        var chairId = housingSelect.closest('.lesson-content').find('.chair-id').val();
        self.loadAuditoriums(lessonPart, chairId);
    };

    /* Биндинги
    ------------------------------------------------------------*/
    ko.bindingHandlers.typeahead = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
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

                    //var teacherSelects = $element.find('select.teacher');
                    self.loadTeachers(elementData, disciplines[item].ChairId/*, teacherSelects*/);
                    // self.loadHousings(elementData);
                    $.each(elementData.LessonParts(), function (index, lessonPart) {
                        lessonPart.HousingId('');
                        // self.loadAuditoriums(lessonPart, disciplines[item].ChairId);
                    });

                    return item;
                },
                matcher: function (item) {
                    if (item.toLowerCase().indexOf(this.query.trim().toLowerCase()) !== -1) {
                        elementData.DisciplineId(disciplines[item].DisciplineId);

                        return item;
                    }

                    elementData.DisciplineId("-1");
                    return this.query;
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

    ko.bindingHandlers.selectPicker = {
        after: ['options'],
        init: function (element, valueAccessor, allBindingsAccessor) {
            if ($(element).is('select')) {
                if (ko.isObservable(valueAccessor())) {
                    if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                        // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                        ko.bindingHandlers.selectedOptions.init(element, valueAccessor, allBindingsAccessor);
                    } else {
                        // regular select and observable so call the default value binding
                        ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
                    }
                }

                var allBindings = allBindingsAccessor();
                //var options = allBindings.selectOptions();
                //var optionValue = allBindings.optionValue;
                //var optionText = allBindings.optionText;
                //var optionContent = allBindings.optionContent;

                //var selectContent = '';
                //$.each(options, function(index, option) {
                //    selectContent += "<option data-subtext='" + optionContent + "' value='" + option[optionValue]() + "'>" + option[optionText]() + "</option>";
                //});
                //$(element).html(selectContent);

                $(element).addClass('selectpicker').selectpicker();
            }
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
            if ($(element).is('select')) {
                var selectPickerOptions = allBindingsAccessor().selectPickerOptions;
                if (typeof selectPickerOptions !== 'undefined' && selectPickerOptions !== null) {
                    var options = selectPickerOptions.optionsArray,
                        optionsText = selectPickerOptions.optionsText,
                        optionsValue = selectPickerOptions.optionsValue,
                        optionsCaption = selectPickerOptions.optionsCaption,
                        isDisabled = selectPickerOptions.disabledCondition || false,
                        resetOnDisabled = selectPickerOptions.resetOnDisabled || false;
                    if (ko.utils.unwrapObservable(options).length > 0) {
                        // call the default Knockout options binding
                        ko.bindingHandlers.options.update(element, options, allBindingsAccessor);
                    }
                    if (isDisabled && resetOnDisabled) {
                        // the dropdown is disabled and we need to reset it to its first option
                        $(element).selectpicker('val', $(element).children('option:first').val());
                    }
                    $(element).prop('disabled', isDisabled);
                }
                if (ko.isObservable(valueAccessor())) {
                    if ($(element).prop('multiple') && $.isArray(ko.utils.unwrapObservable(valueAccessor()))) {
                        // in the case of a multiple select where the valueAccessor() is an observableArray, call the default Knockout selectedOptions binding
                        ko.bindingHandlers.selectedOptions.update(element, valueAccessor);
                    } else {
                        // call the default Knockout value binding
                        ko.bindingHandlers.value.update(element, valueAccessor);
                    }
                }

                $(element).selectpicker('refresh');
            }
        }
    };

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
    ko.mapping.fromJS(data, lessonMapping, self);
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
    ko.mapping.fromJS(data, lessonPartMapping, self);
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