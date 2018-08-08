$(document).ready(function () {
    $.validator.addMethod("unknwon_prior_to_admission_required", function (value, element) {
        var valid = true;
        if (value == "1" && !$("#ImmunizationPriorToAdmissionFlg").is(":checked")) {
            valid = false;
        }

        return (this.optional(element) || valid);
    }, "Vaccine type can only be unknown if it was prior to admission.");

    $.validator.setDefaults({ ignore: [] });
    $("#savePneumoniaVaccine").removeData("validator");
    $("#savePneumoniaVaccine").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#savePneumoniaVaccine"));
    $("#savePneumoniaVaccine").validate();

    if ($("#ImmunizationPriorToAdmissionFlg").is(":checked")) {
        $("#VaccineDate").rules("remove", "required");

    }
    else {
        $("#VaccineDate").rules("add", "required");
    }
    setDateRequired();

    if ($("#OfferedRefusedFlg").is(":checked")) {
        $(".refusedDate").show();
        $("#OfferedRefusedDate").rules("add", "required");
    }
    else {
        $(".refusedDate").hide();
        $(".refusedDate").val("");
        $("#OfferedRefusedDate").rules("remove", "required");
    }

    $("#OfferedRefusedFlg").on("change", function () {
        if ($(this).is(":checked")) {
            $(".refusedDate").show();
            $("#OfferedRefusedDate").rules("add", "required");
        }
        else {
            $(".refusedDate").hide();
            $(".refusedDate").val("");
            $("#OfferedRefusedDate").rules("remove", "required");
            $("#OfferedRefusedDate-error").remove();
            $("#OfferedRefusedDate").valid();
        }
        setDateRequired();
    });
    $(".isDate").datepicker({
        onSelect: function () {
            $(this).valid();
            $("#saveVaccine").focus();
        }
    });
});

$("#saveVaccine").on("click", function (e) {
    e.preventDefault();
    var form = $("#savePneumoniaVaccine");
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
});

function setDateRequired() {
    if ($("#OfferedRefusedFlg").is(":checked") || $("#ImmunizationPriorToAdmissionFlg").is(":checked")) {
        $("#VaccineDate").rules("remove", "required");
    }
    else {
        $("#VaccineDate").rules("add", "required");
    }
    $("#VaccineDate").removeClass("input-validation-error");
    $("#VaccineDate-error").remove();
}
