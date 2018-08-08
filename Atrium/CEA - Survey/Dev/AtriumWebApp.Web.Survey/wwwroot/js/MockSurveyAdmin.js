$("#tabs").tabs();

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});

function swapCells(table, row, x, y) {
    var nRow = row[0];
    var position = table.fnGetPosition(nRow);
    var aData = table.fnGetData(nRow);
    aData.swap(x, y);
    table.fnUpdate(aData, position);
}

function UpdateDeficiencyGroup(selectElement, table, actionUrl) {
    var select = $(selectElement);
    var nRowId = select.closest('tr').attr("id");
    var nRow = select.parents('tr')[0];
    var aData = table.fnGetData(nRow);
    var url = actionUrl + nRowId;
    var selectedValue = select.val();
    $.ajax({
        url: url,
        dataType: "json",
        type: 'POST',
        data: {
            groupId: selectedValue
        },
        success: function (result) {
            var message = aData[0];
            if (result.Success) {
            }
            else {
                message += " could not be moved to ";
                alert(message);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            select.val(null);
            alert("Error communicating with the server.\r\nPlease try again and if you see this message repeatedly the server may be down.");
        }
    });
}
function restoreRow(table, nRow) {
    var aData = table.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    for (var i = 0, iLen = jqTds.length ; i < iLen ; i++) {
        table.fnUpdate(aData[i], nRow, i, false);
    }
}
function editRow(table, nRow) {
    var aData = table.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    jqTds[1].innerHTML = '<input type="text" value="' + aData[1] + '"/>';
    jqTds[2].innerHTML = '<input type="text" size="6" value="' + aData[2] + '"/>';
    jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function PreparePage() {


    var nEditing = null;
    var oDefGroup = $('#deficiencyGroups').dataTable({
        "bAutoWidth": false,
        "sScrollY": "130px",
        "bScrollCollapse": true,
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
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
            "sEmptyTable": "No Federal Deficiencies"
        }
    });

    oDefGroup.on('click', '.edit', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            restoreRow(oDefGroup, nEditing);
            editRow(oDefGroup, nRow);
            nEditing = nRow;
        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            var nRowId = $(this).closest('tr').attr("id");
            var success = SaveDeficiencyGroup(oDefGroup, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }
        }
        else {
            editRow(oDefGroup, nRow);
            nEditing = nRow;
        }
    });

    function SaveDeficiencyGroup(table, nRow, nRowId) {
        var jqInputs = $('input', nRow);
        var url = path + "MockSurveyAdmin/EditDeficiencyGroup/" + nRowId;
        var aData = table.fnGetData(nRow);
        $.ajax({
            url: url,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                description: jqInputs[0].value,
                sortOrder: jqInputs[1].value
            },
            success: function (result) {
                if (result.Success) {
                    $('.Group option[value="' + nRowId + '"]', aData[0] == 'Federal' ? oTableFed : oTableSafety).text(jqInputs[0].value);
                    table.fnUpdate(jqInputs[0].value, nRow, 1, false);
                    table.fnUpdate(jqInputs[1].value, nRow, 2, false);
                    table.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    }

    oDefGroup.on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oDefGroup, nRow);
    });

    oDefGroup.on('click', '.delete', function (e) {
        e.preventDefault();
        if (!confirm("Are you sure you want to delete?")) {
            return;
        }
        var element = $(this);
        var nRow = element.parents('tr')[0];
        var nRowId = element.closest('tr').attr("id");
        var aData = oDefGroup.fnGetData(nRow);
        var url = path + "MockSurveyAdmin/DeleteDeficiencyGroup/" + nRowId;
        $.ajax({
            url: url,
            dataType: "json",
            type: 'POST',
            success: function (result) {
                if (result.Success) {
                    $('.Group option[value="' + nRowId + '"]', aData[0] == 'Federal' ? oTableFed : oTableSafety).remove();
                    oDefGroup.fnDeleteRow(nRow);
                }
                else {
                    alert("Server is not responding. Please reload page and try again.");
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $('#NewDeficiencyGroup').on('click', '.add-button', function (e) {
        var button = $(this);
        var form = button.parents('form');
        var formElement = form.get(0);
        var data = form.serialize();
        var description = form.find("#Description").val();
        var type = form.find("#GroupType option:selected").text();
        var dataSet = oDefGroup.dataTable().fnGetData();
        for (var i = 0; i < dataSet.length; i++) {
            var existingDesc = dataSet[i][1];
            if (existingDesc.toLowerCase() === description.toLowerCase()) {
                var existingType = dataSet[i][0];
                if (existingType == type) {
                    alert("Group already exists for this type.");
                    return false;
                }
            }
        }
        if (!form.valid()) {
            return false;
        }
        e.preventDefault();
        $.ajax({
            url: formElement.action,
            type: 'POST',
            data: data,
            success: function (result) {
                if (result.Success) {
                    var jqDescription = $('#Description');
                    var description = jqDescription.val();
                    var groupType = $('#GroupType').val();
                    var newOption = '<option value="' + result.Id + '">' + description + '</option>';
                    var newEntry = oDefGroup.fnAddData([
                        groupType,
                        description,
                        0,
                        "<a class=\"edit\" href=\"\">Edit</a>",
                        "<a class=\"delete\" href=\"\">Delete</a>"
                    ]);
                    var tr = oDefGroup.fnSettings().aoData[newEntry[0]].nTr;
                    tr.setAttribute("id", result.Id);
                    $('.input-validation-error', '#NewDeficiencyGroup').removeClass('input-validation-error');
                    jqDescription.val('');
                    $('.Group', groupType == 'Federal' ? oTableFed : oTableSafety).append(newOption);
                }
                else {
                    $('#NewDeficiencyGroup').html(result);
                }
            },
            error: function () {
                alert("There was an error communicating with the server.\r\nPlease try again and if you see this message repeatedly the server may be down.");
            }
        });
    });

    var oTableFed = $('#federalDeficiencies').dataTable({
        "bAutoWidth": false,
        "sScrollY": "235px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            {
                "bVisible": false
            },
            {
                "sWidth": "50%"
            },
            {
                "bVisible": false
            },
            {
                "sWidth": "50%"
            },
            {
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Federal Deficiencies"
        }
    });
    oTableFed.fnSort([[0, 'asc']]);

    oTableFed.on('click', "a.moreDesc, a.lessDesc", function (e) {
        e.preventDefault();
        swapCells(oTableFed, $(this).parents('tr'), 1, 2);
    });

    oTableFed.on('click', "a.moreInst, a.lessInst", function (e) {
        e.preventDefault();
        swapCells(oTableFed, $(this).parents('tr'), 3, 4);
    });

    oTableFed.on('click', '.editInst', function (e) {
        e.preventDefault();
        var row = $(this).closest('tr');
        var nRow = row[0];
        var aData = oTableFed.fnGetData(nRow);
        var nRowId = row.attr("id");
        $("#Id").val(nRowId);
        $("#Instructions").val(row.data('full-instructions'));
        $("#RecordType").val("Federal");
        var modal = $('<div />');
        modal.addClass("modal");
        $('body').append(modal);
        var modalContent = $("#InstructionsWindow");
        modalContent.show();
        var top = Math.max($(window).height() / 2 - modalContent[0].offsetHeight / 2, 0);
        var left = Math.max($(window).width() / 2 - modalContent[0].offsetWidth / 2, 0);
        modalContent.css({ top: top, left: left });
    });

    $(".Group", oTableFed).change(function () {
        UpdateDeficiencyGroup(this, oTableFed, path + "MockSurveyAdmin/ChangeFederalDeficiencyGroup/");
    });

    var oTableSafety = $('#safetyDeficiencies').dataTable({
        "bAutoWidth": false,
        "sScrollY": "235px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            {
                "bVisible": false
            },
            {
                "sWidth": "50%"
            },
            {
                "bVisible": false
            },
            {
                "sWidth": "50%"
            },
            {
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Safety Deficiencies"
        }
    });
    oTableSafety.fnSort([[0, 'asc']]);

    oTableSafety.on('click', "a.moreDesc, a.lessDesc", function (e) {
        e.preventDefault();
        swapCells(oTableSafety, $(this).parents('tr'), 1, 2);
    });

    oTableSafety.on('click', "a.moreInst, a.lessInst", function (e) {
        e.preventDefault();
        swapCells(oTableSafety, $(this).parents('tr'), 3, 4);
    });

    oTableSafety.on('click', '.editInst', function (e) {
        e.preventDefault();
        var row = $(this).closest('tr');
        var nRow = row[0];
        var aData = oTableSafety.fnGetData(nRow);
        var nRowId = row.attr("id");
        $("#Id").val(nRowId);
        $("#Instructions").val(row.data('full-instructions'));
        $("#RecordType").val("Safety");
        var modal = $('<div />');
        modal.addClass("modal");
        $('body').append(modal);
        var modalContent = $("#InstructionsWindow");
        modalContent.show();
        var top = Math.max($(window).height() / 2 - modalContent[0].offsetHeight / 2, 0);
        var left = Math.max($(window).width() / 2 - modalContent[0].offsetWidth / 2, 0);
        modalContent.css({ top: top, left: left });
    });

    $(".Group", oTableSafety).change(function () {
        UpdateDeficiencyGroup(this, oTableSafety, path + "MockSurveyAdmin/ChangeSafetyDeficiencyGroup/");
    });

    $("#InstructionsWindow").on('click', '#close', function () {
        $(".modal").hide();
        $("#InstructionsWindow").hide();
        $("#Id").val("");
        $("#Instructions").val("");
        $("#RecordType").val("");
    });

    $("#InstructionsWindow").on('click', "#saveInstructions", function (e) {
        e.preventDefault();
        var form = $("#InstructionsWindow form");
        var data = form.serialize();
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: data,
            success: function (result) {
                if (result.Success) {
                    var id = $("#Id").val();
                    var recordType = $("#RecordType").val();
                    var instructions = $("#Instructions").val();
                    var dom = instructions + '<a class="editInst float-right" href="">Edit</a>';
                    switch (recordType) {
                        case "Federal":
                            var row = $('tr#' + id, oTableFed);
                            var nRow = row[0];
                            oTableFed.fnUpdate(dom, nRow, 3, false);
                            oTableFed.fnUpdate(dom, nRow, 4, false);
                            row.data('full-instructions', instructions);
                            break;
                        case "Safety":
                            var row = $('tr#' + id, oTableSafety);
                            var nRow = row[0];
                            oTableSafety.fnUpdate(dom, nRow, 3, false);
                            oTableSafety.fnUpdate(dom, nRow, 4, false);
                            row.data('full-instructions', instructions);
                            break;
                    }
                    $("#close").click();
                }
                else {
                    $('#InstructionsWindow').html(result);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Error communicating with the server.  Please try again and if you see this message repeatedly the server may be down.");
            }
        });
    });

    //Community Table
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
            },
            null
        ],
        "oLanguage": {
            "sEmptyTable": "No Communities"
        }
    });
    $(".checkbox").change(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        $.ajax({
            url: path + "MockSurveyAdmin/ChangeDataFlgCom",
            dataType: "json",
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "MSU"
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Community has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });
    $(".checkboxReport").change(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        $.ajax({
            url: path + "MockSurveyAdmin/ChangeReportFlgCom",
            dataType: "json",
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "MSU"
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    //Recipients
    var recipientsModal = $("#recipientsModal");
    var recipientsTable;
    $(".manage-recipients").on('click', function (e) {
        e.preventDefault();
        $.ajax({
            url: this.href,
            success: function (data) {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                recipientsModal.html(data);
                recipientsModal.show();
                var top = Math.max($(window).height() / 2 - recipientsModal[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - recipientsModal[0].offsetWidth / 2, 0);
                recipientsModal.css({ top: top, left: left });
                recipientsTable = $('#recipients', recipientsModal).dataTable({
                    "bFilter": false,
                    "bAutoWidth": false,
                    "sScrollY": "330px",
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
                        "sEmptyTable": "No Notification Recipients"
                    }
                });
            },
            error: function () {
                alert("Could not communicate with the server.");
            }
        });
    });
    recipientsModal.on('click', '#close', function () {
        $(".modal").hide();
        recipientsModal.hide();
    });
    recipientsModal.on('click', '#add-notification-recipient', function (e) {
        e.preventDefault();
        var jqThis = $(this);
        var form = jqThis.closest('form');
        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: form.serialize(),
            success: function (response) {
                if (response.Success) {
                    recipientsTable.fnAddData([
                        $('#EmailAddress').val(),
                        '<a class="delete-recipient" href="' + path + 'MockSurveyAdmin/DeleteNotificationRecipient/' + response.Id + '">Delete</a>'
                    ], true);
                    alert('Successfully added notification recipient.');
                }
                else {
                    alert('Please ensure the New Notification Recipient is a valid email address.');
                }
            },
            error: function () {
                alert("Could not communicate with the server or duplicate email address entered.");
            }
        });
    });
    recipientsModal.on('click', '.delete-recipient', function (e) {
        e.preventDefault();
        var element = $(this);
        $.ajax({
            url: this.href,
            type: "post",
            success: function (response) {
                if (response.Success) {
                    var jqRow = element.closest('tr');
                    var row = jqRow.get(0);
                    var rowIndex = recipientsTable.fnGetPosition(row);
                    recipientsTable.fnDeleteRow(rowIndex);
                }
                else {
                    alert('Unexpected error prevented the record from being deleted.');
                }
            },
            error: function () {
                alert("Could not communicate with the server.");
            }
        });
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

}

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