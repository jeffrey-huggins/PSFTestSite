$(document).ready(function () {
    $("#payroll-transfer-section").on("click", ".edit", function () {
        editPTI($(this).closest(".payroll-transfer-item"));
    });
    $("#payroll-transfer-section").on("click", ".delete", function () {
        deletePTI($(this).closest(".payroll-transfer-item"));
    });
    $("#payroll-transfer-section").on("change", "input, select", function () {
        $(this).closest(".payroll-transfer-item").addClass("isDirty");
    });
    $(".source-community").val($("#Communities").val());
    $(".destination-community").val($("#Communities").val());
    $("#employeeProps").last().after("<p><b>General Ledger:</b><br /><span id='DDLGL'></span></p>");
    $("#Communities").change(communityItemChanged);

    $("#EmployeesDropdown").on("change",employeeItemChanged);

    $("#payroll-transfer-section").on("change","fieldset", function (data) {
        if (data.originalEvent == null) {
            var entry = $(data.target).closest(".payroll-transfer-item");
        }
        else {
            var entry = $(data.originalEvent.target).closest(".payroll-transfer-item");
        }
        validateEntry(entry);
    });

    $("#payroll-transfer-section").on("blur", ".payroll-transfer-item .isDate", function (data) {
        var transferDate = $(data.target);
        if (transferDate.val().length == 6) {
            var month = transferDate.val().substr(0, 2);
            var day = transferDate.val().substr(2, 2);
            var year = "20" + transferDate.val().substr(4, 2);

            var parsedDate = month + "/" + day + "/" + year; //new Date(month + "/" + day + "/" + year);

            transferDate.val(parsedDate);

        }
    });

    $("#savePayrollTransfer").click(function (e) {
            saveListAndContinue(false); //probably should split the validation and the save logic. There is duplication here.
    });

    $("#cancel-button").click(function (e) {
        e.preventDefault();
        var editTabLink = $("#returnUrl").attr("href");
        if (editTabLink.charAt(editTabLink.length - 1) != "/") {
            editTabLink += "/";
        }
        window.location.assign(editTabLink);
    });

    $("#AddTransferItem").click(function (e) {
        saveListAndContinue(true);
    });

    $(function () {
        $(".isDate").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + "px",
                    marginLeft: textbox.offsetWidth + "px"
                });
            }
        });
    });
}); //end of document.ready()

function validateEntry(pti) {
    if (isItemValid(pti) == false) {
        pti.addClass("invalid-item");
    } else {
        pti.removeClass("invalid-item");
    }
}

function resetNewItem() {
    $(".new-item #SourceCommunityId").val($("#Communities").val());
    $(".new-item #DestinationCommunityId").val($("#Communities").val());
    $(".new-item #EmployeeId").val("");
    
}

function updateGLAfterDateChange(ptiForm) {
    var employee = ptiForm.find("#EmployeeId").val();
    var transferDate = ptiForm.find(".isDate").val();
    $.ajax({
        url: path + "PayrollTransfer/GetEmployeeLedgersForDate?employeeId=" + employee + "&transferDate=" + transferDate,
        success: function (data) {
            var ledgers = ptiForm.find("#SourceGeneralLedgerId");
            var currentVal = ledgers.val();
            ledgers.empty();
            ledgers.append($("<option><option>").attr("value", "").text("Select a Job Role")[0].outerHTML);

            for (var i = 0; i < data.length; i++) {
                var displayName = data[i].AccountNbr + " - " + data[i].AccountName;
                var value = data[i].GeneralLedgerId;
                if (currentVal == value) {
                    ledgers.append($("<option><option>").attr("value", value).attr("selected","selected").text(displayName)[0].outerHTML);
                }
                else {
                    ledgers.append($("<option><option>").attr("value", value).text(displayName)[0].outerHTML);
                }
            }
        }
    });
}

function employeeItemChanged() {
    //#payroll-transfer-section
    if ($("#EmployeesDropdown").val() != "") {
        var newPTIUrl = path + "PayrollTransfer/CreateOrEditPayrollTransfer?employeeId=" + $("#EmployeesDropdown").val();
        if ($(".payroll-transfer-item").length == 0) {
            $("#payroll-transfer-section").load(newPTIUrl, function () {
                $("#AddTransferItem").removeAttr("disabled");
                $("#savePayrollTransfer").removeAttr("disabled");
                $(".payroll-transfer-item").addClass("new-item");
                $(".payroll-transfer-item").addClass("isDirty");
                $(".isDate").datepicker("destroy");
                $(".isDate").datepicker({
                    fixFocusIE: false,
                    /* blur needed to correctly handle placeholder text */
                    onSelect: function () {
                        this.fixFocusIE = true;
                    },
                    onClose: function () {
                        this.fixFocusIE = true;
                        this.focus();
                        var form = $(this).closest("form");
                        
                        updateGLAfterDateChange(form);
                    },
                    beforeShow: function () {
                        var isIE = /*@cc_on!@*/false || !!document.documentMode;
                        var isEdge = !isIE && !!window.StyleMedia;
                        if (isIE || isEdge) {
                            isIE = true;
                        }
                        var result = isIE ? !this.fixFocusIE : true;
                        this.fixFocusIE = false;
                        return result;
                    }
                });
            });
        }
        else {
            var currentItem = $(".new-item");
            $.get(newPTIUrl, function (data) {
                currentItem.find("#source").html($(data).find("#source").html());
                currentItem.find("#DestinationGeneralLedgerId").val($(data).find("#DestinationGeneralLedgerId").val());
            });
        }
    }
}

