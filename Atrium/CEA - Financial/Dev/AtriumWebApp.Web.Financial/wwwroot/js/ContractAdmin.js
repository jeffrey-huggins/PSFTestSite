function LookupAdminTable(selector) {
    this.editing = null;
    this.dataTable = $(selector).dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": "160px"
            },
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
            "sEmptyTable": "No Records"
        }
    });
    this.restoreRow = function (nRow) {
        var aData = this.dataTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        for (var i = 0, iLen = jqTds.length; i < iLen; i++) {
            this.dataTable.fnUpdate(aData[i], nRow, i, false);
        }
    };
    this.editRow = function (nRow) {
        if (nRow == null) {
            return;
        }
        var aData = this.dataTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        jqTds[0].innerHTML = '<input type="text" size="30" maxlength="30" value="' + aData[0] + '"/>';
        jqTds[1].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
    };
    this.saveRow = function (nRow, nRowId, url) {
        var self = this;
        var jqInputs = $('input', nRow);
        $.ajax({
            url: url,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                id: nRowId,
                name: jqInputs[0].value
            },
            success: function (result) {
                if (result.Success) {
                    self.dataTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                    self.dataTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 1, false);
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

function AdvancedLookupAdminTable(selector) {
    this.editing = null;
    this.dataTable = $(selector).dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": "250px"
            },
            {
                "sWidth": "10px"
            },
            {
                "sWidth": "10px"
            },
            {
                "sWidth": "10px"
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '15px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '15px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Records"
        }
    });
    this.restoreRow = function (nRow) {
        var aData = this.dataTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        for (var i = 0, iLen = jqTds.length; i < iLen; i++) {
            this.dataTable.fnUpdate(aData[i], nRow, i, false);
        }
    };
    this.editRow = function (nRow) {
        if (nRow == null) {
            return;
        }
        var aData = this.dataTable.fnGetData(nRow);

        var isDataChecked = $(".IsDataEntry", nRow).is(":checked");
        var isReportableChecked = $(".IsReportable", nRow).is(":checked");

        var cells = $('>td', nRow);
        cells[0].innerHTML = '<input type="text" size="8" maxlength="16" value="' + aData[0] + '"/>';
        cells[1].innerHTML = '<input type="text" size="2" maxlength="2" value="' + aData[1] + '"/>';
        cells[4].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
    };
    this.saveRow = function (nRow, nRowId, url) {
        var self = this;
        var jqInputs = $('input', nRow);
        var name = jqInputs[0].value;
        var sortOrder = jqInputs[1].value;
        var nameValue = name;
        var sortOrderValue = sortOrder;
        if (this.editing == null) {
            nameValue = $('td', nRow)[0].innerText;
            sortOrderValue = $('td', nRow)[1].innerText;
            name = null;
            sortOrder = null;
        }
        var dataCheckbox = $("#IsDataEntry", nRow);
        var reportCheckbox = $("#IsReportable", nRow);
        $.ajax({
            url: url,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                id: nRowId,
                name: name,
                isDataEntry: dataCheckbox.prop("checked"),
                isReportable: reportCheckbox.prop("checked"),
                sortOrder: sortOrder
            },
            success: function (result) {
                if (result.Success) {
                    self.dataTable.fnUpdate(nameValue, nRow, 0, false);
                    self.dataTable.fnUpdate(sortOrderValue, nRow, 1, false);
                    self.dataTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 4, false);
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

var ContractManagementAdmin = function () {
    function wireupEditClick(lookupAdminTable, url) {
        $(lookupAdminTable.dataTable).on('click', 'a.edit', function (e) {
            e.preventDefault();
            /* Get the row as a parent of the link that was clicked on */
            var nRow = $(this).parents('tr')[0];
            if (lookupAdminTable.editing !== null && lookupAdminTable.editing != nRow) {
                /* A different row is being edited - the edit should be cancelled and this row edited */
                var nEditing = lookupAdminTable.editing;
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

    function wireupCheckBox(lookupAdminTable, url) {
        $(lookupAdminTable.dataTable).on('click', '[type="checkbox"]', function (e) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            lookupAdminTable.saveRow(nRow, nRowId, url);
        });
    }

    return {
        setupLookupAdminTable: function (selector, saveUrl, deleteUrl) {
            var lookupAdminTable = new LookupAdminTable(selector);
            wireupEditClick(lookupAdminTable, saveUrl);
            wireupDeleteClick(lookupAdminTable, deleteUrl);
            wireupCancelClick(lookupAdminTable);

            return lookupAdminTable;
        },
        setupAdvancedLookupAdminTable: function (selector, saveUrl, deleteUrl) {
            var advancedLookupAdminTable = new AdvancedLookupAdminTable(selector);
            wireupEditClick(advancedLookupAdminTable, saveUrl);
            wireupDeleteClick(advancedLookupAdminTable, deleteUrl);
            wireupCancelClick(advancedLookupAdminTable);
            wireupCheckBox(advancedLookupAdminTable, saveUrl);
        }
    }

}();

$(function () {
    $("#tabs").tabs();
    ContractManagementAdmin.setupLookupAdminTable('#addressTypesTable', path + "/ContractAdmin/EditAddressType", path + "/ContractAdmin/DeleteAddressType");
    ContractManagementAdmin.setupLookupAdminTable('#contactTypesTable', path + "/ContractAdmin/EditContactType", path + "/ContractAdmin/DeleteContactType");
    ContractManagementAdmin.setupLookupAdminTable('#categoriesTable', path + "/ContractAdmin/EditCategory", path + "/ContractAdmin/DeleteCategory");
    var subCategoryTable = ContractManagementAdmin.setupLookupAdminTable('#subCategoryTable', path + "/ContractAdmin/EditSubCategory", path + "/ContractAdmin/DeleteSubCategory");

    ContractManagementAdmin.setupAdvancedLookupAdminTable('#paymentTermTable', path + "/ContractAdmin/EditPaymentTerm", path + "/ContractAdmin/DeletePaymentTerm");
    ContractManagementAdmin.setupAdvancedLookupAdminTable('#renewalTypesTable', path + "/ContractAdmin/EditRenewalType", path + "/ContractAdmin/DeleteRenewalType");
    ContractManagementAdmin.setupAdvancedLookupAdminTable('#terminationNoticeTable', path + "/ContractAdmin/EditTerminationNotice", path + "/ContractAdmin/DeleteTerminationNotice");

    $("#newAddressType").click(function (e) {
        e.preventDefault();
        var form = $("#addressTab form");
        var code = $("#newAddressType_Name").val();
        var dataTable = $("#addressTypesTable").dataTable();
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
                    var id = result.data.Id;

                    var addId = dataTable.fnAddData([
                        name,
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

    $("#newContactType").click(function (e) {
        e.preventDefault();
        var form = $("#contactTab form");
        var code = $("#newContactType_Name").val();
        var dataTable = $("#contactTypesTable").dataTable();
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
                    var id = result.data.Id;

                    var addId = dataTable.fnAddData([
                        name,
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

    $("#newCategory").click(function (e) {
        e.preventDefault();
        var form = $("#categoryTab form");
        var code = $("#newContractCategory_Name").val();
        var dataTable = $("#categoriesTable").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Category already exists");
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
                    var id = result.data.Id;

                    var addId = dataTable.fnAddData([
                        name,
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    $("#Categories").append("<option value='" + id + "'>" + name + "</option>");

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

    $("#newSubcategory").click(function (e) {
        e.preventDefault();
        var form = $("#subcategoryTab form");
        var code = $("#newContractSubcategory_Name").val();
        var dataTable = $("#subCategoryTable").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Subcategory already exists");
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
                    var id = result.data.Id;

                    var addId = dataTable.fnAddData([
                        name,
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

    $("#newRenewalType").click(function (e) {
        e.preventDefault();
        var form = $("#renewalTab form");
        var code = $("#newContractRenewal_Name").val();
        var dataTable = $("#renewalTypesTable").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Renewal type already exists");
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
                    var id = result.data.Id;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.IsDataEntry;
                    var report = result.data.IsReportable;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("IsDataEntry", "IsDataEntry", "IsDataEntry", enabled),
                        createCheckboxString("IsReportable", "IsReportable", "IsReportable", report),
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

    $("#newTerminationNotice").click(function (e) {
        e.preventDefault();
        var form = $("#termTab form");
        var code = $("#newTermNotice_Name").val();
        var dataTable = $("#terminationNoticeTable").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Terminiation notice already exists");
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
                    var id = result.data.Id;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.IsDataEntry;
                    var report = result.data.IsReportable;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("IsDataEntry", "IsDataEntry", "IsDataEntry", enabled),
                        createCheckboxString("IsReportable", "IsReportable", "IsReportable", report),
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

    $("#newPaymentTerm").click(function (e) {
        e.preventDefault();
        var form = $("#paymentTab form");
        var code = $("#newPaymentTerm_Name").val();
        var dataTable = $("#paymentTermTable").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Payment term already exists");
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
                    var id = result.data.Id;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.IsDataEntry;
                    var report = result.data.IsReportable;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("IsDataEntry", "IsDataEntry", "IsDataEntry", enabled),
                        createCheckboxString("IsReportable", "IsReportable", "IsReportable", report),
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

    $('#Categories').on('change', function (e) {
        e.preventDefault();
        var selectedCategoryId = $(this).add("option:selected").val();
        $("#SelectedCategoryId").val(selectedCategoryId);
        $.ajax({
            url: path + "/ContractAdmin/GetSubCategories",
            type: 'POST',
            data: { selectedCategoryId: selectedCategoryId },
            datatype: 'json',
            success: function (result) {
                subCategoryTable.dataTable.fnClearTable(true);

                for (var i = 0; i < result.length; i++) {
                    var theId = result[i].Id;
                    var theName = result[i].Name;
                    var editLink = '<a class="edit" href="">Edit</a>';
                    var deleteLink = '<a class="delete" href="">Delete</a>'

                    var newEntry = subCategoryTable.dataTable.fnAddData([theName, editLink, deleteLink], false);
                    var row = subCategoryTable.dataTable.fnSettings().aoData[newEntry[0]].nTr;
                    row.setAttribute("id", theId);
                }

                subCategoryTable.dataTable.fnDraw();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
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

    

    doPoll();

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

function createCheckboxString(id, cssClass, name, value) {
    if (value) {
        var checked = "checked";
    }
    var html = '<input class="' + cssClass + '" id="' + id + '" name="' + name + '" type="checkbox" value="' + value + '" ' + checked + '>';
    html += '<input name="' + name + '" type="hidden" value="' + value + '">';
    return html;
}



$("#exportContracts").on("click", function () {
    var facilityIds = $("#facilityExport").val();
    if (facilityIds.length == 0) {
        alert("At least one facility should be selected to export.");
        return;
    }
    else {
        var facilityUrl = facilityIds.join("&facilityIds=");
        $.ajax({
            url: path + "ContractAdmin/GetContracts?facilityIds=" + facilityUrl,
            success: function (status) {
                doPoll();
            },
            failure: function () {
                alert("There was an error exporting, please refresh the page and try again.");
            }
        });
    }
});

$("#processingSection").on("click", "#refreshProgress", doPoll);

function doPoll() {
    $("#processingSection").load(path + "ContractAdmin/GetProcessingFiles");
    //setTimeout(function () {
    //    doPoll();
    //}, 5000);
}

