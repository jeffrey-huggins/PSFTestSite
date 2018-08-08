

var selectedTab = 0;

function initMainForm() {
    $("#tabs").tabs({
        activate: function (event, ui) {
            selectedTab = ui.newTab.index();
            var dataTables = ui.newPanel.find(".dataTable").filter(function () {
                return this.id != "";
            });
            for (i = 0; i < dataTables.length; i++) {
                var dataTable = $("#" + dataTables[i].id).dataTable();
                dataTable.fnAdjustColumnSizing();
            }
        },
        active: selectedTab
    });

    $("#facilityBudget").change(function () {
        loadCensusDisplay();
        loadOtherRevenueDisplay();
    });
    $("#adminInfo").load(path + "Admin");
    loadLaborDisplay();
    loadCensusDisplay();
    loadOtherRevenueDisplay();
    loadOtherExpDisplay();
    loadOtherExpCalcDisplay();
}

$(function () {
    customSorts();
    initSelector("#selectedFacility", "#facility");
    $("#selectedFacility").change(function () {
        loadBudgetDisplay();
    });
    loadBudgetDisplay();
});

function loadLaborDisplay() {
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    $("#laborInfo").load(path + "Labor?facilityId=" + facility + "&budgetId=" + budget, function () {
        $("#payrollFilters").on("click", "input", function () {
            var table = $("#laborTable").dataTable();
            table.fnDraw();
        });

        $("#laborTable").dataTable({
            "aaSorting": [],
            "iDisplayLength": -1,
            "sDom": "rtS",
            "sScrollY": "400px",
            "aoColumns": [
                { "sSortDataType": "dom-selectGL", "sType": "numeric" },//account
                { "sSortDataType": "dom-textarea" },//desc
                { "sSortDataType": "dom-text", "sType": "numeric" },//hrs
                { "sSortDataType": "dom-text", "sType": "numeric" },//awr
                { "sSortDataType": "dom-text", "sType": "numeric" },//holiday
                { "sSortDataType": "dom-text", "sType": "numeric" },//pto
                { "sSortDataType": "dom-text", "sType": "numeric" },//wi
                { "sSortDataType": "dom-select" },//WIM
                { "sSortDataType": "dom-checkbox" },//bene
                { "sSortDataType": "dom-text", type: 'string' },//payer group

            ]
        });

        $("#laborTable").on("change", "input", function () {
            var row = $(this).closest("tr")[0];
            saveDataTableRow(row);
            saveBudgetRow(row);
        });

        $("#laborTable").on("change", "textarea", function () {
            var row = $(this).closest("tr")[0];
            saveDataTableRow(row);
            if ($(this).hasClass("glaccount")) {
                var desc = $(this).val();
                var glAccount = $(row).find(".laborGL").val();

                $.ajax({
                    url: path + "Labor/ChangeGLAccountDesc",
                    dataType: "json",
                    type: "POST",
                    data: {
                        glAccount: glAccount,
                        description: desc
                    },
                    success: function (result) {
                        if (result.Success) {
                            //loadCensusDisplay();
                        }
                        else {
                            alert(result.Message);
                        }
                    }
                });
            }
        });

        $(".addPayroll").on("click", function () {
            var row = $(this).closest("tr")[0];
            addBudgetRow(row);
        });

        $("#laborTable").on("change", "select", function () {
            var row = $(this).closest("tr")[0];

            if ($(this).hasClass("laborGL")) {
                var row = $(this).closest("tr")[0];
                var selectedOption = $(this).find("option:selected");
                var description = selectedOption.attr("desc");
                var payergroup = selectedOption.attr("payergroup");
                $(row).find(".paygroup").val(payergroup);
                $(row).find(".glaccount").val(description);
                saveDataTableRow(row);
                updateBudgetGLAccount(row);
            }
            else {
                saveDataTableRow(row);
                saveBudgetRow(row);
            }

        });

        $("#laborTable").on("click", "#deleteLaborRow", function () {
            var row = $(this).closest("tr")[0];
            deleteBudgetRow(row);
        });
    });
}

