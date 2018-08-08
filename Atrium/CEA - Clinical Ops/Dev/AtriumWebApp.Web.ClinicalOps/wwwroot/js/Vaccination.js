function PreparePage() {
    var oTable = $("#VaccinationHistoryTable").dataTable({
        "bAutoWidth": false,
        "sScrollY": "300px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            null, // Type
            {
                "aDataSort": [6, 1, 0] // Date
            },
            null, // Refused
            null, // Consent
            {
                "bSearchable": false, //Edit
                "bSortable": false,
                "sWidth": '40px'
            },
            {
                "bSearchable": false, //Delete
                "bSortable": false,
                "sWidth": '40px'
            },
            {
                "bVisible": false // Date Sort Flag
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No entries for date range selected"
        }
    });
    oTable.fnSort([[1, 'desc']]);

    $("#VaccinationHistoryTable").on('click','a.edit', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path + "Vaccination/GetVaccine",
            dataType: "json",
            cache: false,
            type: 'GET',
            data: { vaccineId: nRowId },
            success: function (result) {
                LaunchModal(result.PatientVaccine.VaccineTypeId, result);
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $("#VaccinationHistoryTable").on('click','a.delete', function (e) {
        e.preventDefault();
        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            $.ajax({
                url: path + "Vaccination/DeleteVaccine",
                cache: false,
                type: 'POST',
                data: { vaccineId: nRowId },
                success: function (result) {
                    oTable.fnDeleteRow(nRow);
                },
                error: function (ex) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
    });

    $(function () {
        if ($("#ToDateRangeInvalid").val() == "1") {
            alert('Error: Please enter a valid date in the Infection Date Range "To" Field (mm/dd/yyyy)');
        }
        if ($("#FromDateRangeInvalid").val() == "1") {
            alert('Error: Please enter a valid date in the Infection Date Range "From" Field (mm/dd/yyyy)');
        }
        if ($("#ToDateRangeInFuture").val() == "1") {
            alert('Error: Please enter a valid date that is not in the future in the Infection Date Range "To" Field');
        }
        if ($("#FromDateRangeInFuture").val() == "1") {
            alert('Error: Please enter a valid date that is not in the future in the Infection Date Range "From" Field');
        }
        if ($("#FromAfterTo").val() == "1") {
            alert('Error: You can not have the "From" Date after the "To" Date in the Infection Date Range Fields');
        }
    });

    $(".isDate").datepicker({
        beforeShow: function (textbox, instance) {
            instance.dpDiv.css({
                marginTop: (-textbox.offsetHeight) + 'px',
                marginLeft: textbox.offsetWidth + 'px'
            });
        }
    });

    $("#occurredRangeTo, #occurredRangeFrom").datepicker({
        beforeShow: function (textbox, instance) {
            instance.dpDiv.css({
                marginTop: '5px',
                marginLeft: '0px'
            });
        }
    });

    $("#btnCreate").click(function () {
        LaunchModal($("#VaccineTypes").val());
    });

    SetupFluModal();
    SetupPneumoniaModal();
    SetupTBInitialModal();
    SetupTBAnnualModal();
}


function SetupFluModal() {
    var form = $("#flu");

    $("#btnSave_Flu").click(function () {
        if (ValidateFluForm(form)) {
            SaveVaccination($("form", form), function (result) {
                alert("Vaccination Record has been successfully saved.");
                $("#btnClose_Flu").click();
            }, function (result) {
                alert("Vaccination Record save failure occurred!");
            });
        }
    });

    $("#btnClear_Flu").click(function () {
        $("#VaccineId", form).val("");

        $("#VaccineDate_Flu", form).val("");
        $("#LotNumber_Flu", form).val("");
        $("#ConsentSignedFlg", form).prop("checked", false);
        $("#OfferedRefusedFlg", form).prop("checked", false);
        $("#OfferedRefusedDate_Flu", form).val("");
        $(".offeredRefusedDateShow", form).hide();
        $("#ImmunizationPriorToAdmissionFlg", form).prop("checked", false);
    });

    $("#btnClose_Flu").click(function () {
        $(".modal, #flu").hide();
        $("#btnClear_Flu").click();
    });

    $("#OfferedRefusedFlg", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".offeredRefusedDateShow", form).show();
        }
        else {
            $(".offeredRefusedDateShow", form).hide();
            $("#OfferedRefusedDate_Flu", form).val("");
        }
    });
}

function SetupPneumoniaModal() {
    var form = $("#pneumonia");

    $("#btnSave_Pneumonia").click(function () {
        if (ValidatePneumoniaForm(form)) {
            SaveVaccination($("form", form), function (result) {
                alert("Vaccination Record has been successfully saved.");
                $("#btnClose_Pneumonia").click();
            }, function (result) {
                alert("Vaccination Record save failure occurred!");
            });
        }
    });

    $("#btnClear_Pneumonia").click(function () {
        $("#VaccineId", form).val("");

        $("#PneumoniaVaccineTypes", form).val("");
        $("#VaccineDate_Pneumonia", form).val("");
        $("#NextDueDate", form).val("");
        $("#LotNumber_Pneumonia", form).val("");

        $("#ConsentSignedFlg", form).prop("checked", false);
        $("#OfferedRefusedFlg", form).prop("checked", false);
        $("#OfferedRefusedDate_Pneumonia", form).val("");
        $(".offeredRefusedDateShow", form).hide();
        $("#ImmunizationPriorToAdmissionFlg", form).prop("checked", false);
    });

    $("#btnClose_Pneumonia").click(function () {
        $(".modal, #pneumonia").hide();
        $("#btnClear_Pneumonia").click();
    });

    $("#OfferedRefusedFlg", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".offeredRefusedDateShow", form).show();
        }
        else {
            $(".offeredRefusedDateShow", form).hide();
            $("#OfferedRefusedDate_Pneumonia", form).val("");
        }
    });
}

