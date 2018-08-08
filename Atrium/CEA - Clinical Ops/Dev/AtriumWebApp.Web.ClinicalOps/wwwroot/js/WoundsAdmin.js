$(document).ready(function () {
    var pressureContextId = 0;
    var compositeContextId = 1;

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

    $("#PRWND .EnableDisable").click(function () {
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
                    var message = aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Pressure Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#PRWND .ReportFlg").click(function () {
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
                    var message = aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Pressure Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#PRWND .severity-level").live("change", function (e) {

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
                    var message = aData[0] + "'s severity level has been successfully updated";


                    alert(message);
                } else {

                    alert("The severity level was not successfully updated.");
                }

            }

        });

    });

    $("#PRWND .WoundFlg").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTablePRWND.fnGetData(nRow);
        var woundFlgCode = checkbox.data("woundflg");
        var woundFlgName = checkbox.data("woundflg-name");

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
                    var message = woundFlgName + " has been successfully ";  //aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Pressure Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#PRWND a.delete").live("click", function (e) {
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
    $('#PRWND a.edit').live('click', function (e) {
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
    $('#PRWND a.cancel').live('click', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTablePRWND, nRow);
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
        jqTds[0].innerHTML = '<input type="text" size="45" value="' + aData[0] + '"/>';
        jqTds[1].innerHTML = '<input type="text" size="3" value="' + aData[1] + '"/>';
        jqTds[4].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
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

    $("#SCWND .EnableDisable").click(function () {
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
                    var message = aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Composite Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#SCWND .ReportFlg").click(function () {
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
                    var message = aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Composite Wound has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#SCWND a.delete").live("click", function (e) {
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
    $('#SCWND a.edit').live('click', function (e) {
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
    $('#SCWND a.cancel').live('click', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableSCWND, nRow);
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
});