function communityItemChanged() {
    resetNewItem();
}

function deletePTI(pti) {
    if ($(".payroll-transfer-item").length <= 1) {
        alert("Cannot delete the last entry");
        return;
    }
    pti.addClass("deleteMe");
    var id = pti.find("#Id").val();
    if (id == 0) {
        pti.remove();
        if ($(".new-item").length == 0) {
            var pti = $(".payroll-transfer-item")[0];
            editPTI($(pti));
        }
    }
    else {
        
        $.ajax({
            url: path + "PayrollTransfer/Delete",
            type: "post",
            data: {
                id: id
            }
            , success: function (result) {
                $(".deleteMe").remove();
                if ($(".new-item").length == 0) {
                    var pti = $(".payroll-transfer-item")[0];
                    
                    editPTI($(pti));
                }
            }, error: function (error) {
                alert("Error communicating with the server, try again later");
                
            }
        });
    }
}
var ptiEdited = false;
var ptiEditedEmployee;
function editPTI(pti) {
    if ($(".new-item").length > 0) {
        var currentEditedItem = $(".new-item");
        validateEntry(currentEditedItem);
        currentEditedItem.removeClass("new-item");
    }
    pti.addClass("new-item");
    var community = pti.find("#SourceCommunityId").val();
    ptiEditedEmployee = pti.find("#EmployeeId").val();
    $("#Communities").val(community);
    $("#Communities").change();
    ptiEdited = true;
    $(document).ajaxComplete(function () {
        if (ptiEdited) {
            $("#EmployeesDropdown").val(ptiEditedEmployee);
            $("#EmployeesDropdown").change();
            ptiEdited = false;
        }
    });
    //$("#Communities").change()
}

function saveListAndContinue(getNew) {
    var payrollTransferItems = $("fieldset").children(".isDirty");
    
    //verify values
    var promises = [];
    for (x = 0; x < payrollTransferItems.length; x++) {
        var pti = $(payrollTransferItems[x]);
        if (isItemValid(pti) == false) {
            pti.addClass("invalid-item");

        } else {
            pti.removeClass("invalid-item");
        }

        if (!pti.hasClass("invalid-item")) {
            var form = pti.closest("form");
            var request = form.ajaxSubmit(ptiSaved(pti, getNew));
            promises.push(request);
        }
    }
    if ($(".invalid-item").length > 0){
        alert("Please validate entries highlighted red and try again.\nChanges to that item were not saved.");
    }
    $.when.apply(null, promises).done(function () {
        if (!getNew) {
            if ($(".invalid-item").length == 0){
                window.location.assign($("#returnUrl").attr("href"));
            }
        }
    });

}

var ptiSaved = function (pti, getNew){
    return function (result) {
        if (result.success) {
            pti.find("#Id").val(result.data);
            pti.removeClass("isDirty");
            if (pti.hasClass("new-item") && getNew) {
                var newPTIUrl = path + "PayrollTransfer/CreateOrEditPayrollTransfer?employeeId=" + pti.find("#EmployeeId").val() + "&payrollTransferId=" + result.data;
                $.get(newPTIUrl, function (newPTI) {
                    $("#payroll-transfer-section").prepend(newPTI);
                    var hourCount = pti.find("#HourCnt").val();
                    var dollarCount = pti.find("#PayAmt").val();

                    var newItem = $("#payroll-transfer-section").find(".payroll-transfer-item").first();
                    newItem.find("#HourCnt").val(hourCount);
                    newItem.find("#PayAmt").val(dollarCount);
                    newItem.addClass("new-item");
                    newItem.addClass("isDirty");
                    newItem.find("#Id").val(0);
                    $(".isDate").datepicker({
                        fixFocusIE: false,
                        /* blur needed to correctly handle placeholder text */
                        onSelect: function () {
                            this.fixFocusIE = true;
                        },
                        onClose: function () {
                            this.fixFocusIE = true;
                            this.focus();
                        },
                        beforeShow: function () {
                            var isIE = /*@cc_on!@*/false || !!document.documentMode;
                            var isEdge = !isIE && !!window.StyleMedia;
                            if (isIE || isEdge) {
                                isIE = true;
                            }
                            var result = isIE ? !this.fixFocusIE : true;
                            this.fixFocusIE = false;
                            return result;
                        }
                    });
                    pti.removeClass("new-item");

                });
            }

        }
    }
}

function isItemValid(payrollItem) {
    if (parseInt(payrollItem.find("#EmployeeId").val()) == 0
        || payrollItem.find("#SourceCommunityId option:selected").val().trim().length == 0 || parseInt(payrollItem.find("#SourceCommunityId option:selected").val()) == 0
        || payrollItem.find("#SourceGeneralLedgerId option:selected").val().trim().length == 0 || parseInt(payrollItem.find("#SourceGeneralLedgerId option:selected").val()) == 0
        || payrollItem.find("#DestinationCommunityId option:selected").val().trim().length == 0 || parseInt(payrollItem.find("#DestinationCommunityId option:selected").val()) == 0
        || payrollItem.find("#DestinationGeneralLedgerId option:selected").val().trim().length == 0 || parseInt(payrollItem.find("#DestinationGeneralLedgerId option:selected").val()) == 0
        || payrollItem.find(".isDate").val().trim().length == 0 || ValidateDate(payrollItem.find(".isDate").val()) == false) {
        return false;
    } else {
        return true;
    }
}