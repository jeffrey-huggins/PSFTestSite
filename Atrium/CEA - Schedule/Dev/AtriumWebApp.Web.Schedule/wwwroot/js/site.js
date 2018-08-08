// Write your Javascript code.
$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxStop(function () {
    HideProgress();
});