$(document).ready(function () {

    $.validator.addMethod("items_required", function (value, element) {
        if (inProgress("PPDSection") || inProgress("questionaireSection") || inProgress("BMATSection") || inProgress("xRaySection")) {
            return true;
        }
        else {
            return false;

        }
    }, "PPD Step 1, Chest X-Ray, Questionnaire or BMAT must be completed.");

    $.validator.addMethod("PPD1_ReadDateValidation", function (value, element) {
        var givenDate = $("#VaccineDate").val();
        return this.optional(element) || checkDayDifference(value, givenDate, 2, 4);
    }, "Read Date must be 2-4 days after the given date");
    $.validator.addMethod("PPD2_ReadDateValidation", function (value, element) {
        var givenDate = $("#PPDStep2GivenDate").val();
        return this.optional(element) || checkDayDifference(value, givenDate, 2, 4);
    }, "Read Date must be 2-4 days after the given date");

    $.validator.setDefaults({ ignore: [] });
    $("#saveTB2StepVaccine").removeData("validator");
    $("#saveTB2StepVaccine").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#saveTB2StepVaccine"));
    $("#saveTB2StepVaccine").validate();

    toggleField("#PPDStep1ReactionFlg", "#PPDStep1ReactionMeasurementId", true);
    toggleField("#PPDStep2ReactionFlg", "#PPDStep2ReactionMeasurementId", true);
    toggleField("#BMATFlg", "#BMATDate");

    toggleField("#QuestionnaireCompleteFlg", "#QuestionnaireCompleteDate");

    $("#PPDStep1ReactionFlg").on("change", function () {
        toggleField("#PPDStep1ReactionFlg", "#PPDStep1ReactionMeasurementId", true);
    });
    $("#PPDStep2ReactionFlg").on("change", function () {
        toggleField("#PPDStep2ReactionFlg", "#PPDStep2ReactionMeasurementId", true);
    });
    $("#QuestionnaireCompleteFlg").on("change", function () {
        toggleField("#QuestionnaireCompleteFlg", "#QuestionnaireCompleteDate");
    });
    $("#BMATFlg").on("change", function () {
        toggleField("#BMATFlg", "#BMATDate");
    });
    var value = $("input[id='ChestXRayFlg']:checked").val();
    if (value == "true") {
        showField("#ChestXRayDate", true);
        hideField("#QuestionnaireCompleteFlg", true);
        $("#QuestionnaireCompleteFlg").change();
    }
    else if (value == "false") {
        hideField("#ChestXRayDate", true);
        showField("#QuestionnaireCompleteFlg", true);
        $("#ChestXRayDate").change();
    }
    else {
        hideField("#ChestXRayDate", true);
        hideField("#QuestionnaireCompleteFlg", true);
        $("#QuestionnaireCompleteFlg").change();
        $("#ChestXRayDate").change();
    }
    $("input[id='ChestXRayFlg']").on("change", function () {
        var value = $(this).val();
        if (value == "true") {
            showField("#ChestXRayDate", true);
            hideField("#QuestionnaireCompleteFlg", true);
            $("#QuestionnaireCompleteFlg").change();
        }
        else if (value == "false") {
            hideField("#ChestXRayDate", true);
            showField("#QuestionnaireCompleteFlg", true);
            $("#ChestXRayDate").change();
        }
        else {
            hideField("#ChestXRayDate", true);
            hideField("#QuestionnaireCompleteFlg", true);
            $("#QuestionnaireCompleteFlg").change();
            $("#ChestXRayDate").change();
        }
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
    if (inProgress("PPDSection")) {
        $("#VaccineDate").rules("add", "required");
        $("#PPDStep1SiteId").rules("add", "required");

    } else {
        $("#VaccineDate").rules("remove", "required");
        $("#VaccineDate").removeClass("input-validation-error");
        $("#VaccineDate" + "-error").remove();
        $("#PPDStep1SiteId").rules("remove", "required");
        $("#PPDStep1SiteId").removeClass("input-validation-error");
        $("#PPDStep1SiteId" + "-error").remove();

    }
    if (inProgress("PPD2Section")) {
        $("#PPDStep2GivenDate").rules("add", "required");
        $("#PPDStep2SiteId").rules("add", "required");

    } else {
        $("#PPDStep2GivenDate").rules("remove", "required");
        $("#PPDStep2GivenDate").removeClass("input-validation-error");
        $("#PPDStep2GivenDate" + "-error").remove();
        $("#PPDStep2SiteId").rules("remove", "required");
        $("#PPDStep2SiteId").removeClass("input-validation-error");
        $("#PPDStep2SiteId" + "-error").remove();

    }
    var form = $("#saveTB2StepVaccine");
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

function hideField(fieldSelector, isRow) {
    $(fieldSelector).hide();
    $(fieldSelector).rules("remove", "required");
    $(fieldSelector).removeClass("input-validation-error");
    $(fieldSelector + "-error").remove();
    if (isRow) {
        $(fieldSelector).closest(".row").hide();
    }

}

function showField(fieldSelector, isRow) {
    $(fieldSelector).show();
    $(fieldSelector).rules("add", "required");
    if (isRow) {
        $(fieldSelector).closest(".row").show();
    }

}

function toggleField(checkboxSelector, fieldToggleSelector, isRow) {
    if ($(checkboxSelector).is(":checked")) {
        showField(fieldToggleSelector, isRow);
    }
    else {
        hideField(fieldToggleSelector, isRow);

    }

}

function checkDayDifference(day1, day2, minDiff, maxDiff) {
    var readDate = getMomentDate(day1);
    var givenDate = getMomentDate(day2);
    if (checkDateTime(readDate) != "ok" && checkDateTime(givenDate) != "ok") {
        return false;
    }
    var difference = readDate.diff(givenDate, "days");
    if (difference < minDiff || difference > maxDiff) {
        return false;

    }
    return true;

}