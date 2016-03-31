$(function () {

    /* Отправка формы */
    //$("#modal-submit").click(function () {
    //    var lessonPart1 = new LessonPartViewModel({
    //        LessonId: '0',
    //        LessonTypeId: '1',
    //        TeacherId: $("#first-discipline .lesson-teacher[data-order='1'] .teacher").val(),
    //        AuditoriumId : $("#first-discipline .lesson-teacher[data-order='1'] .auditorium").val(),
    //        DisciplineId: $('#first-discipline .discipline-id').val(),
    //        IsNotActive: 'false'
    //    });
    //    var lessonPart2 = new LessonPartViewModel({
    //        LessonId: '0',
    //        LessonTypeId: '1',
    //        TeacherId: $("#first-discipline .lesson-teacher[data-order='2'] .teacher").val(),
    //        AuditoriumId: $("#first-discipline .lesson-teacher[data-order='2'] .auditorium").val(),
    //        DisciplineId: $('#first-discipline .discipline-id').val(),
    //        IsNotActive: 'false'
    //    });
    //    var lessonPart3 = new LessonPartViewModel({
    //        LessonId: '0',
    //        LessonTypeId: '1',
    //        TeacherId: $("#second-discipline .lesson-teacher[data-order='1'] .teacher").val(),
    //        AuditoriumId: $("#second-discipline .lesson-teacher[data-order='1'] .auditorium").val(),
    //        DisciplineId: $('#second-discipline .discipline-id').val(),
    //        IsNotActive: 'false'
    //    });
    //    var lessonPart4 = new LessonPartViewModel({
    //        LessonId: '0',
    //        LessonTypeId: '1',
    //        TeacherId: $("#second-discipline .lesson-teacher[data-order='2'] .teacher").val(),
    //        AuditoriumId: $("#second-discipline .lesson-teacher[data-order='2'] .auditorium").val(),
    //        DisciplineId: $('#second-discipline .discipline-id').val(),
    //        IsNotActive: 'false'
    //    });

    //    var editViewModel = new EditLessonViewModel({
    //        GroupId: $('#edit-lesson .group-id').val(),
    //        WeekNumber: $('#edit-lesson .week-number').val(),
    //        DayNumber: $('#edit-lesson .day-number').val(),
    //        ClassNumber: $('#edit-lesson .class-number').val(),
    //        Lessons: [lessonPart1, lessonPart2, lessonPart3, lessonPart4]
    //    });

    //    $.ajax({
    //        type: "POST",
    //        url: "/Home/Edit",
    //        data: editViewModel,
    //        success: function (result) {
    //            $("#edit-lesson").modal('hide');
    //        },
    //        error: function () {
    //            alert("failure");
    //        }
    //    });

    //});
});

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