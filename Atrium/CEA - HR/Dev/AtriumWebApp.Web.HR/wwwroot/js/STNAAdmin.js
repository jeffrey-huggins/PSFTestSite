var measureContextId = 10;

function initCommunities() {
    //Community Table
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "fixedColumns": false,
        "sScrollY": "600px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Communities"
        }
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
                appCode: "STNA"
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

    $(".checkboxPayer").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var payer = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        var nRowId = $(this).closest('tr').attr("id");

        $.ajax({
            url: path + "Base/ChangePayerIncludeFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: nRowId,
                dFlag: checked,
                appCode: "STNA",
                payer: payer
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

function initFacilities() {
    $("#training-facility-table").dataTable({
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": false,
        "bInfo": false,
        "bAutoWidth": false
    });
    $("#training-facility-table").on("click", ".edit", function (e) {
        e.preventDefault();
        var editPath = this.href;

        $.ajax({
            url: editPath
        }).done(function (result) {
            $("#facilityEditModal").html(result);
            $("#facilityEditModal form").validate();
            $("#facilityEditModal #ContactPhone").rules("add", "phoneUS");
            initFacilityModal($("#facilityEditModal"));
        });
    });

}

function initTrainingAction() {
    var oTableActionItems = $("#training-action-item-table").dataTable({
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": false,
        "bInfo": false,
        "bAutoWidth": false
    });

    var nEditing = null;


    $("#training-action-item-table").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableActionItems.fnGetData(nRow);

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
                appCode: "STNA",
                socCode: measureContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableActionItems.fnDeleteRow(nRow);
                } else {
                    alert("Item has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    //Edit
    $('#training-action-item-table').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableActionItems, nEditing);
            editRow(oTableActionItems, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowMeasure(oTableActionItems, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableActionItems, nRow);
            nEditing = nRow;

        }
    });
    $('#training-action-item-table').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableActionItems, nRow);
    });
}

function initTrainingDialog() {

    $("#facilityEditModal").on("click","input[type='submit']", function (e) {
        e.preventDefault();
        var form = $("#facilityEditModal form");
        var code = $("#TrainingFacilityName").val();
        var dataTable = $("#training-facility-table").dataTable();
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
                    var name = result.data.TrainingFacilityName;
                    var id = result.data.STNATrainingFacilityId;
                    var address1 = result.data.Address1;
                    var address2 = result.data.Address2;
                    var city = result.data.City;
                    var state = result.data.StateCd;
                    var zip = result.data.ZipCode;
                    var contactName = result.data.ContactName;
                    var contactPhone = result.data.ContactPhone;
                    var contactEmail = result.data.ContactEmail;

                    var currentRow = $("#training-facility-table").find("#" + id)[0];

                    var addId = dataTable.fnUpdate([
                        name,
                        address1,
                        address2,
                        city,
                        state,
                        zip,
                        contactName,
                        contactPhone,
                        contactEmail,
                        '<a href= "' + path + 'STNAAdmin/Edit/' + id + '" class="edit" > Edit</a > | <a class="select-communities" href="' + path + 'STNAAdmin/GetCommunityAssociations/' + id + '">Associate Communities</a>'
                    ], currentRow);

                    form[0].reset();
                    $("#facilityEditModal").dialog("close");
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

    $("#facilityEditModal").on("click","input[type='button']", function () {
        $("#facilityEditModal form")[0].reset();
        $("#facilityEditModal").dialog("close");
    });


    $("#facilityCreateModal input[type='submit']").on("click", function (e) {
        e.preventDefault();
        var form = $("#facilityCreateModal form");
        var code = $("#TrainingFacilityName").val();
        var dataTable = $("#training-facility-table").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Training Facility with that name already exists.");
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
                    var name = result.data.TrainingFacilityName;
                    var id = result.data.STNATrainingFacilityId;
                    var address1 = result.data.Address1;
                    var address2 = result.data.Address2;
                    var city = result.data.City;
                    var state = result.data.StateCd;
                    var zip = result.data.ZipCode;
                    var contactName = result.data.ContactName;
                    var contactPhone = result.data.ContactPhone;
                    var contactEmail = result.data.ContactEmail;
                    var addId = dataTable.fnAddData([
                        name,
                        address1,
                        address2,
                        city,
                        state,
                        zip,
                        contactName,
                        contactPhone,
                        contactEmail,
                        '<a href= "'+path+'STNAAdmin/Edit/'+id+'" class="edit" > Edit</a > | <a class="select-communities" href="'+path+'STNAAdmin/GetCommunityAssociations/'+id+'">Associate Communities</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                    $("#facilityCreateModal").dialog("close");
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

    $("#facilityCreateModal input[type='button']").on("click", function () {
        $("#facilityCreateModal form")[0].reset();
        $("#facilityCreateModal").dialog("close");
    });

}


$(document).ready(function () {
    $("#tabs").tabs();
    initCommunities();
    initFacilities();
    initTrainingAction();
    initTrainingDialog();
    $("#facilityCreateModal form").validate();
    $("#ContactPhone").rules("add", "phoneUS");

    $("#newFacility").click(function (e) {
        e.preventDefault();
        initFacilityModal($("#facilityCreateModal"));
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
                appCode: "STNA"
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



    $("#SaveLookback").validate({
        rules: {
            LookbackDays: {
                required: true,
                number: true
            }
        }
    });

    //Hospital Associations
    var communityAssociationModal = $("#communityAssocationModal");
    var communitySelectionsTable;
    $("#training-facility-table").on("click", ".select-communities", function (e) {//$(".select-communities").live('click', function (e) {//
        e.preventDefault();
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
    communityAssociationModal.on('click', '#close', function () {
        $(".modal").hide();
        communityAssociationModal.hide();
    });

    communityAssociationModal.on('change', '.selectCommunity', function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = communitySelectionsTable.fnGetData(nRow);
        var contractorId = $("#STNATrainingFacilityId").val();//$("#CommunityId").val();
        $.ajax({
            url: path + "STNAAdmin/ChangeCommunityAssociation",
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
                    alert("Community has failed to be associated. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            },
            error: function () {
                alert("Error communicating with server.");
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

function initFacilityModal($dialog) {
    var width = $dialog.width();
    var height = $dialog.height();
    $dialog.dialog({
        dialogClass: "",
        closeOnEscape: true,
        modal: true,
        open: function (event, ui) {
            $('div.loading').hide();
            $(".ui-dialog").css("width", "0px");
            $(".ui-dialog").css("height", "0px");
            $(".ui-dialog").css("top", "-10px");
            $(".ui-dialog").css("left", "-10px")
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        }
    });
    $dialog.find(".ui-resizable-handle").hide();
    $dialog.dialog("open");

    var top = ((Math.max($(window).height()) / 2) - (height / 2));
    var left = ((Math.max($(window).width()) / 2) - (width / 2));
    $dialog.css({ top: top, left: left, width: width + "px", height: height + "px" });
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
    jqTds[0].innerHTML = '<input type="text" size="60" value="' + aData[0] + '"/>';
    //jqTds[1].innerHTML = '<input type="text" size="3" value="' + aData[1] + '"/>';
    jqTds[1].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRowMeasure(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: 0, //jqInputs[1].value,
            appCode: "STNA",
            socCode: measureContextId
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                //oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 1, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}

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