$("#adminFacilities").click(function () {
    $("#facilityDialog").load(path + "Admin/Facilities", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#selectedfacilityAdmin").change(function () {
            $(".facilityInfo").hide();

            var value = $(this).val();
            $("#facilityDialog #" + value).show();
        });
        $(".facilityInfo").on("change", "input, select", function () {
            var form = $(this).closest("form");
            updateFacilityFields(form);
            var url = form.attr("action");
            if (form.valid()) {
                $.post(url, form.serialize(), function (data) {
                    if (data.Success) {

                    } else {
                        alert(data.Message);
                    }
                }).fail(function () {
                    alert("Server is not responding, please try again.");
                });
            }
        });
        initSelector("#selectedfacilityAdmin", "#facilityAdmin");
        var value = $("#selectedfacilityAdmin").val();
        $("#facilityDialog #" + value).show();
        $("#newfacilityAdmin").on("click", function () {
            $("#facilityAdminEditor").hide();
            $("#facilityAdminCreator").show();
            $("#facilityDialog").dialog({
                width: 1000,
                height: 500
            });
        });

        $("#cancelNewFacility").on("click", function () {
            $("#facilityEditor").show();
            $("#facilityCreator").hide();
            $("#facilityDialog").dialog({
                width: 850,
                height: 340
            });
        });

        $("#facilityDialog").dialog({
            width: 850,
            height: 340
        });

        $("#deletefacilityAdmin").on("click", function () {
            var strconfirm = confirm("Are you sure you want to delete?");
            if (strconfirm == false) {
                return;
            }
            var id = $("#selectedfacilityAdmin").val();
            $.post(path + "Admin/DeleteFacility?id=" + id, function (data) {
                if (data.Success) {
                    $("#facilityDialog").dialog("close");
                    $("#adminFacilities").click();
                } else {
                    alert(data.Message);
                }
            }).fail(function () {
                alert("Server is not responding, please try again.");
            });
        });

        $("#facilityTable tbody tr").on("click", function () {
            clearSelection();
            if (!$(this).hasClass("negative")) {
                $(".negative").removeClass("negative");
            }
            $(this).toggleClass("negative");
        });

        $("#saveNewFacility").on("click", function () {

            if ($(".negative").length == 0) {
                alert("Select a source facility to copy data from");
                return;
            }
            var copyFrom = $(".negative").attr("rownum");
            $("#newCopyFrom").val(copyFrom);

            var form = $("#newFacilityForm");
            updateFacilityFields(form);
            var url = form.attr("action");
            if (form.valid()) {
                $.post(url, form.serialize(), function (data) {
                    if (data.Success) {
                        $("#facilityDialog").dialog("close");
                        $("#adminFacilities").click();
                    } else {
                        alert(data.Message);
                    }
                }).fail(function () {
                    alert("Server is not responding, please try again.");
                });
            }
        });
    });
});

function updateFacilityFields(form) {
    var wageIncrease = form.find("#wageIncrease");

    var wageValue = null;

    if (wageIncrease.val()) {
        wageValue = Number(wageIncrease.val().replace("%", "")) / 100;
    }

    wageIncrease.siblings("#WageIncreasePercentage").attr("value", wageValue);
    var mgtFee = form.find("#mgtFee");
    var value = null;
    if (mgtFee.val()) {
        value = Number(mgtFee.val().replace("%", "")) / 100;;
    }
    mgtFee.siblings("#MgtFeeCalcPercent").attr("value", value);
}

$("#adminGL").click(function () {
    $("#glDialog").load(path + "Admin/GeneralLedger", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#glDialog").dialog({
            width: 645,
            height: 380
        });
        $("#selectedgenLedger").change(function () {
            var value = $(this).val();
            $("#glEditSection").load(path + "Admin/EditGl?GLId=" + value, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });
        $("#newgenLedger").click(function () {
            $("#glEditSection").load(path + "Admin/EditGl", function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });
        $("#deletegenLedger").click(function () {
            var id = $("#selectedgenLedger").val();
            $.post(path + "Admin/DeleteGl?id=" + id).done(function (response) {
                if (response.Success) {
                    $("#adminGL").click();
                }
            }).fail(function () {
                alert("There was an error communicating with the server.  Please try again");
            });
        });
        initSelector("#selectedgenLedger", "#genLedger");
        $("#genLedgerSelector").blur();
    });
});

$("#adminPT").click(function () {
    $("#ptDialog").load(path + "Admin/PayerTypes", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#ptDialog").dialog({
            width: 645,
            height: 280
        });

        $("#selectedpayType").change(function () {
            var value = $(this).val();
            $("#ptEditSection").load(path + "Admin/EditPT?PTId=" + value, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });
        $("#newpayType").on("click", function () {
            $("#ptEditSection").load(path + "Admin/EditPT", function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });

        $("#deletepayType").on("click", function () {
            var value = $("#selectedpayType").val();
            $.post(path + "Admin/DeletePT?PTId=" + value, function (data, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
                if (data.Success) {
                    $("#ptDialog").dialog("close");
                    $("#adminPT").click();
                } else {
                    alert(data.Message);
                }
            });
        });
        initSelector("#selectedpayType", "#payType");
        $("#payTypeSelector").blur();

    });


});

