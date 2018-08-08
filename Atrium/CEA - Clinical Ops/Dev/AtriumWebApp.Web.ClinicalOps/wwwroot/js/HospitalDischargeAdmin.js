var dischargeContextId = 0;
var dnrrContextId = 2;
var hospitalContextId = 3;

function initDischarge() {
    var oTableDischarge = $('#DischargeReasons').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Discharge Reasons"
        }
    });
    oTableDischarge.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#DischargeReasons").on("click",".EnableDisable",function () {
        var checkbox = $(this);
        var nRow = checkbox.parents('tr')[0];
        var aData = oTableDischarge.fnGetData(nRow);
        var checked = checkbox.is(":checked");
        var dischargeContextId = checkbox.hasClass('ER') ? 0 : 1;
        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "HOD",
                hodCode: dischargeContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Discharge has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#DischargeReasons").on("click", ".ReportFlg", function () {
        var checkbox = $(this);
        var checked = checkbox.is(":checked");
        var nRow = checkbox.parents('tr')[0];
        var aData = oTableDischarge.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlgReport",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "HOD",
                hodCode: dischargeContextId
            },
            success: function (result) {
                if (result.Success) {
                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Discharge has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#DischargeReasons").on("click", ".TopN", function () {
        var checkbox = $(this);
        var checked = checkbox.is(":checked");
        var nRow = checkbox.parents('tr')[0];
        var aData = oTableDischarge.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlgTopN",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Discharge has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $('#DischargeReasons').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableDischarge, nEditing);
            editRow(oTableDischarge, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowDischarge(oTableDischarge, nEditing, nRowId, dischargeContextId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableDischarge, nRow);
            nEditing = nRow;

        }
    });
    $('#DischargeReasons').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableDischarge, nRow);
    });
    $("#DischargeReasons").on("click", " a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableDischarge.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "JSON/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "HOD",
                hodCode: dischargeContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableDischarge.fnDeleteRow(nRow);
                } else {
                    alert("Discharge has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#Discharge").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#Discharge form");
        var code = $("#NewDischarge_DischargeReasonDesc").val();
        if (alreadyExist(code, $("#DischargeReasons").dataTable())) {
            alert("Discharge Reason already exists");
            return false;
        }
        if (!form.valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: "POST",
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    var reason = result.data.DischargeReasonDesc;
                    var sort = result.data.SortOrder;
                    var erEnabled = result.data.ERDataEntryFlg;
                    var hospitalEnabled = result.data.HospitalDataEntryFlg;
                    var reportFlag = result.data.ReportFlg;
                    var Top_N = result.data.Top_N;
                    var id = result.data.DischargeReasonId;

                    var addId = $("#DischargeReasons").dataTable().fnAddData([
                        reason,
                        sort,
                        createCheckboxString("EREnableDisable", "EnableDisable ER", "EREnableDisable", erEnabled),
                        createCheckboxString("HospitalEnableDisable", "EnableDisable Hospital", "HospitalEnableDisable", hospitalEnabled),
                        createCheckboxString("ReportFlg", "ReportFlg", "ReportFlg", reportFlag),
                        createCheckboxString("TopN", "TopN", "TopN", Top_N),
                        '<a class="edit" href="">Edit</a>',
                        ''
                    ]);
                    var newRow = $("#DischargeReasons").dataTable().fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

}

function initDidNotReturnReasons() {
    var oTableDNRR = $('#DidNotReturnReasons').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "bScrollCollapse": true,
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '400px'
            },
            null,
            {
                "bSortable": false
            },
            {
                "bSortable": false
            },
            {
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Did Not Return Reasons"
        }
    });
    oTableDNRR.fnSort([[1, 'asc']]);
    var nEditingDNRR = null;

    $("#DidNotReturnReasons").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = checkbox.is(":checked");
        var nRow = checkbox.parents('tr')[0];
        var aData = oTableDNRR.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "HOD",
                hodCode: dnrrContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Did Not Return Reason has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#DidNotReturnReasons").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableDNRR.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "JSON/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "HOD",
                hodCode: dnrrContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableDNRR.fnDeleteRow(nRow);
                } else {
                    alert("Did Not Return Reason has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#DidNotReturnReasons').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingDNRR !== null && nEditingDNRR != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableDNRR, nEditingDNRR);
            editRowDNRR(oTableDNRR, nRow);
            nEditingDNRR = nRow;

        }
        else if (nEditingDNRR == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowDNRR(oTableDNRR, nEditingDNRR, nRowId, dnrrContextId);
            if (success) {
                nEditingDNRR = null;
            }

        }
        else {
            /* No row currently being edited */
            editRowDNRR(oTableDNRR, nRow);
            nEditingDNRR = nRow;

        }
    });
    $('#DidNotReturnReasons').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableDNRR, nRow);
    });

    $("#Return").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#Return form");
        var code = $("#NewDNRR_DidNotReturnReasonDesc").val();
        if (alreadyExist(code, $("#DidNotReturnReasons").dataTable())) {
            alert("Did Not Return Reason already exists");
            return false;
        }
        if (!form.valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: "POST",
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    var reason = result.data.DidNotReturnReasonDesc;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.DidNotReturnReasonId;

                    var addId = $("#DidNotReturnReasons").dataTable().fnAddData([
                        reason,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        ''
                    ]);
                    var newRow = $("#DidNotReturnReasons").dataTable().fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

}

