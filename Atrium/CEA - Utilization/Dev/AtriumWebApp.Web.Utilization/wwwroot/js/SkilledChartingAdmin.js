$(document).ready(function () {
    $("#tabs").tabs();
    $("#tabs").tabs({
        activate: function (event, ui) {
            var dataTables = ui.newPanel.find(".dataTable").filter(function () {
                return this.id != "";
            });
            for (i = 0; i < dataTables.length; i++) {
                var dataTable = $("#" + dataTables[i].id).dataTable();
                dataTable.fnAdjustColumnSizing();
            }
        }
    });
    $("#guidelineTab").on("click", "#AddDocGuideline", function (e) {
        e.preventDefault();
        var form = $("#guidelineTab form");
        var code = $("#GuidelineName").val();
        var dataTable = $("#Guidelines").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Guideline already exists");
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
                    var name = result.data.GuidelineName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.GuidelineId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="queues" href="'+path+'SkilledChartingAdmin/DocumentationQueues?guidelineId=' + id +'">Queues</a>',
                        '<a class="edit" href="">Edit</a><a class="delete" href="">Delete</a>',
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


    var oTableGuidelines = $('#Guidelines').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '350px'
            },
            {
                "sWidth": '100px'
            },
            {
                "sWidth": '130px'
            },
            null,
            {
                "sWidth": '125px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Guidelines"
        }
    });

    $("#Guidelines").on("click", "a.queues", function (e) {
        e.preventDefault();
        var link = this.href;
        $("#queueModal").load(link, function () {
            $("#queueModal").dialog({
                width: 750,
                modal: true,
                resizable: false
            });
        });
    });

    oTableGuidelines.fnSort([[1, 'asc'], [0, 'asc']]);
    var nEditing = null;

    $("#Guidelines").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "SkilledChartingAdmin/DeleteGuideline",
            cache: false,
            type: 'POST',
            data: {
                guidelineId: nRowId
            },
            success: function (result) {
                oTableGuidelines.fnDeleteRow(nRow);
            },
            error: function (ex) {
                alert("Guideline has failed to be deleted. Please try again " +
                    "and if you see this message repeatedly the server may be down.");
            }
        });
    });

    // Edit
    $('#Guidelines').on('click', 'a.edit', function (e) {
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

    $('#Guidelines').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        ResetRow($(this).parents('tr')[0]);
    });

    function ResetRow(nRow) {
        var aData = oTableGuidelines.fnGetData(nRow);
        oTableGuidelines.fnUpdate(aData[0], nRow, 0, false);
        oTableGuidelines.fnUpdate(aData[1], nRow, 1, false);
        oTableGuidelines.fnUpdate('<a class="edit" href="">Edit</a> <a class="delete" href="">Delete</a>', nRow, 4, false);
        nEditing = null;
    }

    function EditRow(nRow) {
        var aData = oTableGuidelines.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        jqTds[0].innerHTML = '<input type="text" size="50" maxlength="256" value="' + aData[0] + '"/>';
        jqTds[1].innerHTML = '<input type="text" size="1" maxlength="3" value="' + aData[1] + '"/>';
        jqTds[4].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
        nEditing = nRow;
    }

    function SaveRow(nRow, nRowId) {
        var jqInputs = $('input', nRow);
        $.ajax({
            url: path + "SkilledChartingAdmin/SaveGuideline",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                GuidelineId: nRowId,
                GuidelineName: jqInputs[0].value,
                SortOrder: jqInputs[1].value
            },
            success: function () {
                oTableGuidelines.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTableGuidelines.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTableGuidelines.fnUpdate('<a class="edit" href="">Edit</a> <a class="delete" href="">Delete</a>', nRow, 4, false);
                oTableGuidelines.fnSort([[1, 'asc'], [0, 'asc']]);
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
            url: path + "SkilledChartingAdmin/UpdateGuidelineDataEntryFlag",
            cache: false,
            type: 'POST',
            data: {
                guidelineId: nRowId,
                dataEntryFlag: checked
            },
            success: function () {
            },
            error: function (ex) {
                checkbox.attr("checked", !checked);
                alert("Guideline has failed to be enabled/disabled. Please try again " +
                    "and if you see this message repeatedly the server may be down.");
            }
        });
    });

    //Community Table
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "600px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Communities"
        }
    });

    $(".checkbox").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "Base/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "SKC"
            },
            success: function (result) {
                if (result.Success) {
                } else {
                    checkbox.attr("checked", !checked);
                    alert("Community has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $("#SaveLookback").validate({
        rules: {
            LookbackDays: {
                required: true,
                number: true
            }
        }
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