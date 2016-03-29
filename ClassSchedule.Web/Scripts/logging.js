// Ограничим количество ошибок, которые могут насыпаться с одной страницы
var errorsLimit = 8;

// Универсальный обработчик ошибок, отправляет лог на сервер, где мы всё сможем видеть.
window.onerror = function (message, url, line, charPos, error) {
    if (!errorsLimit) {
        return;
    }
    errorsLimit--;

    var data = {
        "Message": message,
        "Url": url,
        "Line": line
    };

    if (charPos != undefined) {
        data["CharPos"] = charPos;
    }
    if (error != undefined && error.stack) {
        data["Stack"] = error.stack;
    }

    var errurl = "/Error/LogClientError";
    //$.post(errurl, JSON.stringify(data));

    $.ajax({
        type: 'POST',
        url: errurl,
        data: JSON.stringify(data),
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        processData: false
    });
};