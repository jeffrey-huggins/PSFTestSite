function PreparePage() {
    var oTable = $("#PatientPASRRTable").dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "rt",  //"sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null, // PASRR/Sig Change Type
            {
                "aDataSort": [11], // Completed
                "sWidth": "50px"
            },
            {
                "sWidth": "50px" // Hospital Exempt
            },
            {
                "sWidth": "50px" // Expiration
            },
            {
                "sWidth": "75px" // Stay > 30 Days
            },
            {
                "sWidth": "50px", // Dementia Exempt
                "bVisible": false
            },
            {
                "sWidth": "50px" // Level II Needed
            },
            {
                "sWidth": "50px" // Level II Requested
            },
            {
                "sWidth": "50px" // Level II Completed
            },
            {
                "sWidth": "25px", // Edit
                "bSortable": false
            },
            {
                "sWidth": "35px", // Delete
                "bSortable": false
            },
            {
                "bVisible": false // Date Sort Flag
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No entries during the selected date range."
        }
    });
    oTable.fnSort([[1, "desc"]]);

    $("#PatientPASRRTable").on('click','a.edit', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path +"PASRR/GetPASRRLog",
            dataType: "json",
            cache: false,
            type: 'GET',
            data: { PASRRLogId: nRowId },
            success: function (result) {
                LaunchModal(result);
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $("#PatientPASRRTable").on('click','a.delete', function (e) {
        e.preventDefault();
        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            $.ajax({
                url: path +"PASRR/DeletePASRRLog",
                cache: false,
                type: 'POST',
                data: { pasrrLogId: nRowId },
                success: function (result) {
                    oTable.fnDeleteRow(nRow);
                },
                error: function (ex) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
    });

    //$(function () {
    //    if ($("#ToDateRangeInvalid").val() == "1") {
    //        alert('Error: Please enter a valid date in the Occurred Date Range "To" Field (mm/dd/yyyy)');
    //    }
    //    if ($("#FromDateRangeInvalid").val() == "1") {
    //        alert('Error: Please enter a valid date in the Occurred Date Range "From" Field (mm/dd/yyyy)');
    //    }
    //    if ($("#ToDateRangeInFuture").val() == "1") {
    //        alert('Error: Please enter a valid date that is not in the future in the Occurred Date Range "To" Field');
    //    }
    //    if ($("#FromDateRangeInFuture").val() == "1") {
    //        alert('Error: Please enter a valid date that is not in the future in the Occurred Date Range "From" Field');
    //    }
    //    if ($("#FromAfterTo").val() == "1") {
    //        alert('Error: You can not have the "From" Date after the "To" Date in the Occurred Date Range Fields');
    //    }
    //});

    $(".isDate").datepicker({
        beforeShow: function (textbox, instance) {
            instance.dpDiv.css({
                marginTop: (-textbox.offsetHeight) + "px",
                marginLeft: textbox.offsetWidth + "px"
            });
        }
    });

    $("#occurredRangeTo, #occurredRangeFrom").datepicker({
        beforeShow: function (textbox, instance) {
            instance.dpDiv.css({
                marginTop: "5px",
                marginLeft: "0px"
            });
        }
    });

    $("#btnCreate").click(function () {
        LaunchModal();
    });

    $("#PASRRType").change(function () {
        if($("#PASRRType").val() == "")
        {
            $("#btnCreate").prop("disabled", true);
        }
        else 
        {
            $("#btnCreate").prop("disabled", false);
        }
    });
    
    if ($("#CurrentCommunityStateCode").val() == "MI") {
        oTable.fnSetColumnVis(5, true);
        $(".dementiaExemptShow").show();
    }

    SetupModal();
}

function SetupModal() {
    $("#btnSave").click(function () {
        var form = $("#SavePASRRLog");
        if (ValidateForm(form)) {
            $.ajax({
                url: path +"PASRR/SavePASRRLog",
                dataType: "json",
                cache: false,
                traditional: true,
                type: "POST",
                data: form.serialize(),
                success: function (result) {
                    alert("PASRR Record has been successfully saved.");
                    UpdatePatientPASRRTable(form, result);
                    $("#PASRRType").val("");
                    $("#btnClose").click();
                },
                error: function (ex) {
                    alert("PASRR Record save failure occurred!");
                }
            });
        }
    });

    $("#btnClear").click(function () {
        var form = $("#SavePASRRLog");
        $("input:text", form).val("");
        $("input:checkbox", form).prop("checked", false);
        $("select", form).val("");

        $(".hospitalExemptFieldShow", form).hide();
        $(".levelIIFieldShow", form).hide();
    });

    $("#btnClose").click(function () {
        $(".modal, #popupModal").hide();
        $("#btnClear").click();

        $("#modalTitle").text("");
        $("#PASRRLogId").val("");
        $("#PASRRTypeId").val("");
        $(".sigChangeTypeShow").hide();
        $(".hospitalExemptShow").show();
    });

    $("#HospitalExemption").click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".hospitalExemptFieldShow").show();
            var completeDate = $("#CompleteDate").val();
            if (!IsFieldEmpty(completeDate) && IsDateValid(completeDate)) {
                $("#HospitalExemptionExpirationDate").val(moment(completeDate).add(30, "days").format('L'));
            }
        }
        else {
            $(".hospitalExemptFieldShow").hide();
            $("#HospitalExemptionExpirationDate").val("");
            $("#StayGreaterThan30Days").prop("checked", false);
        }
    });

    $("#LevelIINeeded").click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $(".levelIIFieldShow").show();
        }
        else {
            $(".levelIIFieldShow").hide();
            $("#LevelIIRequestedDate").val("");
            $("#LevelIICompletedDate").val("");
        }
    });
}

