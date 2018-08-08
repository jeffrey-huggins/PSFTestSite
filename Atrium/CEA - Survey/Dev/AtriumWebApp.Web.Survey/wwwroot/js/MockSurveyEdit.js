function swapCells(table, row, x, y) {
    var nRow = row[0];
    var position = table.fnGetPosition(nRow);
    var aData = table.fnGetData(nRow);
    aData.swap(x, y);
    table.fnUpdate(aData, position);
}

function PreparePage() {
    var table = $('#deficiencies').dataTable({
        "bAutoWidth": false,
        "sScrollY": "560px",
        "bScrollCollapse": true,
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            {
                "bVisible": false
            },
            {
                "sWidth": "30%"
            },
            {
                "bVisible": false
            },
            {
                "sWidth": "30%"
            },
            {
                "bVisible": false
            },
            null,
            {
                "bVisible": false
            },
            {
                "sWidth": "30%"
            },
            {
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Federal Deficiencies"
        }
    });
    table.fnSort([[0, 'asc']]);

    table.on("click", ".create", function (e) {
        e.preventDefault();
        var row = $(this).closest('tr');
        var nRow = row[0];
        var nRowId = row.attr("id");
        var aData = table.fnGetData(nRow);
        $("#CitationId").val("");
        $("#DeficiencyId").val(nRowId);
        $("#Tag").val(aData[0]);
        $("#Description").val(row.data('full-description'));
        $("#MockSeverity").val("Finding");
        $("#FindingDetails").val("");
        var modal = $('<div />');
        modal.addClass("modal");
        $('body').append(modal);
        var modalWindow = $("#citationWindow");
        modalWindow.show();
        var top = Math.max($(window).height() / 2 - modalWindow[0].offsetHeight / 2, 0);
        var left = Math.max($(window).width() / 2 - modalWindow[0].offsetWidth / 2, 0);
        modalWindow.css({ top: top, left: left });
        table.fnAdjustColumnSizing();
    });

    table.on("click", ".edit", function (e) {
        e.preventDefault();
        var row = $(this).closest('tr');
        var nRow = row[0];
        var nRowId = row.attr("id");
        var aData = table.fnGetData(nRow);
        $("#CitationId").val(aData[5]);
        $("#DeficiencyId").val(nRowId);
        $("#Tag").val(aData[0]);
        $("#Description").val(row.data('full-description'));
        $("#MockSeverity").val(aData[6]);
        $("#FindingDetails").val(row.data('full-details'));
        var modal = $('<div />');
        modal.addClass("modal");
        $('body').append(modal);
        var modalWindow = $("#citationWindow");
        modalWindow.show();
        var top = Math.max($(window).height() / 2 - modalWindow[0].offsetHeight / 2, 0);
        var left = Math.max($(window).width() / 2 - modalWindow[0].offsetWidth / 2, 0);
        modalWindow.css({ top: top, left: left });
        table.fnAdjustColumnSizing();
    });

    table.on("click", ".delete", function (e) {
        e.preventDefault();
        if (confirm("Are you sure you want to delete this citation?") === true) {
            var jqRow = $(this).parents('tr');
            var nRow = jqRow[0];
            var nRowId = jqRow.attr("id");
            var tableData = table.fnGetData(nRow);
            var data = {
                CitationId: tableData[5],
                GroupType: $('#GroupType').val()
            };
            $.ajax({
                url: "../DeleteCitation",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: data,
                success: function (result) {
                    if (result.Success) {
                        var position = table.fnGetPosition(nRow);
                        tableData[5] = '';
                        tableData[6] = '';
                        tableData[7] = '';
                        tableData[8] = '';
                        tableData[9] = '<button class="create">Create</button>';
                        table.fnUpdate(tableData, position);
                    }
                    else {
                        alert("Error: Event not found in database");
                    }
                    
                },
                error: function (jqXHR, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
    });

    table.on("click", "a.moreDesc, a.lessDesc", function (e) {
        e.preventDefault();
        swapCells(table, $(this).parents('tr'), 1, 2);
    });

    table.on("click", "a.moreInst, a.lessInst", function (e) {
        e.preventDefault();
        swapCells(table, $(this).parents('tr'), 3, 4);
    });

    table.on("click", "a.moreDetail, a.lessDetail", function (e) {
        e.preventDefault();
        swapCells(table, $(this).parents('tr'), 7, 8);
    });

    $("#close").click(function () {
        $(".modal").hide();
        $("#citationWindow").hide();
    });

    $("#saveCitation").click(function () {
        var form = $("#SaveCitation")
        var data = form.serialize();
        var url = $("#CitationId").val() ? "../EditCitation" : "../CreateCitation";
        $.ajax({
            url: url,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: data,
            success: function (result) {
                if (result.Success) {
                    var deficiencyId = $('#DeficiencyId').val();
                    var jqRow = $('#' + deficiencyId);
                    var row = jqRow.get(0);
                    var position = table.fnGetPosition(row);
                    var rowData = table.fnGetData(row);
                    var findingDetails = $('#FindingDetails').val();
                    rowData[5] = result.Id;
                    rowData[6] = $('#MockSeverity').val();
                    rowData[7] = findingDetails;
                    rowData[8] = findingDetails;
                    rowData[9] = '<button class="edit">Edit</button> <button class="delete">Delete</button>';
                    table.fnUpdate(rowData, position);
                    jqRow.data('full-details', findingDetails);
                    alert("Citation has been successfully saved.");
                    $("#close").click();
                }
                else {
                    alert("Could not successfully save citation. Please reload the page and try again");
                }
                
            },
            error: function (jqXHR, error, errortext) {
                alert("Server is not responding. Please reload the page and try again");
            }
        });
    });

    $("#clearCitation").click(function () {
        $("#MockSeverity").val("Finding");
        $("#FindingDetails").val("");
    });
}