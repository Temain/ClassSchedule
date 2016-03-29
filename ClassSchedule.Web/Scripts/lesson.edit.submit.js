$(function () {

    /* Отправка формы */
    $("#modal-submit").click(function () {
        var lesson1 = new LessonViewModel({
            LessonId: '0',
            LessonTypeId: '1',
            // ChairId: $('#first-discipline .chair-id').val(),
            TeacherId: $("#first-discipline .lesson-teacher[data-order='1'] .teacher").val(),
            AuditoriumId : $('#first-discipline .auditorium').val(),
            DisciplineId: $('#first-discipline .discipline-id').val(),
            IsNotActive: 'false'
        });
        var lesson2 = new LessonViewModel({
            LessonId: '0',
            LessonTypeId: '1',
            // ChairId: $('#first-discipline .chair-id').val(),
            TeacherId: $("#first-discipline .lesson-teacher[data-order='2'] .teacher").val(),
            AuditoriumId: $('#first-discipline .auditorium').val(),
            DisciplineId: $('#first-discipline .discipline-id').val(),
            IsNotActive: 'false'
        });
        var lesson3 = new LessonViewModel({
            LessonId: '0',
            LessonTypeId: '1',
            // ChairId: $('#second-discipline .chair-id').val(),
            TeacherId: $("#second-discipline .lesson-teacher[data-order='1'] .teacher").val(),
            AuditoriumId: $('#second-discipline .auditorium').val(),
            DisciplineId: $('#second-discipline .discipline-id').val(),
            IsNotActive: 'false'
        });
        var lesson4 = new LessonViewModel({
            LessonId: '0',
            LessonTypeId: '1',
            // ChairId: $('#second-discipline .chair-id').val(),
            TeacherId: $("#second-discipline .lesson-teacher[data-order='2'] .teacher").val(),
            AuditoriumId: $('#second-discipline .auditorium').val(),
            DisciplineId: $('#second-discipline .discipline-id').val(),
            IsNotActive: 'false'
        });

        var editViewModel = new EditLessonViewModel({
            GroupId: $('#edit-lesson .group-id').val(),
            WeekNumber: $('#edit-lesson .week-number').val(),
            DayNumber: $('#edit-lesson .day-number').val(),
            ClassNumber: $('#edit-lesson .class-number').val(),
            Lessons: [lesson1, lesson2, lesson3, lesson4]
        });

        $.ajax({
            type: "POST",
            url: "/Home/Edit",
            data: editViewModel,
            success: function (result) {
                $("#edit-lesson").modal('hide');
            },
            error: function () {
                alert("failure");
            }
        });

    });
});

function EditLessonViewModel(editLessonViewModel) {
    var self = this;
    self.GroupId = editLessonViewModel.GroupId || '';
    self.WeekNumber = editLessonViewModel.WeekNumber || '';
    self.DayNumber = editLessonViewModel.DayNumber || '';
    self.ClassNumber = editLessonViewModel.ClassNumber || '';
    self.Lessons = editLessonViewModel.Lessons || [];
}

function LessonViewModel(lessonViewModel) {
    var self = this;
    self.LessonId = lessonViewModel.LessonId || '';
    self.LessonTypeId = lessonViewModel.LessonTypeId || '';
    self.LessonTypeName = lessonViewModel.LessonTypeName || '';
    // self.ChairId = lessonViewModel.ChairId || '';
    self.DisciplineId = lessonViewModel.DisciplineId || '';
    self.DisciplineName = lessonViewModel.DisciplineName || '';
    self.TeacherId = lessonViewModel.TeacherId || '';
    self.TeacherFullName = lessonViewModel.TeacherFullName || '';
    self.AuditoriumId = lessonViewModel.AuditoriumId || '';
    self.AuditoriumName = lessonViewModel.AuditoriumName || '';
    self.IsNotActive = lessonViewModel.IsNotActive || '';
}