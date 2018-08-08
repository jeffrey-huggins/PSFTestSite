
$(document).ready(function () {
    $("#tabs").tabs();
    $("#CommunityTable").dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
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
            },
            null
        ],
        "oLanguage": {
            "sEmptyTable": "No Communities"
        }
    });
    $("#CommunityTable .checkboxDisplay").change(function (data) {
        var checked = $(this).prop("checked");
        var commId = $(this).attr("id");
        var appCode = "SCH";
        var comName = $(this).closest("tr").children(":first-child").text();
        $.ajax({
            url: path + "/ScheduleAdmin/ChangeDataFlgCom",
            dataType: "json",
            type: 'POST',
            data: {
                community: commId,
                dFlag: checked,
                appCode: "SCH"
            },
            success: function (result) {
                if (result.Success) {
                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Community has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
                //SetCountDownTime(1800);
            }
        });
    });
    $("#CommunityTable .checkboxReport").change(function (data) {
        var checked = $(this).prop("checked");
        var commId = $(this).attr("id");
        var appCode = "SCH";
        var comName = $(this).closest("tr").children(":first-child").text();
        $.ajax({
            url: path + "/ScheduleAdmin/ChangeReportFlgCom",
            dataType: "json",
            type: 'POST',
            data: {
                community: commId,
                dFlag: checked,
                appCode: "SCH"
            },
            success: function (result) {
                if (result.Success) {
                }
                else {
                    checkbox.attr("checked", !checked);
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
                //SetCountDownTime(1800);
            }
        });
    });
    $(".manage-arearooms").on('click', function (e) {
        e.preventDefault();
        $.ajax({
            url: this.href,
            success: function (result) {
                $("#areaRoomsModal").html(result);
                var $dialog = $("#areaRoomsModal");
                var width = $dialog.width();
                var height = $dialog.height();
                //$dialog.empty();
                $dialog.dialog({
                    //title: "Warning on SAVE - Employee not selected",
                    dialogClass: "",
                    closeOnEscape: true,
                    modal: true,
                    open: function (event, ui) {
                        $('div.loading').hide();
                        window.HideProgress();
                        $(".ui-dialog").css("width", "0px");
                        $(".ui-dialog").css("height", "0px");
                        $(".ui-dialog").css("top", "-10px");
                        $(".ui-dialog").css("left", "-10px")
                        $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
                    },
                    close: function (event, ui) {
                        $('div.loading').hide();
                        window.HideProgress();
                    }
                });
                $dialog.draggable({ disabled: false });
                $dialog.dialog("open");

                var top = ((Math.max($(window).height()) / 2) - ($dialog.height() / 2));
                var left = ((Math.max($(window).width()) / 2) - ($dialog.width() / 2));

                $dialog.css({ top: top, left: left });
            },
            error: function () {
                alert("Could not communicate with the server.");
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