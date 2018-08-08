
var ajaxUri;
if (document.location.pathname.charAt(document.location.pathname.length - 1) != "/") {
    ajaxUri = document.location.pathname + "/";
} else {
    ajaxUri = document.location.pathname
}

 

function PopulateWoundNoteFromExisting(isNew, nRowId) {
    $.ajax({
        url: ajaxUri + "GetRowWound",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: { rowId: nRowId },
        success: function (result) {
            if (isNew) {
                $("#WoundEditingId").val("");
                $("#DateNoted").val("");
                $("#Stages").attr("disabled", null);
            }
            else {
                $("#WoundEditingId").val(nRowId);
                $("#DateNoted").val(result.NotedDate);
                //Do we disable Status here?
                $("#Stages").attr("disabled", "disabled");
            }
            $("#Stages").val(result.PressureWoundStageId);
            $("#Treatment").val(result.Treatment);
            $("#Measurements").val(result.Measurement);
            $("#ReliefDevice").val(result.ReliefDevice);
            $("#Status").val(result.Status);
            $("#LabsDiagnostic").val(result.LabDiagnostic);
            if (result.Drainage) {
                $("#DrainageCheckBox").prop("checked", true);
                $("#Drainage").removeClass("hidden");
                $("#Drainage").val(result.DrainageDesc);
            } else {
                $("#DrainageCheckBox").prop("checked", false);
                $("#Drainage").addClass("hidden");
                $("#Drainage").val("");
            }
            $("#Intervention").val(result.Intervention);
            $("#Dietary").val(result.DietaryDate);
            $("#Physician").val(result.PhysicianDate);
            $("#Family").val(result.FamilyDate);
            $("#IdealBodyWeight").val(result.IdealBodyWeight);
            $("#ActualWeight").val(result.ActualWeight);
            $("#FoodIntake").val(result.FoodIntake);
            $("#SkinTurgor").val(result.SkinTurgor);
            $("#Urine").val(result.Urine);
            $("#PainLevel").val(result.PainLevel);
            $("#Progress").val(result.Progress);
            $("#Signature").val(result.Signature);
            $("#PWLength").val(result.LengthNbr);
            $("#PWWidth").val(result.WidthNbr);
            if (result.DepthFlg == true) {
                $("#PWDepth").val(result.DepthNbr);
                $(".depth-field").removeClass("hidden");
            } else {
                $("#PWDepth").val(null);
                $(".depth-field").addClass("hidden");
            }
            if (isNew && !result.Measurement) {
                $("#PWDepth").val("");
                $("#PWLength").val("");
                $("#PWWidth").val("");
                $("#Drainage").val("");
                $("#LabsDiagnostic").val("");
                $("#DrainageCheckBox").prop("checked", false);
                $("#Drainage").addClass("hidden");
                $("#Treatment").val("");
            }
            $('.show-note', '#wound').click();
            SetCountDownTime(1800);
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}

function SetupFallModalDataTable(selector) {
    var dataTable = $(selector).dataTable({
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "bVisible": false
            },
            null,
            {
                "bSortable": false
            }
        ]
    });
    return dataTable;
}

function PopulateAntiNoteFromExisting(isNew, nRow, nRowId) {
    //var aData = oTableAnti.fnGetData(nRow);
    $.ajax({
        url: ajaxUri + "GetRowAnti",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId
        },
        success: function (result) {
            if (isNew) {
                $("#AntiEditingId, #DateAntipsychotic").val("");
            }
            else {
                $("#AntiEditingId").val(nRowId);
                $("#DateAntipsychotic").val(result.NotedDate);
            }
            $("#ReducedAnti").prop("checked", result.ReducedFlg);
            $("#DateReducedAnti").val(result.ReducedDate);
            $("#ReviewedAnti").prop("checked", result.ReviewedFlg);
            //$("#DateReviewedAnti").val(result.ReviewedDate);
            $("#QrtrlyAssessment").prop("checked", result.QrtrlyAssessmentFlg);
            $("#Recommendation").val(result.RecommendationDesc);
            if (result.ReducedFlg) {
                $("#DateReducedAnti").removeClass("hidden");
            } else {
                $("#DateReducedAnti").addClass("hidden");
            }
            //if (result.ReviewedFlg) {
            //    $("#DateReviewedAnti, #reducedShow").removeClass("hidden");
            //} else {
            //    $("#DateReviewedAnti, #reducedShow").addClass("hidden");
            //}
            if ($("#SOCMeasure").val() == 11) {
                $("#DateAIMS").val(result.LastAimsTestDate);
                $("#TargetedBehavior").val(result.TargetedBehaviors);


                $("#DateOccurredLabel").prop("textContent", "Date Initial Order:");
                $("#ReducedLabel").prop("textContent", "GDR:");
                $("#ReviewedLabel").prop("textContent", "IDT Reviewed:");
                $("#ReviewedAnti").addClass("hidden");
                //$("#DateReviewedAnti").removeClass("hidden");
                $("#reducedShow").removeClass("hidden");
                $("#ReducedAnti").addClass("hidden");
                $("#DateReducedAnti").removeClass("hidden");
                $(".with-diagnosis-only").removeClass("hidden");
            }
            $('.show-note', '#antipsychotic').click();
            SetCountDownTime(1800);
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
    //$("#AntiEditingId").val(nRowId);
    //$("#DateAntipsychotic").val(aData[0]);
    //$("#Recommendation").val(aData[1]);
}

function PopulateRestraintNoteFromExisting(isNew, nRowId) {
    $.ajax({
        url: ajaxUri + "GetRowRestraint",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: { rowId: nRowId },
        success: function (result) {
            if (isNew) {
                $("#RestraintEditingId").val("");
                $("#DateReduced").val("");
            }
            else {
                $("#RestraintEditingId").val(nRowId);
                $("#DateReduced").val(result.ReducedDate);
            }
            $("#RestraintTypes").val(result.SOCRestraintId);
            if (result.DiagnosisSupportsRestraintFlg) {
                $("#DiagnosisRestraint").prop("checked", true);
                $("#DiagnosisRestraintDesc").removeClass("hidden");
                $("#DiagnosisRestraintDesc").val(result.DiagnosisSupportsRestraintDesc);
            } else {
                $("#DiagnosisRestraint").prop("checked", false);
                $("#DiagnosisRestraintDesc").addClass("hidden");
                $("#DiagnosisRestraintDesc").val("");
            }
            $("#RestraintComments").val(result.Comments);
            $('.show-note', '#restraint').click();
            SetCountDownTime(1800);
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}

function DeleteNote(oTable, url, idSelector, clearButtonSelector, button) {
    var totalRows = oTable.fnSettings().fnRecordsDisplay();
    if (totalRows == 1) {
        alert("Cannot delete this row. One Noted Record must remain for each Standards of Care entry.");
        return;
    }
    var delConfirm = confirm("Are you sure you want to delete?");
    if (delConfirm == true) {
        var nRow = $(button).parents('tr')[0];
        var nRowId = $(button).closest('tr').attr("id");
        $.ajax({
            url: url,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { rowId: nRowId },
            success: function (result) {
                if (result.Success) {
                    oTable.fnDeleteRow(nRow);
                    if ($(idSelector).val() == nRowId) {
                        $(clearButtonSelector).click();
                    }
                }
                else {
                    alert("Error: Event not found in database");
                }
                SetCountDownTime(1800);
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    }
}

function InputMeasure(subAppCode, contentSelector) {
    var modal = $('<div />');
    modal.addClass("modal");
    $('body').append(modal);
    var content = $(contentSelector);
    $("#SubAppCode").val(subAppCode);
    $('.date-occurred-container', content).append($("#DateOccurred"));
    $('.date-resolved-container', content).append($("#DateResolved"));
    content.show();
    var top = Math.max($(window).height() / 2 - content[0].offsetHeight / 2, 0);
    var left = Math.max($(window).width() / 2 - content[0].offsetWidth / 2, 0);
    content.css({ top: top, left: left });
}

function UpdateTableWithSaved(table, result) {
    var rowData = [
        result.SOCMeasureName,
        result.OccurredDate,
        result.OtherInformation,
        result.ResolvedDate,
        "<a class=\"edit\" href=\"\">Edit</a>",
        "<a class=\"delete\" href=\"\">Delete</a>"
    ];
    var editingId = $("#EditingId").val();
    if (editingId != "") {
        var element = document.getElementById(editingId);
        if (element != null) {
            var rowNumber = table.fnGetPosition(element);
            table.fnUpdate(rowData, rowNumber);
            element.setAttribute("id", result.SOCEventId);
        }
    }
    else {
        var newEntry = table.fnAddData(rowData);
        var tr = table.fnSettings().aoData[newEntry[0]].nTr;
        tr.setAttribute("id", result.SOCEventId);
    }
    $("#EditingId").val(result.SOCEventId);
}

function SaveMeasure(form, url, table, successCallback, failureCallback) {
    form.prepend($("#MeasureId"));
    form.prepend($("#SubAppCode"));
    form.prepend($("#EditingId"));
    $.ajax({
        url: url,
        dataType: "json",
        cache: false,
        type: 'POST',
        data: form.serialize(), //FormData doesn't work with Internet Explorer
        success: function (result) {
            if (!result.Success) {
                if (failureCallback) {
                    failureCallback(result);
                }
                SetCountDownTime(1800);
                return;
            }
            UpdateTableWithSaved(table, result);
            if (successCallback) {
                successCallback(result);
            }
            SetCountDownTime(1800);
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload the page and try again");
        }
    });
}

//Id, Display Name
var GeneralBoxesToValidate = [["SOCEventWound_Site", "Site"], ["Treatment", "Treatment"], ["Drainage", "Drainage"], ["Intervention", "Intervention"]];
var PressureBoxesToValidate = [["Stages", "Stage"], ["ReliefDevice", "Relief Device"], ["Status", "Status"], ["PWLength", "Length"], ["PWWidth", "Width"]];
var CompositeBoxesToValidate = [["SOCEventWound_CompositeWoundDescribeId", "Wound Description"], ["Measurements", "Measurements"]];

function MeasureFormIsValid() {
    if (!DateOccurredIsValid() || !DateResolvedIsValid()) {
        return false;
    }
    return true;
}

var myStages = JSON.parse($("#stagesData").val());
function GetSelectedStageData() {
    var selectedValue = $("#Stages").find("option:selected").val();
    $("#SelectedStage").val(selectedValue);
    for (var x = 0; x < myStages.length; x++) {
        var stage = myStages[x];
        if (stage.PressureWoundStageId == selectedValue) {
            return stage;
        }
    }
}

function ValidateWoundMeasurement(field, fieldName) {
    var fieldValue = field.val();
    if (isNaN(fieldValue)) {
        alert(fieldName + " must be a number.");
        return false;
    }

    if (fieldValue < 0.0) {
        alert(fieldName + " cannot be below 0.0");
        field.val(0.0);
        return false;
    } else if (fieldValue > 99.9) {
        alert(fieldName + " cannot be above 99.9");
        field.val(99.9);
        return false;
    } else {
        return true;
    }
}

function WoundFormIsValid() {
    if (!DateOccurredIsValid() || !DateResolvedIsValid()) {
        return false;
    }
    var requiredMessage = " is a required field.";
    var invalidMessage = " must be in the format mm/dd/yyyy.";

    //Validate Unavoidable (if checked, file must be attached)
    if ($("#SOCEventWound_UnavoidableFlg").is(":checked") == true) {
        if ($("#wound-document")[0] == undefined) {
            if ($(".file-link")[0] == undefined) {
                return false;
            }
        } else {
            if ($("#wound-document").val().length == 0) {
                alert("If the wound is Unavoidable, please attach document.");
                return false;
            }
        }
    }
    if ($('.IncludeNote', '#wound').val() === "true") {
        var dateNoted = $("#DateNoted").val();
        if (dateNoted == "") {
            alert("Date Noted" + requiredMessage);
            return false;
        }
        if (isInvalidDate(dateNoted)) {
            alert("Date Noted" + invalidMessage);
            return false;
        }
        if (IsDateFuture(dateNoted)) {
            alert("Date Noted cannot be in the future.");
            return false;
        }
        for (var i = 0; i < GeneralBoxesToValidate.length; i++) {
            var box = $("#" + GeneralBoxesToValidate[i][0]);
            box.removeClass("input-validation-error");
            var val = $("#" + GeneralBoxesToValidate[i][0]).val();
            if (val == "" && !box.hasClass("hidden")) {
                box.addClass("input-validation-error");
                alert(GeneralBoxesToValidate[i][1] + requiredMessage);
                return false;
            }
        }

        if ($("#SubAppCode").val() == "PRWNDPRIOR" || $("#SubAppCode").val() == "PRWNDAFTER") {
            for (var i = 0; i < PressureBoxesToValidate.length; i++) {
                var box = $("#" + PressureBoxesToValidate[i][0]);
                box.removeClass("input-validation-error");
                var val = $("#" + PressureBoxesToValidate[i][0]).val();
                if (val == "") {
                    box.addClass("input-validation-error");
                    alert(PressureBoxesToValidate[i][1] + requiredMessage);
                    return false;
                }
            }

            //if Stage == 2, 3, 4... validate Depth
            var selectedStage = GetSelectedStageData();
            if (selectedStage.LengthFlg == true) {
                if (!ValidateWoundMeasurement($("#PWLength"), "Length")) {
                    return false;
                }
            }
            if (selectedStage.WidthFlg == true) {
                if (!ValidateWoundMeasurement($("#PWWidth"), "Width")) {
                    return false;
                }
            }
            if (selectedStage.DepthFlg == true) {
                if (!ValidateWoundMeasurement($("#PWDepth"), "Depth")) {
                    return false;
                }
            }
            if (selectedStage.SeverityLevelNbr != null) {
                var highestSeverityLevel = $("#WoundHighestSeverityLevel").val();
                if (selectedStage.SeverityLevelNbr < highestSeverityLevel) {
                    alert("Stage cannot decrease in severity.");
                    return false;
                }
            }

            if (isInvalidDate($("#Dietary").val())) {
                alert("Dietary" + invalidMessage);
                return false;
            }
            if (isInvalidDate($("#Physician").val())) {
                alert("Physician" + invalidMessage);
                return false;
            }
            if (isInvalidDate($("#Family").val())) {
                alert("Family" + invalidMessage);
                return false;
            }
        }
        else {
            for (var i = 0; i < CompositeBoxesToValidate.length; i++) {
                var box = $("#" + CompositeBoxesToValidate[i][0]);
                box.removeClass("input-validation-error");
                var val = $("#" + CompositeBoxesToValidate[i][0]).val();
                if (val == "") {
                    box.addClass("input-validation-error");
                    alert(CompositeBoxesToValidate[i][1] + requiredMessage);
                    return false;
                }
            }
        }
    }
    return true;
}

function FallFormIsValid() {
    if (!DateOccurredIsValid()) {
        return false;
    }
    if (!ValidateHr($("#Hr").val())) {
        alert("Hr must be a number between 1 and 12");
        return false;
    }
    if (!ValidateMin($("#Min").val())) {
        alert("Min must be a number between 0 and 59");
        return false;
    }
    if ($("#FallLocations").val() == "") {
        alert("A Fall Location must be selected.");
        return false;
    }
    var submit = true;
    var interventions = $("#InterventionTable input:checked").length == 0;
    var fallTypes = $("#typeOfFall input:checked").length == 0;
    var injuryTypes = $("#typeOfInjury input:checked").length == 0;
    var treatments = $("#TreatmentTable input:checked").length == 0;

    if (interventions || fallTypes || injuryTypes || treatments) {
        alert("All tables are required. Please select at least 1 from each.");
        submit = false;
    }

    return submit;
}

function AntiPsychoticFormIsValid() {
    if (!DateOccurredIsValid() || !DateResolvedIsValid()) {
        return false;
    }
    if ($("#SOCEventAntiPsychotic_AntiPsychoticDiagnosisId option:selected").val() == "") {
        alert("Diagnosis is a required field.");
        return false;
    }
    if ($("#SOCEventAntiPsychotic_AntiPsychoticMedicationId option:selected").val() == "") {
        alert("Antipsychotic is a required field.");
        return false;
    }
    if ($('.IncludeNote', '#antipsychotic').val() === "true") {
        if ($("#SOCEventAntiPsychotic_Diagnosis").val() == "") {
            alert("Diagnosis is a required field.");
            return false;
        }
        var date = $("#DateAntipsychotic").val();
        if (date == "") {
            alert("Date is a required field.");
            return false;
        }
        if (isInvalidDate(date)) {
            alert("Date must be in the format mm/dd/yyyy.");
            return false;
        }
        var checked = $("#ReducedAnti").is(":checked");
        date = $("#DateReducedAnti").val();
        if (checked && date == "") {
            alert("Date Reduced is a required field.");
            return false;
        }
        if (checked && isInvalidDate(date)) {
            alert("Date Reduced must be in the format mm/dd/yyyy.");
            return false;
        }
        if ($("#Recommendation").val() == "") {
            alert("Recommendation is a required field.");
            return false;
        }
    }
    return true;
}

function CatheterFormIsValid() {
    if (!DateOccurredIsValid() || !DateResolvedIsValid()) {
        return false;
    }
    if ($("#SOCEventCatheter_SOCCatheterTypeId").val() == "") {
        alert("Catheter Type is a required field.");
        return false;
    }
    return true;
}

function RestraintFormIsValid() {
    if (!DateOccurredIsValid() || !DateResolvedIsValid()) {
        return false;
    }
    if ($('.IncludeNote', '#restraint').val() === "true") {
        if ($("#RestraintTypes").val() == "") {
            alert("Restraint Type is a required field.");
            return false;
        }
        var date = $("#DateReduced").val();
        //if (date == "") {
        //    alert("Reduced Date is a required field.");
        //    return false;
        //}
        if (isInvalidDate(date)) {
            alert("Reduced Date must be in the format mm/dd/yyyy.");
            return false;
        }
    }
    return true;
}

function DateOccurredIsValid() {
    var date = $("#DateOccurred").val();
    if (date == "") {
        alert("Occurred date is a required field.");
        return false;
    }
    if (isInvalidDate(date)) {
        alert("Occurred date must be in the format mm/dd/yyyy.");
        return false;
    }
    if (IsDateFuture(date)) {
        alert("Occurred date cannot be in the future.");
        return false;
    }
    return true;
}

function DateResolvedIsValid() {
    var date = $("#DateResolved").val();
    if (isInvalidDate(date)) {
        alert("Resolved date must be in the format mm/dd/yyyy.");
        return false;
    }
    if (IsDateFuture(date)) {
        alert("Resolved date cannot be in the future.");
        return false;
    }
    if (new Date($("#DateOccurred").val()) > new Date(date)) {
        alert("Resolved Date cannot occur before the Date Occurred.");
        return false;
    }
    return true;
}

function isInvalidDate(value) {
    return value != "" && !ValidateDate(value);
}

$(document).ready(function () {
    var ajaxBaseUri;
    if (document.location.pathname.charAt(document.location.pathname.length - 1) != "/") {
        ajaxBaseUri = document.location.pathname + "/";
    } else {
        ajaxBaseUri = document.location.pathname
    }

    var oTable = $('#window-table').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        //"sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '100px' },
            { "sWidth": '40px' },
            { "sWidth": '60px' },
            { "sWidth": '50px' },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '40px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '50px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No entries for date range selected"
        }
    });
    var nEditing = null;
    oTable.fnSort([[1, 'desc']]);

    $("#window-table_length").css("display", "none");

    $('#window-table').on('click', 'a.edit', function (e) {
        e.preventDefault();
        ShowProgress();
        var nRowId = $(this).closest('tr').attr("id");

        $.post(ajaxBaseUri + "ChangeCurrentSOCEvent", { rowId: nRowId }).done(function () {
            window.location.reload();
        });
    });
    $('#window-table').on('click', 'a.delete', function (e) {
        e.preventDefault();

        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            $.ajax({
                url: ajaxBaseUri + "DeleteRow",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: { rowId: nRowId },
                success: function (result) {
                    if (result.Success) {
                        oTable.fnDeleteRow(nRow);
                        SetCountDownTime(1800);
                    }
                    else {
                        alert("Error: Event not found in database");
                    }
                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
    });

    $(".show-note").click(function (e) {
        e.preventDefault();
        var button = $(this);
        var parent = button.parent();
        var note = $(".note", parent);
        button.hide();
        note.show();
        $('.IncludeNote', parent).val("true");
    });

    $(".dismiss-note").click(function (e) {
        e.preventDefault();
        var note = $(this).closest(".note");
        var parent = note.parent();
        var button = $(".show-note", parent);
        note.hide();
        button.show();
        $('.IncludeNote', parent).val("false");
    });

    var oTableWounds = $('#woundsTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        //"sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false
            },
            {
                "bSearchable": false,
                "bSortable": false
            },
            {
                "bSearchable": false,
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No wound notes recorded"
        }
    });
    oTableWounds.fnSort([[0, 'desc']]);

    $("#woundsTable").on('click', 'a.edit', function (e) {
        e.preventDefault();
        PopulateWoundNoteFromExisting(false, $(this).closest('tr').attr("id"));
    });

    $("#woundsTable").on('click', 'a.copy', function (e) {
        e.preventDefault();
        PopulateWoundNoteFromExisting(true, $(this).closest('tr').attr("id"));
    });

    $("#woundsTable").on('click', 'a.delete', function (e) {
        var oTableWounds = $('#woundsTable').dataTable();
        e.preventDefault();
        DeleteNote(oTableWounds, ajaxBaseUri + "DeleteRowWound", "#WoundEditingId", "#clearWound", this);
    });

    var oTableIntervention = SetupFallModalDataTable('#InterventionTable');
    oTableIntervention.fnSort([[0, 'asc']]);

    var oTableFallType = SetupFallModalDataTable('#typeOfFall');
    oTableFallType.fnSort([[0, 'asc']]);

    var oTableInjury = SetupFallModalDataTable('#typeOfInjury');
    oTableInjury.fnSort([[0, 'asc']]);

    var oTableTreatment = SetupFallModalDataTable('#TreatmentTable');
    oTableTreatment.fnSort([[0, 'asc']]);

    $(".checkbox").click(function () {
        var checkbox = $(this);
        var checked = checkbox.is(':checked');
        var nTable = checkbox.closest('table').attr("id");
        var nRow = checkbox.parents('tr')[0];
        var nRowId = checkbox.closest('tr').attr("id");
        alert("CHECKBOX");
        $.ajax({
            url: ajaxBaseUri + "CheckBoxSelected",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                table: nTable,
                flag: checked
            },
            success: function (result) {
                if (!result.Success && result.Reason == 0) {
                    checkbox.attr('checked', false);
                    if (nTable != "Diagnoses")
                        alert("Error: Can only have " + result.NumberOfSelections + " " + nTable.substring(0, nTable.length - 1) + "(s) selected.");
                    else {
                        alert("Error: Can only have " + result.NumberOfSelections + " Diagnosis selected.");
                    }
                }
                SetCountDownTime(1800);
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }

        });
    });

    var oTableAnti = $('#antiTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        //"sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false
            },
            {
                "bSearchable": false,
                "bSortable": false
            },
            {
                "bSearchable": false,
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No antipsychotic dates recorded"
        }
    });
    oTableAnti.fnSort([[0, 'desc']]);

    $("#antiTable").on('click', 'a.edit', function (e) {
        e.preventDefault();
        PopulateAntiNoteFromExisting(false, $(this).parents('tr')[0], $(this).closest('tr').attr("id"));
    });

    $("#antiTable").on('click', 'a.copy', function (e) {
        e.preventDefault();
        PopulateAntiNoteFromExisting(true, $(this).parents('tr')[0], $(this).closest('tr').attr("id"));
    });

    $("#antiTable").on('click', 'a.delete', function (e) {
        e.preventDefault();
        DeleteNote(oTableAnti, ajaxBaseUri + "DeleteRowAnti", "#AntiEditingId", "#clearAnti", this);
    });



    $("#antipsychotic").on("change", "#SOCEventAntiPsychotic_AntiPsychoticDiagnosisId", function (e) {
        AntiPsychoticDiagnosisCheck();
    });

    var oTableRestraint = $('#restTable').dataTable({
        "bFilter": false,
        //"sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            {
                "sWidth": "5px",
                "bSearchable": false,
                "bSortable": false
            },
            {
                "sWidth": "5px",
                "bSearchable": false,
                "bSortable": false
            },
            {
                "sWidth": "5px",
                "bSearchable": false,
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No restraint notes recorded"
        }
    });
    oTableRestraint.fnSort([[1, 'desc']]);

    $("#restTable").on('click', 'a.edit', function (e) {
        e.preventDefault();
        PopulateRestraintNoteFromExisting(false, $(this).closest('tr').attr("id"));
    });

    $("#restTable").on('click', "a.copy", function (e) {
        e.preventDefault();
        PopulateRestraintNoteFromExisting(true, $(this).closest('tr').attr("id"));
    });

    $("#restTable").on('click', 'a.delete', function (e) {
        e.preventDefault();
        DeleteNote(oTableRestraint, ajaxBaseUri + "DeleteRowRestraint", "#RestraintEditingId", "#clearRest", this);
    });

    $(function () {
        $("#DateOccurred, #DateResolved").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#occurredRangeTo, #occurredRangeFrom, .isDate").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: '0px',
                    marginLeft: '0px'
                });
            }
        });
    });

    $(function () {
        if ($("#ToDateRangeInvalid").val() == "1") {
            alert('Error: Please enter a valid date in the Occurred Date Range "To" Field (mm/dd/yyyy)');
        }
        if ($("#FromDateRangeInvalid").val() == "1") {
            alert('Error: Please enter a valid date in the Occurred Date Range "From" Field (mm/dd/yyyy)');
        }
        if ($("#ToDateRangeInFuture").val() == "1") {
            alert('Error: Please enter a valid date that is not in the future in the Occurred Date Range "To" Field');
        }
        if ($("#FromDateRangeInFuture").val() == "1") {
            alert('Error: Please enter a valid date that is not in the future in the Occurred Date Range "From" Field');
        }
        if ($("#FromAfterTo").val() == "1") {
            alert('Error: You can not have the "From" Date after the "To" Date in the Occurred Date Range Fields');
        }
    });

    //Measure
    $("#SOCMeasure").change(function () {
        var value = $("#SOCMeasure :selected").val();
        $('#MeasureId').val(value);
    });

    


    $("#saveMeasure").click(function () {
        var form = $("#SaveEvent");
        if (form.valid() && MeasureFormIsValid()) {
            SaveMeasure(form, ajaxBaseUri + "SaveMeasure", oTable, function (result) {
                alert("Record has been successfully saved.");
                $("#closeMeasure").click();
            });
        }
    });

    $("#closeMeasure").click(function () {
        $(".modal, #event").hide();
    });

    $("#createSOC").click(function () {
        var oTableAnti = $('#antiTable').dataTable();
        var oTableWounds = $('#woundsTable').dataTable();
        $('#EditingId, #DateOccurred, #DateResolved').val('');
        oTableWounds.fnClearTable();
        $("#SOCEventWound_Site, #SOCEventWound_CompositeWoundDescribeId, #SOCEventCatheter_SOCCatheterTypeId, #SOCEventAntiPsychotic_AntiPsychoticMedicationId, #OtherDescriptionRow, #SOCEventWound_AdmittedWithFlg, #admitted").attr("disabled", null).val('');
        $("#SOCEventWound_UnavoidableFlg, #SOCEventWound_AffectedByOther, #AdmittedWithFlg").prop("checked", false);
        $("#SOCEventWound_AffectedByDiabetes, #SOCEventWound_AffectedByIncontinence, #SOCEventWound_AffectedByParalysis, #SOCEventWound_AffectedBySepsis, #SOCEventWound_AffectedByEndStageDisease").prop('checked', false);
        $("#AdmittedWithFlg").attr("disabled", null);
        $("#WoundHighestSeverityLevel").val("");
        $("#clearWound").click();
        $("#clearFall").click();
        oTableRestraint.fnClearTable();
        $("#DateAntipsychotic, #Recommendation, #SOCEventAntiPsychotic_Diagnosis, #ConsentDateAnti").val('');
        $("#SOCEventAntiPsychotic_Diagnosis").attr("readonly", null);
        $("#SOCEventAntiPsychotic_Diagnosis").css("background-color", "");
        oTableAnti.fnClearTable();
        $(".dismiss-note, .show-note").hide();
        $(".note").show();
        $('.IncludeNote').val("true");
        InputEvent();
    });

    //Wound
    $("#saveWound").click(function () {
        var form = $("#SaveWound");
        if (form.valid() && WoundFormIsValid()) {
            //can't upload an input file via AJAX on IE

            form.prepend($("#MeasureId"));
            form.prepend($("#SubAppCode"));
            form.prepend($("#EditingId"));
            ShowProgress();
            form.ajaxSubmit({
                success: function (data, textStatus, jqXHR, $form) {
                    try {
                        if (data.indexOf("<pre>") == 0) {
                            data = data.substring(5, data.length - 6);
                        }
                        var result = $.parseJSON(data);
                        if (result.Success) {
                            window.location.replace(path + "StandardsOfCare/");
                        }
                        else {
                            if (result.Reason == 0) {
                                alert("A note for this day already exists.  Edit the currently existing note.");
                                window.location.replace(path + "StandardsOfCare/");
                            }
                        }
                    }
                    catch (error) {
                        window.location.replace(path + "StandardsOfCare/");
                    }
                },
                error: function () {
                    window.location.replace(path + "StandardsOfCare/");
                    alert("Failure communicating with the server.");
                }
            });
            //form.submit();

            //SaveMeasure(form, path + "StandardsOfCare/SaveWound", oTable, function (result) {
            //    alert("Wound Record has been successfully saved.");
            //    if (result.SOCEventWoundNotedId !== 0) {
            //        var tableValues = [
            //            result.NotedDate,
            //            result.Site,
            //            result.Describe,
            //            "<a class=\"copy\" href=\"\">Copy</a>",
            //            "<a class=\"edit\" href=\"\">Edit</a>",
            //            "<a class=\"delete\" href=\"\">Delete</a>"
            //        ];
            //        if ($("#WoundEditingId").val() != "") {
            //            var rowNumber = oTableWounds.fnGetPosition($("#woundsTable tr#" + result.SOCEventWoundNotedId)[0]);
            //            oTableWounds.fnUpdate(tableValues, rowNumber);
            //            return;
            //        }
            //        var newEntry = oTableWounds.fnAddData(tableValues);
            //        var tr = oTableWounds.fnSettings().aoData[newEntry[0]].nTr;
            //        tr.setAttribute("id", result.SOCEventWoundNotedId);
            //    }
            //    $("#SOCEventWound_Site, #SOCEventWound_CompositeWoundDescribeId").attr("disabled", "disabled");
            //    $("#clearWound").click();
            //    $("#closeWound").click();
            //}, function (result) {
            //    if (result.Reason == 0)
            //        alert("Error: Cannot have two entries with the same Date Noted. Please change the date.");
            //    if (result.Reason == 1)
            //        alert("Error: Could not save to the database. Please try again.");
            //    if (result.Reason == 2)
            //        alert("Error: One of the text area's content is too long and information would be truncated. Please make your text shorter.");
            //});
        }
    });

    $("#SOCEventWound_AffectedByOther").change(function () {
        var checked = $(this).is(":checked");
        if (checked)
            $("#OtherDescriptionRow").show();
        else
            $("#OtherDescriptionRow").hide();
    });

    $("#DrainageCheckBox").click(function () {
        var checked = $(this).is(":checked");
        if (checked)
            $("#Drainage").removeClass("hidden");
        else
            $("#Drainage").addClass("hidden");
    });

    $("#AdmittedWithFlg").click(function () {
        var checked = $(this).is(":checked");
        $("#SOCEventWound_AdmittedWithFlg").val(checked);
    });



    $("#SaveWound").on("click", ".file-link .delete-item", function (e) {

        if (confirm("Are you sure you would like to delete '" + $(this).siblings("a").text().trim() + "'?")) {

            var documentId = $(this).data("document-id");
            $.ajax({
                url: ajaxBaseUri + "DeleteWoundDocument",
                type: "POST",
                dataType: "json",
                data: { documentId: documentId },
                success: function (result) {

                    $(".file-link")[0].innerHTML = '<input type="file" name="files" id="wound-document" class="fileSelection pressure" />';
                }
            });
        }

    });

    $("#SOCEventWound_UnavoidableFlg").click(function () {
        setWoundDocumentState();
    });
    setWoundDocumentState();

    $("#clearWound").click(function () {
        $("#WoundEditingId, #DateNoted, #Stages, #PWLength, #PWWidth, #PWDepth, #wound-document, #LabsDiagnostic").val("");
        if ($("#SOCEventWound_Site").attr("disabled") == null)
            $("#SOCEventWound_Site").val("");
        $("#SaveWound textarea").each(function () {
            $(this).val("");
        });
        $("#DrainageCheckBox").prop("checked", false);
        $("#Drainage").addClass("hidden");
        $(".file-link")[0].innerHTML = '<input type="file" name="files" id="wound-document" class="fileSelection pressure" />';
        setWoundDocumentState();

        if ($("#AdmittedWithFlg").attr("disabled") == null) {
            $("#AdmittedWithFlg").prop("checked", false);
            $("#SOCEventWound_AdmittedWithFlg").val("false");
        }
    });

    $("#closeWound").click(function () {
        $(".modal, #wound").hide();
    });

    //Fall
    $("#saveFall").click(function () {
        var form = $("#SaveFall");
        if (form.valid() && FallFormIsValid()) {
            SaveMeasure(form, ajaxBaseUri + "SaveFall", oTable, function (result) {
                alert("Fall Record has been successfully saved.");
                $("#closeFall").click();
            });
        }
    });

    $("#closeFall").click(function () {
        $(".modal, #fall").hide();
    });

    $("#clearFall").click(function () {
        $("#Hr, #Min, #FallLocations, #SOCEventFall_RootCause").val("");
        $("#SOCEventFallAMPM").val("AM");

        $("#SaveFall input").prop("checked", false);
    });

    //AntiPsychotic
    $("#saveAnti").click(function () {
        var oTableAnti = $('#antiTable').dataTable();
        var form = $("#SaveAntipsychotic");
        if (form.valid() && AntiPsychoticFormIsValid()) {
            SaveMeasure(form, ajaxBaseUri + "SaveAntipsychotic", oTable, function (result) {
                alert("AntiPsychotic Record has been successfully saved.");
                if (result.SOCEventAntiPsychoticNotedId !== 0) {
                    var tableValues = [
                        result.NotedDate,
                        result.RecommendationDesc,
                        "<a class=\"copy\" href=\"\">Copy</a>",
                        "<a class=\"edit\" href=\"\">Edit</a>",
                        "<a class=\"delete\" href=\"\">Delete</a>"
                    ];
                    if ($("#AntiEditingId").val() != "") {
                        var rowNumber = oTableAnti.fnGetPosition($("#antiTable tr#" + result.SOCEventAntiPsychoticNotedId)[0]);
                        oTableAnti.fnUpdate(tableValues, rowNumber);
                        return;
                    }
                    var newEntry = oTableAnti.fnAddData(tableValues);
                    var tr = oTableAnti.fnSettings().aoData[newEntry[0]].nTr;
                    tr.setAttribute("id", result.SOCEventAntiPsychoticNotedId);
                }
                $("#SOCEventAntiPsychotic_AntiPsychoticMedicationId").attr("disabled", "disabled");
                $("#SOCEventAntiPsychotic_Diagnosis").attr("readOnly", "true");
                $("#SOCEventAntiPsychotic_Diagnosis").css("background-color", "lightgray");
                $("#clearAnti").click();
                $("#closeAnti").click();
            }, function (result) {
                if (result.Reason == 0)
                    alert("Error: Cannot have two entries with the same Date Noted. Please change the date.");
                if (result.Reason == 1)
                    alert("Error: Could not save to the database. Please try again.");
                if (result.Reason == 2)
                    alert("Error: The Recommendation text content is too long and information would be truncated. Please make your text shorter.");
            });
        }
    });

    $("#closeAnti").click(function () {
        $(".modal, #antipsychotic").hide();
    });

    $("#clearAnti").click(function () {

        $("#AntiEditingId, #DateAntipsychotic, #Recommendation, #TargetedBehavior, #DateAIMS").val("");
        if ($("#SOCEventAntiPsychotic_AntiPsychoticMedicationId").attr("disabled") == null) {
            $("#SOCEventAntiPsychotic_AntiPsychoticMedicationId").val("");
        }
        if ($("#SOCMeasure").val() != 11) {
            $("#DateReducedAnti").val("").addClass("hidden");
            $("#reducedShow").addClass("hidden");
        }

        $("#SaveAntipsychotic input").prop("checked", false);
    });

    $("#ReducedAnti").click(function () {
        var checked = $(this).is(":checked");

        if (checked) {
            $("#DateReducedAnti").removeClass("hidden");
        } else {
            $("#DateReducedAnti").addClass("hidden");
        }
    });

    $("#ReviewedAnti").click(function () {
        var checked = $(this).is(":checked");

        if (checked) {
            $(" #reducedShow").removeClass("hidden");
        } else {
            $("#reducedShow, #DateReducedAnti").addClass("hidden");
            $("#ReducedAnti").prop("checked", false);
            $("#DateReducedAnti").val("");
        }
    });

    //Catheter
    $("#saveCath").click(function () {
        var form = $("#SaveCatheter");
        if (form.valid() && CatheterFormIsValid()) {
            SaveMeasure(form, ajaxBaseUri + "SaveCatheter", oTable, function (result) {
                alert("Catheter Record has been successfully saved.");
                $("#SOCEventCatheter_SOCCatheterTypeId").attr("disabled", "disabled");
                $("#closeCath").click();
            });
        }
    });

    $("#closeCath").click(function () {
        $(".modal").hide();
        $("#catheter").hide();
    });

    //Restraint
    $("#saveRest").click(function () {
        var form = $("#SaveRestraint");
        var oTableRestraint = $('#restTable').dataTable();
        if (form.valid() && RestraintFormIsValid()) {
            SaveMeasure(form, ajaxBaseUri + "SaveRestraint", oTable, function (result) {
                alert("Restraint Record has been successfully saved.");
                if (result.SOCEventRestraintNotedId !== 0) {
                    var tableValues = [
                        result.Restraint,
                        result.ReducedDate,
                        result.SupportFlg,
                        "<a class=\"copy\" href=\"\">Copy</a>",
                        "<a class=\"edit\" href=\"\">Edit</a>",
                        "<a class=\"delete\" href=\"\">Delete</a>"
                    ];
                    if ($("#RestraintEditingId").val() != "") {
                        var rowNumber = oTableRestraint.fnGetPosition($("#restTable tr#" + result.SOCEventRestraintNotedId)[0]);
                        oTableRestraint.fnUpdate(tableValues, rowNumber);
                        SetCountDownTime(1800);
                        return;
                    }
                    var newEntry = oTableRestraint.fnAddData(tableValues);
                    var tr = oTableRestraint.fnSettings().aoData[newEntry[0]].nTr;
                    tr.setAttribute("id", result.SOCEventRestraintNotedId);
                }
                $("#clearRest").click();
                $("#closeRest").click();
            }, function (result) {
                if (result.Reason == 0)
                    alert("Error: Cannot have two entries with the same Date Reduced. Please change the date.");
                if (result.Reason == 1)
                    alert("Error: Could not save to the database. Please try again.");
                if (result.Reason == 2)
                    alert("Error: One of the text area's content is too long and information would be truncated. Please make your text shorter.");
            });
        }
    });

    $("#closeRest").click(function () {
        $(".modal, #restraint").hide();
    });

    $("#DiagnosisRestraint").click(function () {
        var checked = $(this).is(":checked");
        if (checked)
            $("#DiagnosisRestraintDesc").removeClass("hidden");
        else
            $("#DiagnosisRestraintDesc").addClass("hidden");
    });

    $("#clearRest").click(function () {
        $("#RestraintEditingId, #RestraintTypes, #DateReduced, #DateNotedRest, #RestraintComments").val("");
        $("#DiagnosisRestraint").prop("checked", false);
        $("#DiagnosisRestraintDesc").val("").addClass("hidden");
    });

    $("#Residents").change(function () {
        var resident = $("#Residents option:selected").val();
        if (resident == "") {
            $("#SideDDL").submit();
        }
    });

    //populate Stage dropdown data
    var myStageObject = JSON.parse($("#stagesData").val());
    $("#Stages").change(function (e) {
        var selectedValue = $(this).find("option:selected").val();
        $("#SelectedStage").val(selectedValue);
        for (var x = 0; x < myStageObject.length; x++) {
            var stage = myStageObject[x];
            if (stage.PressureWoundStageId == selectedValue) {
                if (stage.LengthFlg == true) {
                    $(".length-field").removeClass("hidden");
                } else {
                    $("#PWLength").val(null);
                    $(".length-field").addClass("hidden");
                }

                if (stage.WidthFlg == true) {
                    $(".width-field").removeClass("hidden");
                } else {
                    $("#PWWidth").val(null);
                    $(".width-field").addClass("hidden");
                }

                if (stage.DepthFlg == true) {
                    $(".depth-field").removeClass("hidden");
                } else {
                    $("#PWDepth").val(null);
                    $(".depth-field").addClass("hidden");
                }
            }
        }


    });

    optional();

    if ($("#SOCMeasure").val() == null) {
        $("#SOCMeasure").val("");
    }
});

