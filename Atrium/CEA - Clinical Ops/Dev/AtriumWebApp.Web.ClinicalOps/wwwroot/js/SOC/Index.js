$(".isDate").datepicker();

$("#SOCMeasure").on("change", function () {
    $("#createSOC").removeAttr("disabled");

});

function updateTable() {
    var from = $("#occurredRangeFrom").val();
    var fromValid = checkDate(from);
    if (fromValid == "invalid") {
        alert("Error: Please enter a valid From Date (mm/dd/yyyy)");
        return false;
    }
    else if (fromValid == "infuture") {
        alert("Error: You can not have a From date that is in the future");
        return false;
    }

    var to = $("#occuredRangeTo").val();
    var toValid = checkDate(to);
    if (toValid == "invalid") {
        alert("Error: Please enter a valid To Date (mm/dd/yyyy)");
        return false;
    }
    else if (toValid == "infuture") {
        alert("Error: You can not have a To date that is in the future");
        return false;
    }
    var resident = $("#Residents option:selected").val();
    if (resident != "") {
        var url = path + "StandardsOfCare/GetSOCEventList?patientId=" + resident + "&fromString=" + encodeURIComponent(from) + "&toString=" + encodeURIComponent(to);
        $("#window").load(url);
        $("#selectedResidentView").load(path + "StandardsOfCare/PatientInfo?patientId=" + resident, function () {
            //"Standards Of Care for " + SOCCurrentResidentName;
            $("#title").text("Standards Of Care for " + $("#residentName").text());
        });
        $("#measuresSelection").show();
        $(".instruction").hide();
    }

}
$("#updateTable").click(function () {
    updateTable();

});

function launchEditModal(url) {
    $("#modal").load(url, function () {
        $("#modal").dialog({
            width: "auto",
            height: "auto",
            resizable: false,
            modal: true,
            position: { my: "center top", at: "center top", of: window },
            open: function () {
                //$(this).find("[type=submit]").focus();//.parent().focus();
            },
            close: function () {
                $(this).dialog('destroy');

            }
        });

    });

}

$("#createSOC").click(function () {
    var measureId = $("#SOCMeasure option:selected").val();
    var residentId = $("#Residents option:selected").val();
    var url = path + "StandardsOfCare/EditEvent?measureId=" + measureId + "&residentId=" + residentId;
    launchEditModal(url);
});
$("#sideBar").on("change", "#Residents", function () {
    updateTable();
});
$("#sideBar").on("click", "#lookbackUpdate", function (e) {
    e.preventDefault();
    var lookbackDate = getMomentDate($("#LookbackDate").val());
    var lookbackCheck = checkDate($("#LookbackDate").val());
    if (lookbackCheck == "invalid") {
        alert("Error: Please enter a valid Last Census Date (mm/dd/yyyy)");
    }
    else if (lookbackCheck == "infuture") {
        alert("Error: You can not have a Census date that is in the future");
    }
    else {
        ShowProgress();
        $("#SideDDL").ajaxSubmit({
            url: path + "StandardsOfCare/UpdateSideBar",
            type: "POST",
            success: function (result) {
                $("#sideBar").html(result);
                initSidebar();
            }

        });
    }

});

$("#sideBar").on("change", "#Communities", function () {
    ShowProgress();
    $("#SideDDL").ajaxSubmit({
        url: path + "StandardsOfCare/UpdateSideBar",
        type: "POST",
        success: function (result) {
            $("#sideBar").html(result);
            initSidebar();
        }

    });
});


function initSidebar() {
    $("#LookbackDate").datepicker({
        beforeShow: function (textbox, instance) {
            instance.dpDiv.css({
                marginTop: '10px',
                marginLeft: '0px'
            });
        }
    });
    $("#Residents").val("");
    $("#title").text("Standards Of Care");
    $(".instruction").show();
    $("#measuresSelection").hide();
    $("#window").html("");

}
    //checkDate