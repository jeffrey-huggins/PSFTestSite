$(document).ready(function () {
    var ajaxBaseUri;
    if (document.location.pathname.charAt(document.location.pathname.length - 1) != "/") {
        ajaxBaseUri = document.location.pathname + "/";
    } else {
        ajaxBaseUri = document.location.pathname
    }
    var oTable = $('#window-table').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        //"sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '100px' },
            { "sWidth": '40px' },
            { "sWidth": '60px' },
            { "sWidth": '50px' },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '40px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '50px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No entries for date range selected"
        }
    });
    var nEditing = null;
    oTable.fnSort([[1, 'desc']]);

    $("#window-table_length").css("display", "none");

    $('#window-table').on('click', 'a.edit', function (e) {
        e.preventDefault();
        launchEditModal(this.href);
    });
    $('#window-table').on('click', 'a.delete', function (e) {
        e.preventDefault();

        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            $.ajax({
                url: ajaxBaseUri + "DeleteRow",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: { rowId: nRowId },
                success: function (result) {
                    if (result.Success) {
                        oTable.fnDeleteRow(nRow);
                        SetCountDownTime(1800);
                    }
                    else {
                        alert("Error: Event not found in database");
                    }
                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
    });
});