function SetupTBInitialModal() {
    var form = $("#tb_initial");

    $("#btnSave_TB_Initial").click(function () {
        if (ValidateTBInitialForm(form)) {
            SaveVaccination($("form", form), function (result) {
                alert("Vaccination Record has been successfully saved.");
                $("#btnClose_TB_Initial").click();
            }, function (result) {
                alert("Vaccination Record save failure occurred!");
            });
        }
    });

    $("#btnClear_TB_Initial").click(function () {
        $("#VaccineId", form).val("");
        $("#ConsentSignedFlg", form).prop("checked", false);

        $("#PPDStep1GivenDate", form).val("");
        $("#PPDStep1Site", form).val("");
        $("#PPDStep1ReadDate", form).val("");
        $("#PPDStep1ReactionFlg", form).prop("checked", false);
        $("#PPDStep1ReactionMeasurement", form).val("");
        $(".reactionMeasurementStep1Show", form).hide();
        $("#PPDStep1LotNumber", form).val("");

        $("#PPDStep2GivenDate", form).val("");
        $("#PPDStep2Site", form).val("");
        $("#PPDStep2ReadDate", form).val("");
        $("#PPDStep2ReactionFlg", form).prop("checked", false);
        $("#PPDStep2ReactionMeasurement", form).val("");
        $(".reactionMeasurementStep2Show", form).hide();
        $("#PPDStep2LotNumber", form).val("");

        $("#ChestXRayFlg_Y", form).prop("checked", false);
        $("#ChestXRayFlg_N", form).prop("checked", false);
        $("#ChestXRayFlg_NA", form).prop("checked", true);
        $("#ChestXRayDate", form).val("");
        $("#ChestXRayQuestionnaireCompleteFlg", form).prop("checked", false);
        $("#ChestXRayQuestionnaireCompleteDate", form).val("");
        $(".chestXRayDateShow", form).hide();
        $(".questionnaireShow", form).hide();
        $(".questionnaireDateShow", form).hide();

        $("#BMATFlg", form).prop("checked", false);
        $("#BMATDate_Initial", form).val("");
        $(".bmatDateShow", form).hide();
    });

    $("#btnClose_TB_Initial").click(function () {
        $(".modal, #tb_initial").hide();
        $("#btnClear_TB_Initial").click();
    });

    $("#PPDStep1ReactionFlg", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".reactionMeasurementStep1Show", form).show();
        }
        else {
            $(".reactionMeasurementStep1Show", form).hide();
            $("#PPDStep1ReactionMeasurement", form).val("");
        }
    });

    $("#PPDStep2ReactionFlg", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".reactionMeasurementStep2Show", form).show();
        }
        else {
            $(".reactionMeasurementStep2Show", form).hide();
            $("#PPDStep2ReactionMeasurement", form).val("");
        }
    });

    $("#ChestXRayFlg_Y", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".chestXRayDateShow", form).show();
            $(".questionnaireShow", form).hide();
            $("#ChestXRayQuestionnaireCompleteFlg", form).prop("checked", false);
            $("#ChestXRayQuestionnaireCompleteDate", form).val("");
        }
    });

    $("#ChestXRayFlg_N", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".questionnaireShow", form).show();
            $(".chestXRayDateShow", form).hide();
            $("#ChestXRayDate", form).val("");
        }
    });

    $("#ChestXRayFlg_NA", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".chestXRayDateShow", form).hide();
            $("#ChestXRayDate", form).val("");
            $(".questionnaireShow", form).hide();
            $("#ChestXRayQuestionnaireCompleteFlg", form).prop("checked", false);
            $("#ChestXRayQuestionnaireCompleteDate", form).val("");
        }
    });

    $("#ChestXRayQuestionnaireCompleteFlg", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".questionnaireDateShow", form).show();
        }
        else {
            $(".questionnaireDateShow", form).hide();
            $("#ChestXRayQuestionnaireCompleteDate", form).val("");
        }
    });

    $("#BMATFlg", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".bmatDateShow", form).show();
        }
        else {
            $(".bmatDateShow", form).hide();
            $("#BMATDate_Initial", form).val("");
        }
    });
}

