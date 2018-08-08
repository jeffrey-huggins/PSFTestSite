$(document).ready(function () {

    $.validator.setDefaults({ ignore: [] });
    $("#saveTBAnnualVaccine").removeData("validator");
    $("#saveTBAnnualVaccine").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#saveTBAnnualVaccine"));
    $("#saveTBAnnualVaccine").validate();

    toggleField("#PPDReactionFlg", "#PPDReactionMeasurementId", true);
    toggleField("#QuestionnaireCompleteFlg", "#QuestionnaireCompleteDate");
    toggleField("#BMATFlg", "#BMATDate");
    $("#PPDReactionFlg").on("change", function () {
        toggleField("#PPDReactionFlg", "#PPDReactionMeasurementId", true);
    });
    $("#QuestionnaireCompleteFlg").on("change", function () {
        toggleField("#QuestionnaireCompleteFlg", "#QuestionnaireCompleteDate");
    });
    $("#BMATFlg").on("change", function () {
        toggleField("#BMATFlg", "#BMATDate");
    });
    $(this).parent().focus();
    $(".isDate").datepicker({
        onSelect: function () {
            $(this).valid();
            $("#saveVaccine").focus();
        }
    });

});

$("#saveVaccine").on("click", function (e) {
    e.preventDefault();
    if (inProgress("PPDSection")) {
        $("#VaccineDate").rules("add", "required");
        $("#PPDSiteId").rules("add", "required");

    } else {
        $("#VaccineDate").rules("remove", "required");
        $("#VaccineDate").removeClass("input-validation-error");
        $("#VaccineDate" + "-error").remove();
        $("#PPDSiteId").rules("remove", "required");
        $("#PPDSiteId").removeClass("input-validation-error");
        $("#PPDSiteId" + "-error").remove();

    }
    var form = $("#saveTBAnnualVaccine");
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

function toggleField(checkboxSelector, fieldToggleSelector, isRow) {
    if ($(checkboxSelector).is(":checked")) {
        $(fieldToggleSelector).show();
        $(fieldToggleSelector).rules("add", "required");
        if (isRow) {
            $(fieldToggleSelector).closest(".row").show();
        }
    }
    else {
        $(fieldToggleSelector).hide();
        $(fieldToggleSelector).rules("remove", "required");
        $(fieldToggleSelector).removeClass("input-validation-error");
        $(fieldToggleSelector + "-error").remove();
        if (isRow) {
            $(fieldToggleSelector).closest(".row").hide();
        }

    }

}