function LaunchModal(data) {
    var div = $("<div />");
    div.addClass("modal");
    $("body").append(div);

    var modal = $("#popupModal");
    modal.show();

    var top = Math.max($(window).height() / 3 - modal[0].offsetHeight / 3, 0);
    var left = Math.max($(window).width() / 2 - modal[0].offsetWidth / 2, 0);
    modal.css({ top: top, left: left });

    // Fill form with existing record data
    if (data) {
        $("#modalTitle").text(data.PASRRType.PASRRTypeName);
        $("#PASRRTypeId").val(data.PASRRTypeId);
        $("#PASRRLogId").val(data.PASRRLogId);
        $("#CompleteDate").val(GetDateString(data.CompleteDate));
        if (data.PASRRTypeId == 2) { // Sig Change
            $(".sigChangeTypeShow").show();
            $("#SigChangeType").val(data.SigChangeTypeId);
        }
        if (data.PASRRTypeId == 3) { // Annual
            $(".hospitalExemptShow").hide();
        }
        else {
            $("#HospitalExemption").prop("checked", data.HospitalExemption);
            if (data.HospitalExemption) {
                $(".hospitalExemptFieldShow").show();
                $("#HospitalExemptionExpirationDate").val(GetDateString(data.HospitalExemptionExpirationDate));
                $("#StayGreaterThan30Days").prop("checked", data.StayGreaterThan30Days);
            }
        }
        $("#DementiaExemption").prop("checked", data.DementiaExemption);
        $("#LevelIINeeded").prop("checked", data.LevelIINeeded);
        if (data.LevelIINeeded) {
            $(".levelIIFieldShow").show();
            $("#LevelIIRequestedDate").val(GetDateString(data.LevelIIRequestedDate));
            $("#LevelIICompletedDate").val(GetDateString(data.LevelIICompletedDate));
        }
    }
    else {
        $("#modalTitle").text($("#PASRRType option:selected").text());
        $("#PASRRTypeId").val($("#PASRRType").val());
        if ($("#PASRRTypeId").val() == "2") { // Sig Change
            $(".sigChangeTypeShow").show();
        }
        if ($("#PASRRTypeId").val() == "3") { // Annual
            $(".hospitalExemptShow").hide();
        }
    }
}