function SetupTBAnnualModal() {
    var form = $("#tb_annual");

    $("#btnSave_TB_Annual").click(function () {
        if (ValidateTBAnnualForm(form)) {
            SaveVaccination($("form", form), function (result) {
                alert("Vaccination Record has been successfully saved.");
                $("#btnClose_TB_Annual").click();
            }, function (result) {
                alert("Vaccination Record save failure occurred!");
            });
        }
    });

    $("#btnClear_TB_Annual").click(function () {
        $("#VaccineId", form).val("");
        $("#ConsentSignedFlg", form).prop("checked", false);

        $("#PPDGivenDate", form).val("");
        $("#PPDSite", form).val("");
        $("#PPDReadDate", form).val("");
        $("#PPDReactionFlg", form).prop("checked", false);
        $("#PPDReactionMeasurement", form).val("");
        $(".reactionMeasurementShow", form).hide();
        $("#PPDLotNumber", form).val("");

        $("#QuestionnaireCompleteFlg", form).prop("checked", false);
        $("#QuestionnaireCompleteDate", form).val("");
        $(".questionnaireShow", form).hide();
        $(".questionnaireDateShow", form).hide();

        $("#BMATFlg", form).prop("checked", false);
        $("#BMATDate_Annual", form).val("");
        $(".bmatDateShow", form).hide();
    });

    $("#btnClose_TB_Annual").click(function () {
        $(".modal, #tb_annual").hide();
        $("#btnClear_TB_Annual").click();
    });

    $("#PPDReactionFlg", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".reactionMeasurementShow", form).show();
        }
        else {
            $(".reactionMeasurementShow", form).hide();
            $("#PPDReactionMeasurement", form).val("");
        }
    });

    $("#QuestionnaireCompleteFlg", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".questionnaireDateShow", form).show();
        }
        else {
            $(".questionnaireDateShow", form).hide();
            $("#QuestionnaireCompleteDate", form).val("");
        }
    });

    $("#BMATFlg", form).click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".bmatDateShow", form).show();
        }
        else {
            $(".bmatDateShow", form).hide();
            $("#BMATDate_Annual", form).val("");
        }
    });
}


function LaunchModal(vaccineType, data) {
    var form;

    if (vaccineType == 1)
        form = LaunchFluModal(data);
    else if (vaccineType == 2)
        form = LaunchPneumoniaModal(data);
    else if (vaccineType == 3)
        form = LaunchTBAnnualModal(data);
    else if (vaccineType == 4)
        form = LaunchTBInitialModal(data);
    else
        return;

    $("#VaccineType", form).val(vaccineType);
}

function LaunchFluModal(data) {
    var modal = $('<div />');
    modal.addClass("modal");
    $('body').append(modal);

    var form = $("#flu");
    form.show();

    var top = Math.max($(window).height() / 2 - form[0].offsetHeight / 2, 0);
    var left = Math.max($(window).width() / 2 - form[0].offsetWidth / 2, 0);
    form.css({ top: top, left: left });

    // Fill form with existing record data
    if (data) {
        $("#VaccineId", form).val(data.PatientVaccineId);
        $("#VaccineDate_Flu", form).val(GetDateString(data.VaccineDate));
        $("#LotNumber_Flu", form).val(data.LotNumber);
        $("#ConsentSignedFlg", form).prop("checked", data.PatientVaccine.ConsentSignedFlg);
        $("#OfferedRefusedFlg", form).prop("checked", data.PatientVaccine.OfferedRefusedFlg);
        if (data.PatientVaccine.OfferedRefusedFlg) {
            $(".offeredRefusedDateShow", form).show();
            $("#OfferedRefusedDate_Flu", form).val(GetDateString(data.PatientVaccine.OfferedRefusedDate));
        }
        $("#ImmunizationPriorToAdmissionFlg", form).prop("checked", data.PatientVaccine.ImmunizationPriorToAdmissionFlg);
    }

    return form;
}

