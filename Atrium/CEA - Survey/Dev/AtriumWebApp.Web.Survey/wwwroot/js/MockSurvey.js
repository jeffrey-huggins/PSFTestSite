function PreparePage(baseUrl) {
    var formContent = $('#formContent');
    if ($("#MockSurveyId").length !== 1) {
        formContent.hide();
    }
    var table = $('#MockSurveys').dataTable({
        "bAutoWidth": false,
        "bSmart": false,
        "sScrollY": "400px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aaSorting": [
            [0, "desc"]
        ],
        "aoColumns": [
            null,
            null,
            null,
            null,
            null,
            {
                "sWidth": "30px",
                "bSearchable": false,
                "bSortable": false
            },
            {
                "sWidth": "40px",
                "bSearchable": false,
                "bSortable": false
            },
            {
                "sWidth": "50px",
                "bSearchable": false,
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No current Mock Surveys for Community"
        }
    });
    table.on('click', '.delete-link', function (e) {
        var element = $(this);
        e.preventDefault();
        if (confirm("Are you sure you would like to delete this mock survey?")) {
            ShowProgress();
            $.ajax({
                url: element.attr('href'),
                type: 'post',
                success: function () {
                    var jqRow = element.closest('tr');
                    var row = jqRow.get(0);
                    var rowIndex = table.fnGetPosition(row);
                    table.fnDeleteRow(rowIndex);
                    if (jqRow.attr('id') == $('#MockSurveyId').val()) {
                        formContent.hide();
                    }
                    
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Could not communicate with the server.');
                },
                complete: function (jqXHR) {
                    HideProgress();
                }
            });
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
                PrepareForm();
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            },
            complete: function (jqXHR) {
                HideProgress();
            }
        });
    });
    table.on('click', 'a.follow-up-link', function (e) {
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
    $("#create").click(function (e) {
        var communitySelect = $('#CurrentCommunity');
        var communityId = $("option:selected", communitySelect).text();
        var url = this.href + "/" + communityId;
        e.preventDefault();
        ShowProgress();
        $.ajax({
            url: url,
            type: 'POST',
            success: function (data) {
                formContent.html(data);
                formContent.show();
                PrepareForm();
                var id = $('#MockSurveyId').val();
                var rowData = [
                    $('#MockSurveyDate').val(),
                    "",
                    "",
                    "",
                    "",
                    '<a class="edit-link" href="' + baseUrl + '/Edit/' + id + '">Edit</a>',
                    '<a class="delete-link" href="' + baseUrl + '/Delete/' + id + '">Delete</a>',
                    ""
                ];
                var newEntry = table.fnAddData(rowData, true);
                var tr = table.fnSettings().aoData[newEntry[0]].nTr;
                tr.setAttribute("id", id);
                
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            },
            complete: function (jqXHR, textStatus) {
                HideProgress();
            }
        });
    });
    formContent.on('click', '#Save', function (e) {
        var form = $('#MockSurvey');
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
                            $('#MockSurveyDate').val(),
                            result.FormattedClosedDate,
                            $('#FollowUpDate').val(),
                            originalRowData[3],
                            originalRowData[4],
                            data.IsClosed && !data.IsAdministrator ? "" : '<a class="edit-link" href="' + baseUrl + '/Edit/' + id + '">Edit</a>',
                            data.IsClosed && !data.IsAdministrator ? "" : '<a class="delete-link" href="' + baseUrl + '/Delete/' + id + '">Delete</a>',
                            originalRowData[7]
                        ];
                        table.fnUpdate(rowData, row[0]);
                    }
                    formContent.hide();
                }
                else {
                    formContent.html(result);
                    PrepareForm();
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
    formContent.on('click', '#SaveFollowUp', function (e) {
        var form = $('#FollowUp');
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
                            originalRowData[3],
                            result.FormattedClosedDate,
                            originalRowData[5],
                            originalRowData[6],
                            data.IsClosed && !data.IsAdministrator ? "" : '<a class="follow-up-link" href="' + baseUrl + '/FollowUp/' + id + '">Follow-up</a>'
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
    function PrepareForm() {
        if (!$('#CloseSignature').prop('disabled')) {
            $('#CloseSignature').prop('disabled', !$('#Close').prop('checked'));
        }
        $(".isDate").datepicker();
    }
    formContent.on('change', '#Close', function () {
        $('#CloseSignature').prop('disabled', !this.checked);
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