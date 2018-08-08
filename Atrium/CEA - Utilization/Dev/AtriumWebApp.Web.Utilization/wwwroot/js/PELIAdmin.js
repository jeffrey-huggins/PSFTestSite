function LookupAdminTable(selector) {
    this.editing = null;
    this.dataTable = $(selector).dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '200px' },
            null,
            {
                "mRender": function (data, type, full) {
                    var output = '<input type="checkbox" name="EnableDisable" class="EnableDisable" value="' + data;
                    if (data === true || data === "True" || data === "true") {
                        output = output + '" checked="checked" />'
                    }
                    else {
                        output = output + '" />'
                    }
                    return output;
                }
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px',
                "bVisible": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Records"
        }
    });
    this.restoreRow = function (nRow) {
        var aData = this.dataTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        for (var i = 0, iLen = jqTds.length ; i < iLen ; i++) {
            this.dataTable.fnUpdate(aData[i], nRow, i, false);
        }
    };
    this.editRow = function (nRow) {
        var aData = this.dataTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        jqTds[0].innerHTML = '<input type="text" size="30" maxlength="30" value="' + aData[0] + '"/>';
        jqTds[1].innerHTML = '<input type="text" size="3" value="' + aData[1] + '"/>';
        jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
    };
    this.enableDisable = function (nRow, nRowId, url) {
        var self = this;
        var rowData = this.dataTable.fnGetData(nRow);
        var checkbox = $('.EnableDisable', nRow);
        var checked = checkbox.is(":checked");
        $.ajax({
            url: url,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                id: nRowId,
                desc: rowData[0],
                order: rowData[1],
                allowDataEntry: checked
            },
            success: function (result) {
                if (result.Success) {
                    self.dataTable.fnUpdate(checked, nRow, 2, false);
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Record has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    };
    this.saveRow = function (nRow, nRowId, url) {
        var self = this;
        var jqInputs = $('input', nRow);
        var enabled = $('.EnableDisable', nRow).is(':checked');
        $.ajax({
            url: url,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                id: nRowId,
                desc: jqInputs[0].value,
                order: jqInputs[1].value,
                allowDataEntry: enabled
            },
            success: function (result) {
                if (result.Success) {
                    self.dataTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                    self.dataTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                    self.dataTable.fnUpdate(enabled, nRow, 2, false);
                    self.dataTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    };
    this.deleteRow = function (nRow, nRowId, url) {
        var self = this;
        $.ajax({
            url: url,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { id: nRowId },
            success: function (result) {
                if (result.Success) {
                    self.dataTable.fnDeleteRow(nRow);
                }
                else {
                    alert("Error: Event not found in database");
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    };
}

var PELIAdmin = function () {
    function wireupEditClick(lookupAdminTable, url) {
        $(lookupAdminTable.dataTable).on('click','a.edit', function (e) {
            e.preventDefault();

            /* Get the row as a parent of the link that was clicked on */
            var nRow = $(this).parents('tr')[0];
            if (lookupAdminTable.editing !== null && lookupAdminTable.editing != nRow) {
                /* A different row is being edited - the edit should be cancelled and this row edited */
                lookupAdminTable.restoreRow(nEditing);
                lookupAdminTable.editRow(nRow);
                lookupAdminTable.editing = nRow;
            }
            else if (lookupAdminTable.editing == nRow && this.innerHTML == "Save") {
                /* This row is being edited and should be saved */
                var nRowId = $(this).closest('tr').attr("id");
                lookupAdminTable.saveRow(lookupAdminTable.editing, nRowId, url);
                lookupAdminTable.editing = null;
            }
            else {
                /* No row currently being edited */
                lookupAdminTable.editRow(nRow);
                lookupAdminTable.editing = nRow;
            }
        });
    }

    function wireupDeleteClick(lookupAdminTable, url) {
        $(lookupAdminTable.dataTable).on('click', 'a.delete', function (e) {
            e.preventDefault();

            var delConfirm = confirm("Are you sure you want to delete?");
            if (delConfirm == true) {
                var nRow = $(this).parents('tr')[0];
                var nRowId = $(this).closest('tr').attr("id");
                lookupAdminTable.deleteRow(nRow, nRowId, url);
            }
        });
    }

    function wireupCancelClick(lookupAdminTable) {
        $(lookupAdminTable.dataTable).on('click', 'a.cancel', function (e) {
            e.preventDefault();

            var nRow = $(this).parents('tr')[0];
            lookupAdminTable.restoreRow(nRow);
            lookupAdminTable.editing = null
        });
    }

    return {
        setupLookupAdminTable: function (selector, saveUrl, deleteUrl) {
            var lookupAdminTable = new LookupAdminTable(selector);
            wireupEditClick(lookupAdminTable, saveUrl);
            wireupDeleteClick(lookupAdminTable, deleteUrl);
            wireupCancelClick(lookupAdminTable);
            $(lookupAdminTable.dataTable).on("click", ".EnableDisable", function () {
                if (!lookupAdminTable.editing) {
                    var nRow = $(this).parents('tr')[0];
                    var nRowId = $(this).closest('tr').attr("id");
                    lookupAdminTable.enableDisable(nRow, nRowId, saveUrl);
                }
            });
            return lookupAdminTable;
        },
        setupCommunityTable: function (selector, displayUpdateUrl, reportUpdateUrl) {
            var communityTable = new CommunityAdminTable(selector);
            communityTable.wireupDisplayUpdateHandler(displayUpdateUrl);
            communityTable.wireupReportUpdateHandler(reportUpdateUrl);
            return communityTable;
        }
    }
}();

$(document).ready(function () {
    $("#tabs").tabs();
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

    $("#newPELIType").click(function (e) {
        e.preventDefault();
        var form = $("#reasonTab form");
        var code = $("#Description").val();
        var dataTable = $("#PELITypes").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Reason already exists");
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
                    var name = result.data.Description;
                    var id = result.data.Id;
                    var sortOrder = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var addId = dataTable.fnAddData([
                        name,
                        sortOrder,
                        enabled,
                        '<a class="edit" href="">Edit</a>',
                        ''
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

    //Community Table
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
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
            url: path + "/Base/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "PELI"
            },
            success: function (result) {
                if (result.Success) {
                } else {
                    checkbox.attr("checked", !checked);
                    alert("Community has failed to be enabled/disabled. Please try again " +
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
    PELIAdmin.setupLookupAdminTable('#PELITypes', path + "/PELIAdmin/EditType", path + "/PELIAdmin/DeleteType");
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