$("#adminBT").click(function () {
    $("#btDialog").load(path + "AdminBudget/BudgetTypes", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#btDialog").dialog({
            width: 645,
            height: 160
        });

        $("#selectedbudgetType").change(function () {
            var value = $(this).val();
            $("#btEditSection").load(path + "AdminBudget/EditBT?BTId=" + value, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });
        $("#newbudgetType").on("click", function () {
            $("#btEditSection").load(path + "AdminBudget/EditBT", function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });

        $("#deletebudgetType").on("click", function () {
            var value = $("#selectedbudgetType").val();
            $.post(path + "AdminBudget/DeleteBT?BTId=" + value, function (data, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
                if (data.Success) {
                    $("#btDialog").dialog("close");
                    $("#adminBT").click();
                } else {
                    alert(data.Message);
                }
            });
        });
        initSelector("#selectedbudgetType", "#budgetType");
        $("#budgetTypeSelector").blur();

    });


});

$("#adminBudYr").click(function () {
    $("#byDialog").load(path + "AdminBudget/EditBY", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#byDialog").dialog({
            width: 645,
            height: 200
        });
        $("#saveBudgetYear").on("click", function () {
            var form = $("#editBY");
            if (!form.valid()) {
                return;
            }
            $.ajax({
                url: form.attr("action"),
                dataType: "json",
                type: "POST",
                data: form.serialize(),
                success: function (result) {
                    if (result.Success) {

                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }

            });
        });
        $("#cancelBudgetYear").on("click", function () {
            $("#byDialog").dialog("close");
        });
    });
});

$("#adminRegion").click(function () {
    $("#RegionDialog").load(path + "AdminLocation/Regions", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#RegionDialog").dialog({
            width: 645,
            height: 160
        });

        $("#selectedRegion").change(function () {
            var value = $(this).val();
            $("#RegionEditSection").load(path + "AdminLocation/EditRegion?RegionId=" + value, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });

        initSelector("#selectedRegion", "#Region");
        $("#RegionSelector").blur();

    });
});

$("#adminStates").click(function () {
    $("#stateDialog").load(path + "AdminLocation/States", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#stateDialog").dialog({
            width: 645,
            height: 160
        });

        $("#selectedstate").change(function () {
            var value = $(this).val();
            $("#stateEditSection").load(path + "AdminLocation/EditState?StateId=" + value, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });
        initSelector("#selectedstate", "#state");
        $("#stateSelector").blur();


    });

});

$("#adminExpCalc").click(function () {

    $("#calculationsDialog").load(path + "AdminCalcs/OtherCalcs", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#calculationsDialog").dialog({
            width: 645,
            height: 260
        });

        $("#selectedCalculations").change(function () {
            var value = $(this).val();
            $("#CalculationsEditSection").load(path + "AdminCalcs/EditOtherCalcs?id=" + value, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });

        $("#newCalculations").click(function () {
            $("#CalculationsEditSection").load(path + "AdminCalcs/EditOtherCalcs?id=0", function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });

        $("#deleteCalculations").click(function () {
            var id = $("#selectedCalculations").val();
            $.post(path + "AdminCalcs/DeleteOtherCalcs?id=" + id, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
                if (response.Success) {
                    $("#adminExpCalc").click();
                }

            });
        });

        initSelector("#selectedCalculations", "#Calculations");
        $("#CalculationsSelector").blur();


    });

});

$("#adminExpCalcValue").click(function () {
    $("#calcValuesDialog").load(path + "AdminCalcs/OtherCalcValues", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#calcValuesDialog").dialog({
            width: 645,
            height: 260
        });

        $("#selectedCalcValue").change(function () {
            var value = $(this).val();
            $("#CalcValueEditSection").load(path + "AdminCalcs/EditOtherCalcValues?id=" + value, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });

        $("#newCalcValueButton").on("click", function () {
            console.log("click");
            $("#CalcValueEditSection").load(path + "AdminCalcs/EditOtherCalcValues?id=0", function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });

        $("#deleteCalcValue").click(function () {
            var id = $("#selectedCalcValue").val();
            $.post(path + "AdminCalcs/DeleteOtherCalcValues?id=" + id, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
                if (response.Success) {
                    $("#adminExpCalcValue").click();
                }

            });
        });

        initSelector("#selectedCalcValue", "#CalcValue");
        $("#CalcValueSelector").blur();


    });

});