function LaunchPneumoniaModal(data) {
    var modal = $('<div />');
    modal.addClass("modal");
    $('body').append(modal);

    var form = $("#pneumonia");
    form.show();

    var top = Math.max($(window).height() / 2 - form[0].offsetHeight / 2, 0);
    var left = Math.max($(window).width() / 2 - form[0].offsetWidth / 2, 0);
    form.css({ top: top, left: left });

    // Fill form with existing record data
    if (data) {
        $("#VaccineId", form).val(data.PatientVaccineId);
        
        $("#PneumoniaVaccineTypes", form).val(data.VaccinePneumoniaTypeId);
        $("#VaccineDate_Pneumonia", form).val(GetDateString(data.VaccineDate));
        $("#NextDueDate", form).val(GetDateString(data.NextDueDate));
        $("#LotNumber_Pneumonia", form).val(data.LotNumber);

        $("#ConsentSignedFlg", form).prop("checked", data.PatientVaccine.ConsentSignedFlg);
        $("#OfferedRefusedFlg", form).prop("checked", data.PatientVaccine.OfferedRefusedFlg);
        if (data.PatientVaccine.OfferedRefusedFlg) {
            $(".offeredRefusedDateShow", form).show();
            $("#OfferedRefusedDate_Pneumonia", form).val(GetDateString(data.PatientVaccine.OfferedRefusedDate));
        }
        $("#ImmunizationPriorToAdmissionFlg", form).prop("checked", data.PatientVaccine.ImmunizationPriorToAdmissionFlg);
    }

    return form;
}

function LaunchTBInitialModal(data) {
    var modal = $('<div />');
    modal.addClass("modal");
    $('body').append(modal);

    var form = $("#tb_initial");
    form.show();

    var top = Math.max($(window).height() / 3 - form[0].offsetHeight / 3, 0);
    var left = Math.max($(window).width() / 2 - form[0].offsetWidth / 2, 0);
    form.css({ top: top, left: left });

    // Fill form with existing record data
    if (data) {
        $("#VaccineId", form).val(data.PatientVaccineId);
        $("#ConsentSignedFlg", form).prop("checked", data.PatientVaccine.ConsentSignedFlg);

        $("#PPDStep1GivenDate", form).val(GetDateString(data.PPDStep1GivenDate));
        $("#PPDStep1Site", form).val(data.PPDStep1SiteId == -1 ? "" : data.PPDStep1SiteId);
        $("#PPDStep1ReadDate", form).val(GetDateString(data.PPDStep1ReadDate));
        $("#PPDStep1ReactionFlg", form).prop("checked", data.PPDStep1ReactionFlg);
        if (data.PPDStep1ReactionFlg) {
            $("#PPDStep1ReactionMeasurement", form).val(data.PPDStep1ReactionMeasurementId);
            $(".reactionMeasurementStep1Show", form).show();
        }
        $("#PPDStep1LotNumber", form).val(data.PPDStep1LotNumber);

        $("#PPDStep2GivenDate", form).val(GetDateString(data.PPDStep2GivenDate));
        $("#PPDStep2Site", form).val(data.PPDStep2SiteId == -1 ? "" : data.PPDStep2SiteId);
        $("#PPDStep2ReadDate", form).val(GetDateString(data.PPDStep2ReadDate));
        $("#PPDStep2ReactionFlg", form).prop("checked", data.PPDStep2ReactionFlg);
        if (data.PPDStep2ReactionFlg) {
            $("#PPDStep2ReactionMeasurement", form).val(data.PPDStep2ReactionMeasurementId);
            $(".reactionMeasurementStep2Show", form).show();
        }
        $("#PPDStep2LotNumber", form).val(data.PPDStep2LotNumber);

        if (IsFieldEmpty(data.ChestXRayFlg)) {
            $("#ChestXRayFlg_NA", form).prop("checked", true);
        }
        else if (data.ChestXRayFlg) {
            $("#ChestXRayFlg_Y", form).prop("checked", true);
            $("#ChestXRayDate", form).val(GetDateString(data.ChestXRayDate));
            $(".chestXRayDateShow", form).show();
        }
        else {
            $("#ChestXRayFlg_N", form).prop("checked", true);
            $("#ChestXRayQuestionnaireCompleteFlg", form).prop("checked", data.QuestionnaireCompleteFlg);
            $(".questionnaireShow", form).show();

            if (data.QuestionnaireCompleteFlg) {
                $("#ChestXRayQuestionnaireCompleteDate", form).val(GetDateString(data.QuestionnaireCompleteDate));
                $(".questionnaireDateShow", form).show();
            }
        }

        $("#BMATFlg", form).prop("checked", data.BMATFlg);
        if (data.BMATFlg) {
            $("#BMATDate_Initial", form).val(GetDateString(data.BMATDate));
            $(".bmatDateShow", form).show();
        }
    }

    return form;
}

