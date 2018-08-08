function loadTabs() {
    loadEquipmentTab();
    loadUpcomingTab();
    loadPastDueTab();

}

function setupEvents() {

    $("#communitySelector").change(function () {
        $("#detailsSection").html("");
        loadTabs();
    });

    $("#addEquipment").on("click", function (e) {
        e.preventDefault();
        var communityId = $("#communitySelector").val();
        editEquipment(this.href + "?communityId=" + communityId);

    });

}

function loadEquipmentTab() {
    var community = $("#communitySelector").val();
    $("#equipmentTab").load(path + "EquipmentManagement/EquipmentList?communityId=" + community, setupEquipmentTab);
}

function loadUpcomingTab() {
    var community = $("#communitySelector").val();
    $("#upcomingTab").load(path + "EquipmentManagement/PlanList?upcoming=true&communityId=" + community, setupUpcomingTab);

}

function loadPastDueTab() {
    var community = $("#communitySelector").val();
    $("#pastDueTab").load(path + "EquipmentManagement/PlanList?upcoming=false&communityId=" + community, setupPastDueTab);

}

function setupEquipmentTab() {
    var dataTable = $("#equipmentTable").DataTable({
        columnDefs: [
            {
                targets: "noSort",
                orderable: false,
                searchable: false
            }

        ],
        autoWidth: false,
        width: 800,
        paging: false
    });
    $("#equipmentTable").on("click", ".edit", function (e) {
        e.preventDefault();
        editEquipment(this.href);
    });
    $("#equipmentTable").on("click", ".details", function (e) {
        e.preventDefault();

        loadDetails(this.href);
    });
    $("#equipmentTable").on("click", ".newRepair", function (e) {
        e.preventDefault();
        editRepair(this.href);
    });

    $("#equipmentTable").on("click", ".newPlan", function (e) {
        e.preventDefault();
        editPlan(this.href);
    });
}

function setupUpcomingTab() {
    $("#upcomingTab").find(".planTable").DataTable({
        columnDefs: [
            {
                targets: "noSort",
                orderable: false,
                searchable: false
            },

        ],
        order: [
            [3, "desc"]
        ],
        language: { emptyTable: "No Plans for this equipment exist" },
        paging: false
    });
    $("#upcomingTab").on("click", ".newInspection", function (e) {
        e.preventDefault();
        editInspection(this.href);
    });
    $("#upcomingTab").on("click", ".edit", function (e) {
        e.preventDefault();
        editPlan(this.href);
    });
}

function setupPastDueTab() {
    $("#pastDueTab").find(".planTable").DataTable({
        columnDefs: [
            {
                targets: "noSort",
                orderable: false,
                searchable: false
            }

        ],
        language: { emptyTable: "No Plans for this equipment exist" },
        paging: false
    });
    $("#pastDueTab").on("click", ".newInspection", function (e) {
        e.preventDefault();
        editInspection(this.href);
    });
    $("#pastDueTab").on("click", ".edit", function (e) {
        e.preventDefault();
        editPlan(this.href);
    });
}

function editEquipment(url) {
    $("#modal").load(url, function () {
        $("#modal").dialog({ height: 360, width: "auto", resize: "auto", modal: true });
        if ($("#EquipmentId").val() == "0") {
            $("#LifeExpectancyCnt").val("");
            $("#PurchaseDate").val("");
            $("#InstalledDate").val("");
        }
        $("#vendorDropDown").chosen({
            no_results_text: "no matches found!",
            width: "200px",
            height: "100px"

        });
        $(".date-picker").datepicker({
            onSelect: function () {
                //alert("WHAT IS GOING ON");
                //ie fix for date picker re-opening after selection in modal windows
                $(this).parent().focus();
            }
        });
        $("#equipmentSubmit").on("click", function (e) {
            e.preventDefault();
            var form = $("#saveEquipmentForm");
            if (!form.valid()) {
                return;
            }
            form.ajaxSubmit({
                success: function (result) {
                    if (result.success) {
                        loadEquipmentTab();
                        reloadDetails();
                        $("#modal").dialog("close");
                    }

                }

            });

        });
    });


}

