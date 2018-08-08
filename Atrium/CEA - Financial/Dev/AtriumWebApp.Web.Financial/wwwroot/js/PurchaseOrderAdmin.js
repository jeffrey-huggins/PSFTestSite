function LookupAdminTable(selector) {
    this.editing = null;
    this.dataTable = $(selector).dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
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
                name: rowData[0],
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
                name: jqInputs[0].value,
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

function ApproverLookupAdminTable(selector) {
    this.editing = null;
    this.dataTable = $(selector).dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '300px' },
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
        jqTds[0].innerHTML = '<input type="text" size="30" maxlength="256" value="' + aData[0] + '"/>';
        jqTds[1].innerHTML = '<input type="text" size="1" value="' + aData[1] + '"/>';
        jqTds[2].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
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
                name: jqInputs[0].value,
                approvalLevel: jqInputs[1].value
            },
            success: function (result) {
                if (result.Success) {
                    self.dataTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                    self.dataTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                    self.dataTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 2, false);
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

function CommunityAdminTable(selector) {
    this.dataTable = $(selector).dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "aaSorting": [],
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
        "oLanguage": {
            "sEmptyTable": "No Communities"
        }
    });
    this.wireupDisplayUpdateHandler = function (url) {
        $(".checkboxDisplay").click(function () {
            var checked = $(this).is(":checked");
            var comId = $(this).attr("id");
            var comName = $(this).parents("tr").children(":first-child").text();
            $.ajax({
                url: url,
                dataType: "json",
                cache: false,
                type: 'POST',
                data: {
                    community: comId,
                    dFlag: checked,
                    appCode: "POR"
                },
                success: function (result) {
                    if (result.Success) {

                    } else {
                        alert("Community has failed to be enabled/disabled. Please try again " +
                            "and if you see this message repeatedly the server may be down.");
                    }
                }
            });
        });
    };
    this.wireupReportUpdateHandler = function (url) {
        $(".checkboxReport").click(function () {
            var checked = $(this).is(":checked");
            var comId = $(this).attr("id");
            var comName = $(this).parents("tr").children(":first-child").text();

            $.ajax({
                url: url,
                dataType: "json",
                cache: false,
                type: 'POST',
                data: {
                    community: comId,
                    dFlag: checked,
                    appCode: "POR"
                },
                success: function (result) {
                    if (result.Success) {

                    } else {
                        alert("Community report flag has failed to be enabled/disabled. Please try again " +
                            "and if you see this message repeatedly the server may be down.");
                    }
                }
            });
        });
    };
}