$("#adminExpCalcValueSetup").click(function () {
    $("#calcValueSetupDialog").load(path + "AdminCalcs/OtherCalcValueSetup", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#calcValueSetupDialog").dialog({
            width: 645,
            height: 560
        });

        $("#selectedOtherCalcValueSetup").change(function () {
            var value = $(this).val();
            $("#OtherCalcValueSetupEditSection").load(path + "AdminCalcs/EditOtherCalcValueSetup?id=" + value, function (response, status, xhr) {
                if (status == "error") {
                    alert("There was an error communicating with the server.  Please try again");
                    return;
                }
            });
        });
        initSelector("#selectedOtherCalcValueSetup", "#OtherCalcValueSetup");
        $("#OtherCalcValueSetupSelector").blur();


    });

});

$("#adminExcelExport").on("click", function () {
    $("#exportExcelDialog").load(path + "AdminExport/ExcelExport", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }

        $("#exportExcelDialog").dialog({
            width: 850,
            height: 560
        });
    });
    //getFacilityFiles("facilities=87&facilities=223");
    //getRegionFiles("regions=1&regions=2");
    //getCompanyFile();
});

function getCompanyFile() {
    $.post(path + "AdminExport/ExportCompanyToExcel", function (data, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server, please try again.");
            return;
        }
        if (data.Success) {
            $("#FileProcessingDialog").dialog({
                modal: true
            });
            ShowProgress();
            doPoll(data.Id);
        }
    });
}

function getRegionFiles(regions) {
    $.post(path + "AdminExport/ExportRegionsToExcel?" + regions, function (data, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server, please try again.");
            return;
        }
        if (data.Success) {
            $("#FileProcessingDialog").dialog({
                modal: true
            });
            ShowProgress();
            doPoll(data.Id);
        }
    });
}

function getFacilityFiles(facilities) {
    $.post(path + "AdminExport/ExportFacilitiesToExcel?" + facilities, function (data, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server, please try again.");
            return;
        }
        if (data.Success) {
            $("#progressbar").progressbar({ max: 100, value: 0 });
            $("#fileProcessStatus").text("Initiating");
            $("#FileProcessingDialog").dialog({
                modal: true
            });
            ShowProgress();
            doPoll(data.Id);
        }
    });
}

function doPoll(id) {

    $.post(path + "AdminExport/GetExportFileStatus?id=" + id, function (data) {
        if (data.Progress == null || data.Progress == -1) {
            alert("There was an error processing the file.");
            $("#FileProcessingDialog").dialog("close");
            return;
        }
        if (data.Progress < 0) {
            alert("No file is currently being generated.  Please try to submit another request.");
            $("#FileProcessingDialog").dialog("close");
            return;
        }
        else if (data.Progress < 100) {
            $("#fileProcessStatus").text(data.Status);
            $("#progressbar").progressbar({ value: data.Progress });
            setTimeout(function () {
                doPoll(id);
            }, 5000);
            return;
        }
        else if (data.Progress >= 100) {
            $("#fileProcessStatus").text(data.Status);
            $("#progressbar").progressbar({ value: 100 });
            $("#FileProcessingDialog").dialog("close");
            window.location = path + "AdminExport/GetExportFile?id=" + id;
        }
        else {
            alert("There was an error processing the file.");
            $("#FileProcessingDialog").dialog("close");
            return;
        }

    });
}

$("#adminRebuild").on("click", function () {
    $.post("AdminExport/RebuildImportFile", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        if (response.Success) {
            alert("Finished rebuilding.");
        }
        else {
            alert("Unable to rebuild database");
        }
    });
});


$("#adminExport").on("click", function () {
    $("#exportCSVDialog").load(path + "AdminExport/CSVExport", function (response, status, xhr) {
        if (status == "error") {
            alert("There was an error communicating with the server.  Please try again");
            return;
        }
        $("#exportCSVDialog").dialog({
            width: 845,
            height: 460
        });
        $("#facilityCSVExportTable tbody tr").on("click", function () {
            if (!$("#facilityCSVExportTable").hasClass("disabledOverlay")) {
                $(this).toggleClass("negative");
            }
        });
        $("#selectAllCSVExport").on("change", function () {
            if ($(this).is(":checked")) {
                $("#facilityCSVExportTable").addClass("disabledOverlay");

            } else {
                $("#facilityCSVExportTable").removeClass("disabledOverlay");
            }
        });

        $("#csvExport").on("click", function () {
            if ($("#selectAllCSVExport").is(":checked")) {
                window.location = path + "AdminExport/ExportFacilitiesToTempTable";
            }
            else {
                if ($("#facilityCSVExportTable").find(".negative").length == 0) {
                    alert("Please select a facility to export or all.")
                }

                var facilities = "";

                $("#facilityCSVExportTable").find(".negative").each(function (index, element) {
                    facilities += $(element).attr("rowNum") + ",";
                });
                window.location = path + "AdminExport/ExportFacilitiesToTempTable?facilityList=" + facilities;
            }
        });
    });
});