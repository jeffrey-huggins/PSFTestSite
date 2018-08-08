$(document).ready(function () {
    var oTable = $('#ADRTable').dataTable({
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "bVisible": false
            },
            null,
            null,
            null,
            null,
            null,
            null,
            {
                "bSortable": false
            },
            {
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No records within this range."
        }
    });
    oTable.fnSort([[3, 'asc']]);

    if ($("#ClosedRequestsShown").val() != "True") {
        oTable.fnFilter("False", 0, false);
    }
    else {
        $("#ClosedRequests").prop("checked", true);
    }
    $("#ClosedRequests").click(function () {
        var isChecked = $(this).is(":checked");
        $.post(path + "/AdditionalDevelopmentRequest/ClosedRequests", { isChecked: isChecked }).done(function () {
            isChecked ? oTable.fnFilter("^.*", 0, true) : oTable.fnFilter("False", 0, false);
        });
    });

    $("#ADRTable a.edit").on("click", function (e) {
        e.preventDefault();
        var nRowId = $(this).closest('tr').attr("id");
        $.post(path + "/AdditionalDevelopmentRequest/SetADR", { rowId: nRowId }).done(function () {
            window.ShowProgress();
            window.location.reload();
        });
    });

    $("#ADRTable a.delete").on("click", function (e) {
        e.preventDefault();
        if (!confirm("Are you sure you want to delete?")) {
            return;
        }
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path + "/AdditionalDevelopmentRequest/DeleteRow",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { rowId: nRowId },
            success: function (result) {
                if (result.Success) {
                    oTable.fnDeleteRow(nRow);
                    if ($("#ADR_RequestId").val() == nRowId) {
                        $.post(path + "/AdditionalDevelopmentRequest/ClearADR").done(function () {
                            window.location.reload();
                        });
                    }
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

    $(function () {
        $(".isDate").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
    });
    //Manual Validations [ID, Display Name, isRequired]
    var datesToValidate = [["ServiceBeginDate", "Service Begin", false], ["ServiceEndDate", "Service End Date", false],
    ["ADRDate", "ADR CMS Date", true], ["ADRReceivedDate", "ADR Received Date", false], ["ADRReturnMailDate", "ADR Returned Mail Date", false],
    ["ADRDenialDate", "ADR Denial Date", false], ["RedeterminationMailDate", "Redetermination Mail Date", false], ["RedeterminationDenialDate", "Redetermination Denial Date", false],
    ["ReconsiderationMailDate", "Reconsideration Mail Date", false], ["ReconsiderationDenialDate", "Reconsideration Denial Date", false],
    ["ALJMailDate", "ALJ Mail Date", false], ["ALJHearingDate", "ALJ Hearing Date", false], ["ALJDenialDate", "ALJ Denial Date", false]
    ];

    var currencyToValidate = [["ARAmount", "AR Amount", true]];
    $("#submitADR").click(function () {
        var requiredMessage = " is a required field.";
        var invalidMessage = " must be in the format mm/dd/yyyy.";
        var invalidCurrencyMessage = " must be in the format X.XX";
        var submit = true;
        var dcn = $('#ADR_DCN').val();
        if (dcn.length != 17) {
            alert('DCN must be 17 characters in length.');
            submit = false;
        }
        for (var i = 0; i < datesToValidate.length; i++) {
            var date = $("#" + datesToValidate[i][0]).val();
            if (datesToValidate[i][2] && date == "") {
                alert(datesToValidate[i][1] + requiredMessage);
                submit = false;
            }
            if (isInvalidDate(date)) {
                alert(datesToValidate[i][1] + invalidMessage);
                submit = false;
            }
        }
        for (var i = 0; i < currencyToValidate.length; i++) {
            var currency = $("#" + currencyToValidate[i][0]).val();
            if (currencyToValidate[i][2] && currency == "") {
                alert(currencyToValidate[i][1] + requiredMessage);
                submit = false;
            }
            if (isInvalidCurrency(currency)) {
                alert(currencyToValidate[i][1] + invalidCurrencyMessage);
                submit = false;
            }
        }
        if (submit) {
            $("#SaveADR").submit();
        }
    });

    $("#clearADR").click(function () {
        $.post(path + "/AdditionalDevelopmentRequest/ClearADR").done(function () {
            window.ShowProgress();
            window.location.reload();
        });
    });

    $("#addToNotes").click(function () {
        if ($("#ADR_RequestId").val() == "") {
            alert("An ADR entry must be saved or edited in order to save notes.");
            return;
        }
        var adrId = $("#ADR_RequestId").val();
        var note = $("#Notes").val();
        if ($.trim(note).length == 0) {
            return;
        }
        $.ajax({
            url: path + "/AdditionalDevelopmentRequest/AddNote",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                adrId: adrId,
                note: note
            },
            success: function (result) {
                if (!result.Success) {
                    if (result.Reason == 1)
                        alert("Error: Could not save to the database. Please try again.");
                    if (result.Reason == 2)
                        alert("Error: The Notes area's content is too long and information would be truncated. Please make your text shorter.");
                    SetCountDownTime(1800);
                    return false;
                }
                $("#Notes").val("");
                $("#ADR_RequestNotes").val(result.UpdateNote);
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }

        });
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
});
function isInvalidDate(value) {
    return value != "" && !ValidateDate(value);
}
function isInvalidCurrency(value) {
    return value != "" && !ValidateCurrency(value);
}