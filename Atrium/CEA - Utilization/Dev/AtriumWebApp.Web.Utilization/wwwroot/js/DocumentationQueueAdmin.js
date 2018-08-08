$(document).ready(function () {
    var oTable = $('#DocumentationQueues').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "475px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '370px'
            },
            {
                "sWidth": '100px'
            },
            {
                "sWidth": '130px'
            },
            {
                "sWidth": '125px'
            },
            {
                "bVisible": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Documentation Queues"
        }
    });

    oTable.fnSort([[1, 'asc'], [4, 'asc']]);
    var nEditing = null;

    $("#DocumentationQueues").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "SkilledChartingAdmin/DeleteDocumentationQueue",
            cache: false,
            type: 'POST',
            data: {
                docQueueId: nRowId
            },
            success: function (result) {
                oTable.fnDeleteRow(nRow);
            },
            error: function (ex) {
                alert("Documentation Queue has failed to be deleted. Please try again " +
                    "and if you see this message repeatedly the server may be down.");
            }
        });
    });

    // Edit
    $('#DocumentationQueues').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            ResetRow(nEditing);
            EditRow(nRow);
        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            SaveRow(nEditing, $(this).closest('tr').attr("id"));
        }
        else {
            /* No row currently being edited */
            EditRow(nRow);
        }
    });

    $('#DocumentationQueues').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        ResetRow($(this).parents('tr')[0]);
    });

    function ResetRow(nRow) {
        var aData = oTable.fnGetData(nRow);
        oTable.fnUpdate(aData[0], nRow, 0, false);
        oTable.fnUpdate(aData[1], nRow, 1, false);
        oTable.fnUpdate('<a class="edit" href="">Edit</a> <a class="delete" href="">Delete</a>', nRow, 3, false);
        nEditing = null;
    }

    function EditRow(nRow) {
        var aData = oTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        jqTds[0].innerHTML = '<textarea rows="5" cols="40" maxlength="512">' + aData[0].replace(/<br>/g, "\n") + '</textarea>';
        jqTds[1].innerHTML = '<input type="text" size="1" maxlength="3" value="' + aData[1] + '"/>';
        jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
        nEditing = nRow;
    }

    function SaveRow(nRow, nRowId) {
        var jqInputs = $('textarea, input', nRow);
        $.ajax({
            url: path + "SkilledChartingAdmin/SaveDocumentationQueue",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                DocumentationQueueId: nRowId,
                DocumentationQueueName: jqInputs[0].value,
                SortOrder: jqInputs[1].value
            },
            success: function (result) {
                oTable.fnUpdate(jqInputs[0].value.replace(/\n/g, "<br>"), nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a> <a class="delete" href="">Delete</a>', nRow, 3, false);
                oTable.fnSort([[1, 'asc'], [4, 'asc']]);
                nEditing = null;
            },
            error: function (ex) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    }

    $(".EnableDisable").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRowId = $(this).closest('tr').attr("id");

        $.ajax({
            url: path + "SkilledChartingAdmin/UpdateDocumentationQueueDataEntryFlag",
            cache: false,
            type: 'POST',
            data: {
                docQueueId: nRowId,
                dataEntryFlag: checked
            },
            success: function () {

            },
            error: function (ex) {
                checkbox.attr("checked", !checked);
                alert("Documentation Queue has failed to be enabled/disabled. Please try again " +
                    "and if you see this message repeatedly the server may be down.");
            }
        });
    });

    $("form").on("click","#AddDocQueue", function (e) {
        e.preventDefault();
        var form = $("#newDocQueueForm");
        form.validate();
        var code = $("#DocumentationQueueName").val();
        $("#GuidelineId").val();
        var dataTable = $("#DocumentationQueues").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Documentation Queue already exists");
            return false;
        }
        if (!form.valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: "POST",
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    var name = result.data.DocumentationQueueName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.DocumentationQueueId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a><a class="delete" href="">Delete</a>',
                        ''
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });
});

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});

function alreadyExist(code, dataTable, index) {
    if (!index) {
        index = 0;
    }
    var dataSet = dataTable.dataTable().fnGetData();
    for (var i = 0; i < dataSet.length; i++) {
        var existingCode = dataSet[i][index];
        if (existingCode.toLowerCase() === code.toLowerCase()) {
            return true;
        }
    }
    return false;
}

function createCheckboxString(id, cssClass, name, value) {
    if (value) {
        var checked = "checked";
    }
    var html = '<input class="' + cssClass + '" id="' + id + '" name="' + name + '" type="checkbox" value="' + value + '" ' + checked + '>';
    html += '<input name="' + name + '" type="hidden" value="' + value + '">';
    return html;
}