$("#sidebar").on("change", "#SelectedEmployeeId", function () {
    var employeeId = $(this).val();
    if (employeeId == "") {
        $("#newHireListSection").html("");
        $("#newHireChecklistSection").html("");
        $("#employeInfo").html('<label class="text-danger">Please choose an Employee</label>');
        return;
    }
    loadNewHireList();
});

$("#sidebar").on("change", "#IncludeTerminated, #LookBackDate, #SelectedFacilityId", function () {
    $("#SelectedEmployeeId").val("0");
    var form = $("#sideBarForm");
    form.ajaxSubmit({
        target: "#sidebar",
        success: function () {
            $("#newHireListSection").html("");
            $("#newHireChecklistSection").html("");
            $("#employeInfo").html('<label class="text-danger">Please choose an Employee</label>');
        }

    });

});

function loadNewHireList() {
    var employeeId = $("#SelectedEmployeeId").val();
    $("#employeInfo").load(path + "NewHire/GetEmployeeInfo?id=" + employeeId);

    $("#newHireListSection").load(path + "NewHire/NewHireListing?employeeId=" + employeeId, function () {
        $("#newHireTable").dataTable({
            "bFilter": false,
            "bAutoWidth": false,
            "fixedColumns": false,
            "sDom": "frtS",
            "iDisplayLength": -1
        });
        $("#newHireTable").on("click", ".Edit", function (e) {
            e.preventDefault();
            var url = this.href;
            loadChecklist(url);

        });
        $("#newHireTable").find(".Edit").first().click();
    });

}

function loadChecklist(url) {
    $("#newHireChecklistSection").load(url, function () {
        $("#newHireChecklistTable").dataTable({
            "bFilter": false,
            "bSort": false,
            "bAutoWidth": false,
            "fixedColumns": false,
            "sDom": "frtS",
            "iDisplayLength": -1
        });
        $(".deleteFile").on("click", function (e) {

            e.preventDefault();
            $.ajax({
                url: this.href,
                success: function (result) {
                    if (result.success) {
                        loadChecklist(url);
                    }
                    else {
                        alert("Error: Event not found in database");
                    }

                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }

            })
        });

        $(".saveFile").on("click", function (e) {
            e.preventDefault();
            var form = $(this).closest("form");
            form.ajaxSubmit({
                dataType: "json",
                iframe: true,
                success: function (result) {
                    if (result.success) {
                        loadChecklist(url);
                    }
                    else {
                        alert(result.data);
                    }

                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }

            });

        });

        $("#CompletedFlg").on("change", function () {
            var id = $(this).attr("data-info-id");
            var checked = $(this).is(":checked");
            $.ajax(path + "NewHire/SetCompletedFlg?newHireId=" + id + "&status=" + checked, {
                success: function (result) {
                    if (result.success) {
                        loadNewHireList();
                    }
                    else {
                        alert(result.data);

                    }

                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }

            });
            //update completed

        });

        $(".expandRows").on("click", function () {
            var id = $(this).closest("tr").attr("checklistid");
            $("[parentid=" + id + "]").show();
            $(this).hide();
            $(this).siblings(".retractRows").show();
        });
        $(".retractRows").on("click", function () {
            var id = $(this).closest("tr").attr("checklistid");
            $("[parentid=" + id + "]").hide();
            $(this).hide();
            $(this).siblings(".expandRows").show();
        });
    });
}