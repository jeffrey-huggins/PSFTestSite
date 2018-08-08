$(document).ready(function () {
    $("#tabs").tabs();
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "aaSorting": [],
        "sScrollY": "600px",
        "sDom": "frtS",
        "iDisplayLength": -1
    });

    $("#StateName").val($("#States option:selected").text());
    $("#PayPeriods").val($("#States option:selected").val());
    $("#States").change(function () {
        var statePayCd = $("#States option:selected").val();
        $("#StateName").val($("#States option:selected").text());
        $("#PayPeriods").val(statePayCd);
    });

    $(".checkbox").click(function () {
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "BaseRiskManagement/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "RMU"
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

    $(".checkboxReport").click(function () {
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "BaseRiskManagement/ChangeReportFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "RMU"
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
});
$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});