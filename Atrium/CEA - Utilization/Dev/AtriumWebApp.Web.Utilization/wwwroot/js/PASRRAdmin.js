$(document).ready(function () {
    $("#tabs").tabs();

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
                appCode: "PASRR"
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
});

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});