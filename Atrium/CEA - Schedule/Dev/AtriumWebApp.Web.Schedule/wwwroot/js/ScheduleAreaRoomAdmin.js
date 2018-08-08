$(document).ready(function () {
    $('#addAreaRoom').on('click', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        var newRoomAreaText = $("#NewRoomArea").val();
        $.ajax({
            url: path + "/ScheduleAdmin/AddRoomArea",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                newRoomArea: newRoomAreaText
            },
            success: function (result) {
                if (result.Success == true) {
                    $("#RoomArea").dataTable().fnAddData([
                        newRoomAreaText,
                        '<a class="editAreaRoom" href="">Edit</a>',
                        '<a class="deleteAreaRoom" href="">Delete</a>'
                    ], true);
                    $("td:contains('" + newRoomAreaText + "')").closest('tr').prop('id', result.id);
                    $("#RoomArea").dataTable().fnDraw();

                    $("#NewRoomArea").val("")
                    alert('Successfully added new Hall/Unit.');
                }
                else {
                    alert('Please ensure the New Hall/Unit is valid.');
                }
            },
            error: function () {
                alert("Could not communicate with the server.");
            }
        });
    });
    $('#closeRoomArea').click(function () { $('.ui-dialog-content').dialog('close'); });

    var dataTable = $("#RoomArea").dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "150px",
        "sDom": "frtS",
        "aoColumns": [
            {
                "bSearchable": false,
                "bSortable": true,
                "sWidth": '85px'
            },//edit
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },//delete
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Halls/Units for this community."
        }
    });
    

    $("#RoomArea").on("click", "a.editAreaRoom", function (e) {
        e.preventDefault();
        var row = $(this).closest("tr");
        if ($("#RoomArea .editing").length > 0 && !row.hasClass("editing")) {
            //a different row is being edited
            var currentRow = $("#RoomArea .editing");
            restoreRow(currentRow);
            editRow(row);
        }
        else if (row.hasClass("editing") && this.innerHTML == "Save") {
            //this row needs to be saved
            saveRow(row)
        }
        else {
            editRow(row);
        }
    });

    $("#RoomArea").on("click", "a.deleteAreaRoom", function (e) {
        e.preventDefault();
        var row = $(this).closest("tr");
        var id = row.attr("id");
        var dt = row.closest("table").dataTable();
        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            $.ajax({
                url: path + "ScheduleAdmin/DeleteAreaRoom",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: { id: id },
                success: function (result) {
                    if (result.Success == true) {
                        dt.fnDeleteRow(row[0]);
                    }
                    else if (result.Msg != null && result.Msg != undefined) {
                        alert("Delete Unsuccessful: " + result.Msg);
                    } else {
                        alert("Error: Event not found in database");
                    }
                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
    });
    $("#RoomArea").on("click", "a.cancelAreaRoom", function (e) {
        var currentRow = $("#RoomArea .editing");
        restoreRow(currentRow);

    });
});

function editRow(row) {
    row.addClass("editing");
    var dt = row.closest("table").dataTable();
    var data = dt.fnGetData(row[0]);
    var TDs = row.find("td");
    TDs[0].innerHTML = '<input type="text" size="30" maxlength="30" value="' + data[0] + '"/>';
    TDs[1].innerHTML = '<a class="editAreaRoom" href="#">Save</a> <a class="cancelAreaRoom" href="#">Cancel</a>';
}

function restoreRow(row) {
    row.removeClass("editing");
    var dt = row.closest("table").dataTable();
    var data = dt.fnGetData(row[0]);
    for (var i = 0; i < data.length; i++) {
        dt.fnUpdate(data[i], row[0], i, false);
    }
}

function saveRow(row) {
    var id = row.attr("id");
    var name = row.find("input").val();
    var dt = row.closest("table").dataTable();
    $.ajax({
        url: path + "/ScheduleAdmin/EditAreaRoom",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            id: id,
            name: name
        },
        success: function (result) {
            if (result.Success == true) {
                dt.fnUpdate(name, row[0], 0, false);
                dt.fnUpdate('<a class="editAreaRoom" href="">Edit</a>', row[0], 1, false);
                row.removeClass("editing");
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}