function LaunchTBAnnualModal(data) {
    var modal = $('<div />');
    modal.addClass("modal");
    $('body').append(modal);

    var form = $("#tb_annual");
    form.show();

    var top = Math.max($(window).height() / 2 - form[0].offsetHeight / 2, 0);
    var left = Math.max($(window).width() / 2 - form[0].offsetWidth / 2, 0);
    form.css({ top: top, left: left });

    // Fill form with existing record data
    if (data) {
        $("#VaccineId", form).val(data.PatientVaccineId);
        $("#ConsentSignedFlg", form).prop("checked", data.PatientVaccine.ConsentSignedFlg);

        $("#PPDGivenDate", form).val(GetDateString(data.PPDGivenDate));
        $("#PPDSite", form).val(data.PPDSiteId == -1 ? "" : data.PPDSiteId);
        $("#PPDReadDate", form).val(GetDateString(data.PPDReadDate));
        $("#PPDReactionFlg", form).prop("checked", data.PPDReactionFlg);
        if (data.PPDReactionFlg) {
            $("#PPDReactionMeasurement", form).val(data.PPDReactionMeasurementId);
            $(".reactionMeasurementShow", form).show();
        }
        $("#PPDLotNumber", form).val(data.PPDLotNumber);

        $("#QuestionnaireCompleteFlg", form).prop("checked", data.QuestionnaireCompleteFlg);
        if (data.QuestionnaireCompleteFlg) {
            $("#QuestionnaireCompleteDate", form).val(GetDateString(data.QuestionnaireCompleteDate));
            $(".questionnaireDateShow", form).show();
        }

        $("#BMATFlg", form).prop("checked", data.BMATFlg);
        if (data.BMATFlg) {
            $("#BMATDate_Annual", form).val(GetDateString(data.BMATDate));
            $(".bmatDateShow", form).show();
        }
    }

    return form;
}


