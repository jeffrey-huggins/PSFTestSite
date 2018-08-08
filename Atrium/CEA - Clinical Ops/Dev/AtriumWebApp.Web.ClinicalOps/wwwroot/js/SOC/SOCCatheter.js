$(document).ready(function () {
    var eventId = $("#SOCEventId").val();

    if (eventId == "0") {
        $("#OccurredDate").val("");
    }
    else {
        $("#SOCCatheterTypeId").attr("disabled", true);

    }
    $(".isDate").datepicker({
        onSelect: function () {
            //ie fix for date picker re-opening after selection in modal windows
            $(this).parent().focus();
        }}).on("change", function (ev) {
        $(this).valid();
        });
    $("#SaveCatheterForm").show();
});
$("#saveEvent").on("click", function (e) {
    e.preventDefault();
    var form = $("#SaveCatheterForm");
    if (!form.valid()) {
        return;

    }
    form.ajaxSubmit({
        iframe: true,
        dataType: 'json',
        success: function (result) {
            if (result.Success) {
                $("#modal").dialog("close");
                updateTable();
            }
            else {
                alert("Error: Event not found in database");
            }

        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });
});


$.validator.setDefaults({ ignore: [] });
$("#SaveCatheterForm").removeData("validator");
$("#SaveCatheterForm").removeData("unobtrusiveValidation");
$.validator.unobtrusive.parse($("#SaveCatheterForm"));
$("#SaveCatheterForm").validate();