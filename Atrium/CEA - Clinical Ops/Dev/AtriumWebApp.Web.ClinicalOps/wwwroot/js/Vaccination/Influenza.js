$(document).ready(function () {
    $.validator.setDefaults({ ignore: [] });
    $("#saveFluVaccine").removeData("validator");
    $("#saveFluVaccine").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#saveFluVaccine"));
    $("#saveFluVaccine").validate();

    if ($("#OfferedRefusedFlg").is(":checked")) {
        $(".refusedDate").show();
        $("#OfferedRefusedDate").rules("add", "required");
        $("#VaccineDate").rules("remove", "required");
    }
    else {
        $(".refusedDate").hide();
        $(".refusedDate").val("");
        $("#OfferedRefusedDate").rules("remove", "required");
        $("#VaccineDate").rules("add", "required");
    }

    $("#OfferedRefusedFlg").on("change", function () {
        if ($(this).is(":checked")) {
            $(".refusedDate").show();
            $("#OfferedRefusedDate").rules("add", "required");
            $("#VaccineDate").rules("remove", "required");
            $("#VaccineDate-error").remove();
        }
        else {
            $(".refusedDate").hide();
            $(".refusedDate").val("");
            $("#OfferedRefusedDate").rules("remove", "required");
            $("#VaccineDate").rules("add", "required");
            $("#VaccineDate-error").remove();
            $("#OfferedRefusedDate-error").remove();
            $("#OfferedRefusedDate").valid();
        }
        $("#VaccineDate").valid();
    });
    $(this).find(".isDate").datepicker({
        onSelect: function () {
            $(this).valid();
            $("#saveVaccine").focus();
        }
    });
});

$("#saveVaccine").on("click", function (e) {
    e.preventDefault();
    var form = $("#saveFluVaccine");
    if (!form.valid()) {
        return;

    }
    form.ajaxSubmit({
        success: function (result) {
            if (result.success) {
                $("#modal").dialog("close");
                updateTable();
            }
            else {
                alert("Error: Vaccine not found in database");
            }

        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });
})