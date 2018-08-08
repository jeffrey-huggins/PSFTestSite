Number.prototype.formatMoney = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

function RefreshBudgetItemData() {
    var filterModel = {
        "BudgetYear": $("#budgetYear").val(),
        "CommunityId": $("#CurrentCommunity").val()
    };
    var URL = window.location.pathname + '/GetBudgetItemData/';
    $.ajax({
        url: URL,
        type: "POST",
        data: filterModel,
        success: function (data) {
            var result = JSON.parse(data);
            var budgetTable = $("#budgetTable").dataTable();
            //clear data table
            budgetTable.fnClearTable();

            //insert data rows
            budgetTable.fnAddData(result.BudgetItems);

            if (budgetTable.fnGetData().length > 0) {
                $("#budget-total-cell").prop("textContent", Number(result.BudgetedAmount).formatMoney(2));
                $("#budget-table-foot").removeClass("hidden");
            } else {
                $("#budget-table-foot").addClass("hidden");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('Could not communicate with the server.');
        }
    });
}

function RefreshBudgetData() {
    var filterModel = {
        "BudgetYear": $("#budgetYear").val(),
        "CommunityId": $("#CurrentCommunity").val()
    };
    var URL = window.location.pathname + '/GetBudgetData/';
    $.ajax({
        url: URL,
        type: "POST",
        data: filterModel,
        success: function (data) {
            var result = JSON.parse(data);
            $("#BedCount").val(result.BedCount);
            $("#AmtPerBed").val(Number(result.AmtPerBed).toFixed(2));
            $("#EmergencyBudgetAmt").val(Number(result.EmergencyBudgetAmt).toFixed(2));
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('Could not communicate with the server.');
        }
    });
}

function ValidateBudgetItemFields() {
    if ($("#budgetItemDescription").val() == "") {
        alert("Description cannot be blank.");
        return false;
    }
    if (isNaN($("#budgetItemAmt").val())) {
        alert("Budget Amount must be a number.");
        return false;
    }

    return true;
};

//function ClearBudgetDataFields() {
//    $("#BedCount").val(0);
//    $("#AmtPerBed").val(Number(0).toFixed(2));
//    $("#EmergencyBudgetAmt").val(Number(0).toFixed(2));
//}

function ClearBudgetItemFields() {
    //$("#budgetYear").val(result.BudgetYear);
    //$("#budgetQuarter").val(result.BudgetQtr);
    $("#budgetItemId").val(0);
    $("#budgetItemCommunityId").val("");
    $("#budgetItemDescription").val("");
    $("#budgetItemComments").val("");
    $("#budgetItemAmt").val(Number(0).toFixed(2));
    $("#budgetItemIsSpecialProject").val(false);
    $("#budgetItemIsSpecialProject").prop("checked", false);
}

function SetupModal() {
    $("#btnModalSave").click(function () {
        //var form = $("#SavePASRRLog");
        // if (ValidateForm(form)) {
        var URL = window.location.pathname + '/SaveEmergencyBudget/';
        var msg = {
            "BudgetYear": $("#budgetYear").val(),
            "BudgetAmt": $("#EmergencyBudgetAmt").val()
        };

        $.ajax({
            url: URL,
            dataType: "json",
            cache: false,
            traditional: true,
            type: "POST",
            data: msg,
            success: function (result) {
                alert("Emergency Budget has been successfully saved.");
                //UpdatePatientPASRRTable(form, result);
                //$("#PASRRType").val("");
                $("#btnModalClose").click();
            },
            error: function (ex) {
                alert("Emergency Budget save failure occurred!");
            }
        });
        //}
    });

    $("#btnModalClear").click(function () {
        //$("input:text", form).val("");
        //$("input:checkbox", form).prop("checked", false);
        $(".modal, #popupModal").hide();
    });

    $("#btnModalClose").click(function () {
        $(".modal, #popupModal").hide();
        //$("#btnModalClear").click();

    });
}

function LaunchModal() {
    var div = $("<div />");
    div.addClass("modal");
    $("body").append(div);

    var modal = $("#popupModal");
    modal.show();

    var top = Math.max($(window).height() / 3 - modal[0].offsetHeight / 3, 0);
    var left = Math.max($(window).width() / 2 - modal[0].offsetWidth / 2, 0);
    modal.css({ top: top, left: left });


}


function PrepareBudgetTable() {
    var formContent = $('#formContent');
    var budgetTable = $("#budgetTable").dataTable({
        "bAutoWidth": false,
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "mData": "BudgetQtr",
                "mRender": function (data, type, full) {
                    return 'Q' + data
                },
                "sWidth": "100px"
            },
            {
                "mData": "Description",
                "sWidth": "400px",
                "aDataSort": [0, 1]
            },
            {
                "mData": "BudgetAmt",
                "mRender": function (data, type, full) {
                    return Number(data).formatMoney(2) //.toFixed(2)
                },
                "sClass": "right",
                "sWidth": "50px"
            },
            {
                "mData": "Id",
                "mRender": function (data, type, full) {
                    return '<a href="#"  class="edit-budget-item" data-budget-item-id="' + data + '">Edit</a>'
                },
                "sWidth": "20px",
                "bSortable": false
            },
            {
                "mData": "Id",
                "mRender": function (data, type, full) {
                    return '<a href="#" class="delete-budget-item" data-budget-item-id="' + data + '" >Delete</a>'
                },
                "sWidth": "30px",
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No records"
        }
    });


    $("#budgetYear").change(function (e) {
        $("#budgetYearTxt").val($("#budgetYear").val());
        $("#modalBudgetYearTxt").prop("textContent", $("#budgetYear").val());
        ClearBudgetItemFields();
        RefreshBudgetData();
        RefreshBudgetItemData();
    });

    budgetTable.on('click', '.edit-budget-item', function (e) {
        var element = $(this);
        var budgetItemId = element.data("budget-item-id");
        var URL = window.location.pathname + '/GetBudgetItem/' + budgetItemId;

        $.ajax({
            url: URL,
            success: function (data) {
                var result = JSON.parse(data);
                $("#budgetYear").val(result.BudgetYear);
                $("#budgetQuarter").val(result.BudgetQtr);
                $("#budgetItemId").val(result.Id);
                $("#budgetItemCommunityId").val(result.CommunityId);
                $("#budgetItemDescription").val(result.Description);
                $("#budgetItemComments").val(result.Comments);
                $("#budgetItemAmt").val(Number(result.BudgetAmt).toFixed(2));
                $("#budgetItemIsSpecialProject").val(result.IsSpecialProject);
                $("#budgetItemIsSpecialProject").prop("checked", result.IsSpecialProject);

                $("#commit-budget-item").removeClass();
                $("#commit-budget-item").addClass("update-budget-item");
                $("#commit-budget-item").text("Save Changes");

                $("#budgetItemView").removeClass("hidden");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    budgetTable.on('click', '.delete-budget-item', function (e) {
        var element = $(this);
        if (confirm("Are you sure you would like to delete this budget item?")) {
            var budgetItemId = element.data("budget-item-id");
            var URL = window.location.pathname + '/DeleteBudgetItem/' + budgetItemId
            $.ajax({
                url: URL,
                success: function (data) {
                    RefreshBudgetItemData();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Could not communicate with the server.');
                }
            });
        }
    });


    $('#saveBudgetData').click(function (e) {
        var msg = {
            "BudgetYear": $("#budgetYear").val(),
            "CommunityId": $("#CurrentCommunity").val(),
            "BedCount": $("#BedCount").val(),
            "AmtPerBed": $("#AmtPerBed").val()
            //"EmergencyBudgetAmt": $("#EmergencyBudgetAmt").val()
        };
        var URL = window.location.pathname + '/SaveBudgetData/';
        $.ajax({
            url: URL,
            type: "POST",
            data: msg,
            success: function (data) {
                alert('Budget information saved.');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    $('.add-budget-item').click(function (e) {
        //formContent.on('click', '.add-budget-item', function (e) {
        var element = $(this);
        ClearBudgetItemFields();
        $("#budgetItemView").removeClass("hidden");

        //we change the class and text for the save budget link.
        $("#commit-budget-item").removeClass();
        $("#commit-budget-item").addClass("save-budget-item");
        $("#commit-budget-item").text("Save Budget Item");
    });


    $('.update-emergency-budget').click(function (e) {
        LaunchModal();
    });

    var budgetItemContent = $('#budgetItemView');
    //$('.save-budget-item').click(function (e) {
    budgetItemContent.on('click', '.save-budget-item', function (e) {
        var element = $(this);

        if (ValidateBudgetItemFields()) {
            var msg = {
                "Id": $("#budgetItemId").val(),
                "BudgetYear": $("#budgetYear").val(),
                "BudgetQtr": $("#budgetQuarter").val(),
                "CommunityId": $("#CurrentCommunity").val(),
                "Description": $("#budgetItemDescription").val(),
                "Comments": $("#budgetItemComments").val(),
                "BudgetAmt": $("#budgetItemAmt").val(),
                "IsSpecialProject": $("#budgetItemIsSpecialProject").prop("checked")
            };
            var URL = window.location.pathname + '/AddBudgetItem/';
            $.ajax({
                url: URL,
                type: "POST",
                data: msg,
                success: function (data) {
                    ClearBudgetItemFields();
                    $("#budgetItemView").addClass("hidden");
                    alert('Budget item was successfully added.');
                    //refresh the data set
                    RefreshBudgetItemData();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Could not communicate with the server.');
                    $("#budgetItemView").removeClass("hidden");
                }
            });

        }
    });

    //$('.update-budget-item').click(function (e) {
    budgetItemContent.on('click', '.update-budget-item', function (e) {
        var element = $(this);
        if (ValidateBudgetItemFields()) {
            var msg = {
                "Id": $("#budgetItemId").val(),
                "BudgetYear": $("#budgetYear").val(),
                "BudgetQtr": $("#budgetQuarter").val(),
                "CommunityId": $("#CurrentCommunity").val(),
                "Description": $("#budgetItemDescription").val(),
                "Comments": $("#budgetItemComments").val(),
                "BudgetAmt": $("#budgetItemAmt").val(),
                "IsSpecialProject": $("#budgetItemIsSpecialProject").prop("checked")
            };

            var URL = window.location.pathname + '/UpdateBudgetItem/';

            $.ajax({
                url: URL,
                type: "POST",
                data: msg,
                success: function (data) {
                    ClearBudgetItemFields();
                    $("#budgetItemView").addClass("hidden");
                    alert('Budget item was successfully updated.');
                    //refresh the data set
                    RefreshBudgetItemData();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Could not communicate with the server.');
                    $("#budgetItemView").removeClass("hidden");
                }
            });


        }
    });

    $('#cancel-budget-item-changes').click(function (e) {
        //formContent.on('click', '#cancel-budget-item-changes', function (e) {
        ClearBudgetItemFields();
        $("#budgetItemView").addClass("hidden");
    });

    SetupModal();
    RefreshBudgetItemData();
}