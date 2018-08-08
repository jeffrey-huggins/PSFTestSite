function PreparePage() {
    var oTable = $("#PatientSkilledChartingTable").dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "300px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": "400px" // Guideline
            },
            {
                "bVisible": false // Sort Order
            },
            {
                "bVisible": false // Documentation Queues
            },
            {
                "sWidth": "50px" // Edit
            },
            {
                "sWidth": "50px" // Delete
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Skilled Charting Guidelines for this resident."
        }
    });
    oTable.fnSort([[1, 'asc'], [0, 'asc']]);

    $("#PatientSkilledChartingTable").on('click',"a.edit", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var data;

        $("#EditFlag").val("true");
        
        if (nRowId != "-1") {
            data = oTable.fnGetData(nRow)[2].split(",");
        }
        else {
            data = oTable.fnGetData(nRow)[2].split(";;");
        }

        LaunchModal(nRowId, data);
    });

    $("#PatientSkilledChartingTable").on('click','a.delete', function (e) {
        e.preventDefault();
        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            var patientId = $("#PatientId").val();
            $.ajax({
                url: path +"SkilledCharting/DeleteSkilledCharting",
                cache: false,
                type: 'POST',
                data: { patientId: patientId, guidelineId: nRowId },
                success: function (result) {
                    oTable.fnDeleteRow(nRow);
                },
                error: function (ex) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
    });

    $("#btnCreate").click(function () {
        var guidelineId = $("#Guidelines").val();
        if (guidelineId != "") {
            var tr = $("#PatientSkilledChartingTable tr[id='" + guidelineId + "']");
            if (tr.length > 0) {
                $("a.edit", tr).click();
            }
            else {
                LaunchModal(guidelineId);
            }
        }
    });

    SetupModal();
}

function SetupModal() {
    $("#btnSave").click(function () {
        var patientId = $("#PatientId").val();
        var guidelineId = $("#GuidelineId").val();
        var form = $("#DocumentationQueues_" + guidelineId);
        var ajaxUrl;
        var data = {
            patientId: patientId,
            guidelineId: guidelineId,
            documentationQueues: []
        };

        if (guidelineId != "-1") {
            ajaxUrl = path +"SkilledCharting/SaveSkilledCharting";
            $("input:checkbox:checked", form).each(function () {
                var nRowId = $(this).closest("tr").attr("id");
                data.documentationQueues.push(nRowId);
            });
        }
        else {
            ajaxUrl = path +"SkilledCharting/SaveSkilledChartingCustom";
            $("input:checkbox:checked", form).each(function () {
                var nRowId = $(this).closest("tr").attr("id");
                var customQueue = $("#CustomQueue_" + nRowId).val();
                if (customQueue != "") {
                    data.documentationQueues.push(customQueue);
                }
            });
        }
        
        if (data.documentationQueues.length == 0) {
            alert("At least one Documentation Queue must be selected.");
            return;
        }

        $.ajax({
            url: ajaxUrl,
            dataType: "json",
            cache: false,
            traditional: true,
            type: "POST",
            data: data,
            success: function (result) {
                alert("Skilled Charting Record has been successfully saved.");
                UpdateGuidelinesTable(form, result);
                $("#Guidelines").val("");
                $("#btnClose").click();
            },
            error: function (ex) {
                alert("Skilled Charting Record save failure occurred!");
            }
        });
    });

    $("#btnClear").click(function () {
        var guidelineId = $("#GuidelineId").val();
        var form = $("#DocumentationQueues_" + guidelineId);
        $("input:checkbox", form).prop("checked", false);
        $("textarea", form).val("");
    });

    $("#btnClose").click(function () {
        $(".modal, #popupModal").hide();
        $(".docQueueTable").hide();
        $("#btnClear").click();

        $("#modalTitle").text("");
        $("#GuidelineId").val("");
        $("#EditFlag").val("false");
    });
}

function LaunchModal(guidelineId, data) {
    var div = $("<div />");
    div.addClass("modal");
    $("body").append(div);

    var modal = $("#popupModal");
    modal.show();

    var form = $("#DocumentationQueues_" + guidelineId);
    form.show();

    if ($("#EditFlag").val() == "true") {
        var oTable = $("#PatientSkilledChartingTable").dataTable();
        var nRow = $("#PatientSkilledChartingTable tr[id='" + guidelineId + "']")[0];
        $("#modalTitle").text(oTable.fnGetData(nRow)[0]);
    }
    else {
        $("#modalTitle").text($("#Guidelines option[value='" + guidelineId + "']").text());
    }

    var top = Math.max($(window).height() / 3 - modal[0].offsetHeight / 3, 0);
    var left = Math.max($(window).width() / 2 - modal[0].offsetWidth / 2, 0);
    modal.css({ top: top, left: left });

    // Fill form with existing record data
    $("#GuidelineId").val(guidelineId);

    if (data) {
        for (var i = 0; i < data.length; i++) {
            if (guidelineId != "-1") {
                $("#" + guidelineId + "_" + data[i], form).prop("checked", true);
            }
            else if (data[i] != "") {
                var queueId = i + 1;
                $("#" + guidelineId + "_" + queueId, form).prop("checked", true);
                $("#CustomQueue_" + queueId).val(data[i]);
            }
        }
    }
}

function UpdateGuidelinesTable(form, data) {
    var oTable = $("#PatientSkilledChartingTable").dataTable();
    var rowData = [
        data.Guidline.GuidelineName,
        data.Guidline.SortOrder,
        data.DocumentationQueues.toString(),
        "<a class=\"edit\" href=\"\">Edit</a>",
        "<a class=\"delete\" href=\"\">Delete</a>"
    ];

    if ($("#EditFlag").val() == "true") {
        var element = document.getElementById(data.Guidline.GuidelineId);
        if (element != null) {
            var rowNumber = oTable.fnGetPosition(element);
            oTable.fnUpdate(rowData, rowNumber);
            element.setAttribute("id", data.Guidline.GuidelineId);
        }
    }
    else {
        var newEntry = oTable.fnAddData(rowData);
        var tr = oTable.fnSettings().aoData[newEntry[0]].nTr;
        tr.setAttribute("id", data.Guidline.GuidelineId);
    }

    oTable.fnSort([[1, 'asc'], [0, 'asc']]);
}