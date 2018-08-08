function PreparePage(baseUrl) {
    var formContent = $('#formContent');
    if ($("#ReviewId").length !== 1) {
        formContent.hide();
    }
    var table = $('#reviewsTable').dataTable({
        "bAutoWidth": false,
        "sScrollY": "600px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aaSorting": [
            [0, "desc"]
        ],
        "aoColumns": [
            null,
            {
                "sWidth": "150px"
            },
            {
                "sWidth": "150px"
            },
            {
                "sWidth": "60px"
            },
            {
                "sWidth": "100px"
            },
            {
                "sWidth": "100px"
            },
            {
                "sWidth": "30px",
                "bSortable": false
            },
            {
                "sWidth": "40px",
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No records within this range."
        }
    });
    table.on('click', '.delete-link', function (e) {
        var element = $(this);
        e.preventDefault();
        if (confirm("Are you sure you would like to delete this quarterly systems review?")) {
            ShowProgress();
            $.ajax({
                url: element.attr('href'),
                type: 'post',
                success: function () {
                    var jqRow = element.closest('tr');
                    var row = jqRow.get(0);
                    var rowIndex = table.fnGetPosition(row);
                    table.fnDeleteRow(rowIndex);
                    if (jqRow.attr('id') == $('#ReviewId').val()) {
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
    table.on('click', '.edit-link', function (e) {
        var element = $(this);
        e.preventDefault();
        ShowProgress();
        $.ajax({
            url: element.attr('href'),
            success: function (data) {
                formContent.html(data);
                formContent.show();
                PrepareForm();
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            },
            complete: function (jqXHR) {
                HideProgress();
            }
        });
    });
    $('#create').click(function (e) {
        var element = $(this);
        var url = element.attr('href') + '/' + $("#CurrentCommunity option:selected").text();
        e.preventDefault();
        ShowProgress();
        $.ajax({
            type: 'post',
            url: url,
            success: function (data) {
                formContent.html(data);
                formContent.show();
                PrepareForm();
                var id = $('#ReviewId').val();
                var today = new Date();
                var rowData = [
                    (today.getMonth()+1)+"/"+today.getDate()+"/"+today.getFullYear(), 
                    $('#BeginSampleDate').val(),
                    $('#EndSampleDate').val(),
                    "False",
                    "",
                    "",
                    '<a class="edit-link" href="' + baseUrl + '/Edit/' + id + '">Edit</a>',
                    '<a class="delete-link" href="' + baseUrl + '/Delete/' + id + '">Delete</a>'
                ];
                var newEntry = table.fnAddData(rowData, true);
                var tr = table.fnSettings().aoData[newEntry[0]].nTr;
                tr.setAttribute("id", id);
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            },
            complete: function (jqXHR, textStatus) {
                HideProgress();
            }
        });
    });
    function RedirectToButtonUrl(button) {
        var element = $(button);
        var url = element.data("button-url");
        ShowProgress();
        window.location.href = url;
    }
    formContent.on('click', '.SectionReviewCriteria', function (e) {
        e.preventDefault();
        RedirectToButtonUrl(this);
    });
    formContent.on('click', '.SectionSelectSamples', function (e) {
        e.preventDefault();
        RedirectToButtonUrl(this);
    });
    formContent.on('click', '#AdditionalQuestionsReviewCriteria', function (e) {
        e.preventDefault();
        RedirectToButtonUrl(this);
    });
    formContent.on('click', '#Save', function (e) {
        e.preventDefault();
        var form = $('#Review', formContent);
        var data = form.serialize();
        ShowProgress();
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: data,
            success: function (data, textStatus, jqXHR) {
                if (data.Success) {
                    var id = $('#ReviewId', form).val();
                    var row = $('#' + id);
                    if (row.length > 0) {
                        var rowData = [
                            data.ReviewDate, //$('#Date').text(),
                            $('#BeginSampleDate').val(),
                            $('#EndSampleDate').val(),
                            data.IsClosed ? "True" : "False",
                            data.FormattedNursingClosedDate,
                            data.FormattedDietaryClosedDate,
                            data.IsClosed && !data.IsAdministrator ? "" : '<a class="edit-link" href="' + baseUrl + '/Edit/' + id + '">Edit</a>',
                            data.IsClosed && !data.IsAdministrator ? "" : '<a class="delete-link" href="' + baseUrl + '/Delete/' + id + '">Delete</a>'
                        ];
                        table.fnUpdate(rowData, row[0]);
                    }
                    formContent.hide();
                }
                else {
                    formContent.html(data);
                    PrepareForm();
                }
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Error communicating with the server.  Please reload and try again.');
            },
            complete: function (jqXHR, textStatus) {
                HideProgress();
            }
        });
    });
    function PrepareForm() {
        if (!$('#CloseNursingSignature').prop('disabled')) {
            $('#CloseNursingSignature').prop('disabled', !$('#CloseNursing').prop('checked'));
        }
        if (!$('#CloseDietarySignature').prop('disabled')) {
            $('#CloseDietarySignature').prop('disabled', !$('#CloseDietary').prop('checked'));
        }
        $(".isDate").datepicker();
    }
    function getQuarter(d) {
        d = d || new Date(); // If no date supplied, use today
        var q = [1, 2, 3, 4];
        return q[Math.floor(d.getMonth() / 3)];
    }
    formContent.on('change', '#CloseNursing', function () {
        $('#CloseNursingSignature').prop('disabled', !this.checked);
    });
    formContent.on('change', '#CloseDietary', function () {
        $('#CloseDietarySignature').prop('disabled', !this.checked);
    });
    formContent.on('change', '#ReviewDateEntry', function () {
        var reviewDate = new Date($("#ReviewDateEntry").val());
        $("#ReviewDate").val($("#ReviewDateEntry").val());
        $("#Quarter").text("Q" + getQuarter(reviewDate));
    });
    $('#CurrentCommunity').change(function () {
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