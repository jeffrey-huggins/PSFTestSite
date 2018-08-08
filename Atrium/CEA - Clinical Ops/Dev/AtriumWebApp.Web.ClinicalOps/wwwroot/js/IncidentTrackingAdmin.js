var incidentContextId = 0;
var locationContextId = 1;
var interventionContextId = 2;
var treatmentContextId = 3;
var regionalNurseId;

function initIncident() {
    var oTableIncident = $('#IncidentTypes').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Incident Types"
        }
    });
    oTableIncident.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#IncidentTypes").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var dataFlg = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableIncident.fnGetData(nRow);
        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: dataFlg,
                appCode: "ITR",
                itrCode: incidentContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Incident has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    $("#IncidentTypes").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableIncident.fnGetData(nRow);

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
                appCode: "ITR",
                itrCode: incidentContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableIncident.fnDeleteRow(nRow);
                } else {
                    alert("Incident has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });

    });

    //Edit
    $('#IncidentTypes').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableIncident, nEditing);
            editRow(oTableIncident, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowIncident(oTableIncident, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableIncident, nRow);
            nEditing = nRow;

        }
    });
    $('#IncidentTypes').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableIncident, nRow);
    });

    $("#admin-body").on("click",".ReportFlg", function () {
        var checkbox = $(this);
        var dataFlg = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableIncident.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlgReport",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: dataFlg,
                appCode: "ITR"
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Incident has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#incidentsTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#incidentsTab form");
        var code = $("#NewIncidentType_PatientIncidentName").val();
        if (alreadyExist(code, $("#IncidentTypes").dataTable())) {
            alert("Incident type already exists");
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
                    var type = result.data.PatientIncidentName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var report = result.data.ReportFlg;
                    var id = result.data.PatientIncidentTypeId;

                    var addId = $("#IncidentTypes").dataTable().fnAddData([
                        type,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        createCheckboxString("ReportFlg", "ReportFlg", "ReportFlg",report),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $("#IncidentTypes").dataTable().fnSettings().aoData[addId[0]].nTr;
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

function initLocations() {
    var oTableLocation = $('#location').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Locations"
        }
    });
    oTableLocation.fnSort([[1, 'asc']]);
    var nEditingLocation = null;

    $("#location").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var dataFlg = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableLocation.fnGetData(nRow);
        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: dataFlg,
                appCode: "ITR",
                itrCode: locationContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Location has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });
    $("#location").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableLocation.fnGetData(nRow);

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
                appCode: "ITR",
                itrCode: locationContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableLocation.fnDeleteRow(nRow);
                } else {
                    alert("Location has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });

    });

    //Edit
    $('#location').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingLocation !== null && nEditingLocation != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableLocation, nEditingLocation);
            editRowNoReport(oTableLocation, nRow);
            nEditingLocation = nRow;

        }
        else if (nEditingLocation == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowLocation(oTableLocation, nEditingLocation, nRowId);
            if (success) {
                nEditingLocation = null;
            }

        }
        else {
            /* No row currently being edited */
            editRowNoReport(oTableLocation, nRow);
            nEditingLocation = nRow;

        }
    });
    $('#location').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableLocation, nRow);
    });

    $("#locationTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#locationTab form");
        var code = $("#NewLocation_PatientIncidentLocationName").val();
        if (alreadyExist(code, $("#location").dataTable())) {
            alert("Location already exists");
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
                    var type = result.data.PatientIncidentLocationName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.PatientIncidentLocationId;

                    var addId = $("#location").dataTable().fnAddData([
                        type,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $("#location").dataTable().fnSettings().aoData[addId[0]].nTr;
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

function initInterventions() {
    var oTableInter = $('#intervention').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Intervention Types"
        }
    });
    oTableInter.fnSort([[1, 'asc']]);
    var nEditingInter = null;

    $("#intervention").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var dataFlg = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableInter.fnGetData(nRow);
        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: dataFlg,
                appCode: "ITR",
                itrCode: interventionContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Intervention has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });
    $("#intervention").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableInter.fnGetData(nRow);

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
                appCode: "ITR",
                itrCode: interventionContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableInter.fnDeleteRow(nRow);
                } else {
                    alert("Intervention has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });

    });

    //Edit
    $('#intervention').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingInter !== null && nEditingInter != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableInter, nEditingInter);
            editRowNoReport(oTableInter, nRow);
            nEditingInter = nRow;

        }
        else if (nEditingInter == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowInter(oTableInter, nEditingInter, nRowId);
            if (success) {
                nEditingInter = null;
            }

        }
        else {
            /* No row currently being edited */
            editRowNoReport(oTableInter, nRow);
            nEditingInter = nRow;

        }
    });
    $('#intervention').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableInter, nRow);
    });

    $("#interventionTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#interventionTab form");
        var code = $("#NewIntervention_PatientIncidentInterventionName").val();
        if (alreadyExist(code, $("#intervention").dataTable())) {
            alert("Intervention already exists");
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
                    var type = result.data.PatientIncidentInterventionName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.PatientIncidentInterventionId;

                    var addId = $("#intervention").dataTable().fnAddData([
                        type,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $("#intervention").dataTable().fnSettings().aoData[addId[0]].nTr;
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

function initTreatments() {
    var oTableTreat = $('#treatment').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Treatments"
        }
    });
    oTableTreat.fnSort([[1, 'asc']]);
    var nEditingTreat = null;

    $("#treatment").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var dataFlg = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableTreat.fnGetData(nRow);
        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: dataFlg,
                appCode: "ITR",
                itrCode: treatmentContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Treatment has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });
    $("#treatment").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableTreat.fnGetData(nRow);

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
                appCode: "ITR",
                itrCode: treatmentContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableTreat.fnDeleteRow(nRow);
                } else {
                    alert("Treatment has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });

    });

    //Edit
    $('#treatment').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingTreat !== null && nEditingTreat != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableTreat, nEditingTreat);
            editRowNoReport(oTableTreat, nRow);
            nEditingTreat = nRow;

        }
        else if (nEditingTreat == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowTreat(oTableTreat, nEditingTreat, nRowId);
            if (success) {
                nEditingTreat = null;
            }

        }
        else {
            /* No row currently being edited */
            editRowNoReport(oTableTreat, nRow);
            nEditingTreat = nRow;

        }
    });
    $('#treatment').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableTreat, nRow);
    });

    $("#treatmentTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#treatmentTab form");
        var code = $("#NewTreatment_PatientIncidentTreatmentName").val();
        if (alreadyExist(code, $("#treatment").dataTable())) {
            alert("Treatment already exists");
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
                    var type = result.data.PatientIncidentTreatmentName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.PatientIncidentTreatmentId;

                    var addId = $("#treatment").dataTable().fnAddData([
                        type,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $("#treatment").dataTable().fnSettings().aoData[addId[0]].nTr;
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

function initCommunities() {
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "600px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Communities"
        }
    });

    var columns = oTableCom.fnSettings().aoColumns;
    for (var i = 0; i < columns.length; i++) {
        if (columns[i].sTitle == "RegionalNurseEmployeeIds") {
            regionalNurseId = i;
            oTableCom.fnSetColumnVis(i, false, true);
            break;
        }
    }

    $(".RegionalNurse").on('click', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableCom.fnGetData(nRow);

        $("#CommunityId").val(nRowId);
        $("#CommunityName").text(aData[0]);

        var regionalNurseIds = aData[regionalNurseId].split("|");

        $("#regionalNurses input").prop("checked", false);

        for (var i = 0; i < regionalNurseIds.length; i++) {
            if (regionalNurseIds[i] != "") {
                $("#regionalNurses tr#" + regionalNurseIds[i].toString() + " input").prop("checked", true);
            }
        }

        $("#regionalNurseModal").dialog({
            width: 800,
            open: function () {
                $("#regionalNurses").dataTable().fnAdjustColumnSizing();
            }
        });

    });

}

