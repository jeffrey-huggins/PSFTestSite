var measureContextId = 10;
var antiPsychoticContextId = 7;
var catheterContextId = 8;
var locationContextId = 2;
var injuryContextId = 3;
var treatmentContextId = 4;
var interventionContextId = 5;
var typeContextId = 6;
var restraintContextId = 9;
var pressureContextId = 0;
var compositeContextId = 1;
var antiPsychoticMedContextId = 11;

function initComposite() {
    var oTableSCWND = $('#SCWND').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No Composite Wound Descriptions"
        }
    });
    oTableSCWND.fnSort([[1, 'asc']]);
    var nEditingSC = null;

    $("#SCWND").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableSCWND.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: compositeContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Composite Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#SCWND").on("click", ".ReportFlg", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableSCWND.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlgReport",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: compositeContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Composite Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#SCWND").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableSCWND.fnGetData(nRow);

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
                appCode: "SOC",
                socCode: compositeContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableSCWND.fnDeleteRow(nRow);
                } else {
                    alert("Composite Wound has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#SCWND').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingSC !== null && nEditingSC != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableSCWND, nEditingSC);
            editRow(oTableSCWND, nRow);
            nEditingSC = nRow;

        }
        else if (nEditingSC == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowSCWND(oTableSCWND, nEditingSC, nRowId);
            if (success) {
                nEditingSC = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableSCWND, nRow);
            nEditingSC = nRow;

        }
    });
    $('#SCWND').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableSCWND, nRow);
    });

    $("#compositeWoundTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#compositeWoundTab form");
        var code = $("#NewCompositeWound_CompositeWoundDescribeName").val();
        var dataTable = $("#SCWND").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Site already exists");
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
                    var name = result.data.CompositeWoundDescribeName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var report = result.data.ReportFlg;
                    var id = result.data.CompositeWoundDescribeId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        createCheckboxString("ReportFlg", "ReportFlg", "ReportFlg", report),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initPressure() {
    var oTablePRWND = $('#PRWND').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            },
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
            },
            {
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Stages"
        }
    });
    oTablePRWND.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#PRWND").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTablePRWND.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: pressureContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Pressure Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#PRWND").on("click", ".ReportFlg", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTablePRWND.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlgReport",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: pressureContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Pressure Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#PRWND").on("change", ".severity-level", function (e) {

        var nRow = $(this).parents('tr')[0];
        var aData = oTablePRWND.fnGetData(nRow);
        var severityLevel = $(this).val();

        $.ajax({
            url: path + "JSON/UpdateStageSeverity",
            dataType: "json",
            type: "POST",
            data: {
                id: $(nRow).attr("id"),
                severityLevel: severityLevel
            },
            success: function (result) {
                if (result.Success) {

                } else {

                    alert("The severity level was not successfully updated.");
                }

            }

        });

    });
    $("#PRWND").on("click", ".WoundFlg", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTablePRWND.fnGetData(nRow);
        var woundFlgCode = checkbox.data("woundflg");
        var woundFlgName = checkbox.attr("name");

        switch (woundFlgName) {
            case("ThresholdFlg"):
                woundFlgCode = 0;
                break;
            case("LengthFlg"):
                woundFlgCode = 1;
                break;
            case("WidthFlg"):
                woundFlgCode = 2;
                break;
            case("DepthFlg"):
                woundFlgCode = 3;
                break;
        }
        $.ajax({
            url: path + "JSON/ChangeDataFlgWound",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: pressureContextId,
                socWoundFlg: woundFlgCode
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Pressure Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#PRWND").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTablePRWND.fnGetData(nRow);

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
                appCode: "SOC",
                socCode: pressureContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTablePRWND.fnDeleteRow(nRow);
                } else {
                    alert("Pressure Wound has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#PRWND').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTablePRWND, nEditing);
            editRow(oTablePRWND, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowPRWND(oTablePRWND, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTablePRWND, nRow);
            nEditing = nRow;

        }
    });
    $('#PRWND').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTablePRWND, nRow);
    });

    $("#pressureWoundTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#pressureWoundTab form");
        var code = $("#NewPressureWound_PressureWoundStageName").val();
        var dataTable = $("#PRWND").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Site already exists");
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
                    var name = result.data.PressureWoundStageName;
                    var level = result.data.SeverityLevelNbr;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var report = result.data.ReportFlg;
                    var threshold = result.data.IncludeInThresholdFlg;
                    var length = result.data.LengthFlg;
                    var width = result.data.WidthFlg;
                    var depth = result.data.DepthFlg;

                    var id = result.data.PressureWoundStageId;
                    var pressureWoundLevelHtml = $("#hiddenServerityLevels").html();
                    if (level === null) {
                        level = "";
                    }
                    pressureWoundLevelHtml = $(pressureWoundLevelHtml).find("option[value='" + level + "']").attr("selected", "selected").parent()[0].outerHTML;

                    var addId = dataTable.fnAddData([
                        name,
                        pressureWoundLevelHtml,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        createCheckboxString("ReportFlg", "ReportFlg", "ReportFlg", report),
                        createCheckboxString("ThresholdFlg", "WoundFlg", "ThresholdFlg", threshold),
                        createCheckboxString("LengthFlg", "WoundFlg", "LengthFlg", length),
                        createCheckboxString("WidthFlg", "WoundFlg", "WidthFlg", width),
                        createCheckboxString("DepthFlg", "WoundFlg", "DepthFlg", depth),
                        '',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initRestraint() {
    var oTableRest = $('#restraint').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No Restraints"
        }
    });
    oTableRest.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#restraint").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableRest.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: restraintContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Restraint has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#restraint").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableRest.fnGetData(nRow);

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
                appCode: "SOC",
                socCode: restraintContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableRest.fnDeleteRow(nRow);
                } else {
                    alert("Restraint has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#restraint').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableRest, nEditing);
            editRow(oTableRest, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowRest(oTableRest, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableRest, nRow);
            nEditing = nRow;

        }
    });
    $('#restraint').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableRest, nRow);
    });

    $("#restraintTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#restraintTab form");
        var code = $("#NewRestraint_SOCRestraintName").val();
        var dataTable = $("#restraint").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Site already exists");
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
                    var name = result.data.SOCRestraintName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.SOCRestraintId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initFallType() {
    var oTableType = $('#type').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No Fall Injury Types"
        }
    });
    oTableType.fnSort([[1, 'asc']]);
    var nEditingType = null;

    $("#type").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableType.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: typeContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Injury Type has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#type").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableType.fnGetData(nRow);

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
                appCode: "SOC",
                socCode: typeContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableType.fnDeleteRow(nRow);
                } else {
                    alert("Fall Injury Type has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#type').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingType !== null && nEditingType != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableType, nEditingType);
            editRow(oTableType, nRow);
            nEditingType = nRow;

        }
        else if (nEditingType == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(oTableType, nEditingType, nRowId, typeContextId);
            if (success) {
                nEditingType = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableType, nRow);
            nEditingType = nRow;

        }
    });
    $('#type').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableType, nRow);
    });

    $("#fallTypeTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#fallTypeTab form");
        var code = $("#NewFallType_SOCFallTypeName").val();
        var dataTable = $("#type").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Fall type already exists");
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
                    var name = result.data.SOCFallTypeName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.SOCFallTypeId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initFallIntervention() {
    var oTableInter = $('#intervention').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No Fall Injury Types"
        }
    });
    oTableInter.fnSort([[1, 'asc']]);
    var nEditingInter = null;

    $("#intervention").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableInter.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: interventionContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Intervention has failed to be enabled/disabled. Please try again " +
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
                appCode: "SOC",
                socCode: interventionContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableInter.fnDeleteRow(nRow);
                } else {
                    alert("Fall Intervention has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#intervention').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingInter !== null && nEditingInter != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableInter, nEditingInter);
            editRow(oTableInter, nRow);
            nEditingInter = nRow;

        }
        else if (nEditingInter == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(oTableInter, nEditingInter, nRowId, interventionContextId);
            if (success) {
                nEditingInter = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableInter, nRow);
            nEditingInter = nRow;

        }
    });
    $('#intervention').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableInter, nRow);
    });

    $("#fallInterventionTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#fallInterventionTab form");
        var code = $("#NewFallIntervention_SOCFallInterventionName").val();
        var dataTable = $("#intervention").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Fall type already exists");
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
                    var name = result.data.SOCFallInterventionName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.SOCFallInterventionId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initFallTreatment() {
    var oTableTreat = $('#treatment').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No Fall Treatments"
        }
    });
    oTableTreat.fnSort([[1, 'asc']]);
    var nEditingTreat = null;

    $("#treatment").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableTreat.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: treatmentContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Treatment has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#treatment").on("click", ".Threshold", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableTreat.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeThresholdFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: treatmentContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Treatment Threshold has failed to be enabled/disabled. Please try again " +
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
                appCode: "SOC",
                socCode: treatmentContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableTreat.fnDeleteRow(nRow);
                } else {
                    alert("Fall Treatment has failed to be deleted. Please try again " +
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
            editRow(oTableTreat, nRow);
            nEditingTreat = nRow;

        }
        else if (nEditingTreat == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(oTableTreat, nEditingTreat, nRowId, treatmentContextId);
            if (success) {
                nEditingTreat = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableTreat, nRow);
            nEditingTreat = nRow;

        }
    });
    $('#treatment').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableTreat, nRow);
    });

    $("#fallTreatmentTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#fallTreatmentTab form");
        var code = $("#NewFallTreatment_SOCFallTreatmentName").val();
        var dataTable = $("#treatment").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Fall treatment already exists");
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
                    var name = result.data.SOCFallTreatmentName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var threshold = result.data.IncludeInThresholdFlg;
                    var id = result.data.SOCFallTreatmentId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        createCheckboxString("Threshold", "Threshold", "Threshold", threshold),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initFallInjury() {
    var oTableInjury = $('#injury').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No Fall Injury Types"
        }
    });
    oTableInjury.fnSort([[1, 'asc']]);
    var nEditingInjury = null;

    $("#injury").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableInjury.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: injuryContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Injury Type has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#injury").on("click", ".Threshold", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableInjury.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeThresholdFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: injuryContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Injury Type Threshold has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#injury").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableInjury.fnGetData(nRow);

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
                appCode: "SOC",
                socCode: injuryContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableInjury.fnDeleteRow(nRow);
                } else {
                    alert("Fall Injury Type has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#injury').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingInjury !== null && nEditingInjury != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableInjury, nEditingInjury);
            editRow(oTableInjury, nRow);
            nEditingInjury = nRow;

        }
        else if (nEditingInjury == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(oTableInjury, nEditingInjury, nRowId, injuryContextId);
            if (success) {
                nEditingInjury = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableInjury, nRow);
            nEditingInjury = nRow;

        }
    });
    $('#injury').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableInjury, nRow);
    });

    $("#fallInjuryTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#fallInjuryTab form");
        var code = $("#NewFallInjury_SOCFallInjuryTypeName").val();
        var dataTable = $("#injury").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Fall injury type already exists");
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
                    var name = result.data.SOCFallInjuryTypeName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var threshold = result.data.IncludeInThresholdFlg;
                    var id = result.data.SOCFallInjuryTypeId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        createCheckboxString("Threshold", "Threshold", "Threshold", threshold),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initFallLocation() {
    var oTableLocation = $('#location').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No Fall Locations"
        }
    });
    oTableLocation.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#location").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableLocation.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: locationContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Location has failed to be enabled/disabled. Please try again " +
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
                appCode: "SOC",
                socCode: locationContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableLocation.fnDeleteRow(nRow);
                } else {
                    alert("Fall Location has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#location').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableLocation, nEditing);
            editRow(oTableLocation, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(oTableLocation, nEditing, nRowId, locationContextId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableLocation, nRow);
            nEditing = nRow;

        }
    });
    $('#location').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableLocation, nRow);
    });

    $("#fallLocationTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#fallLocationTab form");
        var code = $("#NewFallLocation_SOCFallLocationName").val();
        var dataTable = $("#location").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Fall location already exists");
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
                    var name = result.data.SOCFallLocationName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.SOCFallLocationId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initCatheter() {
    var oTableCath = $('#catheter').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
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
            "sEmptyTable": "No Catheters"
        }
    });
    oTableCath.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#catheter").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableCath.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: catheterContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Catheter has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#catheter").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableCath.fnGetData(nRow);

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
                appCode: "SOC",
                socCode: catheterContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableCath.fnDeleteRow(nRow);
                } else {
                    alert("Catheter has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#catheter').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableCath, nEditing);
            editRow(oTableCath, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowCath(oTableCath, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableCath, nRow);
            nEditing = nRow;

        }
    });

    $('#catheter').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableCath, nRow);
    });

    $("#catheterTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#catheterTab form");
        var code = $("#NewCatheter_SOCCatheterTypeName").val();
        var dataTable = $("#catheter").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Catheter type already exists");
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
                    var name = result.data.SOCCatheterTypeName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.SOCCatheterTypeId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initAntipsychotic() {
    var oTableAnti = $('#anti').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No AntiPsychotic Diagnoses"
        }
    });
    oTableAnti.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#anti").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableAnti.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: antiPsychoticContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("AntiPsychotic has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#anti").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableAnti.fnGetData(nRow);

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
                appCode: "SOC",
                socCode: antiPsychoticContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableAnti.fnDeleteRow(nRow);
                } else {
                    alert("AntiPsychotic has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#anti').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableAnti, nEditing);
            editRow(oTableAnti, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowAnti(oTableAnti, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableAnti, nRow);
            nEditing = nRow;

        }
    });
    $('#anti').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableAnti, nRow);
    });

    $("#antipsychoticTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#antipsychoticTab form");
        var code = $("#NewAntipsychotic_AntiPsychoticDiagnosisDesc").val();
        var dataTable = $("#anti").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Antipsychotic diagnosis already exists");
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
                    var name = result.data.AntiPsychoticDiagnosisDesc;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.AntiPsychoticDiagnosisId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initAntipsychoticMed() {
    var oTableAnti = $('#antiMed').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No AntiPsychotic Medications"
        }
    });
    oTableAnti.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#antiMed").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableAnti.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: antiPsychoticMedContextId
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("AntiPsychotic has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#antiMed").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableAnti.fnGetData(nRow);

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
                appCode: "SOC",
                socCode: antiPsychoticMedContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableAnti.fnDeleteRow(nRow);
                } else {
                    alert("AntiPsychotic has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#antiMed').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableAnti, nEditing);
            editRow(oTableAnti, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowAntiMed(oTableAnti, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableAnti, nRow);
            nEditing = nRow;

        }
    });
    $('#antiMed').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableAnti, nRow);
    });

    $("#antipsychoticMedTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#antipsychoticMedTab form");
        var code = $("#AntiPsychoticMedicationName").val();
        var dataTable = $("#antiMed").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Antipsychotic medication already exists");
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
                    var name = result.data.AntiPsychoticMedicationName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.AntiPsychoticMedicationId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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

