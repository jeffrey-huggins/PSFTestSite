function codeExist(code, dataTable, index) {
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
    jqTds[0].innerHTML = '<input type="text" size="10" value="' + aData[0] + '"/>';
    jqTds[2].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function InitSurveyTypes() {
    var oTableSurveyType = $('#SurveyTypes').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Survey Types"
        }
    });
    var nEditing = null;

    $("#SurveyTypes").on('click', 'a.delete', function (e) {
        e.preventDefault();
        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm) {
            return;
        }
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path + "SurveyAdmin/DeleteRowSurveyType",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { rowId: nRowId },
            success: function (result) {
                if (result.Success) {
                    oTableSurveyType.fnDeleteRow(nRow);
                }
                else {
                    alert("Error: " + result.Message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $("#SurveyTypes").on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableSurveyType, nEditing);
            editRow(oTableSurveyType, nRow);
            nEditing = nRow;
        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowSurveyType(oTableSurveyType, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableSurveyType, nRow);
            nEditing = nRow;
        }
    });
    $('#SurveyTypes').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableSurveyType, nRow);
    });
    $("#survey").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var code = $("#SurveyType_SurveyTypeDesc").val();
        if (codeExist(code, $("#SurveyTypes").dataTable())) {
            alert("Survey Type already exists");
            return false;
        }
        if (!$("#survey form").valid()) {
            return false;
        }

        var form = $("#survey form");
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: 'POST',
            data: form.serialize(),
            success: function (result) {
                if (result.success) {
                    var id = result.data.SurveyTypeId;
                    var desc = result.data.SurveyTypeDesc;
                    var complaintFlag = result.data.ComplaintFlg;
                    var checked = "checked";
                    if (!complaintFlag) {
                        checked = "";
                    }
                    var addId = $('#SurveyTypes').dataTable().fnAddData([
                        desc,
                        '<input class="checkboxComplaint" id="' + id + '" name="ComplaintFlg" type="checkbox" value="' + complaintFlag + '" '+ checked + '>',
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $('#SurveyTypes').dataTable().fnSettings().aoData[addId[0]].nTr;
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

function saveRowSurveyType(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);
    $.ajax({
        url: path + "SurveyAdmin/EditRowSurveyType",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            complaintFlag: jqInputs[1].checked
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 2, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}

function InitSAS() {
    var oTableSAS = $('#SAS').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Scope and Severity"
        }
    });
    var nEditingSAS = null;

    $("#SAS").on('click', 'a.delete', function (e) {
        e.preventDefault();
        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm) {
            return;
        }
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path + "SurveyAdmin/DeleteRowSAS",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { rowId: nRowId },
            success: function (result) {
                if (result.Success) {
                    oTableSAS.fnDeleteRow(nRow);
                }
                else {
                    alert("Error: " + result.Message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $("#SAS").on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingSAS !== null && nEditingSAS != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableSAS, nEditingSAS);
            editRow(oTableSAS, nRow);
            nEditingSAS = nRow;
        }
        else if (nEditingSAS == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowSAS(oTableSAS, nEditingSAS, nRowId);
            if (success) {
                nEditingSAS = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableSAS, nRow);
            nEditingSAS = nRow;
        }
    });
    $('#SAS').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableSAS, nRow);
    });
    $("#sasCode").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#sasCode form");
        var code = $("#SASModel_SASCode").val();
        if (codeExist(code, $("#SAS").dataTable())) {
            alert("SAS Code already exists");
            return false;
        }
        if (!$("#sasCode form").valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: 'POST',
            data: form.serialize(),
            success: function (result) {
                if (result.success) {
                    var id = result.data.SASId;
                    var code = result.data.SASCode;
                    var scope = result.data.Scope;
                    var severityLevel = result.data.SeverityLevel;
                    var addId = $("#SAS").dataTable().fnAddData([
                        code,
                        scope,
                        severityLevel,
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $('#SAS').dataTable().fnSettings().aoData[addId[0]].nTr;
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

function saveRowSAS(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);
    $.ajax({
        url: path + "SurveyAdmin/EditRowSAS",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 1, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}

function editRowFedDef(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    var selectTd = $(jqTds[1]);
    var options = $("#FedDef_Deficiency_AtriumPayerGroupCode").html();
    var select = $('<select>').attr('id', 'FedDefPayerGroup').html(options);
    select.val(selectTd.data('select-value'));
    jqTds[0].innerHTML = '<input type="text" size="10" value="' + aData[0] + '"/>';
    selectTd.empty().append(select);
    jqTds[2].innerHTML = '<textarea>' + aData[2] + '</textarea>';
    jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRowFedDef(oTable, nRow, nRowId) {
    var payerGroupSelect = $('#FedDefPayerGroup');
    var jqInputs = $('input', nRow);
    var desc = $('textarea', nRow);
    var aData = oTable.fnGetData(nRow);
    $.ajax({
        url: path + "SurveyAdmin/EditRowFedDef",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            def: jqInputs[0].value,
            payerGroup: payerGroupSelect.val(),
            description: desc.val()
        },
        success: function (result) {
            if (result.Success) {
                var payerGroupTd = payerGroupSelect.parent('td');
                payerGroupTd.data('select-value', payerGroupSelect.val());
                aData = [
                    jqInputs[0].value,
                    $('option:selected', payerGroupSelect).text(),
                    desc.val(),
                    '<a class="edit" href="">Edit</a>',
                    aData[4]
                ];
                oTable.fnUpdate(aData, nRow);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}

function InitFedDefs() {
    var oTableFedDef = $('#FedDefs').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,//Tag Code
            null,//Payer Group
            null,//Description
            null,//Edit
            null//Delete
        ],
        "oLanguage": {
            "sEmptyTable": "No Federal Deficiencies"
        }
    });
    var nEditingFedDef = null;

    $("#FedDefs").on('click', 'a.delete', function (e) {
        e.preventDefault();
        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm) {
            return;
        }
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path + "SurveyAdmin/DeleteRowFedDef",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { rowId: nRowId },
            success: function (result) {
                if (result.Success) {
                    oTableFedDef.fnDeleteRow(nRow);
                }
                else {
                    alert("Error: " + result.Message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $("#FedDefs").on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingFedDef !== null && nEditingFedDef != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableFedDef, nEditingFedDef);
            editRowFedDef(oTableFedDef, nRow);
            nEditingFedDef = nRow;
        }
        else if (nEditingFedDef == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowFedDef(oTableFedDef, nEditingFedDef, nRowId);
            if (success) {
                nEditingFedDef = null;
            }
        }
        else {
            /* No row currently being edited */
            editRowFedDef(oTableFedDef, nRow);
            nEditingFedDef = nRow;
        }
    });
    $('#FedDefs').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableFedDef, nRow);
    });
    $("#fedTags").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var code = $("#FedDef_Deficiency_TagCode").val();
        if (codeExist(code, $("#FedDefs").dataTable())) {
            alert("Federal Tag Code already exists");
            return false;
        }
        if (!$("#fedTags form").valid()) {
            return false;
        }
        var form = $("#fedTags form");
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: 'POST',
            data: form.serialize(),
            success: function (result) {
                if (result.success) {
                    var id = result.data.Id;
                    var code = result.data.TagCode;
                    var payerGroupCode = result.data.AtriumPayerGroupCode;
                    var payerGroupDesc = $("#FedDef_Deficiency_AtriumPayerGroupCode option[value='" + payerGroupCode + "'").text();
                    var desc = result.data.Description;

                    var addId = $("#FedDefs").dataTable().fnAddData([
                        code,
                        payerGroupDesc,
                        desc,
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $('#FedDefs').dataTable().fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    $(newRow).find("td")[1].setAttribute("data-select-value", payerGroupCode);
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

function InitStateDefs() {
    var oTableStateDef = $('#StateDefs').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            null,
            null,
        ],
        "oLanguage": {
            "sEmptyTable": "No State Deficiencies"
        }
    });
    oTableStateDef.fnSort([[1, 'asc']]);
    var nEditingStateDef = null;

    $("#StateDefs").on('click', 'a.delete', function (e) {
        e.preventDefault();
        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm) {
            return;
        }
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path + "SurveyAdmin/DeleteRowStateDef",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { rowId: nRowId },
            success: function (result) {
                if (result.Success) {
                    oTableStateDef.fnDeleteRow(nRow);
                }
                else {
                    alert("Error: " + result.Message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $("#StateDefs").on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingStateDef !== null && nEditingStateDef != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableStateDef, nEditingStateDef);
            editRowStateDef(oTableStateDef, nRow);
            nEditingStateDef = nRow;
        }
        else if (nEditingStateDef == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowStateDef(oTableStateDef, nEditingStateDef, nRowId);
            if (success) {
                nEditingStateDef = null;
            }
        }
        else {
            /* No row currently being edited */
            editRowStateDef(oTableStateDef, nRow);
            nEditingStateDef = nRow;
        }
    });
    $('#StateDefs').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableStateDef, nRow);
    });
    $("#stateCode").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var code = $("#StateDef_Deficiency_TagCode").val();
        if (codeExist(code, $("#StateDefs").dataTable(),1)) {
            alert("State Code already exists");
            return false;
        }
        if (!$("#stateCode form").valid()) {
            return false;
        }

        var form = $("#stateCode form");
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: 'POST',
            data: form.serialize(),
            success: function (result) {
                if (result.success) {
                    var id = result.data.Id;
                    var state = result.data.StateCode;
                    var code = result.data.TagCode;
                    var desc = result.data.Description;

                    var addId = $("#StateDefs").dataTable().fnAddData([
                        state,
                        code,
                        desc,
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $('#StateDefs').dataTable().fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
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

function editRowStateDef(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    var selectString = '<select id="StateDefStateCd">';
    selectString = selectString + $("#StateDef_Deficiency_StateCode").html().toString();
    selectString = selectString + '</select>';
    jqTds[0].innerHTML = selectString;
    $("#StateDefStateCd").val(aData[0]);
    jqTds[1].innerHTML = '<input type="text" size="4" value="' + aData[1] + '"/>';
    jqTds[2].innerHTML = '<textarea>' + aData[2] + "</textarea>";
    jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRowStateDef(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);
    var desc = $('textarea', nRow);
    var jqSelect = $('select', nRow);
    var aData = oTable.fnGetData(nRow);
    $.ajax({
        url: path + "SurveyAdmin/EditRowStateDef",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            stateCd: jqSelect[0].value,
            def: jqInputs[0].value,
            description: desc.val()
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqSelect[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[0].value, nRow, 1, false);
                oTable.fnUpdate(desc.val(), nRow, 2, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}

function InitSafetyDefs() {
    var oTableSafetyDef = $('#SafetyDefs').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            null,
        ],
        "oLanguage": {
            "sEmptyTable": "No Safety Deficiencies"
        }
    });
    var nEditingSafetyDef = null;

    $("#SafetyDefs").on('click', 'a.delete', function (e) {
        e.preventDefault();
        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm) {
            return;
        }
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path + "SurveyAdmin/DeleteRowSafetyDef",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { rowId: nRowId },
            success: function (result) {
                if (result.Success) {
                    oTableSafetyDef.fnDeleteRow(nRow);
                }
                else {
                    alert("Error: " + result.Message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });


    $("#SafetyDefs").on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingSafetyDef !== null && nEditingSafetyDef != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableSafetyDef, nEditingSafetyDef);
            editRowSafetyDef(oTableSafetyDef, nRow);
            nEditingSafetyDef = nRow;
        }
        else if (nEditingSafetyDef == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowSafetyDef(oTableSafetyDef, nEditingSafetyDef, nRowId);
            if (success) {
                nEditingSafetyDef = null;
            }
        }
        else {
            /* No row currently being edited */
            editRowSafetyDef(oTableSafetyDef, nRow);
            nEditingSafetyDef = nRow;
        }
    });
    $('#SafetyDefs').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableSafetyDef, nRow);
    });

    $("#safetyCodes").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var code = $("#SafetyDef_TagCode").val();
        if (codeExist(code, $("#SafetyDefs").dataTable(), 1)) {
            alert("Safety Tag Code already exists");
            return false;
        }
        if (!$("#safetyCodes form").valid()) {
            return false;
        }
        
        var form = $("#safetyCodes form");
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: 'POST',
            data: form.serialize(),
            success: function (result) {
                if (result.success) {
                    var id = result.data.Id;
                    var code = result.data.TagCode;
                    var desc = result.data.Description;

                    var addId = $("#SafetyDefs").dataTable().fnAddData([
                        code,
                        desc,
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $('#SafetyDefs').dataTable().fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
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

function editRowSafetyDef(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    jqTds[0].innerHTML = '<input type="text" size="10" value="' + aData[0] + '"/>';
    jqTds[1].innerHTML = '<textarea>' + aData[1] + "</textarea>";
    jqTds[2].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRowSafetyDef(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);
    var aData = oTable.fnGetData(nRow);
    var desc = $('textarea', nRow);
    $.ajax({
        url: path + "SurveyAdmin/EditRowSafetyDef",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            def: jqInputs[0].value,
            description: desc.val()
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(desc.val(), nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 2, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}

function InitCommunity() {
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Communities"
        }
    });
    $(".checkbox").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        ShowProgress();
        $.ajax({
            url: path + "SurveyAdmin/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "CSU"
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Community has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            },
            complete: function () {
                HideProgress();
            }
        });
    });
    $(".checkboxReport").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        ShowProgress();
        $.ajax({
            url: path + "SurveyAdmin/ChangeReportFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "CSU"
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            },
            complete: function () {
                HideProgress();
            }
        });
    });
    $(".checkboxComplaint").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var typeId = $(this).attr("id");
        var survey = $(this).parents("tr").children(":first-child").text();
        ShowProgress();
        $.ajax({
            url: path + "SurveyAdmin/ChangeComplaintFlgSurvey",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                surveyTypeId: typeId,
                complaintFlag: checked
            },
            success: function (result) {
                if (result.Success) {
                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Complaint flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            },
            complete: function () {
                HideProgress();
            }
        });
    });

}

$(document).ready(function () {
    $("#tabs").tabs();
    InitSurveyTypes();
    InitSAS();
    InitFedDefs();
    InitStateDefs();
    InitSafetyDefs();
    InitCommunity();
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
    $(".Description #close").click(function () {
        $(".modal").hide();
        $(".Description").hide();
        $("#EditingId").val("");
        $("#Description").val("");
        $("#TableName").val("");
    });

    $(".Description #saveDescription").click(function () {
        var editingId = $("#EditingId").val();
        var tableName = $("#TableName").val();
        var nRow = $("#" + tableName + ' tr#' + editingId)[0];
        switch (tableName) {
            case "FedDefs":
                oTableFedDef.fnUpdate($("#Description").val(), nRow, 6, false);
                break;
            case "StateDefs":
                oTableStateDef.fnUpdate($("#Description").val(), nRow, 6, false);
                break;
            case "SafetyDefs":
                oTableSafetyDef.fnUpdate($("#Description").val(), nRow, 5, false);
                break;
        }
        $(".Description #close").click();
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

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});