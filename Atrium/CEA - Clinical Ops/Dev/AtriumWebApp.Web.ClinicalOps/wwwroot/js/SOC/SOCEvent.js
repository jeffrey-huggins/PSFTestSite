$(document).ready(function () {
    var eventId = $("#SOCEventId").val();

    if (eventId == "0") {
        $("#OccurredDate").val("");
    }
    $(".isDate").datepicker().on("change", function (ev) {
        $(this).valid();
    });
    $("#SaveGenericForm").show();
});
$("#saveEvent").on("click", function (e) {
    e.preventDefault();
    var form = $("#SaveGenericForm");
    if (!form.valid()) {
        return;

    }
    form.ajaxSubmit({
        dataType: "json",
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
$("#SaveGenericForm").removeData("validator");
$("#SaveGenericForm").removeData("unobtrusiveValidation");
$.validator.unobtrusive.parse($("#SaveGenericForm"));
$("#SaveGenericForm").validate();