function PreparePage(baseUrl) {
    var formContent = $('#formContent');
    if ($("#MockSurveyId").length !== 1) {
        formContent.hide();
    }
    var table = $('#MockSurveys').dataTable({
        "bAutoWidth": false,
        "bSmart": false,
        "sScrollY": "400px",
        "sDom": "rtS",
        "iDisplayLength": -1,
        "aaSorting": [
            [0, "desc"]
        ],
        "aoColumns": [
            null,
            null,
            null,
            null,
            {
                "sWidth": "50px",
                "bSearchable": false,
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No current closed Mock Surveys for Community"
        }
    });
    table.on('click', 'a.edit-link', function (e) {
        e.preventDefault();
        ShowProgress();
        $.ajax({
            url: this.href,
            success: function (data) {
                formContent.html(data);
                formContent.show();
                
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            },
            complete: function (jqXHR) {
                HideProgress();
            }
        });
    });
    formContent.on('click', '#Save', function (e) {
        var form = $('#PlanOfCorrections');
        var data = form.serialize();
        e.preventDefault();
        ShowProgress();
        $.ajax({
            type: form.attr("method"),
            url: form.attr("action"),
            data: data,
            success: function (result) {
                if (result.Success) {
                    var id = $('#MockSurveyId', form).val();
                    var row = $('#' + id);
                    if (row.length > 0) {
                        var originalRowData = table.fnGetData(row.get(0));
                        var rowData = [
                            originalRowData[0],
                            originalRowData[1],
                            originalRowData[2],
                            result.FormattedClosedDate,
                            data.IsClosed ? "" : '<a class="edit-link" href="' + baseUrl + '/PlanCorrections/' + id + '">Plan Corrections</a>'
                        ];
                        table.fnUpdate(rowData, row[0]);
                    }
                    formContent.hide();
                }
                else {
                    formContent.html(result);
                }
                
            },
            error: function (jqXHR, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            },
            complete: function (jqXHR) {
                HideProgress();
            }
        });
    });
    $("#CurrentCommunity").change(function () {
        var community = $('#CurrentCommunity').val();
        if (community != '') {
            $('#CommunitySelectionForm').submit();
        }
    });
    $(".isDate").datepicker();
    if ($("#ToDateRangeInvalid").val() == "1") {
        alert('Error: Please enter a valid date in the Date Range "To" Field (mm/dd/yyyy)');
    }
    if ($("#FromDateRangeInvalid").val() == "1") {
        alert('Error: Please enter a valid date in the Date Range "From" Field (mm/dd/yyyy)');
    }
    if ($("#ToDateRangeInFuture").val() == "1") {
        alert('Error: Please enter a valid date that is not in the future in the Date Range "To" Field');
    }
    if ($("#FromDateRangeInFuture").val() == "1") {
        alert('Error: Please enter a valid date that is not in the future in the Date Range "From" Field');
    }
    if ($("#FromAfterTo").val() == "1") {
        alert('Error: You can not have the "From" Date after the "To" Date in the Date Range Fields');
    }
}