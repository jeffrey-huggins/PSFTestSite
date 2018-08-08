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
                "sWidth": '50px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '50px'
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
                    appCode: "PRTR"
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
                    appCode: "PRTR"
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
        $( lookupAdminTable.dataTable).on('click','a.edit', function (e) {
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

    $("#NewType").keyup(function () {
        if($("#NewType").val().trim())
        {
            $("#NewTypeSubmit").prop("disabled", false);
        } else {
            $("#NewTypeSubmit").prop("disabled", true);
        }
    });

    $("#NewVendorClass").keyup(function () {
        if ($("#NewVendorClass").val().trim()) {
            $("#NewVendorSubmit").prop("disabled", false);
        } else {
            $("#NewVendorSubmit").prop("disabled", true);
        }
    });

    $("#NewAssetClass").keyup(function () {
        if ($("#NewAssetClass").val().trim()) {
            $("#NewAssetSubmit").prop("disabled", false);
        } else {
            $("#NewAssetSubmit").prop("disabled", true);
        }
    });

    $("#NewApprover").keyup(function () {
        if ($("#NewApprover").val().trim()) {
            $("#NewApproverSubmit").prop("disabled", false);
        } else {
            $("#NewApproverSubmit").prop("disabled", true);
        }
    });

    
    return {
        
        setupCommunityTable: function (selector, displayUpdateUrl, reportUpdateUrl) {
            var communityTable = new CommunityAdminTable(selector);
            communityTable.wireupDisplayUpdateHandler(displayUpdateUrl);
            communityTable.wireupReportUpdateHandler(reportUpdateUrl);
            return communityTable;
        }
    }
}();


function initContractorTable() {
    var nEditing = null;
    var oTable = $('#contractor-table').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        //"sScrollY": "370px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": "150px"
            },
            {
                "sWidth": "150px"
            },
            {
                "sWidth": "200px"
            },
            {
                //"sWidth": "100px",
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Communities"
        }
    });
    $("#contractor-table").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTable.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "PayrollTransferAdmin/DeleteContractor",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                id: nRowId,
            },
            success: function (result) {
                if (result.Success) {
                    oTable.fnDeleteRow(nRow);
                } else {
                    if (result.Message) {
                        alert(result.Message);
                    }
                    else {
                        alert("Contractor has failed to be deleted. Please try again " +
                            "and if you see this message repeatedly the server may be down.");
                    }
                }

            }
        });

    });
    $("#contractor-table").on("click", "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTable, nEditing);
            editRow(oTable, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(oTable, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTable, nRow);
            nEditing = nRow;

        }

    });
    $('#contractor-table').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTable, nRow);
    });
    $("#contractor-table").on("click", ".select-communities", function (e) {//$(".select-communities").live('click', function (e) {//
        e.preventDefault();
        var communityAssociationModal = $("#communityAssocationModal");
        $.ajax({
            url: this.href,
            success: function (data) {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                communityAssociationModal.html(data);
                communityAssociationModal.show();
                var top = Math.max($(window).height() / 2 - communityAssociationModal[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - communityAssociationModal[0].offsetWidth / 2, 0);
                communityAssociationModal.css({ top: top, left: left });
                communitySelectionsTable = $('#selectedCommunities', communityAssociationModal).dataTable({
                    "bFilter": false,
                    "bAutoWidth": false,
                    "sScrollY": "370px",
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
                        "sEmptyTable": "No Communities"
                    }
                });
            },
            error: function () {
                alert("Could not communicate with the server.");
            }
        });
    });

    $("#contractorTab input[type='submit']").on("click", function (e) {
        e.preventDefault();
        var form = $("#contractorTab form");
        var dataTable = $("#contractor-table").dataTable();
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
                    var firstname = result.data.FirstName;
                    var id = result.data.PTContractorId;
                    var lastname = result.data.LastName;
                    var vendorNbr = result.data.VendorNbr;

                    var addId = dataTable.fnAddData([
                        firstname,
                        lastname,
                        vendorNbr,
                        '<a href= "" class="edit" >Edit</a > | <a class="select-communities" href="' + path + 'PayrollTransferAdmin/GetCommunityAssociations/' + id + '">Associate Communities</a> | <a href="" class="delete">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    console.log(result);
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {

                alert("Server is not responding. Please reload page and try again");

            }
        });

    });

}

function initCommunitAssociation() {
    var communityAssociationModal = $("#communityAssocationModal");
    

    communityAssociationModal.on('click', '#close', function () {
        $(".modal").hide();
        communityAssociationModal.hide();
    });

    communityAssociationModal.on('change', '.selectCommunity', function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var communitySelectionsTable = $('#selectedCommunities', communityAssociationModal).dataTable();
        var aData = communitySelectionsTable.fnGetData(nRow);
        var contractorId = $("#ContractorId").val();
        $.ajax({
            url: path + "PayrollTransferAdmin/ChangeCommunityAssociation",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                contractorId: contractorId,
                communityId: nRowId,
                isAssociated: checked
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Hospital has failed to be associated. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            },
            error: function () {
                alert("Error communicating with server.");
            }
        });
    });

}

$(document).ready(function () {
    $("#tabs").tabs();
    initContractorTable();
    initCommunitAssociation();
    PurchaseOrderAdmin.setupCommunityTable('#CommunityTable', path + "PayrollTransferAdmin/ChangeDataFlgCom", path + "PayrollTransferAdmin/ChangeReportFlgCom");
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
    jqTds[0].innerHTML = '<input type="text" value="' + aData[0] + '"/>';
    jqTds[1].innerHTML = '<input type="text" value="' + aData[1] + '"/>';
    jqTds[2].innerHTML = '<input type="text" value="' + aData[2] + '"/>';
    jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRow(oTable, nRow, nRowId, contextId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "PayrollTransferAdmin/EditContractor",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            id: nRowId,
            firstName: jqInputs[0].value,
            lastName: jqInputs[1].value,
            vendorNbr: jqInputs[2].value
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate(jqInputs[2].value, nRow, 2, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a> | <a class="select-communities" href="' + path+'PayrollTransferAdmin/GetCommunityAssociations/' + nRowId + '">Associate Communities</a> | <a href="" class="delete">Delete</a>', nRow, 3, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });

    //oTable.fnStandingRedraw();
    return true;
}