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

    if (resident != "0" && resident != "") {
        var url = path + "IncidentTracking/PatientInfo?=" + resident;
        $("#selectedResidentView").load(url, function () {
            //"Standards Of Care for " + SOCCurrentResidentName;
            $("#title").text("Incident Tracking for " + $("#residentName").text());
        });
        $("#listing").load(path + "IncidentTracking/GetIncidentList?patientId=" + resident + "&fromString=" + encodeURIComponent(from) + "&toString=" + encodeURIComponent(to), function () {
            $("#incidentTable").dataTable({
                "bFilter": false,
                "bAutoWidth": false,
                "aaSorting": [[2, "desc"]],
                "sDom": "frtS",
                "iDisplayLength": -1
            });
        });
        loadForm();
    }

}
function loadForm() {
    var resident = $("#Residents option:selected").val();
    if (resident == "" || resident == "0") {
        $(".instruction").hide();
        $("#listing").html("");
        $("#title").text("Incident Tracking");
    }
    else {
        $("#editForm").load(path + "IncidentTracking/EditIncident?patientId=" + resident, editFormLoaded);
    }
}
var addIndex = 0;
function editFormLoaded() {
    addIndex = 0;
    $("#addDocument").on("click", function (e) {
        var row = $("<div>").load(path + "IncidentTracking/CreateNewDocument", function () {
            ApplyUniqueItemIndex($(row), addIndex, "Documents");
            $("#incidentDocuments").append(row);
            addIndex++;
        });

    });
    $("#incidentDocuments").on("click", ".delete-item", function () {
        var div = $(this).closest("div");
        var id = div.find("[data-field='Id']").val();
        if (id != "0") {
            var url = path + "IncidentTracking/DeleteIncidentEventDocument?id=" + id;
            $.ajax({
                url: url,
                success: function (result) {
                    if (result.success) {
                        
                        div.remove();
                    }
                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
        else {
            $(this).closest("div").remove();
            FixDocumentIndex();
        }
        
    });
    $.validator.setDefaults({ ignore: [] });
    $("#saveIncidentForm").removeData("validator");
    $("#saveIncidentForm").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#saveIncidentForm"));
    $("#saveIncidentForm").validate();

    if ($("#Event_PatientIncidentEventId").val() == "0") {
        $("#Event_IncidentDateTime").val("");
    }
    $("#interventionTable").dataTable({
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "bSort":false,
        "aoColumns": [
            {
                "bSortable": false
            },
            {
                "bSortable": false
            }
        ]
    });
    $("#treatmentTable").dataTable({
        "bAutoWidth": false,
        "sScrollY": "150px",
        "bSort":false,
        "sDom": "rt",
        "iDisplayLength": -1,
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "bSortable": false
            },
            {
                "bSortable": false
            }
        ]

    });
    $(".instruction").hide();
    $(".isDate").datepicker();
    $(".isDateTime").datetimepicker({
        timeFormat: "hh:mm tt"
    });
    if ($("#Event_ReportedToStateFlg").is(":checked")) {
        $("#Event_ReportedToStateDateTime").rules("add", {
            no_future_datetime: true,
            required: true,
            date_before_incident: true
        });
    }
    $(".reportedState").toggle($("#Event_ReportedToStateFlg").is(":checked"));
    $("#Event_ReportedToStateFlg").on("click", function () {
        $(".reportedState").toggle($("#Event_ReportedToStateFlg").is(":checked"));
        if ($(this).is(":checked")) {
            $("#Event_ReportedToStateDateTime").rules("add", {
                required: true,
                date_before_incident: true,
                no_future_datetime: true
            });
        }
        else {
            $("#Event_ReportedToStateDateTime").rules("remove");
            $("#Event_ReportedToStateDateTime").val("");
        }
    })
    $(".inBuildingDate").toggle($("#Event_StateInBuildingFlg").is(":checked"));
    if ($("#Event_StateInBuildingFlg").is(":checked")) {
        $("#Event_StateInBuildingDate").rules("add", {
            required: true,
            date_before_incident_time: true,
            no_future_date: true
        });
    }
    $("#Event_StateInBuildingFlg").on("click", function () {
        $(".inBuildingDate").toggle($("#Event_StateInBuildingFlg").is(":checked"));
        if ($(this).is(":checked")) {
            $("#Event_StateInBuildingDate").rules("add", {
                required: true,
                date_before_incident_time: true,
                no_future_date: true
            });
        }
        else {
            $("#Event_StateInBuildingDate").rules("remove");
            $("#Event_StateInBuildingDate").val("");
        }
    })
    $(".regionSelected").toggle($("#Event_RegionalNurseClosedFlg").is(":checked"));
    if ($("#Event_RegionalNurseClosedFlg").is(":checked")) {
        $("#Event_RegionalNurseEmployeeId").rules("add", {
            required: true
        });

    }
    $("#Event_RegionalNurseClosedFlg").on("click", function () {
        $(".regionSelected").toggle($("#Event_RegionalNurseClosedFlg").is(":checked"));
        if ($(this).is(":checked")) {
            $("#Event_RegionalNurseEmployeeId").rules("add", {
                required: true
            });
        }
        else {
            $("#Event_RegionalNurseEmployeeId").rules("remove");
            $("#Event_RegionalNurseEmployeeId").val("");
        }
    })
    $("#saveIncident").on("click", function (e) {
        e.preventDefault();
        var form = $("#saveIncidentForm");
        var fileNames = [];
        var filesUploading = $("[type='file'");
        var filesUploaded = $("#incidentDocuments").find("a");
        for (var i = 0; i < filesUploaded.length; i++) {
            var filename = $(filesUploaded[i]).text().trim();
            fileNames.push(filename);
        }

        for (var i = 0; i < filesUploading.length; i++) {
            var filepath = $(filesUploading[i]).val();
            var splitFilePath = filepath.split("\\");
            var filename = splitFilePath[splitFilePath.length - 1];
            if (fileNames.indexOf(filename) >= 0) {
                alert("A file with the name " + filename + " is being added twice.  Delete the old file or rename the new file to continue.");
                return;
            }
            else {
                fileNames.push(filename);
            }
        }
        if (!form.valid()) {
            return;

        }
        form.ajaxSubmit({
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result.success) {
                    if ($("#SaveDocuments").find("input").length > 0) {
                        FixDocumentIndex();
                        var eventId = result.id;
                        $("[data-field='ParentId']").val(eventId);
                        var docForm = $("#SaveDocuments");
                        docForm.ajaxSubmit({

                        });
                    }

                    updateTable();
                    loadForm();
                }
                else {
                    alert(result.data);
                }

            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }

        }

        );


    });
    $("#clearIncident").on("click", function (e) {
        e.preventDefault();
        loadForm();
    });


}

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
            url: path + "IncidentTracking/UpdateSideBar",
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
        url: path + "IncidentTracking/UpdateSideBar",
        type: "POST",
        success: function (result) {
            $("#sideBar").html(result);
            initSidebar();
        }

    });
});

$("#listing").on("click", ".edit", function (e) {
    e.preventDefault();
    $("#editForm").load(this.href, editFormLoaded);

});
$("#listing").on("click", ".delete", function (e) {
    e.preventDefault();
    $.ajax({
        type: "POST",
        url: this.href,
        success: function (result) {
            if (result.Success) {
                updateTable();
            }
            else {
                alert("Unable to delete row, try refreshing the page and try again.");
            }
        }
    });
});
$(".isDate").datepicker();
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
    $("#title").text("Incident Tracking");
    $(".instruction").show();
    $("#editForm").html("");
    $("#listing").html("");
}
function ApplyUniqueItemIndex(row, index, uniqueIdentifier) {
    row.find("input").each(function () {
        var field = $(this).data('field');
        this.id = uniqueIdentifier + '_' + index + '__' + field;
        this.setAttribute('name', uniqueIdentifier + '[' + index + '].' + field);
    });
}

function FixDocumentIndex() {
    var documents = $("#documentSection").find("[type='file']");
    for (addIndex = 0; addIndex < documents.length; addIndex++) {
        ApplyUniqueItemIndex($(documents[addIndex]).closest("div"), addIndex, "Documents");
    }
}

$("#updateTable").on("click", updateTable);