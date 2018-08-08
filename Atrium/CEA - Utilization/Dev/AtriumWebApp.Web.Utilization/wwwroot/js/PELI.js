function PreparePage() {
    var oTable = $("#PatientPELITable").dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "rt",  //"sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "bVisible": false // PELILogId
            },
            {
                "bVisible": false // PatientId
            },
            {
                "aDataSort": [2], // AdmitDate
                "sWidth": "50px"
            },
            {
                "sWidth": "50px" // CompletedDate
            },
            {
                "sWidth": "75px" // PELIType
            },
            {
                "sWidth": "25px", // Edit
                "bSortable": false
            }
            //,
            //{
            //    "sWidth": "35px", // Delete
            //    "bSortable": false,
            //    "bVisible": false
            //}
        ],
        "oLanguage": {
            "sEmptyTable": "No entries during the selected date range."
        }
    });
    oTable.fnSort([[1, "desc"]]);

    $("#PatientPELITable").on('click','a.edit', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path +"PELI/GetPELILog",
            dataType: "json",
            cache: false,
            type: 'GET',
            data: { PELILogId: nRowId },
            success: function (result) {
                LaunchModal(result);
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    //$("#PatientPELITable a.delete").live('click', function (e) {
    //    e.preventDefault();
    //    var delConfirm = confirm("Are you sure you want to delete?");
    //    if (delConfirm == true) {
    //        var nRow = $(this).parents('tr')[0];
    //        var nRowId = $(this).closest('tr').attr("id");
    //        $.ajax({
    //            url: path +"PELI/DeletePELILog",
    //            dataType: "json",
    //            cache: false,
    //            type: 'POST',
    //            data: { PELILogId: nRowId },
    //            success: function (result) {
    //                oTable.fnDeleteRow(nRow);
    //            },
    //            error: function (ex) {
    //                alert("Server is not responding. Please reload page and try again");
    //            }
    //        });
    //    }
    //});

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

    $("#PELIType").change(function () {
        if($("#PELIType").val() == "")
        {
            $("#btnCreate").prop("disabled", true);
        }
        else 
        {
            $("#btnCreate").prop("disabled", false);
        }
    });

    SetupModal();
}

function SetupModal() {
    $("#btnSave").click(function () {
        var form = $("#SavePELILog");
        if (ValidateForm(form)) {
            $.ajax({
                url: path +"PELI/SavePELILog",
                dataType: "json",
                cache: false,
                traditional: true,
                type: "POST",
                data: form.serialize(),
                success: function (result) {
                    alert("PELI Record has been successfully saved.");
                    UpdatePatientPELITable(form, result);
                    $("#btnClose").click();
                },
                error: function (ex) {
                    alert("PELI Record save failure occurred!");
                }
            });
        }
    });

    $("#btnClear").click(function () {
        var form = $("#SavePELILog");
        $("input:text", form).val("");
        $("input:checkbox", form).prop("checked", false);
        $("select", form).val("");

        //$(".hospitalExemptFieldShow", form).hide();
        //$(".levelIIFieldShow", form).hide();
    });

    $("#btnClose").click(function () {
        $(".modal, #popupModal").hide();
        $("#btnClear").click();

        $("#modalTitle").text("");
        $("#PELILogId").val("");
        $("#PELITypeId").val("");
        //$(".hospitalExemptShow").show();
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
        $("#modalTitle").text("Add PELI Log for " + $('#PELICurrentResidentName').val());
        $("#PELILogId").val(data.PELILogId);
        $("#PatientId").val(data.PatientId);
        $("#AdmitDate").val(GetDateString(data.AdmitDate));
        $("#CompletedDate").val(GetDateString(data.CompletedDate));
        //alert('LaunchModal: data.PELITypeId==' + data.PELITypeId);
        //$("#PELITypeId").val(data.PELITypeId).change();
        $('#PELITypeModal')
            .find('option')
            .removeAttr('selected')
            .filter('[value=' + data.PELITypeId + ']')
            .attr('selected', true);
    }
    else {
        $("#modalTitle").text($("#PELIType option:selected").text());
        $("#PELILogId").val("");
        $("#PatientId").val(data.PatientId);
        $("#AdmitDate").val(GetDateString(new Date()));
        $("#CompletedDate").val(GetDateString(new Date()));
        $("#PELITypeId").val(data.PELITypeId).change();
    }
}

function ValidateForm(form) {
    var admitDate = $("#AdmitDate", form).val();
    if (IsFieldEmpty(admitDate)) {
        alert("Admitted Date is a required field.");
        return false;
    }
    if (!IsDateValid(admitDate)) {
        alert("Admitted Date must be in the format mm/dd/yyyy.");
        return false;
    }
    if (IsDateFuture(admitDate)) {
        alert("Admitted Date cannot be in the future.");
        return false;
    }

    var completedDate = $("#CompletedDate", form).val();
    //if (IsFieldEmpty(completedDate)) {
    //    alert("Completed Date is a required field.");
    //    return false;
    //}
    if (completedDate != "" && !IsDateValid(completedDate)) {
        alert("Completed Date must be in the format mm/dd/yyyy.");
        return false;
    }
    if (completedDate != "" && IsDateFuture(completedDate)) {
        alert("Completed Date cannot be in the future.");
        return false;
    }
    return true;
}

function UpdatePatientPELITable(form, data) {
    var oTable = $("#PatientPELITable").DataTable();
    var elemTR = $("#PatientPELITable").find('tr#' + data.PELILogId);
    //var elemSelect = $(elemTR).find('select#rec_PELITypeId').clone();
    var peliReasonDesc = $('#PELITypeModal option:selected').text();

    //    .find('option')
    //    .removeAttr('selected')
    //    .filter('[value=' + data.PELITypeId + ']')
    //    .attr('selected', true);
    //elemSelect.prop('disabled', 'disabled');
    //var outerHTMLSelect = elemSelect.wrap('<p>').parent().html();
    //outerHTMLSelect.replace(/"/g, '\"');
    // NOTE: must ensure ALL data is a String by either .toString() or "" + val.
    var rowData = [
        data.PELILogId,
        (""+data.PatientId),
        GetDateString(data.AdmitDate),
        (data.CompletedDate != undefined && data.CompletedDate.length > 0 ? GetDateString(data.CompletedDate) : ""),
        peliReasonDesc,
        "<a class=\"edit\" href=\"\">Edit</a>"
    ];

    var PELILogId = $("#PELILogId", form).val();
    if (PELILogId != "") {
        var element = document.getElementById(data.PELILogId);
        if (element != null) {
            var rowNumber = oTable.fnGetPosition(element);
            oTable.fnUpdate(rowData, rowNumber);
            element.setAttribute("id", data.PELILogId);
        }
    }
    else {
        var newEntry = oTable.fnAddData(rowData);
        var tr = oTable.fnSettings().aoData[newEntry[0]].nTr;
        tr.setAttribute("id", data.PELILogId);
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