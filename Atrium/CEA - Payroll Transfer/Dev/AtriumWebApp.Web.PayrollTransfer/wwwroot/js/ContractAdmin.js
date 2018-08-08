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
        for (var i = 0, iLen = jqTds.length ; i < iLen ; i++) {
            this.dataTable.fnUpdate(aData[i], nRow, i, false);
        }
    };
    this.editRow = function (nRow) {
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
        for (var i = 0, iLen = jqTds.length ; i < iLen ; i++) {
            this.dataTable.fnUpdate(aData[i], nRow, i, false);
        }
    };
    this.editRow = function (nRow) {
        var aData = this.dataTable.fnGetData(nRow);

        var isDataChecked = $(".IsDataEntry", nRow).is(":checked");
        var isReportableChecked = $(".IsReportable", nRow).is(":checked");

        var cells = $('>td', nRow);
        cells[0].innerHTML = '<input type="text" size="8" maxlength="16" value="' + aData[0] + '"/>';
        cells[1].innerHTML = '<input type="text" size="2" maxlength="2" value="' + aData[1] + '"/>';
        if (isDataChecked)
        {
            cells[2].innerHTML = '<input name="IsDataEntry" class="IsDataEntry" type="checkbox" value="true" checked />';
        }
        else
        {
            cells[2].innerHTML = '<input name="IsDataEntry" class="IsDataEntry" type="checkbox" value="true" />';
        }
        if (isReportableChecked)
        {
            cells[3].innerHTML = '<input name="IsReportable" class="IsReportable" type="checkbox" value="true" checked />';
        }
        else
        {
            cells[3].innerHTML = '<input name="IsReportable" class="IsReportable" type="checkbox" value="true" />';
        }

        cells[4].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
    };
    this.saveRow = function (nRow, nRowId, url) {
        var self = this;
        var jqInputs = $('input', nRow);

        var dataCheckbox = $(".IsDataEntry", nRow);
        var reportCheckbox = $(".IsReportable", nRow);

        $.ajax({
            url: url,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                id: nRowId,
                name: jqInputs[0].value,
                isDataEntry: $(dataCheckbox).is(":checked"),
                isReportable: $(reportCheckbox).is(":checked"),
                sortOrder: jqInputs[1].value
            },
            success: function (result) {
                if (result.Success) {
                    $(dataCheckbox).attr("disabled", "disabled");
                    $(reportCheckbox).attr("disabled", "disabled");

                    self.dataTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                    self.dataTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
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
        $('a.delete', lookupAdminTable.dataTable).on('click', function (e) {
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
        $(lookupAdminTable.dataTable).on('click','a.cancel', function (e) {
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
            
            return lookupAdminTable;
        },
        setupAdvancedLookupAdminTable: function (selector, saveUrl, deleteUrl) {
            var advancedLookupAdminTable = new AdvancedLookupAdminTable(selector);
            wireupEditClick(advancedLookupAdminTable, saveUrl);
            wireupDeleteClick(advancedLookupAdminTable, deleteUrl);
            wireupCancelClick(advancedLookupAdminTable);
        }
    }

}();

function PreparePage() {

    ContractManagementAdmin.setupLookupAdminTable('#addressTypesTable', path + "ContractAdmin/EditAddressType", path + "ContractAdmin/DeleteAddressType");
    ContractManagementAdmin.setupLookupAdminTable('#contactTypesTable', path + "ContractAdmin/EditContactType", path + "ContractAdmin/DeleteContactType");
    ContractManagementAdmin.setupLookupAdminTable('#categoriesTable', path + "ContractAdmin/EditCategory", path + "ContractAdmin/DeleteCategory");
    var subCategoryTable = ContractManagementAdmin.setupLookupAdminTable('#subCategoryTable', path + "ContractAdmin/EditSubCategory", path + "ContractAdmin/DeleteSubCategory");

    ContractManagementAdmin.setupAdvancedLookupAdminTable('#paymentTermTable', path + "ContractAdmin/EditPaymentTerm", path + "ContractAdmin/DeletePaymentTerm");
    ContractManagementAdmin.setupAdvancedLookupAdminTable('#renewalTypesTable', path + "ContractAdmin/EditRenewalType", path + "ContractAdmin/DeleteRenewalType");
    ContractManagementAdmin.setupAdvancedLookupAdminTable('#terminationNoticeTable', path + "ContractAdmin/EditTerminationNotice", path + "ContractAdmin/DeleteTerminationNotice");

    $("#submitSubCategory").click(function (event) {
        var element = $(this);
        event.preventDefault();

        var selectedId = $("#SelectedCategoryId").val();
        var newType = $("#newSubCategoryType").val();

        $.ajax({
            url: path + "ContractAdmin/CreateSubCategory",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { newType: newType, SelectedCategoryId: selectedId },
            success: function (result) {
                var deleteCell = '<a class="delete" href="">Delete</a>';
                var editCell = '<a class="edit" href="">Edit</a>';
                var newData = [newType, editCell, deleteCell];
                                
                var newEntry = subCategoryTable.dataTable.fnAddData(newData, true);

                var tr = subCategoryTable.dataTable.fnSettings().aoData[newEntry[0]].nTr;
                tr.setAttribute("id", result.NewSubCategory.Id);
                $("#newSubCategoryType").val("");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });


    $('#Categories').on('change', function (e) {
        e.preventDefault();
        var selectedCategoryId = $(this).add("option:selected").val();
        $("#SelectedCategoryId").val(selectedCategoryId);
        $.ajax({
            url: path + "ContractAdmin/GetSubCategories",
            type: 'POST',
            data: { selectedCategoryId: selectedCategoryId },
            datatype: 'json',
            success: function (result) {
                subCategoryTable.dataTable.fnClearTable(true);

                for (var i = 0 ; i < result.length ; i++) {
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
}