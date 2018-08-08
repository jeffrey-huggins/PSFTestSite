
function initSidebar() {
    $("#listing").html("");
    $("#title").text("Employee Infection Control");
    $(".instruction").show();
    $("#editForm").html("");
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
        var url = path + "EmployeeInfectionControl/EmployeeInfo?employeeId=" + resident;
        $("#selectedEmployeeView").load(url, function () {
            //"Standards Of Care for " + SOCCurrentResidentName;
            $("#title").text("Employee Vaccinations for " + $("#residentName").text());
        });
        $("#editForm").load(path + "EmployeeInfectionControl/EditOrCreateInfection?employeeId=" + resident, loadEditForm);
        $("#listing").load(path + "EmployeeInfectionControl/GetInfectionList?employeeId=" + resident + "&fromString=" + encodeURIComponent(from) + "&toString=" + encodeURIComponent(to), function () {
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
$("#sideBar").on("change", "#ShowTerminatedEmplyees", function () {
    if (!$(this).is(":checked")) {
        ShowProgress();
        $("#SideDDL").ajaxSubmit({
            url: path + "EmployeeInfectionControl/UpdateEmployeeSideBar",
            type: "POST",
            success: function (result) {
                $("#sideBar").html(result);
                initSidebar();
            }

        });

    }

});
$("#sideBar").on("click", "#lookbackUpdate", function (e) {
    e.preventDefault();
    $("#Residents").val("");
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

function loadEditForm() {
    var resident = $("#Residents option:selected").val();
    var ifcId = $("#EmployeeIFCEventId").val();
    if (ifcId == "0") {
        $("#OnsetDate").val("");
        $("#MissedWorkDaysCnt").val("");
    }

    $(".isDate").datepicker();

    $("#saveInfectionForm").on("click", function (e) {
        e.preventDefault();
        var form = $("#SaveInfection");
        if (!form.valid()) {
            return;

        }
        form.ajaxSubmit({
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
    $("#clearInfectionForm").on("click", function (e) {
        e.preventDefault();
        $("#editForm").load(path + "EmployeeInfectionControl/EditOrCreateInfection?employeeId=" + resident, loadEditForm);
    });
    $.validator.setDefaults({ ignore: [] });
    $("#SaveInfection").removeData("validator");
    $("#SaveInfection").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#SaveInfection"));
    $("#SaveInfection").validate();
}