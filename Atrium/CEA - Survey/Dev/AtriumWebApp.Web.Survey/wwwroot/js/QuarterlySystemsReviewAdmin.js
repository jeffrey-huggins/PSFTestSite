function StandardsOfCareMeasureTable(selector) {
    var dataTable = $(selector).dataTable({
        "bFilter": false,
        "bAutoWidth": true,
        "sScrollY": "95px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '400px'
            },
            {
                "sWidth": '120px'
            },
            {
                "sWidth": '60px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Standards of Care Measures"
        }
    });
    var nEditing = null;
    $(dataTable).on('click',"a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(dataTable, nEditing);
            editRow(dataTable, nRow);
            nEditing = nRow;
        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(dataTable, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(dataTable, nRow);
            nEditing = nRow;
        }
    });
    $(dataTable).on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(dataTable, nRow);
    });
    function restoreRow(oTable, nRow) {
        var aData = oTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        for (var i = 0, iLen = jqTds.length ; i < iLen ; i++) {
            oTable.fnUpdate(aData[i], nRow, i, false);
        }
    }
    function editRow(oTable, nRow) {
        var aData = oTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        jqTds[0].innerHTML = '<input type="text" size="35" value="' + aData[0] + '"/>';
        jqTds[1].innerHTML = '<input type="text" size="3" value="' + aData[1] + '"/>';
        jqTds[2].innerHTML = '<input type="text" size="3" value="' + aData[2] + '"/>';
        jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
    }
    function saveRow(oTable, nRow, nRowId) {
        var jqInputs = $('input', nRow);
        $.ajax({
            url: path + "QuarterlySystemsReviewAdmin/EditStandardsOfCareMeasure",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                Id: nRowId,
                Name: jqInputs[0].value,
                ThresholdBonusPoints: jqInputs[1].value,
                SortOrder: jqInputs[2].value
            },
            success: function (result) {
                if (result.Success) {
                    oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                    oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                    oTable.fnUpdate(jqInputs[2].value, nRow, 2, false);
                    oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
                    $('.socMeasure' + nRowId).html(jqInputs[0].value);
                }
                
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    }
}

function GeneralMeasureTable(selector) {
    var dataTable = $(selector).dataTable({
        "bFilter": false,
        "bAutoWidth": true,
        "sScrollY": "95px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '400px'
            },
            {
                "sWidth": '100px'
            },
            {
                "sWidth": '60px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Other Measures"
        }
    });
    var nEditing = null;
    $(dataTable).on('click',"a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(dataTable, nEditing);
            editRow(dataTable, nRow);
            nEditing = nRow;
        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(dataTable, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(dataTable, nRow);
            nEditing = nRow;
        }
    });
    $(dataTable).on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(dataTable, nRow);
    });
    function restoreRow(oTable, nRow) {
        var aData = oTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        for (var i = 0, iLen = jqTds.length ; i < iLen ; i++) {
            oTable.fnUpdate(aData[i], nRow, i, false);
        }
    }
    function editRow(oTable, nRow) {
        var aData = oTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        jqTds[0].innerHTML = '<input class="name" type="text" size="35" value="' + aData[0] + '"/>';
        $('#RequiresPatientSample', $(jqTds[1])).prop('disabled', false);
        jqTds[2].innerHTML = '<input class="sortOrder" type="text" size="3" value="' + aData[2] + '"/>';
        jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
    }
    function saveRow(oTable, nRow, nRowId) {
        var name = $('.name', nRow).val();
        var checkbox = $('#RequiresPatientSample', nRow);
        var sortOrder = $('.sortOrder', nRow).val();
        $.ajax({
            url: path + "QuarterlySystemsReviewAdmin/EditGeneralMeasure",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                Id: nRowId,
                Name: name,
                RequiresPatientSample: checkbox.prop('checked'),
                SortOrder: sortOrder
            },
            success: function (result) {
                if (result.Success) {
                    if (checkbox.prop('checked')) {
                        checkbox.attr("checked", "checked");
                    }
                    else {
                        checkbox.removeAttr("checked");
                    }
                    checkbox.prop('disabled', true);
                    var checkboxMarkup = checkbox.parent().html();
                    oTable.fnUpdate(name, nRow, 0, false);
                    oTable.fnUpdate(checkboxMarkup, nRow, 1, true);
                    oTable.fnUpdate(sortOrder, nRow, 2, false);
                    oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
                    $('.generalMeasure' + nRowId).html(name);
                }
                
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    }
}

function QuestionTable(selector, updateUrl) {
    var dataTable = $(selector).dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '200px'
            },
            {
                "sWidth": '400px'
            },
            {
                "sWidth": '60px'
            },
            {
                "sWidth": '60px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Review Questions"
        }
    });
    var nEditing = null;
    $(dataTable).on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(dataTable, nEditing);
            editRow(dataTable, nRow);
            nEditing = nRow;
        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(dataTable, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(dataTable, nRow);
            nEditing = nRow;
        }
    });
    $(dataTable).on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(dataTable, nRow);
    });
    function restoreRow(oTable, nRow) {
        var aData = oTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        for (var i = 0, iLen = jqTds.length ; i < iLen ; i++) {
            oTable.fnUpdate(aData[i], nRow, i, false);
        }
    }
    function editRow(oTable, nRow) {
        var aData = oTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        jqTds[1].innerHTML = '<textarea type="text" cols="30" rows="3">' + aData[1] + '</textarea>';
        jqTds[2].innerHTML = '<input type="text" size="3" value="' + aData[2] + '"/>';
        jqTds[3].innerHTML = '<input type="text" size="3" value="' + aData[3] + '"/>';
        jqTds[4].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
    }
    function saveRow(oTable, nRow, nRowId) {
        var jqInputs = $('input', nRow);
        var text = $('textarea', nRow).val();
        $.ajax({
            url: updateUrl,
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                Id: nRowId,
                Text: text,
                MaxPoints: jqInputs[0].value,
                SortOrder: jqInputs[1].value
            },
            success: function (result) {
                if (result.Success) {
                    oTable.fnUpdate(text, nRow, 1, false);
                    oTable.fnUpdate(jqInputs[0].value, nRow, 2, false);
                    oTable.fnUpdate(jqInputs[1].value, nRow, 3, false);
                    oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 4, false);
                }
                
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    }
}

function PreparePage() {
    $("#tabs").tabs();

    var socMeasureTable = new StandardsOfCareMeasureTable("#SOCMeasures");
    var socQuestionTable = new QuestionTable("#SOCQuestions", path + "QuarterlySystemsReviewAdmin/EditStandardsOfCareQuestion");
    var generalMeasureTable = new GeneralMeasureTable("#GeneralMeasures");
    var generalQuestionTable = new QuestionTable("#GeneralQuestions", path + "QuarterlySystemsReviewAdmin/EditGeneralQuestion");
    var communityTable = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "745px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Law Firms"
        }
    });
    $(".checkbox", communityTable).click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        $.ajax({
            url: path + "QuarterlySystemsReviewAdmin/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "QSR"
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Community has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
                
            }
        });
    });
    $(".checkboxReport", communityTable).click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        $.ajax({
            url: path + "QuarterlySystemsReviewAdmin/ChangeReportFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "QSR"
            },
            success: function (result) {
                if (result.Success) {
                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
                
            }
        });
    });

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
}

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});