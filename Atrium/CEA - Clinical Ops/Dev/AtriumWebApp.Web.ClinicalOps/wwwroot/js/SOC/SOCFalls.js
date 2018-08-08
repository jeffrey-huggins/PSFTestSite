$(document).ready(function () {
    var eventId = $("#FallEvent_SOCEventId").val();

    if (eventId == "0") {
        $("#FallEvent_FallTime").val("");
    }

    $("#FallEvent_FallTime").datetimepicker({
        timeFormat: "hh:mm tt"

    });

    $("#interventionTable").dataTable({
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "bVisible": false
            },
            null,
            {
                "bSortable": false
            }
        ]
    });
    $("#typeTable").dataTable({
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "bVisible": false
            },
            null,
            {
                "bSortable": false
            }
        ]
    });
    $("#injuryTable").dataTable({
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "bVisible": false
            },
            null,
            {
                "bSortable": false
            }
        ]
    });
    $("#treatmentTable").dataTable({
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "bVisible": false
            },
            null,
            {
                "bSortable": false
            }
        ]
    });
    $("#saveFallForm").show();
});

$("#submitFallForm").on("click", function (e) {
    e.preventDefault();
    var form = $("#saveFallForm");
    if (!form.valid()) {
        return;

    }
    form.ajaxSubmit({
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Success) {
                $("#modal").dialog("close");
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


$.validator.addMethod("SOC_OneFromTableSelected", function (value, element) {
    var valid = true;
    var interventionCount = $("#interventionTable input:checked").length;
    var typeCount = $("#typeTable input:checked").length;
    var injuryCount = $("#injuryTable input:checked").length;
    var treatmentCount = $("#treatmentTable input:checked").length;
    return (interventionCount > 0 && typeCount > 0 && injuryCount > 0 && treatmentCount > 0);
}, "Check at least one item from each table.");


$.validator.setDefaults({ ignore: [] });
$("#saveFallForm").removeData("validator");
$("#saveFallForm").removeData("unobtrusiveValidation");
$.validator.unobtrusive.parse($("#saveFallForm"));
$("#saveFallForm").validate();