function InputEvent() {
    var oTableWounds = $('#woundsTable').dataTable();
    var oTableRestraint = $('#restTable').dataTable();
    var oTableAnti = $('#antiTable').dataTable();
    var socMeasure = $("#MeasureId");
    if (socMeasure.val() == 1) {
        $(".injury").show();
        InputMeasure("FALL", "#fall");
    }
    else if (socMeasure.val() == 3 || socMeasure.val() == 9 || socMeasure.val() == 10) {
        InputMeasure(null, "#event");
    }
    else if (socMeasure.val() == 4) {
        $(".pressure").show();
        $(".pressureAfter").hide();
        $(".composite").hide();
        InputMeasure("PRWNDPRIOR", "#wound");
        if ($("#SOCEventWound_AffectedByOther").is(":checked"))
            $("#OtherDescriptionRow").show();
        else
            $("#OtherDescriptionRow").hide();
        oTableWounds.fnAdjustColumnSizing();
    }
    else if (socMeasure.val() == 5) {
        $(".pressure").show();
        $(".pressureAfter").show();
        $(".composite").hide();
        if ($("#SOCEventWound_AffectedByOther").is(":checked"))
            $("#OtherDescriptionRow").show();
        else
            $("#OtherDescriptionRow").hide();
        InputMeasure("PRWNDAFTER", "#wound");
        oTableWounds.fnAdjustColumnSizing();
    }
    else if (socMeasure.val() == 6) {
        InputMeasure("RESTRAINT", "#restraint");
        oTableRestraint.fnAdjustColumnSizing();
    }
    else if (socMeasure.val() == 24) {
        $(".pressure").hide();
        $(".pressureAfter").hide();
        $(".composite").show();
        InputMeasure("SCWND", "#wound");
        oTableWounds.fnAdjustColumnSizing();
    }
    else if (socMeasure.val() == 7) {
        InputMeasure('', "#event");
    }
    else if (socMeasure.val() == 8) {
        InputMeasure("CATHETER", "#catheter");
    }
    else if (socMeasure.val() == 11 || socMeasure.val() == 23) {
        InputMeasure('ANTIPSYCH', "#antipsychotic");
        //if (socMeasure.val() == 11) show the 11-specific fields (or rename corresponding labels); else hide them;
        UpdateAntiLabels(socMeasure.val());

        $("#SOCEventAntiPsychotic_AntiPsychoticMedicationId").val($("#antiMedicationId").val());
        $("#SOCEventAntiPsychotic_AntiPsychoticDiagnosisId").val($("#antiDiagnosisId").val());

        AntiPsychoticDiagnosisCheck();
        oTableAnti.fnAdjustColumnSizing();
    }
}

