$(function () {
    $.ajax({
        url: "/Dictionary/EducationYear",
        type: "POST",
        // data: JSON.stringify({ yearId: yearId }),
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            var years = "";
            $.each(data, function (index, year) {
                if (year.IsSelectedYear) {
                    $("#account-year").html("<i class='fa fa-calendar fa-32px'></i><br><span class='nav-icon-text'></span>" + year.EducationYearName + " Учебный год" /*<span class='caret'></span>"*/);
                } else {
                    years += "<li><a href='/Account/ChangeYear/" + year.EducationYearId + "'>" + year.EducationYearName + "</a></li>";
                }
            });

            $("#years").append(years);
        }
    });
});