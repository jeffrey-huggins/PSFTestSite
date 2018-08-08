function PreparePage() {
    $("#tabs").tabs();

    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
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
    $(".checkbox").change(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        $.ajax({
            url: path + "MockSurveyPlanOfCorrectionAdmin/ChangeDataFlgCom",
            dataType: "json",
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "MSUPOC"
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Community has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
                
            }
        });
    });
    $(".checkboxReport").change(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        $.ajax({
            url: path + "MockSurveyPlanOfCorrectionAdmin/ChangeReportFlgCom",
            dataType: "json",
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "MSUPOC"
            },
            success: function (result) {
                if (result.Success) {
                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
                
            }
        });
    });
}

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});