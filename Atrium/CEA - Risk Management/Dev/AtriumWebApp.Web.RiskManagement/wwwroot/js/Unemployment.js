
function setupNotesSection() {
    var claimId = $("#ClaimId").val();
    $("#note").load(path + "RMUnemployment/CreateOrEditNote?claimId=" + claimId, setupNoteForm);
    $("#noteList").load(path + "RMUnemployment/NotesList?claimId=" + claimId, setupNoteList);

}
function setupNoteForm() {
    $("#noteSubmit").on("click", function (e) {
        e.preventDefault();
        var form = $("#noteForm");
        if (form.valid()) {
            form.ajaxSubmit({
                success: function (result) {
                    if (result.success) {
                        setupNotesSection();
                    }
                }
            });
        }
    });
    $("#noteClear").on("click", function (e) {
        e.preventDefault();
        var claimId = $("#ClaimId").val();
        $("#note").load(path + "RMUnemployment/CreateOrEditNote?claimId=" + claimId, setupNoteForm);
    });
}
function setupNoteList() {
    $("#noteTable").dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "rtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No notes for this claim"
        }
    });
    $("#noteTable").on("click", ".edit", function (e) {
        e.preventDefault();
        $("#note").load(this.href, setupNoteForm);
    });
    $("#noteTable").on("click", ".delete", function (e) {
        e.preventDefault();
        $.ajax({
            url: this.href,
            success: function (result) {
                if (result.success) {
                    setupNotesSection();

                }

            }

        })

    });
}

function setupPaymentSection() {
    var claimId = $("#ClaimId").val();
    $("#payment").load(path + "RMUnemployment/CreateOrEditPayment?claimId=" + claimId, setupPaymentForm);
    $("#paymentList").load(path + "RMUnemployment/PaymentList?claimId=" + claimId, setupPaymentList);
}
function setupPaymentForm() {
    var benefitId = $("#BenefitKey").val();
    if (benefitId == "-1") {
        $("#BenefitDate").val("");
    }
    if (benefitId == "-1" || benefitId == "-2") {
        $("#BenefitAmt").val("");
    }
    $("#submitPayment").on("click", function (e) {
        e.preventDefault();
        var form = $("#payoutForm");
        if (form.valid()) {
            form.ajaxSubmit({
                success: function (result) {
                    if (result.success) {
                        setupPaymentSection();

                    }

                }

            });

        }
    });
    $("#clearPayment").on("click", function (e) {
        setupPaymentSection();
    });
    $(".isDate").datepicker();

}
function setupPaymentList() {
    $("#payoutTable").dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "rtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No payments for this claim"
        }
    });
    $("#payoutTable").on("click", ".edit", function (e) {
        e.preventDefault();
        $("#payment").load(this.href, setupPaymentForm);
    });
    $("#payoutTable").on("click", ".delete", function (e) {
        e.preventDefault();
        $.ajax({
            url: this.href,
            success: function (result) {
                if (result.success) {
                    setupPaymentSection();
                }
                else {
                    alert(result.data);

                }
            }
        });
    });
}

function setupForm() {
    var claimId = $("#ClaimId").val();

    if (claimId == "") {
        $("#ClaimReceivedDate").val("");
        $("#ApplicationDate").val("");
        $("#MaxClaimAmt").val("");
    }
    else {
        setupPaymentSection();
        setupNotesSection();
    }
    $("#PreventableComments").toggle($("#PreventableFlg").is(":checked"));
    $("#PreventableFlg").on("change", function () {
        $("#PreventableComments").toggle($("#PreventableFlg").is(":checked"));
    });
    $(".isDate").datepicker();
    $("#saveClaim").on("click", function (e) {
        e.preventDefault();
        var form = $("#claimForm");
        if (form.valid()) {
            form.ajaxSubmit({
                success: function (result) {
                    if (result.success) {
                        loadForms();

                    }

                }

            });

        }
    });

    $("#newClaim").on("click", function (e) {
        e.preventDefault();
        loadForms();
    });

}
function loadForms() {
    var employee = $("#Residents").val();
    var url = path + "RMUnemployment/EmployeeInfo?employeeId=" + employee;
    $("#payment").html("Edit a claim to add payments or notes.");
    $("#paymentList").html("");
    $("#note").html("");
    $("#noteList").html("");
    $("#selectedEmployeeView").load(url, function () {
        $("#title").text("Unemployment Claim for " + $("#residentName").text());
    });
    $("#editForm").load(path + "RMUnemployment/CreateOrEditClaim?employeeId=" + employee, setupForm);

    $("#claimsList").load(path + "RMUnemployment/ClaimList?employeeId=" + employee, function () {
        $("#claimTable").dataTable({
            "bFilter": false,
            "bAutoWidth": false,
            "sScrollY": "200px",
            "sDom": "rtS",
            "iDisplayLength": -1,
            "oLanguage": {
                "sEmptyTable": "No claims for this employee"
            }
        });
        $("#claimTable").on("click", ".edit", function (e) {
            e.preventDefault();
            $("#editForm").load(this.href, setupForm);
        });
        $("#claimTable").on("click", ".delete", function (e) {
            e.preventDefault();
            $.ajax({
                url: this.href,
                success: function (result) {
                    if (result.success) {
                        loadForms();

                    }

                }

            })

        });
    });

}

function initSidebar() {
    $("#listing").html("");
    $("#title").text("Unemployment Claim");
    $(".instruction").show();
    $("#editForm").html("");
    $("#claimsList").html("");
    $("#Residents").val("");
}

$("#sideBar").on("change", "#Residents", function () {
    if ($("#Residents").val() != "" && $("#Residents").val() != "0") {
        $(".instruction").hide();
        loadForms();
    }
    else {
        $(".instruction").show();
        $("#listing").html("");
    }

});
$("#sideBar").on("change", "#ShowTerminatedEmployees", function () {
    $("#SideDDL").ajaxSubmit({
        url: path + "RMUnemployment/UpdateEmployeeSideBar",
        type: "POST",
        success: function (result) {
            $("#sideBar").html(result);
            initSidebar();
        }

    });

});
$("#sideBar").on("click", "#lookbackUpdate", function (e) {
    e.preventDefault();
    $("#Residents").val("");
    var lookbackDate = moment($("#LookbackDate").val(), 'MM/DD/YYYY', true);
    if (!lookbackDate.isValid()) {
        lookbackDate = moment($("#LookbackDate").val(), 'M/DD/YYYY', true);
    }

    if (!lookbackDate.isValid()) {
        alert("Error: Please enter a valid Termination Date (mm/dd/yyyy)");
    }
    else if (moment().diff(lookbackDate) < 0) {
        alert("Error: You can not have a Termination date that is in the future");
    }
    else {
        ShowProgress();
        $("#SideDDL").ajaxSubmit({
            url: path + "RMUnemployment/UpdateEmployeeSideBar",
            type: "POST",
            success: function (result) {
                $("#sideBar").html(result);
                initSidebar();
            }

        });
    }

});
$("#sideBar").on("change", "#Communities", function () {
    $("#SideDDL").ajaxSubmit({
        url: path + "RMUnemployment/UpdateEmployeeSideBar",
        type: "POST",
        success: function (result) {
            $("#sideBar").html(result);
            initSidebar();
        }
    });
});