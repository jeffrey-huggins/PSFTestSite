function ArchiveFlg() {
    //alert("Enter #ArchiveFlg");
    var isChecked = $('input#ArchiveFlg').prop('checked');
    if (isChecked == true) {
        //alert('clicked input[type=checkbox]#ArchiveFlg: COMMUNITY ROW Archive is set to true ..');
        // Check ALL
        $('input#ArchiveFlg').prop('checked', true);
        $('input#AllArchiveFlg').prop('checked', true);
        $('input[id^=ContractCommunities_][id$=__ArchiveFlg]').prop('checked', true);
        $('input[id^=Documents_][id$=__ArchiveFlg]').prop('checked', true);
    };

    // NOTE: if disabled this will NOT SAVE to DB!
    //// Toggle Enable/Disable ALL
    //$('input#AllArchiveFlg').prop("disabled", isChecked);
    //$('input[id^=ContractCommunities_][id$=__ArchiveFlg]').prop("disabled", isChecked);
    //$('input[id^=Documents_][id$=__ArchiveFlg]').prop("disabled", isChecked);
};

function PreparePage() {
    var addIndex = 0;
    var formContent = $('#formContent');
    var isChoosing = false;

    // Provider
    $("body").on("change", 'input#ArchiveFlg', function (e) {
        ArchiveFlg();
    });
    // Checks all Communities
    $("body").on("change", 'input#AllArchiveFlg', function (e) {
        //alert("enter #AllArchiveFlg");
        if ($(this).prop('checked') == true) {
            $('input[id^=ContractCommunities_][id$=__ArchiveFlg]').prop('checked', true);
        };
    });
    // Community Row
    $("body").on("change", 'input[type=checkbox][id^=ContractCommunities_][id$=__ArchiveFlg]', function (e) {
        //alert("Enter ContractCommunities_[#]__ArchiveFlg");
        e.preventDefault();
        if ($(this).prop('checked') == false) {
            //alert('clicked input[type=checkbox][id$=__ArchiveFlg: PROVIDER Archive is set to false ..');
            $('input#ArchiveFlg').prop('checked', false);
            $('input#AllArchiveFlg').prop('checked', false);
            $(this).prop('checked', false);
        };
    });
    // Document Row
    $("body").on("change", 'input[id^=Documents_][id$=__ArchiveFlg]', function (e) {
        //alert("enter Documents_[#]__ArchiveFlg");
        if ($(this).prop('checked') == false) {
            $('input#ArchiveFlg').prop('checked', false);
        };
        //$(this).prop('checked', false);
    });

    function prepContractForm() {
        var options = {
            target: '#contractEditForm',
            success: function (data, textStatus, jqXHR, $form) {
                try {
                    var result = data;

                    if (result.Success) {
                        window.location.replace(path + "ContractManagement/Contracts");
                        $('#contractEditForm').hide();
                    }
                } catch (error) { }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        }

        $('#contractEditForm').submit(function () {
            $(this).ajaxSubmit(options);
            return false;
        });
    }

    var contractsTable = $('#contractsTable').dataTable({
        "bAutoWidth": false,
        "sScrollY": "600px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aaSorting": [
            [0, "desc"]
        ],
        "aoColumns": [
            {
                "sWidth": "180px"
            },
            {
                "sWidth": "20px"
            },
            {
                "sWidth": "120px",
                "aDataSort": [2, 3]
            },
            {
                "sWidth": "150px",
                "aDataSort": [3, 2]
            },
            {
                "sWidth": "0px",
                "bSortable": false
            },
            {
                "sWidth": "0px",
                "bSortable": false
            },
            {
                "sWidth": "30px",
                "bSortable": false
            },
            {
                "sWidth": "30px"
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
        //"columnDefs": [
        //    {
        //    "targets": [ 2 ],
        //    "orderData": [ 2, 3 ]
        //}, {
        //    "targets": [ 3 ],
        //    "orderData": [ 3, 2 ]
        //} ],
        "oLanguage": {
            "sEmptyTable": "No records within this range."
        }
    });

    $('#createContract').click(function (e) {
        var element = $(this);
        e.preventDefault();
        $.ajax({
            url: element.attr('href'),
            success: function (data) {
                formContent.html(data);
                InitializeDates();
                formContent.show();

                prepContractForm();
                UpdateSelectedCommunties(data);

                HideTableIfNoRows($("#contractDocuments"));
                HideTableIfNoRows($("#contractCommunities"));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    $("#CommunitySelections").on("chosen:showing_dropdown", function (evt, params, chosen) {
        isChoosing = true;
    });

    $("#CommunitySelections").on("chosen:hiding_dropdown", function (evt, params, chosen) {
        isChoosing = false;
        $("#CommunitySelections").trigger("change");
    });

    $("#CommunitySelections").on("change", function (evt, params, chosen) {
        if (!isChoosing) {
            evt.preventDefault();
            var selectedCommunities = "";
            $("select option:selected").each(function () {
                selectedCommunities += $(this).val() + " ";
            });

            if (selectedCommunities != '')
                $("#CommunitySelectionForm").submit();
        }
    });

    formContent.on('change', '#ContractCategoryId', function (e) {
        e.preventDefault();
        $.ajax({
            url: path + "/ContractManagement/GetSubCategories",
            type: 'POST',
            data: { selectedCategoryId: $(this).val() },
            datatype: 'json',
            success: function (data, textStatus, jqXHR) {
                var elements = "";
                $.each(data, function () {
                    elements = elements + '<option value="' + this.Id + '">' + this.Name + '</option>'
                });

                $("#ContractSubCategoryId").empty().append(elements);
            }
        });

        var selectedId = $(this).val();

        SelectedCategoryIsProvider(selectedId, function (result) {
            result ? ShowCredentiallingColumns() : HideCredentiallingColumns();
        });
    });

    formContent.on('click', '#contractDocuments .delete-item', function (e) {
        e.preventDefault();
        $(this).closest('tr').remove();
    });

    formContent.on('click', '#contractCommunities .delete-item', function (e) {
        e.preventDefault();
        $(this).closest('tr').remove();
    });

    formContent.on('click', '#addContractDocument', function (e) {
        e.preventDefault();
        $.ajax({
            url: path + "/ContractManagement/CreateContractDocument",
            success: function (data, textStatus, jqXHR) {
                var row = $(data);
                var uniqueIdentifier = 'Documents';
                ApplyUniqueItemIndex(row, addIndex, uniqueIdentifier);
                addIndex++;

                var documentTable = $('#contractDocuments');
                $('#contractDocuments').append(row);
                InitializeDates();
                ShowTable($("#contractDocuments"));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });
    });

    formContent.on('click', '#addContractCommunity', function (e) {
        e.preventDefault();
        $.ajax({
            url: path + "/ContractManagement/CreateContractCommunity",
            cache: false,
            success: function (data, textStatus, jqXHR) {
                var row = $(data);
                var listOfSelectedValues = [];
                $(".SelectedCommunity :selected").each(function (index, element) { listOfSelectedValues[index] = $(element).val(); });
                var documentTable = $('#contractCommunities');
                $('#contractCommunities').append(row);

                var uniqueIdentifier = 'ContractCommunity';
                ApplyUniqueItemIndex(row, addIndex, uniqueIdentifier);
                addIndex++;
                InitializeDates();


                ShowTable($("#contractCommunities"));

                var communityElementOfNewRow = $('.SelectedCommunity', row);
                var allValues = [];
                $('.SelectedCommunity option', row).each(function (index, element) { allValues[index] = $(element).val(); });
                var openValues = $(allValues).not(listOfSelectedValues);

                communityElementOfNewRow.val(openValues.first());

                setCredentiallingColumnVisibility();
                disableSelectedContractCommunities();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });
    });

    formContent.on('change', '.SelectedCommunity', function (e) {
        disableSelectedContractCommunities();
    });

    formContent.on('change', '#contractCommunities .columnValue', function (e) {
        var element = $(this);
        var fieldValue = element.val();
        var fieldSet = element.data('field');
        var parentTable = element.closest('table');
        var columnElements = $('#contractCommunities tbody [data-field="' + fieldSet + '"]');

        columnElements.each(function (index, anElement) {
            var element = $(anElement);
            if (!element.val() || element.val() == -1) {
                element.val(fieldValue);
            }
        });
    });

    formContent.on('click', '#submitContract', function (e) {
        e.preventDefault();
        var element = $(this);
        var form = element.closest('form');
        var url = form.attr('action');

        $("#ErrorResult").empty();

        var uniqueIdentifier = 'Documents';
        $('#contractDocuments tbody tr').each(function (index) {
            ApplyUniqueItemIndex($(this), index, uniqueIdentifier);
        });

        var uniqueIdentifier = 'ContractCommunities';
        var communitiesAreValid = true;
        $('#contractCommunities tbody tr').each(function (index, element) {
            ApplyUniqueItemIndex($(this), index, uniqueIdentifier);
        });

        if (!AllCommunitiesAreValid()) {
            $("#ErrorResult").append('<li>Payment Term, Termination Notice, and Renewal must have a value selected for each community.</li>');
            return false;
        }

        var options = {
            target: '#contractEditForm',
            success: function (data, textStatus, jqXHR, $form) {
                try {
                    if (data.indexOf("<pre>") == 0) {
                        data = data.substring(5, data.length - 6);
                    }
                    var result = $.parseJSON(data);

                    if (result.Success) {
                        window.location.replace(path + "ContractManagement/Contracts");
                    }
                    else {
                        form.html(result);
                    }
                } catch (error) { form.html(data); }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        }
        $(form).ajaxSubmit(options);
    });

    function AllCommunitiesAreValid() {
        var isValid = true;
        $('#contractCommunities tbody tr').each(function (index, element) {

            var renewalValue = $("[data-field='ContractRenewalId']", element).val();
            var noticeValue = $("[data-field='ContractTerminationNoticeId']", element).val();
            var paymentValue = $("[data-field='ContractPaymentTermId']", element).val();

            var isRowValid = renewalValue > 0 && noticeValue > 0 && paymentValue > 0;

            isValid = isValid && isRowValid;
        });
        return isValid;
    }

    function contractCommunityIsValid(row) {
        var renewalIndex = $("select[name='ContractRenewalId'] option:selected", row).index();
        var noticeIndex = $("select[name='ContractTerminationNoticeId'] option:selected", row).index();
        var paymentIndex = $("select[name='ContractPaymentTermId'] option:selected", row).index();
        return renewalIndex != 0 && noticeIndex != 0 && paymentIndex != 0;
    }

    formContent.on('click', '#clearContract', function (e) {
        window.location.replace(path + "ContractManagement/Contracts");
    });

    contractsTable.on('click', '.deleteContract', function (e) {
        var element = $(this);
        e.preventDefault();
        if (confirm("Are you sure you would like to delete this Contract?")) {
            $.ajax({
                url: element.attr('href'),
                type: 'post',
                success: function (result) {
                    if (result.Success) {
                        var jqRow = element.closest('tr');
                        var row = jqRow.get(0);
                        var rowIndex = contractsTable.fnGetPosition(row);
                        contractsTable.fnDeleteRow(rowIndex);
                    }
                    SetCountDownTime(1800);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Could not communicate with the server.');
                }
            });
        }
    });

    contractsTable.on('click', '.editContract', function (e) {
        var element = $(this);
        e.preventDefault();
        $.ajax({
            url: element.attr('href'),
            success: function (data) {
                formContent.html(data);
                InitializeDates();
                formContent.show();

                prepContractForm();
                UpdateSelectedCommunties(data);
                disableSelectedContractCommunities();
                HideTableIfNoRows($("#contractDocuments"));

                setCredentiallingColumnVisibility();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    contractsTable.on('click', '.viewContract', function (e) {
        var element = $(this);
        e.preventDefault();
        $.ajax({
            url: element.attr('href'),
            success: function (data) {
                formContent.html(data);
                formContent.show();
                UpdateSelectedCommunties(data);
                HideTableIfNoRows($("#contractDocuments"));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    function SelectedCategoryIsProvider(selectedId, callback) {
        $.ajax({
            url: path + "/ContractManagement/IsProvider",
            type: 'POST',
            data: { selectedCategoryId: selectedId },
            datatype: 'json',
            success: callback
        });
    }

    function HideCredentiallingColumn() {
        var communityTable = $('#contractCommunities');
        $('td:nth-child(7),th:nth-child(7)', communityTable).hide();
    }

    function HideCredentiallingCell(row) {

        $('td:nth-child(7),th:nth-child(7)', row).hide();
    }

    function ShowCredentiallingColumn() {
        var communityTable = $('#contractCommunities');
        $('td:nth-child(7),th:nth-child(7)', communityTable).fadeIn(1000);
    }

    function HideReCredentiallingColumn() {
        var communityTable = $('#contractCommunities');
        $('td:nth-child(8),th:nth-child(8)', communityTable).hide();
    }

    function HideReCredentiallingCell(row) {

        $('td:nth-child(8),th:nth-child(8)', row).hide();
    }

    function ShowReCredentiallingColumn() {
        var communityTable = $('#contractCommunities');
        $('td:nth-child(8),th:nth-child(8)', communityTable).fadeIn(1000);
    }

    function HideCredentiallingColumns() {
        HideCredentiallingColumn();
        HideReCredentiallingColumn();
    }

    function ShowCredentiallingColumns() {
        ShowCredentiallingColumn();
        ShowReCredentiallingColumn();
    }

    function UpdateSelectedCommunties(viewModel) {

        //Need to init the control
        //Chosen configuration (http://harvesthq.github.io/chosen/options.html) 
        $('.chosen-contract-select').chosen({
            placeholder_text_multiple: "Select Communities"
        });

        $('#CommunitySearchArea').hide(0);
        $('#ContractsGrid').hide(0);
    }

    function disableSelectedContractCommunities() {
        enableAllDropDownListItems();

        var allLists = $('.SelectedCommunity');
        allLists.each(function (itemIndex, selectList) {
            disableAllSelectedItems(selectList);
        });
    }

    function disableAllSelectedItems(selectList) {
        var selectedItems = $(".SelectedCommunity :selected");// option[selected='selected']");

        selectedItems.each(function (index, element) {
            var selectedValue = $(element).val();
            var parent = $(element).parent();
            if (parent.is(selectList)) {
                return true;
            }

            $("option[value='" + selectedValue + "']", selectList).attr('disabled', 'disabled');
        });
    }

    function enableAllDropDownListItems() {
        $(".SelectedCommunity option[disabled='disabled']").removeAttr('disabled');
    }

    function setCredentiallingColumnVisibility() {
        var selectedCategoryId = $('#ContractCategoryId').val();
        SelectedCategoryIsProvider(selectedCategoryId, function (result) {
            result ? ShowCredentiallingColumns() : HideCredentiallingColumns();
        });
    }

}