function ValidateFluForm(form) {
    var vaccineDate = $("#VaccineDate_Flu", form).val();
    var offeredRefused = $("#OfferedRefusedFlg", form).is(":checked");
    if (!offeredRefused && IsFieldEmpty(vaccineDate)) {
        alert("Vaccine Date is a required field.");
        return false;
    }
    if (!IsFieldEmpty(vaccineDate) && !IsDateValid(vaccineDate)) {
        alert("Vaccine Date must be in the format mm/dd/yyyy.");
        return false;
    }
    if (IsDateFuture(vaccineDate)) {
        alert("Vaccine Date cannot be in the future.");
        return false;
    }

    if (offeredRefused) {
        var offeredRefusedDate = $("#OfferedRefusedDate_Flu", form).val();
        if (IsFieldEmpty(offeredRefusedDate)) {
            alert("Offered and Refused Date is a required field.");
            return false;
        }
        if (!IsDateValid(offeredRefusedDate)) {
            alert("Offered and Refused Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(offeredRefusedDate)) {
            alert("Offered and Refused Date cannot be in the future.");
            return false;
        }
    }

    return true;
}

function ValidatePneumoniaForm(form) {
    var vaccineType = $("#PneumoniaVaccineTypes").val();
    if (IsFieldEmpty(vaccineType)) {
        alert("Vaccine Type is a required field.");
        return false;
    }

    var priorToAdmission = $("#ImmunizationPriorToAdmissionFlg", form).is(":checked");
    if (!priorToAdmission && vaccineType == 1) { // Unknown
        alert("Vaccine Type cannot be 'Unknown' unless immunization was prior to admission.");
        return false;
    }

    var offeredRefused = $("#OfferedRefusedFlg", form).is(":checked");
    var vaccineDate = $("#VaccineDate_Pneumonia", form).val();
    if (!priorToAdmission && !offeredRefused && IsFieldEmpty(vaccineDate)) {
        alert("Vaccine Date is a required field unless immunization was prior to admission.");
        return false;
    }
    if (!IsFieldEmpty(vaccineDate) && !IsDateValid(vaccineDate)) {
        alert("Vaccine Date must be in the format mm/dd/yyyy.");
        return false;
    }
    if (IsDateFuture(vaccineDate)) {
        alert("Vaccine Date cannot be in the future.");
        return false;
    }

    var nextDueDate = $("#NextDueDate", form).val();
    if (!offeredRefused && IsFieldEmpty(nextDueDate)) {
        alert("Next Due Date is a required field.");
        return false;
    }
    if (!offeredRefused && !IsDateValid(nextDueDate)) {
        alert("Next Due Date must be in the format mm/dd/yyyy.");
        return false;
    }

    if (offeredRefused) {
        var offeredRefusedDate = $("#OfferedRefusedDate_Pneumonia", form).val();
        if (IsFieldEmpty(offeredRefusedDate)) {
            alert("Offered and Refused Date is a required field.");
            return false;
        }
        if (!IsDateValid(offeredRefusedDate)) {
            alert("Offered and Refused Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(offeredRefusedDate)) {
            alert("Offered and Refused Date cannot be in the future.");
            return false;
        }
    }

    return true;
}

function ValidateTBInitialForm(form) {
    // PPD Step 1
    var step1GivenDate = $("#PPDStep1GivenDate", form).val();
    var step1Site = $("#PPDStep1Site", form).val();
    var step1ReadDate = $("#PPDStep1ReadDate", form).val();
    var step1Reaction = $("#PPDStep1ReactionFlg", form).is(":checked");
    var step1ReactionMeasurement = $("#PPDStep1ReactionMeasurement", form).val();
    var step1LotNumber = $("#PPDStep1LotNumber", form).val();
    var step1 = !IsFieldEmpty(step1GivenDate) || !IsFieldEmpty(step1Site) || !IsFieldEmpty(step1ReadDate) || step1Reaction || !IsFieldEmpty(step1LotNumber);

    if (step1) {
        if (IsFieldEmpty(step1GivenDate)) {
            alert("PPD Step 1 Given Date is a required field.");
            return false;
        }
        if (!IsDateValid(step1GivenDate)) {
            alert("PPD Step 1 Given Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(step1GivenDate)) {
            alert("PPD Step 1 Given Date cannot be in the future.");
            return false;
        }

        if (IsFieldEmpty(step1Site)) {
            alert("PPD Step 1 Site is a required field.");
            return false;
        }

        if (!IsFieldEmpty(step1ReadDate) && !IsDateValid(step1ReadDate)) {
            alert("PPD Step 1 Read Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(step1ReadDate)) {
            alert("PPD Step 1 Read Date cannot be in the future.");
            return false;
        }
        if (!IsFieldEmpty(step1ReadDate) && (GetDateDiff(step1GivenDate, step1ReadDate) < 2 || GetDateDiff(step1GivenDate, step1ReadDate) > 4)) {
            alert("PPD Step 1 Read Date must be 2-4 days after the PPD Step 1 Given Date.");
            return false;
        }

        if (step1Reaction & IsFieldEmpty(step1ReactionMeasurement)) {
            alert("PPD Step 1 Reaction Measurement is a required field.");
            return false;
        }
    }
    
    // PPD Step 2
    var step2GivenDate = $("#PPDStep2GivenDate", form).val();
    var step2Site = $("#PPDStep2Site", form).val();
    var step2ReadDate = $("#PPDStep2ReadDate", form).val();
    var step2Reaction = $("#PPDStep2ReactionFlg", form).is(":checked");
    var step2ReactionMeasurement = $("#PPDStep2ReactionMeasurement", form).val();
    var step2LotNumber = $("#PPDStep2LotNumber", form).val();
    var step2 = !IsFieldEmpty(step2GivenDate) || !IsFieldEmpty(step2Site) || !IsFieldEmpty(step2ReadDate) || step2Reaction || !IsFieldEmpty(step2LotNumber);

    if (step2) {
        if (IsFieldEmpty(step2GivenDate)) {
            alert("PPD Step 2 Given Date is a required field.");
            return false;
        }
        if (!IsDateValid(step2GivenDate)) {
            alert("PPD Step 2 Given Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(step2GivenDate)) {
            alert("PPD Step 2 Given Date cannot be in the future.");
            return false;
        }

        if (IsFieldEmpty(step2Site)) {
            alert("PPD Step 2 Site is a required field.");
            return false;
        }

        if (!IsFieldEmpty(step2ReadDate) && !IsDateValid(step2ReadDate)) {
            alert("PPD Step 2 Read Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(step2ReadDate)) {
            alert("PPD Step 2 Read Date cannot be in the future.");
            return false;
        }
        if (!IsFieldEmpty(step2ReadDate) && (GetDateDiff(step2GivenDate, step2ReadDate) < 2 || GetDateDiff(step2GivenDate, step2ReadDate) > 4)) {
            alert("PPD Step 2 Read Date must be 2-4 days after the PPD Step 2 Given Date.");
            return false;
        }

        if (step2Reaction & IsFieldEmpty(step2ReactionMeasurement)) {
            alert("PPD Step 2 Reaction Measurement is a required field.");
            return false;
        }
    }

    // Chest X-Ray
    var chestXRayYes = $("#ChestXRayFlg_Y", form).is(":checked");
    var chestXRayNo = $("#ChestXRayFlg_N", form).is(":checked");
    var questionnaire = false;
    if (chestXRayYes) {
        var chestXRayDate = $("#ChestXRayDate", form).val();
        if (IsFieldEmpty(chestXRayDate)) {
            alert("Chest X-Ray Date Taken is a required field.");
            return false;
        }
        if (!IsDateValid(chestXRayDate)) {
            alert("Chest X-Ray Date Taken must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(chestXRayDate)) {
            alert("Chest X-Ray Date Taken cannot be in the future.");
            return false;
        }
    }
    if (chestXRayNo) {
        questionnaire = $("#ChestXRayQuestionnaireCompleteFlg", form).is(":checked");
        if (questionnaire) {
            var questionnaireDate = $("#ChestXRayQuestionnaireCompleteDate", form).val();
            if (IsFieldEmpty(questionnaireDate)) {
                alert("Questionnaire Complete Date is a required field.");
                return false;
            }
            if (!IsDateValid(questionnaireDate)) {
                alert("Questionnaire Complete Date must be in the format mm/dd/yyyy.");
                return false;
            }
            if (IsDateFuture(questionnaireDate)) {
                alert("Questionnaire Complete Date cannot be in the future.");
                return false;
            }
        }
    }

    // BMAT
    var bmat = $("#BMATFlg", form).is(":checked");
    if (bmat) {
        var bmatDate = $("#BMATDate_Initial", form).val();
        if (IsFieldEmpty(bmatDate)) {
            alert("BMAT Date is a required field.");
            return false;
        }
        if (!IsDateValid(bmatDate)) {
            alert("BMAT Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(bmatDate)) {
            alert("BMAT Date cannot be in the future.");
            return false;
        }
    }

    if (!step1 && !chestXRayYes && !questionnaire && !bmat) {
        alert("PPD Step 1, Chest X-Ray, Questionnaire or BMAT must be completed.");
        return false;
    }

    return true;
}

function ValidateTBAnnualForm(form) {
    // PPD
    var ppdGivenDate = $("#PPDGivenDate", form).val();
    var ppdSite = $("#PPDSite", form).val();
    var ppdReadDate = $("#PPDReadDate", form).val();
    var ppdReaction = $("#PPDReactionFlg", form).is(":checked");
    var ppdReactionMeasurement = $("#PPDReactionMeasurement", form).val();
    var ppdLotNumber = $("#PPDLotNumber", form).val();
    var ppd = !IsFieldEmpty(ppdGivenDate) || !IsFieldEmpty(ppdSite) || !IsFieldEmpty(ppdReadDate) || ppdReaction || !IsFieldEmpty(ppdLotNumber);

    if (ppd) {
        if (IsFieldEmpty(ppdGivenDate)) {
            alert("PPD Given Date is a required field.");
            return false;
        }
        if (!IsDateValid(ppdGivenDate)) {
            alert("PPD Given Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(ppdGivenDate)) {
            alert("PPD Given Date cannot be in the future.");
            return false;
        }

        if (IsFieldEmpty(ppdSite)) {
            alert("PPD Site is a required field.");
            return false;
        }

        if (!IsFieldEmpty(ppdReadDate) && !IsDateValid(ppdReadDate)) {
            alert("PPD Read Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(ppdReadDate)) {
            alert("PPD Read Date cannot be in the future.");
            return false;
        }
        if (!IsFieldEmpty(ppdReadDate) && (GetDateDiff(ppdGivenDate, ppdReadDate) < 2 || GetDateDiff(ppdGivenDate, ppdReadDate) > 4)) {
            alert("PPD Read Date must be 2-4 days after the PPD Given Date.");
            return false;
        }

        if (ppdReaction & IsFieldEmpty(ppdReactionMeasurement)) {
            alert("PPD Reaction Measurement is a required field.");
            return false;
        }
    }

    var questionnaire = $("#QuestionnaireCompleteFlg", form).is(":checked");
    if (questionnaire) {
        var questionnaireDate = $("#QuestionnaireCompleteDate", form).val();
        if (IsFieldEmpty(questionnaireDate)) {
            alert("Questionnaire Complete Date is a required field.");
            return false;
        }
        if (!IsDateValid(questionnaireDate)) {
            alert("Questionnaire Complete Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(questionnaireDate)) {
            alert("Questionnaire Complete Date cannot be in the future.");
            return false;
        }
    }

    // BMAT
    var bmat = $("#BMATFlg", form).is(":checked");
    if (bmat) {
        var bmatDate = $("#BMATDate_Annual", form).val();
        if (IsFieldEmpty(bmatDate)) {
            alert("BMAT Date is a required field.");
            return false;
        }
        if (!IsDateValid(bmatDate)) {
            alert("BMAT Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (IsDateFuture(bmatDate)) {
            alert("BMAT Date cannot be in the future.");
            return false;
        }
    }

    if (!ppd && !questionnaire && !bmat) {
        alert("PPD, Questionnaire or BMAT must be completed.");
        return false;
    }

    return true;
}


function SaveVaccination(form, successCallback, failureCallback) {
    $.ajax({
        url: $(form).attr("action"),
        dataType: "json",
        cache: false,
        type: $(form).attr("method"),
        data: form.serialize(),
        success: function (result) {
            UpdateVaccineHistoryTable(form, result);

            if (successCallback)
                successCallback(result);

            $("#VaccineTypes").val("");
        },
        error: function (ex) {
            if (failureCallback) {
                failureCallback(ex);
            }
            else {
                alert("Server is not responding. Please reload the page and try again");
            }
        }
    });
}

function UpdateVaccineHistoryTable(form, data) {
    var oTable = $("#VaccinationHistoryTable").dataTable();
    var rowData = GetRowData(data);

    var vaccineId = $("#VaccineId", form).val();
    if (vaccineId != "") {
        var element = document.getElementById(vaccineId);
        if (element != null) {
            var rowNumber = oTable.fnGetPosition(element);
            oTable.fnUpdate(rowData, rowNumber);
            element.setAttribute("id", data.PatientVaccineId);
        }
    }
    else {
        var newEntry = oTable.fnAddData(rowData);
        var tr = oTable.fnSettings().aoData[newEntry[0]].nTr;
        tr.setAttribute("id", data.PatientVaccineId);
    }

    oTable.fnSort([[1, 'desc']]);
}

function GetRowData(data) {
    if($("#IsEmployeeVaccine").val().toLowerCase() == "true")
    {
        return [
        data.VaccineType.VaccineTypeName,
        IsFieldEmpty(data.VaccineDate) ? "Prior to Admission" : GetDateString(data.VaccineDate),
        data.OfferedRefusedFlg ? "Yes" : "No",
        "",
        "<a class=\"edit\" href=\"\">Edit</a>",
        "<a class=\"delete\" href=\"\">Delete</a>",
        IsFieldEmpty(data.VaccineDate) ? "0" : GetSortableDateString(data.VaccineDate)
        ];
    } else {
        return [
        data.VaccineType.VaccineTypeName,
        IsFieldEmpty(data.VaccineDate) ? "Prior to Admission" : GetDateString(data.VaccineDate),
        data.OfferedRefusedFlg ? "Yes" : "No",
        data.ConsentSignedFlg ? "Yes" : "No",
        "<a class=\"edit\" href=\"\">Edit</a>",
        "<a class=\"delete\" href=\"\">Delete</a>",
        IsFieldEmpty(data.VaccineDate) ? "0" : GetSortableDateString(data.VaccineDate)
        ];
    }
}


function IsFieldEmpty(value) {
    return value === "" || value === null || value === undefined;
}

function IsDateValid(value) {
    return !IsFieldEmpty(value) && ValidateDate(value);
}

function GetDateString(value) {
    if (!IsFieldEmpty(value)) {
        var dt = new Date(value);
        var month = dt.getUTCMonth() + 1;
        var day = dt.getUTCDate();
        var year = dt.getUTCFullYear();
        return month + "/" + day + "/" + year;
    }
    return "";
}

function GetSortableDateString(value) {
    if (!IsFieldEmpty(value)) {
        var dt = new Date(value);
        var month = (dt.getUTCMonth() + 1).toString().padLeft(2, "0");
        var day = dt.getUTCDate().toString().padLeft(2, "0");
        var year = dt.getUTCFullYear().toString();
        return year + "-" + month + "-" + day;
    }
    return "";
}

function GetDateDiff(startDate, endDate) {
    return moment(endDate).diff(moment(startDate), "days");
}

String.prototype.padLeft = function (n, str) {
    return Array(n - String(this).length + 1).join(str || '0') + this;
}