function addBudgetRow(row) {
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    var glAccount = $(row).find(".laborGL").val();
    var hoursPPD = $(row).find(".hours").val();
    var awrVal = $(row).find(".awr").val().replace("$", "");
    var awr = Number(awrVal);
    var holidayVal = $(row).find(".holiday").val().replace("%", "");
    var holiday = Number(holidayVal) / 100;
    var ptoVal = $(row).find(".pto").val().replace("%", "");
    var pto = Number(ptoVal) / 100;
    var wiCd = $(row).find(".wi").val();
    var wiMonth = $(row).find(".wim").val();
    var bene = $(row).find(".bene").prop("checked");
    $.ajax({
        url: path + "Labor/Create",
        dataType: "json",
        type: "POST",
        data: {
            FacilityID: facility,
            BudgetID: budget,
            GLAccountIndex: glAccount,
            HoursPPD: hoursPPD,
            AverageWageRate: awr,
            HolidayPercentage: holiday,
            PTOPercentage: pto,
            WageIncreaseCd: wiCd,
            WageIncreaseMonth: wiMonth,
            BenefitCalculationFlag: bene
        },
        success: function (result) {
            if (result.Success) {
                loadLaborDisplay();
            }
            else {
                alert(result.Message);
            }
        }
    });
}

function deleteBudgetRow(row) {
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    var glAccount = $(row).find(".laborGL").val();
    $.ajax({
        url: path + "Labor/Delete",
        dataType: "json",
        type: "POST",
        data: {
            facilityID: facility,
            budgetID: budget,
            glAccount: glAccount
        },
        success: function (result) {
            if (result.Success) {
                $(row).closest("table").dataTable().fnDeleteRow(row);
            }
            else {
                alert(result.Message);
            }
        }
    });
}

function updateBudgetGLAccount(row) {
    //UpdateGLAccount
    var oldGlAccount = $(row).attr("gl");
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    var glAccount = $(row).find(".laborGL").val();
    var hoursPPD = $(row).find(".hours").val();
    var awrVal = $(row).find(".awr").val().replace("$", "");
    var awr = Number(awrVal);
    var holidayVal = $(row).find(".holiday").val().replace("%", "");
    var holiday = Number(holidayVal) / 100;
    var ptoVal = $(row).find(".pto").val().replace("%", "");
    var pto = Number(ptoVal) / 100;
    var wiCd = $(row).find(".wi").val();
    var wiMonth = $(row).find(".wim").val();
    var bene = $(row).find(".bene").val();

    $.ajax({
        url: path + "Labor/UpdateGLAccount",
        dataType: "json",
        type: "POST",
        data: {
            FacilityID: facility,
            BudgetID: budget,
            GLAccountIndex: glAccount,
            HoursPPD: hoursPPD,
            AverageWageRate: awr,
            HolidayPercentage: holiday,
            PTOPercentage: pto,
            WageIncreaseCd: wiCd,
            WageIncreaseMonth: wiMonth,
            BenefitCalculationFlag: bene,
            oldGlID: oldGlAccount
        },
        success: function (result) {
            if (result.Success) {
                //loadCensusDisplay();
            }
            else {
                alert(result.Message);
            }
        }
    });
}

function saveBudgetRow(row) {
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    var glAccount = $(row).find(".laborGL").val();
    var hoursPPD = $(row).find(".hours").val();
    var awrVal = $(row).find(".awr").val().replace("$", "");
    var awr = Number(awrVal);
    var holidayVal = $(row).find(".holiday").val().replace("%", "");
    var holiday = Number(holidayVal) / 100;
    var ptoVal = $(row).find(".pto").val().replace("%", "");
    var pto = Number(ptoVal) / 100;
    var wiCd = $(row).find(".wi").val();
    var wiMonth = $(row).find(".wim").val();
    var bene = $(row).find(".bene").val();


    $.ajax({
        url: path + "Labor/Edit",
        dataType: "json",
        type: "POST",
        data: {
            FacilityID: facility,
            BudgetID: budget,
            GLAccountIndex: glAccount,
            HoursPPD: hoursPPD,
            AverageWageRate: awr,
            HolidayPercentage: holiday,
            PTOPercentage: pto,
            WageIncreaseCd: wiCd,
            WageIncreaseMonth: wiMonth,
            BenefitCalculationFlag: bene
        },
        success: function (result) {
            if (result.Success) {
                //loadCensusDisplay();
            }
            else {
                alert(result.Message);
            }
        }
    });
}

