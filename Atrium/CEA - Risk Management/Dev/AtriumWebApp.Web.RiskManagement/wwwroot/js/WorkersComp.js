
$(document).ready(function () {
    var oTableNotes = $('#NotesTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "rtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '40px'
            },
            {
                "sWidth": '60px'
            },
            {
                "sWidth": '75px'
            },
            {
                "bSearchable": false,
                "bVisible": false
            },
            {
                "sWidth": '30px',
                "bSortable": false
            },
            {
                "sWidth": '30px',
                "bSortable": false
            },
            {
                "bSearchable": false,
                "bVisible": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No notes for the claim"
        }
    });
    oTableNotes.fnSort([[6, 'desc'], [1, 'desc']]);
    $("#NotesTable_length").css("display", "none");

    $("#NotesTable a.delete").on("click", function (e) {
        e.preventDefault();

        if (!confirm("Are you sure you want to delete?"))
            return;

        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");

        $.ajax({
            url: path + "BaseRiskManagement/DeleteNote",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "RMW"
            },
            success: function (result) {
                if (result.Success) {
                    oTableNotes.fnDeleteRow(nRow);
                }
                else {
                    alert("Error: Note did not delete properly. Server may not be responding");
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }

        });

    });

    $("#NotesTable").on("click", "a.edit", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableNotes.fnGetData(nRow);

        $("#NotesId").val(nRowId);
        $("#UNotes").val(HTMLDecode(aData[3]));
    });

    var oTableExpenses = $('#expensesTable').dataTable({
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "rtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '40px'
            },
            {
                "sWidth": '50px'
            },
            {
                "sWidth": '100px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No expenses for the claim"
        }
    });
    var nEditing = null;
    oTableExpenses.fnSort([[1, 'desc']]);
    $("#expensesTable_length").css("display", "none");
    $("#expensesTable_filter").css("display", "none");

    var oTableDiagnosis = $('#diagnosisTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "rtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '20px'
            },
            {
                "sWidth": '50px'
            },
            {
                "bSearchable": false,
                "bVisible": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No diagnoses for the claim"
        }
    });
    oTableDiagnosis.fnSort([[0, 'desc']]);
    $("#diagnosisTable_length").css("display", "none");

    $("#NotesTable").on('click', 'a.moreNotes', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        var aData = oTableNotes.fnGetData(nRow);

        alert(HTMLDecode(aData[3]));
    });

    $("#diagnosisTable").on('click', 'a.moreNotes', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        var aData = oTableDiagnosis.fnGetData(nRow);

        alert(HTMLDecode(aData[2]));
    });

    var validatorClaim = $("#SaveClaimDetails").validate({
        rules: {
            "CompClaim.ClaimTypeId": {
                required: true
            },
            "CompClaim.FROIDate": {
                dateFuture: true,
                dateUS: true
            },
            "CompClaim.ReportedtoCarrierDate": {
                required: true,
                dateFuture: true,
                dateUS: true
            },
            "CompClaim.LightDutyBeginDate": {
                dateFuture: true,
                dateUS: true
            },
            "CompClaim.LightDutyEndDate": {
                dateFuture: true,
                dateUS: true
            },
            "CompClaim.FullDutyBeginDate": {
                dateFuture: true,
                dateUS: true
            },
            "CompClaim.FullDutyEndDate": {
                dateFuture: true,
                dateUS: true
            }
        },
        invalidHandler: function () {
            HideProgress();

        }

    });

    $(function () {
        $("#FROI, #ReportedToCarrier, #CompClaim_LightDutyBeginDate, #CompClaim_LightDutyEndDate, #CompClaim_FullDutyBeginDate, #CompClaim_FullDutyEndDate").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#FROI, #ReportedToCarrier, #CompClaim_LightDutyBeginDate, #CompClaim_LightDutyEndDate, #CompClaim_FullDutyBeginDate, #CompClaim_FullDutyEndDate").addClass("isDate");
    });

    $("#expenseType").change(function () {
        var newType = $("#expenseType option:selected").val();

        if (newType == "") {
            oTableExpenses.fnFilter('', 0);
            oTableExpenses.fnFilter('');
        } else
            oTableExpenses.fnFilter("^" + newType + "$", 0, true);
    });

    if ($("#CompClaim_LegalFirmFlg").is(":checked")) {
        showLitigated();
    } else {
        hideLitigated();
    }
    if ($("#CompClaim_VOCRehabFlg").is(":checked")) {
        showVOCRehab();
    } else {
        hideVOCRehab();
    }
    if ($("#CompClaim_TCMFlg").is(":checked")) {
        showTCM();
    } else {
        hideTCM();
    }
    if ($("#CompClaim_PreventableFlg").is(":checked")) {
        showPreventable();
    } else {
        hidePreventable();
    }
    if ($("#CompClaim_HighExposureFlg").is(":checked")) {
        showHighExposure();
    } else {
        hideHighExposure();
    }

    var insurance = $("#CompClaim_InsuranceCarrierId").val();
    if (insurance != "") {
        $("#Insurance").val(insurance);
    }
    var litigated = $("#CompClaim_LegalFirmID").val();
    if (litigated != "") {
        $("#Litigated").val(litigated);
    }
    var vocRehab = $("#CompClaim_VOCRehabID").val();
    if (vocRehab != "") {
        $("#VOCRehab").val(vocRehab);
    }
    var tcm = $("#CompClaim_TCMId").val();
    if (tcm != "") {
        $("#TCM").val();
    }

    $("#CompClaim_LegalFirmFlg").click(function () {
        if ($("#CompClaim_LegalFirmFlg").is(":checked")) {
            showLitigated();
        } else {
            hideLitigated();
        }
    });
    $("#CompClaim_VOCRehabFlg").click(function () {
        if ($("#CompClaim_VOCRehabFlg").is(":checked")) {
            showVOCRehab();
        } else {
            hideVOCRehab();
        }
    });
    $("#CompClaim_TCMFlg").click(function () {
        if ($("#CompClaim_TCMFlg").is(":checked")) {
            showTCM();
        } else {
            hideTCM();
        }
    });

    $("#CompClaim_PreventableFlg").click(function () {
        if ($("#CompClaim_PreventableFlg").is(":checked")) {
            showPreventable();
        } else {
            hidePreventable();
        }
    });

    $("#CompClaim_HighExposureFlg").click(function () {
        if ($("#CompClaim_HighExposureFlg").is(":checked")) {
            showHighExposure();
        } else {
            hideHighExposure();
        }
    });

    $(".cancel").click(function () {
        $("#CompClaim_LightDutyBeginDate").attr("value", "");
        $("#CompClaim_LightDutyEndDate").attr("value", "");
        $("#CompClaim_FullDutyBeginDate").attr("value", "");
        $("#CompClaim_FullDutyEndDate").attr("value", "");
        $("textarea").val("");

        hideLitigated();
        hideVOCRehab();
        hideTCM();
        hidePreventable();
        hideHighExposure();

        $("#SaveClaimDetails input").prop("checked", false);
    });

    $("#btnAddNotes").click(function () {
        if ($("#UNotes").val() == "") {
            alert("Notes value is required");
            return false;
        }
    });
});
function showLitigated() {
    $("#Litigated").removeAttr("disabled").css("visibility", "visible");
}
function hideLitigated() {
    $("#Litigated").attr("disabled", "disabled").css("visibility", "hidden");
}
function showVOCRehab() {
    $("#VOCRehab").removeAttr("disabled").css("visibility", "visible");
}
function hideVOCRehab() {
    $("#VOCRehab").attr("disabled", "disabled").css("visibility", "hidden");
}
function showTCM() {
    $("#TCM").removeAttr("disabled").css("visibility", "visible");
}
function hideTCM() {
    $("#TCM").attr("disabled", "disabled").css("visibility", "hidden");
}
function showPreventable() {
    $("#CompClaim_PreventableComments").removeAttr("disabled").css("visibility", "visible");
}
function hidePreventable() {
    $("#CompClaim_PreventableComments").attr("disabled", "disabled").css("visibility", "hidden");
}
function showHighExposure() {
    $("#CompClaim_HighExposureComments").removeAttr("disabled").css("visibility", "visible");
}
function hideHighExposure() {
    $("#CompClaim_HighExposureComments").attr("disabled", "disabled").css("visibility", "hidden");
}