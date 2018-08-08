function initCommunities() {
    //Community Table
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": true,
        "sScrollY": "600px",
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
        "language": {
            "emptyTable": "No Communities"
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
                appCode: "ASAP"
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
                appCode: "ASAP"
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

function initComplaints() {
    var oTableComplaint = $('#complaintTypes').dataTable({
        "bFilter": false,
        "bAutoWidth": true,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '300px'
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
            "sEmptyTable": "No Complaint Types"
        }
    });
    oTableComplaint.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#complaintTypes").on("click", ".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableComplaint.fnGetData(nRow);

        $.ajax({
            url: path + "ASAPHotlineAdmin/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Complaint Type has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
    $("#complaintTypes").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableComplaint.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "ASAPHotlineAdmin/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId
            },
            success: function (result) {
                if (result.Success) {
                    oTableComplaint.fnDeleteRow(nRow);
                } else {
                    alert("Complaint Type has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#complaintTypes').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableComplaint, nEditing);
            editRow(oTableComplaint, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(oTableComplaint, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableComplaint, nRow);
            nEditing = nRow;

        }
    });
    $('#complaintTypes').on('click', "a.cancel", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableComplaint, nRow);
    });

    $("#complaintsTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#complaintsTab form");
        var complaint = $("#NewComplaintType_ASAPComplaintTypeDesc").val();
        if (alreadyExist(complaint, $("#complaintTypes").dataTable())) {
            alert("Complaint Type already exists.");
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
                    var complaint = result.data.ASAPComplaintTypeDesc;
                    var sortOrder = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.ASAPComplaintTypeId;
                    var checkboxId = "EnableDisable";
                    if (enabled) {
                        var checked = 'checked="checked"';
                    }
                    var checkBox = '<input ' + checked + ' class="' + checkboxId + '" id="' + checkboxId + '" name="' + checkboxId + '" type="checkbox" value="' + enabled + '">';
                    checkBox += '<input name="' + checkboxId + '" type="hidden" value="false">';
                    var addId = $("#complaintTypes").dataTable().fnAddData([
                        complaint,
                        sortOrder,
                        checkBox,
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $("#complaintTypes").dataTable().fnSettings().aoData[addId[0]].nTr;
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

function initContacts() {
    var oTableContact = $('#contacts').dataTable({
        "bFilter": false,
        "bAutoWidth": true,
        "sScrollY": "150px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            {
                "bSortable": false
            }
        ],
        "language": {
            "emptyTable": "No Employees"
        }
    });

    $("#contacts").on("click","a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "ASAPHotlineAdmin/DeleteContact",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId
            },
            success: function (result) {
                if (result.Success) {
                    oTableContact.fnDeleteRow(nRow);
                } else {
                    alert("Contact has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });


    $("#contactsTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#contactsTab form");
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
                    var firstName = result.data.FirstName;
                    var lastName = result.data.LastName;
                    var name = lastName + ", " + firstName;
                    var eMail = result.data.eMail;
                    var id = result.data.ASAPContactId;
                    var addId = $("#contacts").dataTable().fnAddData([
                        name,
                        eMail,
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = $("#contacts").dataTable().fnSettings().aoData[addId[0]].nTr;
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
        url: path + "ASAPHotlineAdmin/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value
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


$(document).ready(function () {
    $("#tabs").tabs();
    initCommunities();
    initComplaints();
    initContacts();

    $("#SaveLookback").submit(function (e) {
        $.ajax({
            type: 'POST',
            url: $(this).attr("action"),
            data: $(this).serialize()
        });
        return false;
    });
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

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});