function UpdateAntiLabels(measureId) {
    if (measureId == 11) {
        $("#DateOccurredLabel").prop("textContent", "Date Initial Order:");
        $("#ReducedLabel").prop("textContent", "GDR:");
        $("#ReviewedLabel").prop("textContent", "IDT Reviewed:");
        $("#ReviewedAnti").addClass("hidden");
        //$("#DateReviewedAnti").removeClass("hidden");
        $("#reducedShow").removeClass("hidden");
        $("#ReducedAnti").addClass("hidden");
        $("#DateReducedAnti").removeClass("hidden");
        $(".with-diagnosis-only").removeClass("hidden");
    } else {
        $("#DateOccurredLabel").prop("textContent", "Date Occurred:");
        $("#ReviewedLabel").prop("textContent", "Reviewed:");
        $("#ReducedLabel").prop("textContent", "Reduced:");
        $(".with-diagnosis-only").addClass("hidden");
        $("#ReviewedAnti").removeClass("hidden");
        //$("#DateReviewedAnti").addClass("hidden");
        $("#reducedShow").addClass("hidden");
        $("#ReducedAnti").removeClass("hidden");
        $("#DateReducedAnti").addClass("hidden");
    }
}

function AntiPsychoticDiagnosisCheck() {
    if ($("#SOCEventAntiPsychotic_AntiPsychoticDiagnosisId option:selected").text().toLowerCase() == "other") {
        $("#SOCEventAntiPsychotic_OtherDiagnosisDetail").removeClass("hidden");
    } else {
        $("#SOCEventAntiPsychotic_OtherDiagnosisDetail").addClass("hidden");
        $("#SOCEventAntiPsychotic_OtherDiagnosisDetail").val("");
    }
}

function setWoundDocumentState() {
    var checked = $("#SOCEventWound_UnavoidableFlg").is(":checked");
    if (checked) {
        $("#wound-document").attr("disabled", null);
        $(".file-link").removeClass("hidden");
    }
    else {
        $("#wound-document").attr("disabled", "disabled");
        $("#wound-document").val("");
        $(".file-link").addClass("hidden");
    }
}