function editPlan(url) {
    $("#modal").load(url, function () {
        $("#modal").dialog({ height: 360, width: "auto", resize: "auto", modal: true });
        $(".date-picker").datepicker({
            onSelect: function () {
                //ie fix for date picker re-opening after selection in modal windows
                $(this).parent().focus();
            }
        });
        if ($("#EquipmentMaintenancePlanId").val() == "0") {
            $("#StartDate").val("");
        }
        $("#planSubmit").on("click", function (e) {
            e.preventDefault();
            var form = $("#savePlanForm");
            if (!form.valid()) {
                return;
            }
            form.ajaxSubmit({
                success: function (result) {
                    if (result.success) {
                        loadTabs();
                        reloadDetails();
                        $("#modal").dialog("close");
                    }
                }
            });

        });
    });
}

function editRepair(url) {
    $("#modal").load(url, function () {
        $("#modal").dialog({ height: 400, width: 700, modal: true });
        $("#addDocument").on("click", function () {
            var index = $(".documentRow").length;
            $("#documentSection").append($("<div>").load(path + "EquipmentManagement/NewDocument?index=" + index));
        });
        $("#repairSubmit").on("click", function (e) {
            e.preventDefault();
            var form = $("#saveRepairForm");
            if (!form.valid()) {
                return;
            }
            form.ajaxSubmit({
                dataType: "json",
                iframe: true,
                success: function (result) {
                    if (result.success) {
                        loadTabs();
                        reloadDetails();
                        $("#modal").dialog("close");
                    }
                }
            });

        });
    });

}

function editInspection(url) {
    $("#modal").load(url, function () {
        $("#modal").dialog({ height: 400, width: 700 });
        $("#addDocument").on("click", function () {
            var index = $(".documentRow").length;
            $("#documentSection").append($("<div>").load(path + "EquipmentManagement/NewDocument?index=" + index));
        });
        $("#saveInspection").on("click", function (e) {
            e.preventDefault();
            var form = $("#saveInspectionForm");
            if (!form.valid()) {
                return;
            }
            form.ajaxSubmit({
                dataType: "json",
                iframe: true,
                success: function (result) {
                    if (result.success) {
                        loadTabs();
                        reloadDetails();
                        $("#modal").dialog("close");
                    }
                }
            });
        })
    });

}

function loadDetails(url) {
    $("#detailsSection").load(url, function () {
        $("#detailsSection").find(".tabs").tabs();
        $("#closeDetails").on("click", function () {
            $("#detailsSection").html("");

        });
        $(".newRepair").on("click", function (e) {
            e.preventDefault();
            editRepair(this.href);
        });
        $(".edit").on("click", function (e) {
            e.preventDefault();
            editPlan(this.href);
        });
        $(".vendorInfo").on("click", function (e) {
            e.preventDefault();
            loadVendorInfo(this.href);
        });
        $(".newInspection").on("click", function (e) {
            e.preventDefault();
            editInspection(this.href);

        });
        $(".dataToggle").on("click", function (e) {
            e.preventDefault();
            $($(this).attr("data-target")).toggle("fast");

        });
    });

    //plansTab
}

function loadVendorInfo(url) {
    $("#modal").load(url, function () {
        $("#modal").dialog({ height: "auto", width: "auto", resize: "auto" });
    });
}

function reloadDetails() {

    var equipmentDetailsId = $("#equipmentDetailsId");
    if (equipmentDetailsId.length > 0) {
        loadDetails(path + "EquipmentManagement/EquipmentDetails?equipmentId=" + equipmentDetailsId.val());

    }

}

$(document).ready(function () {
    $(".tabs").tabs();
    setupEvents();
    loadTabs();
});