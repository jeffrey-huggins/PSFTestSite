$(document).ready(function () {
    $("#tabs").tabs();
    //Community Table
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "600px",
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
            url: path + "Base/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "VAC"
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
                appCode: "VAC"
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
                appCode: "VAC",
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

    $("#SaveLookback").validate({
        rules: {
            LookbackDays: {
                required: true,
                number: true
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