function initCommunity() {
    //Community Table
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "600px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Law Firms"
        }
    });
    $(".checkbox").click(function () {
        var checkbox = $(this);
        var checked = checkbox.is(":checked");
        var comId = checkbox.attr("id");
        var comName = checkbox.parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "Base/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "HOD"
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Community has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $(".checkboxReport").click(function () {
        var checkbox = $(this);
        var checked = checkbox.is(":checked");
        var comId = checkbox.attr("id");
        var comName = checkbox.parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "Base/ChangeReportFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "HOD"
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $(".checkboxPayer").click(function () {
        var checkbox = $(this);
        var checked = checkbox.is(":checked");
        var payer = checkbox.attr("id");
        var comName = checkbox.parents("tr").children(":first-child").text();
        var nRowId = checkbox.closest('tr').attr("id");

        $.ajax({
            url: path + "Base/ChangePayerIncludeFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: nRowId,
                dFlag: checked,
                appCode: "HOD",
                payer: payer
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#SaveLookback").validate({
        rules: {
            LookbackDays: {
                required: true,
                number: true
            }
        }
    });

    //Hospital Associations
    var hospitalAssociationModal = $("#hospitalAssociationModal");
    var hospitalSelectionsTable;
    $(".select-hospitals").on('click', function (e) {
        e.preventDefault();
        $.ajax({
            url: this.href,
            success: function (data) {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                hospitalAssociationModal.html(data);
                hospitalAssociationModal.show();
                var top = Math.max($(window).height() / 2 - hospitalAssociationModal[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - hospitalAssociationModal[0].offsetWidth / 2, 0);
                hospitalAssociationModal.css({ top: top, left: left });
                hospitalSelectionsTable = $('#selectedHospitals', hospitalAssociationModal).dataTable({
                    "bFilter": false,
                    "bAutoWidth": false,
                    "sScrollY": "370px",
                    "sDom": "frtS",
                    "iDisplayLength": -1,
                    "aoColumns": [
                        {
                            "sWidth": "500px"
                        },
                        {
                            "sWidth": "100px",
                            "bSortable": false
                        }
                    ],
                    "oLanguage": {
                        "sEmptyTable": "No Hospitals"
                    }
                });
            },
            error: function () {
                alert("Could not communicate with the server.");
            }
        });
    });
    hospitalAssociationModal.on('click', '#close', function () {
        $(".modal").hide();
        hospitalAssociationModal.hide();
    });

    hospitalAssociationModal.on('change', '.selectHospital', function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = hospitalSelectionsTable.fnGetData(nRow);
        var communityId = $("#CommunityId").val();
        $.ajax({
            url: path + "HospitalDischargeAdmin/ChangeHospitalAssociation",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                hospitalId: nRowId,
                communityId: communityId,
                isAssociated: checked
            },
            success: function (result) {
                if (result.Success) {
                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Hospital has failed to be associated. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            },
            error: function () {
                alert("Error communicating with server.");
            }
        });
    });
}

function initHospitals() {
    //Hospital Table
    var oTableHospital = $('#Hospitals').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "bScrollCollapse": true,
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '400px'
            },
            null,
            {
                "bSortable": false
            },
            {
                "bSortable": false
            },
            {
                "bSortable": false
            },
            {
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Hospitals"
        }
    });
    oTableHospital.fnSort([[1, 'asc']]);
    var nEditingHospital = null;

    $("#Hospitals").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = checkbox.is(":checked");
        var nRow = checkbox.parents('tr')[0];
        var aData = oTableHospital.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "HOD",
                hodCode: hospitalContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Hospital has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#Hospitals").on("click", ".ReportFlg", function () {
        var checkbox = $(this);
        var checked = checkbox.is(":checked");
        var nRow = checkbox.parents('tr')[0];
        var aData = oTableHospital.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlgReport",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "HOD",
                hodCode: hospitalContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Hospital has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $('#Hospitals').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingHospital !== null && nEditingHospital != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableHospital, nEditingHospital);
            editRowHospital(oTableHospital, nRow);
            nEditingHospital = nRow;
        }
        else if (nEditingHospital == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowHospital(oTableHospital, nEditingHospital, nRowId, hospitalContextId);
            if (success) {
                nEditingHospital = null;
            }

        }
        else {
            /* No row currently being edited */
            editRowHospital(oTableHospital, nRow);
            nEditingHospital = nRow;

        }
    });
    $('#Hospitals').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableHospital, nRow);
    });
    $("#Hospitals").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableHospital.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "JSON/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "HOD",
                hodCode: hospitalContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableHospital.fnDeleteRow(nRow);
                } else {
                    alert("Hospital has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#Hospital").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#Hospital form");
        var code = $("#NewDNRR_DidNotReturnReasonDesc").val();
        if (alreadyExist(code, $("#Hospitals").dataTable())) {
            alert("Did Not Return Reason already exists");
            return false;
        }
        if (!form.valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: "POST",
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    var name = result.data.Name;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.AllowDataEntry;
                    var report = result.data.AllowReporting;
                    var id = result.data.DidNotReturnReasonId;

                    var addId = $("#Hospitals").dataTable().fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        createCheckboxString("EnableDisable", "ReportFlg", "EnableDisable", report),
                        '<a class="edit" href="">Edit</a>',
                        ''
                    ]);
                    var newRow = $("#Hospitals").dataTable().fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

}

