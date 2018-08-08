$(document).ready(function () {
    var locationContextId = 2;
    var injuryContextId = 3;
    var treatmentContextId = 4;
    var interventionContextId = 5;
    var typeContextId = 6;

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

    $("#location .EnableDisable").click(function () {
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
                    var message = aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Location has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#location a.delete").live("click", function (e) {
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
    $('#location a.edit').live('click', function (e) {
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
    $('#location a.cancel').live('click', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableLocation, nRow);
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
        jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
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

    $("#injury .EnableDisable").click(function () {
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
                    var message = aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Injury Type has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#injury .Threshold").click(function () {
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
                    var message = aData[0] + " Threshold has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Injury Type Threshold has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#injury a.delete").live("click", function (e) {
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
    $('#injury a.edit').live('click', function (e) {
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
    $('#injury a.cancel').live('click', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableInjury, nRow);
    });

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

    $("#treatment .EnableDisable").click(function () {
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
                    var message = aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Treatment has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#treatment .Threshold").click(function () {
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
                    var message = aData[0] + " Threshold has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Treatment Threshold has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#treatment a.delete").live("click", function (e) {
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
    $('#treatment a.edit').live('click', function (e) {
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
    $('#treatment a.cancel').live('click', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableTreat, nRow);
    });

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

    $("#intervention .EnableDisable").click(function () {
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
                    var message = aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Intervention has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#intervention a.delete").live("click", function (e) {
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
    $('#intervention a.edit').live('click', function (e) {
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
    $('#intervention a.cancel').live('click', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableInter, nRow);
    });

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

    $("#type .EnableDisable").click(function () {
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
                    var message = aData[0] + " has been successfully ";
                    if (checked)
                        message += "enabled.";
                    else
                        message += "disabled.";
                    alert(message);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Fall Injury Type has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#type a.delete").live("click", function (e) {
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
    $('#type a.edit').live('click', function (e) {
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
    $('#type a.cancel').live('click', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableType, nRow);
    });
});