function initRegionalNurses() {
    var oTableRegionalNurses = $('#regionalNurses').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            {
                "bSortable": false
            }],
        "oLanguage": {
            "sEmptyTable": "No Regional Nurses"
        }
    });

    $(".selectRegionalNurse").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableRegionalNurses.fnGetData(nRow);
        var community = $("#CommunityId").val();

        $.ajax({
            url: path + "IncidentTrackingAdmin/ChangeRegionalNurse",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                nurseId: nRowId,
                community: community,
                rFlag: checked
            },
            success: function (result) {
                if (result.Success) {
                    var nRowCom = $("#CommunityTable tr#" + community)[0];
                    var oTableCom = $("#CommunityTable").dataTable();
                    oTableCom.fnUpdate(result.Nurses, nRowCom, regionalNurseId, true);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Regional Nurse has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
}

function initCloseAllEmployees() {
    var oTableCloseAllEmployee = $('#CloseAllCommunity').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "570px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            {
                "bSortable": false
            }],
        "oLanguage": {
            "sEmptyTable": "No Employees"
        }
    });
    $(".selectCloseAllCommunity").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableCloseAllEmployee.fnGetData(nRow);

        $.ajax({
            url: path + "IncidentTrackingAdmin/ChangeCloseAll",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                employeeId: nRowId,
                cFlag: checked
            },
            success: function (result) {
                if (result.Success) {


                } else {
                    checkbox.prop("checked", !checked);
                    alert("Employee has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
}

function initSharedEvents() {
    $(".checkbox").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "Base/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "ITR"
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
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "Base/ChangeReportFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "ITR"
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
   
    $("#close").click(function () {
        $(".modal").hide();
        $("#regionalNurseModal").hide();
    });    

    $(".checkboxPayer").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var payer = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        var nRowId = $(this).closest('tr').attr("id");

        $.ajax({
            url: path + "Base/ChangePayerIncludeFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: nRowId,
                dFlag: checked,
                appCode: "ITR",
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
}

$(document).ready(function () {
    $("#tabs").tabs();
    initCommunities();
    initIncident();
    initLocations();
    initInterventions();
    initTreatments();
    initRegionalNurses();
    initCloseAllEmployees();
    initSharedEvents();

    $("#SaveLookback").validate({
        rules: {
            LookbackDays: {
                required: true,
                number: true
            }
        }
    });

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

function submitType() {
    document.getElementById("NewType").submit();
}

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});

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
    jqTds[4].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRowIncident(oTable, nRow, nRowId) {
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
            appCode: "ITR",
            itrCode: incidentContextId
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
}
function saveRowLocation(oTable, nRow, nRowId) {
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
            appCode: "ITR",
            itrCode: locationContextId
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
}
function editRowNoReport(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    jqTds[0].innerHTML = '<input type="text" value="' + aData[0] + '"/>';
    jqTds[1].innerHTML = '<input type="text" size="2" value="' + aData[1] + '"/>';
    jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRowInter(oTable, nRow, nRowId) {
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
            appCode: "ITR",
            itrCode: interventionContextId
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
}

function saveRowTreat(oTable, nRow, nRowId) {
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
            appCode: "ITR",
            itrCode: treatmentContextId
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
}


function createCheckboxString(id, cssClass, name, value) {
    if (value) {
        var checked = "checked";
    }
    var html = '<input class="' + cssClass + '" id="' + id + '" name="' + name + '" type="checkbox" value="' + value + '" ' + checked + '>';
    html += '<input name="' + name + '" type="hidden" value="' + value + '">';
    return html;
}

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