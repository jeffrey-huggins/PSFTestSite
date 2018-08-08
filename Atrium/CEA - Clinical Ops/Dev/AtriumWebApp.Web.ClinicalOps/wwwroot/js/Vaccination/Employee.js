
function initSidebar() {
    $("#listing").html("");
    $("#title").text("Employee Vaccinations");
    $(".instruction").show();
    $(".vaccineSelector").hide();
    $("#Residents").val("");
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
        var url = path + "Vaccination/EmployeeInfo?employeeId=" + resident;
        $("#selectedEmployeeView").load(url, function () {
            //"Standards Of Care for " + SOCCurrentResidentName;
            $("#title").text("Employee Vaccinations for " + $("#residentName").text());
        });
        $("#listing").load(path + "Vaccination/GetVaccineList?patientId=" + resident + "&fromString=" + encodeURIComponent(from) + "&toString=" + encodeURIComponent(to) + "&isEmployee=true", function () {
            $("#vaccineTable").dataTable({
                "bFilter": false,
                "bAutoWidth": false,
                "aaSorting": [[1, "desc"]],
                "sDom": "frtS",
                "iDisplayLength": -1,
                "aoColumnDefs": [
                    { "bVisible": false, "aTargets": [3] }
                ]
            });


            $("#vaccineTable").on("click", ".edit", function (e) {
                e.preventDefault();
                var id = $(this).closest("tr").attr("vaccineId");
                var url = path + "Vaccination/EditVaccination?vaccineId=" + id + "&isEmployee=true";
                $("#modal").load(url, formLoaded);
            });
            $("#vaccineTable").on("click", ".delete", function (e) {
                e.preventDefault();
                var id = $(this).closest("tr").attr("vaccineId");
                var url = path + "Vaccination/DeleteVaccine?vaccineId=" + id + "&isEmployee=true";
                $.ajax({
                    url: url,
                    success: function (result) {
                        if (result.success) {
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
        $(".vaccineSelector").show();
        updateTable();
    }
    else {
        $(".instruction").show();
        $(".vaccineSelector").hide();
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
            url: path + "Vaccination/UpdateEmployeeSideBar",
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
        url: path + "Vaccination/UpdateEmployeeSideBar",
        type: "POST",
        success: function (result) {
            $("#sideBar").html(result);
            initSidebar();
        }

    });
});
$(".isDate").datepicker();
$("#updateTable").on("click", updateTable);

$("#sideBar").on("change", "#ShowTerminatedEmplyees", function () {
    if (!$(this).is(":checked")) {
        ShowProgress();
        $("#SideDDL").ajaxSubmit({
            url: path + "Vaccination/UpdateEmployeeSideBar",
            type: "POST",
            success: function (result) {
                $("#sideBar").html(result);
                initSidebar();
            }

        });

    }

});

$("#newVaccine").on("click", function (e) {
    e.preventDefault();
    var residentId = $("#Residents option:selected").val();
    var vaccineId = $("#vaccineType").val();
    if (residentId == "" || residentId == "0" || vaccineId == "") {
        alert("Select a vaccine type to continue.");
        return;
    }
    $("#modal").load(path + "Vaccination/EditVaccination?patientId=" + residentId + "&isEmployee=true&vaccineTypeId=" + vaccineId, function () {
        $("#modal").dialog({
            width: "auto",
            height: "auto",
            resizable: false,
            modal: true,
            open: function () {
                $(this).parent().focus();
            },
            close: function () {
                $(this).dialog('destroy');
            }
        });
    });


});


function formLoaded() {
    $("#modal").dialog({
        width: "auto",
        height: "auto",
        resizable: false,
        modal: true,
        open: function () {
            $(this).parent().focus();
        },
        close: function () {
            $(this).dialog('destroy');
        }
    });
}