var PurchaseOrderAdmin = function () {
    function wireupEditClick(lookupAdminTable, url) {
        $(lookupAdminTable.dataTable).on('click','a.edit', function (e) {
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
            $(lookupAdminTable.dataTable).on("click", ".EnableDisable", function () {
                if (!lookupAdminTable.editing) {
                    var nRow = $(this).parents('tr')[0];
                    var nRowId = $(this).closest('tr').attr("id");
                    lookupAdminTable.enableDisable(nRow, nRowId, saveUrl);
                }
            });
            return lookupAdminTable;
        },
        setupApproverLookupAdminTable: function (selector, saveUrl, deleteUrl) {
            var approverTable = new ApproverLookupAdminTable(selector);
            wireupEditClick(approverTable, saveUrl);
            wireupDeleteClick(approverTable, deleteUrl);
            wireupCancelClick(approverTable);
        },
        setupCommunityTable: function (selector, displayUpdateUrl, reportUpdateUrl) {
            var communityTable = new CommunityAdminTable(selector);
            communityTable.wireupDisplayUpdateHandler(displayUpdateUrl);
            communityTable.wireupReportUpdateHandler(reportUpdateUrl);
            return communityTable;
        }
    }
}();

function initVendors() {
    var modal = $("#vendor");
    var form = $("form", modal);
    var oTable = $("#VendorTable").dataTable({
        "bAutoWidth": false,
        "bFilter": true,
        "sScrollY": "450px",
        "sDom": "frt",
        "iDisplayLength": -1,
        "aoColumns": [
            {   // VendorName
                "bSearchable": true
            },
            {   // VendorClass
                "bSearchable": false,
                "sWidth": "85px"
            },
            {   // CorpVendorId
                "bSearchable": false,
                "sWidth": "75px"
            },
            {   // FullAddress
                "bSearchable": false
            },
            {   // Phone
                "bSearchable": false,
                "sWidth": "100px"
            },
            {   // Fax
                "bSearchable": false,
                "sWidth": "100px"
            },
            {   // Email
                "bSearchable": false
            },
            {   // AllowDataEntry
                "bSearchable": false,
                "sWidth": "80px"
            },
            {   // Edit
                "bSearchable": false,
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No entries for date range selected"
        }
    });
    oTable.fnSort([[0, 'asc']]);

    $("#VendorTable").on('click',"a.edit", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = parseInt($(this).closest('tr').attr("id"));
        $.ajax({
            url: path + "PurchaseOrderAdmin/GetVendor",
            dataType: "json",
            cache: false,
            type: "GET",
            data: { vendorId: nRowId },
            success: function (result) {
                LaunchModal(modal, result);
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $("#btnCreate").click(function (e) {
        e.preventDefault();
        LaunchModal(modal);
    });

    $("#btnSave").click(function () {
        if (ValidateForm()) {
            $.ajax({
                url: $(form).attr("action"),
                dataType: "json",
                cache: false,
                type: $(form).attr("method"),
                data: form.serialize(),
                success: function (result) {
                    alert("Vendor Record has been successfully saved.");
                    UpdateVendorTable(result);
                    $("#btnClose").click();
                },
                error: function (ex) {
                    alert("Server is not responding. Please reload the page and try again");
                }
            });
        }
    });

    $("#btnClear").click(function () {
        $("#VendorName", form).val("");
        $("#VendorClassId", form).val("");
        $("#CorpVendorId", form).val("");
        $("#Address1", form).val("");
        $("#Address2", form).val("");
        $("#City", form).val("");
        $("#StateCd", form).val("");
        $("#ZipCode", form).val("");
        $("#Phone", form).val("");
        $("#Fax", form).val("");
        $("#Email", form).val("");
    });

    $("#btnClose").click(function () {
        $(".modal, #vendor").hide();
        $("#Id", form).val("");
        $("#AllowDataEntry", form).val("true");
        $("#btnClear").click();
    });

    $("#VendorTable").on('click',".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRowId = $(this).closest("tr").attr("id");

        $.ajax({
            url: path + "PurchaseOrderAdmin/UpdateVendorDataEntryFlag",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { vendorId: nRowId, allowDataEntry: checked },
            success: function (result) {
            },
            error: function (data, error, errortext) {
                if (data.status == 200) {
                    return;
                }
                checkbox.prop("checked", !checked);
                alert("Vendor has failed to be enabled/disabled. Please try again " +
                    "and if you see this message repeatedly the server may be down.");
            }
        });
    });
}

$(document).ready(function () {
    $("#tabs").tabs();
    PurchaseOrderAdmin.setupLookupAdminTable('#Types', path + "/PurchaseOrderAdmin/EditType", path + "/PurchaseOrderAdmin/DeleteType");
    PurchaseOrderAdmin.setupLookupAdminTable('#VendorClasses', path + "/PurchaseOrderAdmin/EditVendorClass", path + "/PurchaseOrderAdmin/DeleteVendorClass");
    PurchaseOrderAdmin.setupLookupAdminTable('#AssetClasses', path + "/PurchaseOrderAdmin/EditAssetClass", path + "/PurchaseOrderAdmin/DeleteAssetClass");
    PurchaseOrderAdmin.setupApproverLookupAdminTable('#Approvers', path + "/PurchaseOrderAdmin/EditApprover", path + "/PurchaseOrderAdmin/DeleteApprover");
    PurchaseOrderAdmin.setupCommunityTable('#CommunityTable', path + "/PurchaseOrderAdmin/ChangeDataFlgCom", path + "/PurchaseOrderAdmin/ChangeReportFlgCom");
    initVendors();
    $("#newPurchaseOrderType").click(function (e) {
        e.preventDefault();
        var form = $("#orderTypeTab form");
        var code = $("#NewType_Name").val();
        var dataTable = $("#Types").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Purchase Order Type already exists");
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
                    var enabled = result.data.AllowDataEntry;
                    var id = result.data.Id;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        enabled,
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

    $("#newVendorClass").click(function (e) {
        e.preventDefault();
        var form = $("#vendorClassTab form");
        var code = $("#NewVendorClass_Name").val();
        var dataTable = $("#VendorClasses").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Vendor Class already exists");
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
                    var enabled = result.data.AllowDataEntry;
                    var id = result.data.Id;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        enabled,
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

    $("#newAssetClass").click(function (e) {
        e.preventDefault();
        var form = $("#assetClassTab form");
        var code = $("#NewAssetClass_Name").val();
        var dataTable = $("#AssetClasses").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Asset Class already exists");
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
                    var enabled = result.data.AllowDataEntry;
                    var id = result.data.Id;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        enabled,
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

    $("#newPurchaseOrderApprover").click(function (e) {
        e.preventDefault();
        var form = $("#approverTab form");
        var code = $("#NewApprover_Name").val();
        var dataTable = $("#Approvers").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Approver already exists");
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
                    var level = result.data.ApprovalLevel;
                    var addId = dataTable.fnAddData([
                        name,
                        level,
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

function LaunchModal(form, data) {
    var modal = $('<div />');
    modal.addClass("modal");
    $('body').append(modal);

    form.show();

    var top = Math.max($(window).height() / 2 - form[0].offsetHeight / 2, 0);
    var left = Math.max($(window).width() / 2 - form[0].offsetWidth / 2, 0);
    form.css({ top: top, left: left });

    // Fill form with existing record data
    if (data) {
        $("#Id", form).val(data.Id);
        $("#VendorName", form).val(data.VendorName);
        $("#VendorClassId", form).val(data.VendorClassId);
        $("#CorpVendorId", form).val(data.CorpVendorId);
        $("#Address1", form).val(data.Address1);
        $("#Address2", form).val(data.Address2);
        $("#City", form).val(data.City);
        $("#StateCd", form).val(data.StateCd);
        $("#ZipCode", form).val(data.ZipCode);
        $("#Phone", form).val(data.Phone);
        $("#Fax", form).val(data.Fax);
        $("#Email", form).val(data.Email);
        $("#AllowDataEntry", form).val(data.AllowDataEntry);
    }
}

function ValidateForm(form) {
    if (IsFieldEmpty($("#VendorName", form).val())) {
        alert("Vendor is a required field.");
        return false;
    }
    if (IsFieldEmpty($("#VendorClassId", form).val())) {
        alert("Vendor Class is a required field.");
        return false;
    }
    if (IsFieldEmpty($("#Address1", form).val())) {
        alert("Address 1 is a required field.");
        return false;
    }
    if (IsFieldEmpty($("#City", form).val())) {
        alert("City is a required field.");
        return false;
    }
    if (IsFieldEmpty($("#StateCd", form).val())) {
        alert("State is a required field.");
        return false;
    }
    if (IsFieldEmpty($("#ZipCode", form).val())) {
        alert("Zip Code is a required field.");
        return false;
    }

    return true;
}

function UpdateVendorTable(data) {
    var oTable = $("#VendorTable").dataTable();
    var nRow = document.getElementById(data.Id);

    if (nRow != null) {
        oTable.fnUpdate(data.VendorName, nRow, 0, false);
        oTable.fnUpdate(data.VendorClass.Name, nRow, 1, false);
        oTable.fnUpdate(data.CorpVendorId, nRow, 2, false);
        oTable.fnUpdate(data.FullAddress.replace(/\n/g, "<br />"), nRow, 3, false);
        oTable.fnUpdate(data.Phone, nRow, 4, false);
        oTable.fnUpdate(data.Fax, nRow, 5, false);
        oTable.fnUpdate(data.Email, nRow, 6, false);
    }
    else {
        var rowData = oTable.fnAddData([
            data.VendorName,
            data.VendorClass.Name,
            data.CorpVendorId,
            data.FullAddressRaw,
            data.Phone,
            data.Fax,
            data.Email,
            "<input id=\"EnableDisable_" + data.Id + "\" name=\"EnableDisable\" class=\"EnableDisable\" type=\"checkbox\" checked=\"checked\" value=\"true\" />",
            "<a class=\"edit\" href=\"\">Edit</a>"
        ]);
        nRow = oTable.fnSettings().aoData[rowData[0]].nTr;
        nRow.setAttribute("id", data.Id);
    }

    oTable.fnSort([[0, 'asc']]);
}

function IsFieldEmpty(value) {
    return value === "" || value === null || value === undefined;
}


$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxStop(function () {
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