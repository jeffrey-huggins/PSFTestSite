
function PreparePage() {
    var addIndex = 0;
    var formContent = $('#formContent');

    function HideAllEmptyProviderTables() {
        HideTableIfNoRows($("#documents"));
        HideTableIfNoRows($("#contacts"));
        HideTableIfNoRows($("#addresses"));
    }

    var providersTable = $('#contractProvidersTable').dataTable({
        "bAutoWidth": false,
        "sScrollY": "600px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aaSorting": [
            [0, "desc"]
        ],
        "aoColumns": [
            {
                "sWidth": "160px"
            },
            {
                "sWidth": "20px"
            },
            {
                "sWidth": "20px"
            },
            {
                "sWidth": "140px"
            },
            {
                "sWidth": "100px"
            },
            {
                "sWidth": "20px"
            },
            {
                "sWidth": "10px",
                "bSortable": false
            },
            {
                "sWidth": "10px",
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No records within this range."
        }
    });

    function prepProviderForm() {
        //var options = {
        //    target: '#providerEditForm',
        //    success: function (data, textStatus, jqXHR, $form) {
        //        try {
        //            var result = data;

        //            if (result.Success) {
        //                window.location.replace(path + "ContractProvider/Providers");
        //                $('#providerEditForm').hide();
        //            }
        //        } catch (error) { }
        //    },
        //    error: function (jqXHR, textStatus, errorThrown) {
        //        alert('Failure communicating with the server.');
        //    }
        //}

        //$('#providerEditForm').submit(function (e) {
        //    console.log("submitted");
        //    e.preventDefault();
        //    $(this).ajaxSubmit(options);
        //    return false;
        //});
    }

    $('#createProvider').click(function (e) {
        var element = $(this);
        e.preventDefault();
        $.ajax({
            url: element.attr('href'),
            success: function (data) {
                formContent.html(data);
                InitializeDates();
                formContent.show();

                prepProviderForm();
                HideAllEmptyProviderTables();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    providersTable.on('click', '.deleteProvider', function (e) {
        var element = $(this);
        e.preventDefault();
        if (confirm("Are you sure you would like to delete this Provider/Vendor?")) {
            $.ajax({
                url: element.attr('href'),
                type: 'post',
                success: function (result) {
                    if (result.Success) {
                        var jqRow = element.closest('tr');
                        var row = jqRow.get(0);
                        var rowIndex = providersTable.fnGetPosition(row);
                        providersTable.fnDeleteRow(rowIndex);
                    }
                    else {
                        $('#errorResult').html(result.ErrorMessage);
                        $('#errorList').show().delay(5000).fadeOut(5000);
                    }
                    SetCountDownTime(1800);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Could not communicate with the server.');
                }
            });
        }
    });

    providersTable.on('click', '.editProvider', function (e) {
        var element = $(this);
        e.preventDefault();
        $.ajax({
            url: element.attr('href'),
            success: function (data) {
                formContent.html(data);
                InitializeDates();
                formContent.show();

                prepProviderForm();
                HideAllEmptyProviderTables();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    providersTable.on('click', '.viewProvider', function (e) {
        var element = $(this);
        e.preventDefault();
        $.ajax({
            url: element.attr('href'),
            success: function (data) {
                formContent.html(data);
                formContent.show();
                HideAllEmptyProviderTables();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    formContent.on('click', '#submitProvider', function (e) {
        e.preventDefault();
        ShowProgress();
        var element = $(this);
        var form = element.closest('form');
        var url = form.attr('action');

        var uniqueIdentifier = 'Documents';
        $('#documents tr').each(function (index) {
            ApplyUniqueItemIndex($(this), index - 1, uniqueIdentifier);
        });

        var uniqueIdentifier = 'Contacts';
        $('#contacts tr').each(function (index) {
            ApplyUniqueItemIndex($(this), index - 1, uniqueIdentifier);
        });

        var uniqueIdentifier = 'Addresses';
        $('#addresses tr').each(function (index) {
            ApplyUniqueItemIndex($(this), index - 1, uniqueIdentifier);
        });


        //var options = {
        //    target: '#providerEditForm',
        //    success: function (data, textStatus, jqXHR, $form) {
        //        try {
        //            var result = data;

        //            if (result.Success) {
        //                window.location.replace(path + "ContractProvider/Providers");
        //                $('#providerEditForm').hide();
        //            }
        //        } catch (error) { }
        //    },
        //    error: function (jqXHR, textStatus, errorThrown) {
        //        alert('Failure communicating with the server.');
        //    }
        //}
        $.ajax({
            url: url,
            type: "POST",
            data: $("#providerEditForm").serialize(),
            success: function (data, textStatus, jqXHR, $form) {
                try {
                    var result = data;

                    if (result.Success) {
                        window.location.replace(path + "ContractProvider/Providers");
                        $('#providerEditForm').hide();
                    }
                } catch (error) { }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });
        //$('#providerEditForm').submit(function (e) {
        //    console.log("submitted");
        //    e.preventDefault();
        //    $(this).ajaxSubmit(options);
        //    return false;
        //});



        return false;
    });

    formContent.on('click', '#addDocument', function (e) {
        e.preventDefault();
        $.ajax({
            url: path + "/ContractProvider/CreateDocument",
            success: function (data, textStatus, jqXHR) {
                var row = $(data);
                var uniqueIdentifier = 'Documents';
                ApplyUniqueItemIndex(row, addIndex, uniqueIdentifier);
                addIndex++;

                var documentTable = $('#documents');
                $('#documents').append(row);
                ShowTable($("#documents"));
                InitializeDates();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });
    });

    formContent.on('click', '#addContact', function (e) {
        e.preventDefault();
        $.ajax({
            url: path + "/ContractProvider/CreateContact",
            success: function (data, textStatus, jqXHR) {
                var row = $(data);
                var uniqueIdentifier = 'Contacts';
                ApplyUniqueItemIndex(row, addIndex, uniqueIdentifier);
                addIndex++;
                $('#contacts').append(row);
                ShowTable($("#contacts"));
                InitializeDates();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });
    });

    formContent.on('click', '#addAddress', function (e) {
        e.preventDefault();
        $.ajax({
            url: path + "/ContractProvider/CreateAddress",
            success: function (data, textStatus, jqXHR) {
                var row = $(data);
                var uniqueIdentifier = 'Addresses';
                ApplyUniqueItemIndex(row, addIndex, uniqueIdentifier);
                addIndex++;
                $('#addresses').append(row);
                ShowTable($("#addresses"));
                InitializeDates();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });
    });

    formContent.on('click', '#documents .delete-item', function (e) {
        e.preventDefault();
        $(this).closest('tr').remove();
    });

    formContent.on('click', '#addresses .delete-item', function (e) {
        e.preventDefault();
        $(this).closest('tr').remove();
    });

    formContent.on('click', '#contacts .delete-item', function (e) {
        e.preventDefault();
        $(this).closest('tr').remove();
    });

    formContent.on('click', '#clearProvider', function (e) {
        window.location.replace(path + "ContractProvider/Providers");
    });

}