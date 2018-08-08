
var _countDowncontainer = 0;
var _currentSeconds = 0;

function ActivateCountDown(strContainerID, initialValue) {
    _countDowncontainer = document.getElementById(strContainerID);

    if (!_countDowncontainer) {
        alert("count down error: container does not exist: " + strContainerID +
			"\nmake sure html element with this ID exists");
        return;
    }

    SetCountdownText(initialValue);
    window.setTimeout("CountDownTick()", 1000);
}

function CountDownTick() {
    if (_currentSeconds <= 0) {
        var modal = $('<div />');
        modal.addClass("modal_black");
        $('body').append(modal);
        var timeoutDiv = $(".timeout");
        timeoutDiv.show();
        var top = Math.max($(window).height() / 2 - timeoutDiv[0].offsetHeight / 2, 0);
        var left = Math.max($(window).width() / 2 - timeoutDiv[0].offsetWidth / 2, 0);
        timeoutDiv.css({ top: top, left: left });
        return;
    }

    SetCountdownText(_currentSeconds - 1);
    window.setTimeout("CountDownTick()", 1000);
}

function SetCountdownText(seconds) {
    //store:
    _currentSeconds = seconds;

    //get minutes:
    var minutes = parseInt(seconds / 60);

    //shrink:
    seconds = (seconds % 60);

    //get hours:
    //var hours = parseInt(minutes / 60);

    //shrink:
    minutes = (minutes % 60);

    //build text:
    var strText = AddZero(minutes) + ":" + AddZero(seconds);

    //apply:
    _countDowncontainer.innerHTML = strText;
}

function AddZero(num) {
    return ((num >= 0) && (num < 10)) ? "0" + num : num + "";
}

function SetCountDownTime(seconds) {
    _currentSeconds = seconds;
}

$(document).ajaxComplete(function () {
    var countdownTime = $("#SessionTimeout").val();
    SetCountdownText(countdownTime);
});