//Datatable will not sort or filter properly without updating the DOM
function saveDataTableRow(row) {
    $(row).find("input").each(function (index, element) {
        $(element).attr("value", $(element).val());
    });
    $(row).find("select").each(function (index, element) {
        var value = $(element).val();
        $(element).val(value);
        $(element).find("option:selected").prop("selected", true);
    });
    $(row).find("textarea").each(function (index, element) {
        $(element).text($(element).val());
    });
    $(row).find("input[type='checkbox']").each(function (index, element) {
        if ($(element).prop("checked")) {
            $(element).attr("checked", "checked");
        }
        else {
            $(element).removeAttr("checked");
        }
    });
}

function loadBudgetDisplay() {
    var facility = $("#selectedFacility").val();
    $("#BudgetDisplay").load(path + "Intact/MainForm?id=" + facility, initMainForm);
    
}

function loadOtherExpCalcDisplay() {
    var facility = $("#selectedFacility").val();
    $("#otherCalcInfo").load(path + "Expenses/CalcValues?facilityId=" + facility, function () {
        $("#facilityCalcs").dataTable({
            "bFilter": false,
            "aaSorting": [],
            "iDisplayLength": -1,
            "sDom": "frtS",
            "sScrollY": "400px",
            "aoColumns": [
                { "sSortDataType": "dom-select", "sType": "numeric" },//month
                { "sSortDataType": "dom-select", "sType": "numeric" },//PPD
                { "sSortDataType": "dom-text", "sType": "numeric" },//Fixed Amt
            ]
        });
        $("#facilityCalcs").on("change", "select", function () {
            //UpdateCalcRowKey(OtherExpensesFacilityGLCalcValue calcValue, int oldCalcValueID, int oldMonthId)
            var facility = $("#selectedFacility").val();
            var row = $(this).closest("tr");
            var oldMonth = row.attr("oldMonthId");
            var oldCalcId = row.attr("oldCalcNameId");
            var month = row.find(".calcMonth").val();
            var calcId = row.find(".calcName").val();
            var calcValue = row.find(".calcValue").val();
            $.ajax({
                url: path + "Expenses/UpdateCalcRowKey",
                dataType: "json",
                type: "POST",
                data: {
                    FacilityID: facility,
                    CalcValueID: calcId,
                    MonthID: month,
                    CalcValue: calcValue,
                    oldCalcValueID: oldCalcId,
                    oldMonthId: oldMonth
                },
                success: function (result) {
                    if (result.Success) {
                        loadOtherRevenue();
                    }
                    else {
                        alert(result.Message);
                        row.find(".calcName").val(oldCalcId);
                        row.find(".calcMonth").val(oldMonth);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });
        });

        $("#facilityCalcs").on("change", "input", function () {
            var facility = $("#selectedFacility").val();
            var row = $(this).closest("tr");
            var month = row.find(".calcMonth").val();
            var calcId = row.find(".calcName").val();
            var calcValue = row.find(".calcValue").val();
            $.ajax({
                url: path + "Expenses/UpdateCalcRow",
                dataType: "json",
                type: "POST",
                data: {
                    FacilityID: facility,
                    CalcValueID: calcId,
                    MonthID: month,
                    CalcValue: calcValue
                },
                success: function (result) {
                    if (result.Success) {
                        loadOtherRevenue();
                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });
        });

        $("#facilityCalcs").on("click", "#deleteCalcRow", function () {
            var facility = $("#selectedFacility").val();
            var row = $(this).closest("tr");
            var oldMonth = row.attr("oldMonthId");
            var oldCalcId = row.attr("oldCalcNameId");
            //RemoveCalcRow
            $.ajax({
                url: path + "Expenses/RemoveCalcRow",
                dataType: "json",
                type: "POST",
                data: {
                    facilityId: facility,
                    calcValueId: oldCalcId,
                    monthId: oldMonth
                },
                success: function (result) {
                    if (result.Success) {
                        $(row).closest("table").dataTable().fnDeleteRow(row);
                        loadOtherRevenue();
                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });
        });

        $("#newCalc").on("click", function () {
            var facility = $("#selectedFacility").val();
            var calcName = $("#newCalcName").val();
            var month = $("#newCalcMonth").val();
            var value = $("#newCalcValue").val();
            $.ajax({
                url: path + "Expenses/CreateCalcRow",
                dataType: "json",
                type: "POST",
                data: {
                    FacilityID: facility,
                    CalcValueID: calcName,
                    MonthID: month,
                    CalcValue: value
                },
                success: function (result) {
                    if (result.Success) {
                        loadOtherExpCalcDisplay();
                        loadOtherRevenue();
                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });
        });
    });
}

function loadOtherExpDisplay(GLAccount) {
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    $("#otherInfo").load(path + "Expenses?facilityId=" + facility + "&budgetId=" + budget, function () {

        $("#otherExpenseID").change(function () {
            loadOtherExp();

        });

        if (GLAccount) {
            $("#otherExpenseID").val(GLAccount);
        }
        initSelector("#otherExpenseID", "#otherExpense");
        loadOtherExp();
    });
}

function loadOtherExp() {
    var GlAccount = $("#otherExpenseID").val();
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    $("#editOtherExpense").load(path + "Expenses/Display?facilityId=" + facility + "&GLAccountIndex=" + GlAccount + "&budgetId=" + budget, function () {
        $("#expenseDetail").dataTable({
            "bFilter": false,
            "aaSorting": [],
            "iDisplayLength": -1,
            "sDom": "frtS",
            "aoColumns": [
                { "sSortDataType": "dom-select", "sType": "numeric" },//month
                { "sSortDataType": "dom-text", "sType": "numeric" },//PPD
                { "sSortDataType": "dom-text", "sType": "numeric" },//Fixed Amt
            ]
        });

        $("#propagateButton").on("click", function () {
            var GlAccount = $("#otherExpenseID").val();
            var budget = $("#facilityBudget").val();
            var facility = $("#selectedFacility").val();

            var recordTypeId = $("input[name='recordType']:checked").val();
            if (recordTypeId == 3) {
                alert("This action cannot be performed on calculations");
                return;
            }
            var propagateValue = $("#propagateValue").val().replace("$", "");
            if (propagateValue) {
                if (isNaN(Number(propagateValue))) {
                    alert("Value to propagate should be empty or a number");
                    return;
                }
            }
            $.ajax({
                url: path + "Expenses/PropogateAmmount",
                dataType: "json",
                type: "POST",
                data: {
                    facilityId: facility,
                    budgetId: budget,
                    GLAccountIndex: GlAccount,
                    recordTypeId: recordTypeId,
                    ammount: propagateValue
                },
                success: function (result) {
                    if (result.Success) {
                        loadOtherExpDisplay(GlAccount);
                    }
                    else {
                        alert(result.data);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });


        });

        $("input[name='recordType']").on("change", function () {
            var recordTypeId = $(this).val();
            if (recordTypeId == 3) {
                $(".propagate").attr("disabled", "disabled");
            } else {
                $(".propagate").removeAttr("disabled");
            }
            var GlAccount = $("#otherExpenseID").val();
            var budget = $("#facilityBudget").val();
            var facility = $("#selectedFacility").val();
            //UpdateRecordType(int facilityId, int GLAccountIndex, int budgetId, short recordTypeId)
            $.ajax({
                url: path + "Expenses/UpdateRecordType",
                dataType: "json",
                type: "POST",
                data: {
                    facilityId: facility,
                    budgetId: budget,
                    GLAccountIndex: GlAccount,
                    recordTypeId: recordTypeId,
                },
                success: function (result) {
                    if (result.Success) {
                        
                    }
                    else {
                        alert(result.data);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });



        });

        $("#newExpense").on("click", function () {
            var GlAccount = $("#otherExpenseID").val();
            var budget = $("#facilityBudget").val();
            var facility = $("#selectedFacility").val();
            var month = $("#newMonth").val();
            var ppd = $("#newExpensePPD").val();
            var fixed = $("#newExpenseAmmount").val();

            $.ajax({
                url: path + "Expenses/CreateRow",
                dataType: "json",
                type: "POST",
                data: {
                    FacilityID: facility,
                    BudgetID: budget,
                    GLAccountIndex: GlAccount,
                    MonthID: month,
                    ExpenseAmount: fixed,
                    DollarsPPD: ppd
                },
                success: function (result) {
                    if (result.Success) {
                        loadOtherExpDisplay(GlAccount);
                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });

        });

        $("#expenseDetail").on("change", "select", function () {
            var GlAccount = $("#otherExpenseID").val();
            var budget = $("#facilityBudget").val();
            var facility = $("#selectedFacility").val();
            var row = $(this).closest("tr");
            var month = row.find(".expenseMonth").val();
            var oldMonth = row.attr("monthId");
            var ppd = row.find(".expensePPD").val().replace("$", "");
            var expense = row.find(".expenseAmount").val().replace("$", "");

            $.ajax({
                url: path + "Expenses/UpdateRowMonth",
                dataType: "json",
                type: "POST",
                data: {
                    FacilityID: facility,
                    BudgetID: budget,
                    GLAccountIndex: GlAccount,
                    MonthID: month,
                    ExpenseAmount: expense,
                    DollarsPPD: ppd,
                    oldMonth: oldMonth
                },
                success: function (result) {
                    if (result.Success) {

                    }
                    else {
                        alert(result.Message);
                        row.find(".expenseMonth").val(oldMonth);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });
        });

        $("#expenseDetail").on("change", "input", function () {
            var GlAccount = $("#otherExpenseID").val();
            var budget = $("#facilityBudget").val();
            var facility = $("#selectedFacility").val();
            var row = $(this).closest("tr");
            var month = row.find(".expenseMonth").val();
            var ppd = row.find(".expensePPD").val().replace("$","");
            var expense = row.find(".expenseAmount").val().replace("$","");

            $.ajax({
                url: path + "Expenses/UpdateRow",
                dataType: "json",
                type: "POST",
                data: {
                    FacilityID: facility,
                    BudgetID: budget,
                    GLAccountIndex: GlAccount,
                    MonthID: month,
                    ExpenseAmount: expense,
                    DollarsPPD: ppd
                },
                success: function (result) {
                    if (result.Success) {

                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });

        });

        $("#expenseGL").on("change", function () {
            var oldGlAccount = $("#otherExpenseID").val();
            var budget = $("#facilityBudget").val();
            var facility = $("#selectedFacility").val();
            var newGlAccount = $("#expenseGL").val();
            $.ajax({
                url: path + "Expenses/ChangeGLAccount",
                dataType: "json",
                type: "POST",
                data: {
                    facilityId: facility,
                    budgetId: budget,
                    oldGLAccountIndex: oldGlAccount,
                    newGLAccountIndex: newGlAccount,
                },
                success: function (result) {
                    if (result.Success) {

                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });
        });


    });

}

function loadCensusDisplay(GLAccount) {
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    $("#censusInfo").load(path + "Census?facilityId=" + facility + "&budgetId=" + budget, function () {
        $("#glAccountID").change(function () {
            loadCensus();

        });
        if (GLAccount) {
            $("#glAccountID").val(GLAccount);
        }

        initSelector("#glAccountID", "#censusLedger");
        $("#newCensusLedger").on("click", function () {
            $("#censusNewSelectorModal").dialog({
                dialogClass: "no-close",
                buttons: [
                    {
                        text: "Create",
                        click: function () {
                            createCensus(this);

                        }
                    },
                    {
                        text: "Cancel",
                        click: function () {
                            $(this).dialog("close");
                        }
                    }
                ]
            });
        });
        $("#deleteCensusLedger").on("click", function () {
            var facility = $("#selectedFacility").val();
            var GlAccount = $("#glAccountID").val();
            var budget = $("#facilityBudget").val();
            var otherGL = $("#otherRevenueID").val();
            $.ajax({
                url: path + "Census/Delete",
                dataType: "json",
                type: "POST",
                data: {
                    facilityId: facility,
                    budgetId: budget,
                    GLAccountIndex: GlAccount
                },
                success: function (result) {
                    if (result.Success) {
                        loadCensusDisplay();
                    }
                    else {
                        alert(result.Message);
                    }
                }
            });
        });
        loadCensus();
    });
}

function loadCensus() {
    var GlAccount = $("#glAccountID").val();
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    $("#editCensus").load(path + "Census/Display?facilityId=" + facility + "&GLAccountIndex=" + GlAccount + "&budgetId=" + budget, function () {
        $("#censusDetail").dataTable({
            "bFilter": false,
            "aaSorting": [],
            "iDisplayLength": -1,
            "sDom": "frtS",
            "aoColumns": [
                { "sSortDataType": "dom-select", "sType": "numeric" },//month
                { "sSortDataType": "dom-text", "sType": "numeric" },//ADC
                { "sSortDataType": "dom-text", "sType": "numeric" },//ADR
                { "sSortDataType": "dom-text", "sType": "numeric" },//Fixed Amt
                { "sSortDataType": "dom-text", "sType": "numeric" },//Monthly ADC
            ]
        });


        $("#censusGLAccountID").on("change", updateCensusLedger);

        $("#newglAccountID").html($("#censusGLAccountID").html());

        $("#censusDetail").on("change", "input", function () {
            var facility = $("#selectedFacility").val();
            var oldGlAccount = $("#censusGLAccountID").val();
            var budget = $("#facilityBudget").val();
            var row = $(this).closest("tr");
            var dailyCensus = row.find("#census_AvgDailyCensus").val();
            var dailyRate = row.find("#census_AvgDailyRate").val().replace("$", "");
            var monthId = row.find("select").val();

            if (dailyCensus && isNaN(Number(dailyCensus))) {
                alert("ADC should be a number.  Please double check the input and try again.");
                return;
            }

            if (dailyRate && isNaN(Number(dailyRate))) {
                alert("ADR should be a number.  Please double check the input and try again.");
                return;
            }

            $.ajax({
                url: path + "Census/Edit",
                dataType: "json",
                type: "POST",
                data: {
                    facilityId: facility,
                    budgetId: budget,
                    oldGLAccountIndex: oldGlAccount,
                    dailyCensus: dailyCensus,
                    dailyRate: dailyRate,
                    monthId: monthId
                },
                success: function (result) {
                    if (result.Success) {
                        row.find("#fixedAmt").val(result.fixedAmmount);
                        row.find("#monthlyADC").val(result.monthlyADC);
                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });
        });
    });

}

function loadOtherRevenueDisplay(GLAccount) {
    var budget = $("#facilityBudget").val();
    var facility = $("#selectedFacility").val();
    $("#otherRevenueInfo").load(path + "OtherRevenue?facilityId=" + facility + "&budgetId=" + budget, function () {

        $("#otherRevenueID").change(function () {
            loadOtherRevenue();
        });

        if (GLAccount) {
            $("#otherRevenueID").val(GLAccount);
        }

        initSelector("#otherRevenueID", "#otherRevenue");
        $("#newOtherRevenue").on("click", function () {
            $("#otherRevNewSelectorModal").dialog({
                dialogClass: "no-close",
                buttons: [
                    {
                        text: "Create",
                        click: function () {
                            createOtherRevenue(this);

                        }
                    },
                    {
                        text: "Cancel",
                        click: function () {
                            $(this).dialog("close");
                        }
                    }
                ]
            });
        });
        $("#deleteOtherRevenue").on("click", function () {
            var facility = $("#selectedFacility").val();
            var GlAccount = $("#otherRevenueID").val();
            var budget = $("#facilityBudget").val();
            var censusGlAccount = $("#glAccountID").val();
            $.ajax({
                url: path + "OtherRevenue/Delete",
                dataType: "json",
                type: "POST",
                data: {
                    facilityId: facility,
                    budgetId: budget,
                    GLAccountIndex: GlAccount
                },
                success: function (result) {
                    if (result.Success) {
                        loadOtherRevenueDisplay();
                    }
                    else {
                        alert(result.Message);
                    }
                }
            });
        });

        loadOtherRevenue();
    });
}

function loadOtherRevenue() {
    var facility = $("#selectedFacility").val();
    var GlAccount = $("#otherRevenueID").val();
    var budget = $("#facilityBudget").val();
    $("#editOtherRevenue").load(path + "OtherRevenue/Display?facilityId=" + facility + "&GLAccountIndex=" + GlAccount + "&budgetId=" + budget, function () {
        $("#otherRevenue").dataTable({
            "bFilter": false,
            "aaSorting": [],
            "iDisplayLength": -1,
            "sDom": "frtS",
            "aoColumns": [
                { "sSortDataType": "dom-select", "sType": "numeric" },//month
                { "sSortDataType": "dom-text", "sType": "numeric" },//Fixed Amt
                { "sSortDataType": "dom-text", "sType": "numeric" }//PPD
            ]
        });

        $("#otherReveGLAccountID").on("change", updateOtherRevenueLedger);
        $("#newOtherRevglAccountID").html($("#otherReveGLAccountID").html());
        $("#otherRevenue").on("change", "input", function () {
            var row = $(this).closest("tr");
            var facility = $("#selectedFacility").val();
            var oldGlAccount = $("#otherRevenueID").val();
            var budget = $("#facilityBudget").val();
            var monthId = row.find("select").val();
            var fixedAmt = row.find("#revenue_RevenueAmt").val().replace("$", "");

            if (fixedAmt && isNaN(Number(fixedAmt))) {
                alert("Fixed Amt should be a number.  Please double check the input and try again.");
                return;
            }
            $.ajax({
                url: path + "OtherRevenue/Edit",
                dataType: "json",
                type: "POST",
                data: {
                    facilityId: facility,
                    budgetId: budget,
                    oldGLAccountIndex: oldGlAccount,
                    fixedAmount: fixedAmt,
                    monthId: monthId
                },
                success: function (result) {
                    if (result.Success) {

                        row.find("#PPD").val(result.PPD);
                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function () {
                    alert("Error communicating with the server, please try again.");
                }
            });
        });

    });
}

function updateOtherRevenueLedger() {
    var facility = $("#selectedFacility").val();
    var oldGlAccount = $("#otherRevenueID").val();
    var GlAccount = $("#glAccountID").val();
    var newGlAccount = $("#otherReveGLAccountID").val();
    var budget = $("#facilityBudget").val();
    $.ajax({
        url: path + "OtherRevenue/UpdateGLAccount",
        dataType: "json",
        type: "POST",
        data: {
            facilityId: facility,
            budgetId: budget,
            oldGLAccountIndex: oldGlAccount,
            newGLAccountIndex: newGlAccount
        },
        success: function (result) {
            if (result.Success) {
                loadOtherRevenueDisplay(newGlAccount);
            }
            else {
                alert(result.Message);
            }
        }
    });

}

function createOtherRevenue(button) {
    var facility = $("#selectedFacility").val();
    var otherGlAccount = $("#newOtherRevglAccountID").val();
    var budget = $("#facilityBudget").val();
    $.ajax({
        url: path + "OtherRevenue/Create",
        dataType: "json",
        type: "POST",
        data: {
            facilityId: facility,
            budgetId: budget,
            GLAccountIndex: otherGlAccount
        },
        success: function (result) {
            if (result.Success) {
                $(button).dialog("close");
                loadOtherRevenueDisplay(otherGlAccount);
            }
            else {
                alert(result.Message);
            }
        }
    });
}

function updateCensusLedger() {
    var facility = $("#selectedFacility").val();
    var oldGlAccount = $("#glAccountID").val();
    var newGlAccount = $("#censusGLAccountID").val();
    var budget = $("#facilityBudget").val();
    $.ajax({
        url: path + "Census/UpdateGLAccount",
        dataType: "json",
        type: "POST",
        data: {
            facilityId: facility,
            budgetId: budget,
            oldGLAccountIndex: oldGlAccount,
            newGLAccountIndex: newGlAccount
        },
        success: function (result) {
            if (result.Success) {
                loadCensusDisplay(newGlAccount);
            }
            else {
                alert(result.Message);
            }
        }
    });

}

function createCensus(button) {
    var facility = $("#selectedFacility").val();
    var GlAccount = $("#newglAccountID").val();
    var budget = $("#facilityBudget").val();
    $.ajax({
        url: path + "Census/Create",
        dataType: "json",
        type: "POST",
        data: {
            facilityId: facility,
            budgetId: budget,
            GLAccountIndex: GlAccount
        },
        success: function (result) {
            if (result.Success) {
                $(button).dialog("close");
                loadCensusDisplay(GlAccount);
            }
            else {
                alert(result.Message);
            }
        }
    });
}

function customSorts() {
    $.fn.dataTableExt.oApi.fnDataUpdate = function (oSettings, nRowObject, iRowIndex) {
        $(nRowObject).find("TD").each(function (i) {
            var iColIndex = oSettings.oApi._fnVisibleToColumnIndex(oSettings, i);
            oSettings.oApi._fnSetCellData(oSettings, iRowIndex, iColIndex, jQuery(this).html());
        });
    };

    $.fn.dataTableExt.afnFiltering.push(function (oSettings, aData, iDataIndex) {
        if (oSettings.nTable.id != "laborTable")
            return true;

        var payerGroup = $(aData[9]).find("input").val();
        //after a sort, payerGroup will be equal to the value of the input instead of the entire td element
        if (!payerGroup)
            payerGroup = aData[9];

        return $("#" + payerGroup).prop("checked");
    });

    /* Create an array with the values of all the input boxes in a column */
    $.fn.dataTableExt.afnSortData['dom-text'] = function (oSettings, iColumn) {
        return jQuery.map(oSettings.oApi._fnGetTrNodes(oSettings), function (tr) {
            var td = jQuery('td:eq(' + iColumn + ')', tr);
            var value = td.find("input").val().replace("$", "").replace("%", "");
            return value;
        });
    }

    $.fn.dataTableExt.afnSortData['dom-textarea'] = function (oSettings, iColumn) {
        return jQuery.map(oSettings.oApi._fnGetTrNodes(oSettings), function (tr) {
            var td = jQuery('td:eq(' + iColumn + ')', tr);
            var value = td.find("textarea").text();
            return value;
        });
    }

    /* Create an array with the values of all the select options in a column */
    $.fn.dataTableExt.afnSortData['dom-selectGL'] = function (oSettings, iColumn) {
        return jQuery.map(oSettings.oApi._fnGetTrNodes(oSettings), function (tr) {
            var td = jQuery('td:eq(' + iColumn + ')', tr);
            var value = td.find("option:selected").attr("number");
            return value;
        });
    }

    /* Create an array with the values of all the select options in a column */
    $.fn.dataTableExt.afnSortData['dom-select'] = function (oSettings, iColumn) {
        return jQuery.map(oSettings.oApi._fnGetTrNodes(oSettings), function (tr) {
            var td = jQuery('td:eq(' + iColumn + ')', tr);
            var value = td.find("select").val();
            return value;
        });
    }

    /* Create an array with the values of all the checkboxes in a column */
    $.fn.dataTableExt.afnSortData['dom-checkbox'] = function (oSettings, iColumn) {
        return jQuery.map(oSettings.oApi._fnGetTrNodes(oSettings), function (tr) {
            var td = jQuery('td:eq(' + iColumn + ')', tr);
            var value = td.find("input[type='checkbox']").attr("checked");
            if (value == "checked")
                value = "1";
            else
                value = "0";
            return value;
        });

    }

}