function initMeasures() {
    var oTableMeasures = $('#Measures').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
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
            "sEmptyTable": "No Quality Measures"
        }
    });
    oTableMeasures.fnSort([[1, 'asc']]);
    var nEditing = null;
    $("#Measures").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableMeasures.fnGetData(nRow);

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
                appCode: "SOC",
                socCode: measureContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableMeasures.fnDeleteRow(nRow);
                } else {
                    alert("Measure has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#Measures').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableMeasures, nEditing);
            editRow(oTableMeasures, nRow, 4);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowMeasure(oTableMeasures, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableMeasures, nRow,4);
            nEditing = nRow;

        }
    });
    $('#Measures').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableMeasures, nRow);
    });
    $("#Measures").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableMeasures.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: measureContextId
            },
            success: function (result) {
                if (result.Success) {

                    $("#QMeasures option:selected").val(checked);
                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Measure has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#Measures").on("click", ".ReportFlg", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableMeasures.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlgReport",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "SOC",
                socCode: measureContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Measure has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#measuresTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#measuresTab form");
        var code = $("#NewMeasure_SOCMeasureName").val();
        var dataTable = $("#Measures").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Measure already exists");
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
                    var name = result.data.SOCMeasureName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var report = result.data.ReportFlg;
                    var id = result.data.SOCMeasureId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        createCheckboxString("ReportFlg", "ReportFlg", "ReportFlg", report),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
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
    //Community Table
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
                appCode: "SOC"
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
                appCode: "SOC"
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
                appCode: "SOC",
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
    $("#fallsTab").tabs();
    initMeasures();
    initCommunities();
    initAntipsychotic();
    initAntipsychoticMed();
    initCatheter();
    initFallLocation();
    initFallInjury();
    initFallTreatment();
    initFallIntervention();
    initFallType();
    initRestraint();
    initPressure();
    initComposite();

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
    $("#fallsTab").tabs({
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

function saveRowSCWND(oTable, nRow, nRowId) {
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
            appCode: "SOC",
            socCode: compositeContextId
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

function saveRowPRWND(oTable, nRow, nRowId) {
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
            appCode: "SOC",
            socCode: pressureContextId
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

function saveRowRest(oTable, nRow, nRowId) {
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
            appCode: "SOC",
            socCode: restraintContextId
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

function restoreRow(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);

    for (var i = 0, iLen = jqTds.length; i < iLen; i++) {
        oTable.fnUpdate(aData[i], nRow, i, false);
    }
}

function editRow(oTable, nRow, editIndex) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    if (!editIndex) {
        editIndex = 3
    }
    jqTds[0].innerHTML = '<input type="text" size="60" value="' + aData[0] + '"/>';
    jqTds[1].innerHTML = '<input type="text" size="3" value="' + aData[1] + '"/>';
    jqTds[editIndex].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRow(oTable, nRow, nRowId, contextId) {
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
            appCode: "SOC",
            socCode: contextId
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

function saveRowAntiMed(oTable, nRow, nRowId) {
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
            appCode: "SOC",
            socCode: antiPsychoticMedContextId
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

function saveRowAnti(oTable, nRow, nRowId) {
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
            appCode: "SOC",
            socCode: antiPsychoticContextId
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

function saveRowMeasure(oTable, nRow, nRowId) {
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
            appCode: "SOC",
            socCode: measureContextId
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

function saveRowCath(oTable, nRow, nRowId) {
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
            appCode: "SOC",
            socCode: catheterContextId
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

function createCheckboxString(id, cssClass, name, value) {
    if (value) {
        var checked = "checked";
    }
    var html = '<input class="' + cssClass + '" id="' + id + '" name="' + name + '" type="checkbox" value="' + value + '" ' + checked + '>';
    html += '<input name="' + name + '" type="hidden" value="' + value + '">';
    return html;
}