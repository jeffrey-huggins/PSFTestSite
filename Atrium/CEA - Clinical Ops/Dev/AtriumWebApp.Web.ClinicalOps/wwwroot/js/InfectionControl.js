
function initSidebar() {
    $("#listing").html("");
    $("#title").text("Infection Control");
    $(".instruction").show();
    $("#Residents").val("");
    $("#editForm").html("");
    $("#listing").html("");
}

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
        $("#editForm").load(path + "InfectionControl/EditOrCreateInfectionVM?patientId=" + resident, loadEditForm);

        var url = path + "InfectionControl/PatientInfo?=" + resident;
        $("#selectedResidentView").load(url, function () {
            $("#title").text("Infection Control for " + $("#residentName").text());
        });

        $("#listing").load(path + "InfectionControl/GetInfectionList?patientId=" + resident + "&fromString=" + encodeURIComponent(from) + "&toString=" + encodeURIComponent(to), function () {
            $("#infectionTable").dataTable({
                "bFilter": false,
                "bAutoWidth": false,
                "aaSorting": [[1, "desc"]],
                "sDom": "frtS",
                "iDisplayLength": -1
            });

            $("#infectionTable").on("click", ".edit", function (e) {
                e.preventDefault();
                $("#editForm").load(this.href, loadEditForm);
            });
            $("#infectionTable").on("click", ".delete", function (e) {
                e.preventDefault();
                //var id = $(this).closest("tr").attr("vaccineId");
                var url = this.href;
                $.ajax({
                    url: url,
                    success: function (result) {
                        if (result.Success) {
                            updateTable();

                        }
                        else {
                            alert(result.data);

                        }

                    },
                    error: function () {
                        alert("Server is not responding. Please reload page and try again");
                    }

                });

            });
        });
        //loadForm();
    }

}

$("#sideBar").on("change", "#Residents", function () {
    if ($("#Residents").val() != "" && $("#Residents").val() != "0") {
        $(".instruction").hide();
        updateTable();
    }
    else {
        $(".instruction").show();
        $("#listing").html("");
    }

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
            url: path + "InfectionControl/UpdateSideBar",
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
        url: path + "InfectionControl/UpdateSideBar",
        type: "POST",
        success: function (result) {
            $("#sideBar").html(result);
            initSidebar();
        }

    });
});
$(".isDate").datepicker();
$("#updateTable").on("click", updateTable);

function wireupRecultureForm() {
    $("#addRecultureDate").on("click", function () {

        var table = $("#recultureTable");
        var rowCount = table.dataTable().fnGetData().length;

        table.dataTable().fnAddData([
            getRecultureInputs(rowCount),
            "<a href='#' class='removeReculture'>Remove</a>"
        ]);
        table.find(".isDate").datepicker({
            onSelect: function () {
                //ie fix for date picker re-opening after selection in modal windows
                $(this).parent().focus();
            }
        });
    });
    $("#recultureTable").on("click", ".removeReculture", function (e) {
        e.preventDefault();
        var row = $(this).closest("tr");
        row.hide();
        row.find("#deleteDate").val("true");
    });
    $("#recultureTable").find(".isDate").each(function (index, element) {
        var row = $(element).closest("tr");
        row.attr("oldValue", $(element).val());
    });
    $("#recultureSave").on("click", function () {
        $("#recultureTable").find(".isDate").each(function (index, element) {
            var row = $(element).closest("tr");
            row.attr("oldValue", $(element).val());
        });
        $("#recultureModal").dialog("close");
    });
}

function getRecultureInputs(row) {
    var html = "<input type='hidden' name='RecultureDates[" + row + "].Reculture.ReCultureId' value='0'/>";
    html += "<input type='hidden' name='RecultureDates[" + row + "].DeleteDate' value='false' id='deleteDate' />";
    html += "<input type='text' name='RecultureDates[" + row + "].Reculture.RecultureDate' class='isDate'/>";
    return html;
}

function loadEditForm() {
    $("#reculture").on("click", function (e) {
        e.preventDefault();
        $("#recultureModal").dialog({
            modal: true,
            close: function (event, ui) {
                $("#recultureTable").find(".isDate").each(function (index, element) {
                    var row = $(element).closest("tr");
                    $(element).val(row.attr("oldValue"));
                    if ($(element).val() == "") {
                        row.hide();
                        row.find("#deleteDate").val("true");
                    }
                });
                $("#recultureModal").dialog("destroy");

            }
        });
    });
    $("[type='checkbox']").removeAttr("data-val-required");
    $("[type='checkbox']").removeAttr("data-val");
    wireupRecultureForm();
    var resident = $("#Residents option:selected").val();
    $("#vaccinationList").load(path + "InfectionControl/GetVaccineList?patientId=" + resident, function () {
        var oTableVac = $("#vaccineTable").dataTable({
            "bFilter": false,
            "bAutoWidth": false,
            "sScrollY": "100px",
            "sDom": "frtS",
            "iDisplayLength": -1,
            "aoColumns": [
                null, // Type
                {
                    "aDataSort": [4, 1, 0] // Date
                },
                null, // Refused
                null, // Consent
                {
                    "bVisible": false // Date Sort Flag
                }
            ]
        });
        oTableVac.fnSort([[1, 'desc']]);
    });
    var ifcId = $("#Event_PatientIFCEventId").val();
    if (ifcId == "0") {
        $("#Event_OnsetDate").val("");
    }
    $(".tableContainer").find("table").dataTable({
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [{
            "bVisible": false
        },
            null,
        {
            "bSortable": false
            }],
        "aaSorting": [[0,'asc']]
    });

    $("#recultureTable").dataTable({
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "rt",
        "iDisplayLength": -1
    });

    $(".filter").on("keyup", function () {
        var container = $(this).closest(".tableContainer");
        var table = container.find("[id$=Table]").dataTable();
        table.fnFilter("^" + $(this).val() + ".*", 1, true);
    });

    $(".clearFilter").on("click", function (e) {
        e.preventDefault();
        var container = $(this).closest(".tableContainer");
        container.find(".filter").val("");
        var table = container.find("[id$=Table]").dataTable();
        table.fnFilter("^.*", 1, true);
    });

    $(".isDate").datepicker();

    $("#saveInfection").on("click", function (e) {
        e.preventDefault();
        var form = $("#saveInfectionControl");
        if (!form.valid()) {
            return;

        }
        form.ajaxSubmit({
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result.success) {
                    updateTable();
                }
                else {
                    alert("Error: Event not found in database");
                }

            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }

        }

        );

    });

    $.validator.addMethod("table-count", function (value, element) {
        var container = $(element).closest("div");
        var min = 0;
        if (container.find(".required").val() == "True") {
            min = 1;
        }
        var max = parseInt($(element).val());
        var count = container.find(":checked").length;
        if (min <= count && max >= count) {
            return true;
        }
        else {
            return false;
        }
    }, "Make sure the proper number of items are checked.");

    $.validator.setDefaults({ ignore: [] });
    $("#saveInfectionControl").removeData("validator");
    $("#saveInfectionControl").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#saveInfectionControl"));
    $("#saveInfectionControl").validate();
}