function ValidateForm(form) {
    var completeDate = $("#CompleteDate", form).val();
    if (IsFieldEmpty(completeDate)) {
        alert("Completed Date is a required field.");
        return false;
    }
    if (!IsDateValid(completeDate)) {
        alert("Completed Date must be in the format mm/dd/yyyy.");
        return false;
    }
    if (IsDateFuture(completeDate)) {
        alert("Completed Date cannot be in the future.");
        return false;
    }

    var pasrrType = $("#PASRRTypeId", form).val();
    if (pasrrType == "2") {
        var sigChangeType = $("#SigChangeType", form).val();
        if (IsFieldEmpty(sigChangeType)) {
            alert("Sig Change Type is a required field.");
            return false;
        }
    }

    var hospitalExemption = $("#HospitalExemption", form).is(":checked");
    var stayGraterThan30Days = $("#StayGreaterThan30Days", form).is(":checked");
    if (hospitalExemption) {
        var expirationDate = $("#HospitalExemptionExpirationDate", form).val();
        if (IsFieldEmpty(expirationDate)) {
            alert("Hospital Exemption Expiration Date is a required field.");
            return false;
        }
        if (!IsDateValid(expirationDate)) {
            alert("Hospital Exemption Expiration Date must be in the format mm/dd/yyyy.");
            return false;
        }
        if (GetDateDiff(completeDate, expirationDate) > 30) {
            alert("Hospital Exemption Expiration Date must be no more than 30 after the Completed Date.");
            return false;
        }
    }

    var levelIINeeded = $("#LevelIINeeded", form).is(":checked");
    if (levelIINeeded) {
        if (hospitalExemption && !stayGraterThan30Days) {
            alert("Level II is not needed with a Hospital Exemption unless the stay will be greater than 30 days.");
            return false;
        }

        var levelIIRequestedDate = $("#LevelIIRequestedDate", form).val();
        if (!IsFieldEmpty(levelIIRequestedDate)) {
            if (!IsDateValid(levelIIRequestedDate)) {
                alert("Level II Requested Date must be in the format mm/dd/yyyy.");
                return false;
            }
            if (IsDateFuture(levelIIRequestedDate)) {
                alert("Level II Requested Date cannot be in the future.");
                return false;
            }
        }

        var levelIICompletedDate = $("#LevelIICompletedDate", form).val();
        if (!IsFieldEmpty(levelIICompletedDate)) {
            if (!IsDateValid(levelIICompletedDate)) {
                alert("Level II Completed Date must be in the format mm/dd/yyyy.");
                return false;
            }
            if (IsDateFuture(levelIICompletedDate)) {
                alert("Level II Completed Date cannot be in the future.");
                return false;
            }
        }
    }

    return true;
}

function UpdatePatientPASRRTable(form, data) {
    var oTable = $("#PatientPASRRTable").dataTable();
    var rowData = [
        data.SigChangeTypeId != -1 ? data.PASRRType.PASRRTypeName + " - " + data.SigChangeType.SigChangeTypeName : data.PASRRType.PASRRTypeName,
        GetDateString(data.CompleteDate),
        data.PASRRTypeId == 3 ? "N/A" : data.HospitalExemption ? "Yes" : "No",
        IsFieldEmpty(data.HospitalExemptionExpirationDate) ? "N/A" : GetDateString(data.HospitalExemptionExpirationDate),
        !data.HospitalExemption ? "N/A" : data.StayGreaterThan30Days ? "Yes" : "No",
        data.DementiaExemption ? "Yes" : "No",
        data.LevelIINeeded ? "Yes" : "No",
        !data.LevelIINeeded ? "N/A" : IsFieldEmpty(data.LevelIIRequestedDate) ? "" : GetDateString(data.LevelIIRequestedDate),
        !data.LevelIINeeded ? "N/A" : IsFieldEmpty(data.LevelIICompletedDate) ? "" : GetDateString(data.LevelIICompletedDate),
        "<a class=\"edit\" href=\"\">Edit</a>",
        "<a class=\"delete\" href=\"\">Delete</a>",
        GetSortableDateString(data.CompleteDate)
    ];

    var pasrrLogId = $("#PASRRLogId", form).val();
    if (pasrrLogId != "") {
        var element = document.getElementById(data.PASRRLogId);
        if (element != null) {
            var rowNumber = oTable.fnGetPosition(element);
            oTable.fnUpdate(rowData, rowNumber);
            element.setAttribute("id", data.PASRRLogId);
        }
    }
    else {
        var newEntry = oTable.fnAddData(rowData);
        var tr = oTable.fnSettings().aoData[newEntry[0]].nTr;
        tr.setAttribute("id", data.PASRRLogId);
    }

    oTable.fnSort([[1, "desc"]]);
}

// Util Functions
function IsFieldEmpty(value) {
    return value === "" || value === null || value === undefined;
}

function IsDateValid(value) {
    return !IsFieldEmpty(value) && ValidateDate(value);
}
var test;
function GetDateString(value) {
    if (!IsFieldEmpty(value)) {
        var dt = new Date(value);
        var month = dt.getUTCMonth() + 1;
        var day = dt.getUTCDate();
        var year = dt.getFullYear();
        return month + "/" + day + "/" + year;
    }
    return "";
}

function GetSortableDateString(value) {
    if (!IsFieldEmpty(value)) {
        var dt = new Date(value);
        var month = (dt.getMonth() + 1).toString().padLeft(2, "0");
        var day = dt.getUTCDate().toString().padLeft(2, "0");
        var year = dt.getFullYear().toString();
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