$(document).ready(function () {
    $("#tabs").tabs();

    initDischarge();
    initDidNotReturnReasons();
    initHospitals();
    initCommunity();


    $("#tabs").tabs({
        activate: function (event, ui) {
            var dataTables = ui.newPanel.find(".dataTable").filter(function () {
                return this.id != "";
            });
            for (i = 0; i < dataTables.length; i++) {
                var dataTable = $("#" + dataTables[i].id).dataTable();
                dataTable.fnAdjustColumnSizing();
            }
        }
    });
});

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});

function alreadyExist(code, dataTable, index) {
    if (!index) {
        index = 0;
    }
    var dataSet = dataTable.dataTable().fnGetData();
    for (var i = 0; i < dataSet.length; i++) {
        var existingCode = dataSet[i][index];
        if (existingCode.toLowerCase() === code.toLowerCase()) {
            return true;
        }
    }
    return false;
}

function restoreRow(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);

    for (var i = 0, iLen = jqTds.length; i < iLen; i++) {
        oTable.fnUpdate(aData[i], nRow, i, false);
    }
}
function editRow(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    jqTds[0].innerHTML = '<input type="text" value="' + aData[0] + '"/>';
    jqTds[1].innerHTML = '<input type="text" value="' + aData[1] + '"/>';
    jqTds[6].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}
function saveRowDischarge(oTable, nRow, nRowId, contextId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value,
            appCode: "HOD",
            hodCode: contextId
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 6, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });

    //oTable.fnStandingRedraw();
    return true;
}

function editRowDNRR(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    jqTds[0].innerHTML = '<input type="text" value="' + aData[0] + '"/>';
    jqTds[1].innerHTML = '<input type="text" value="' + aData[1] + '"/>';
    jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRowDNRR(oTable, nRow, nRowId, contextId) {
    var jqInputs = $('input', nRow);
    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value,
            appCode: "HOD",
            hodCode: contextId
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });

    //oTable.fnStandingRedraw();
    return true;
}
function editRowHospital(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    jqTds[0].innerHTML = '<input type="text" value="' + aData[0] + '"/>';
    jqTds[1].innerHTML = '<input type="text" value="' + aData[1] + '"/>';
    jqTds[4].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRowHospital(oTable, nRow, nRowId, contextId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value,
            appCode: "HOD",
            hodCode: contextId
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 4, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });

    //oTable.fnStandingRedraw();
    return true;
}
function createCheckboxString(id, cssClass, name, value) {
    if (value) {
        var checked = "checked";
    }
    var html = '<input class="' + cssClass + '" id="' + id + '" name="' + name + '" type="checkbox" value="' + value + '" ' + checked + '>';
    html += '<input name="' + name + '" type="hidden" value="' + value + '">';
    return html;
}