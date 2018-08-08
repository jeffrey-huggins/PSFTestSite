function SetReadonly(formContent) {
    $('input[type=text], textarea', formContent).prop('readonly', true);
    $('input[type=checkbox], select, button', formContent).prop('disabled', true);
    $('.OrderStatus', formContent).prop('readonly', false);
}

function InitializeDates() {
    $(".isDate").datepicker({
        beforeShow: function (textbox, instance) {
            instance.dpDiv.css({
                marginTop: (-textbox.offsetHeight) + 'px',
                marginLeft: textbox.offsetWidth + 'px'
            });
        }
    });
}

function AdjustPurchaseOrderItem(row, index) {
    $('td input', row).add('td textarea', row).each(function () {
        var field = $(this).data('field');
        this.id = 'Items_' + index + '__' + field;
        this.setAttribute('name', 'Items[' + index + '].' + field);
    });
}

function FormatDate(dotNetSerializedDateString) {
    var d = new Date(parseInt(dotNetSerializedDateString.substr(6)));
    var date = d.getDate();
    var month = d.getMonth() + 1;
    var year = d.getFullYear();
    return month + "/" + date + "/" + year;
}

function SumPurchaseOrderItems(formContent) {
    var sum = 0;
    $('.TotalCost', formContent).each(function () {
        var value = parseFloat(this.innerHTML);
        sum += value;
    });
    return sum;
}

function UpdateOrderTotals(formContent) {
    var subtotal = SumPurchaseOrderItems(formContent);
    var salesTax = $('#EstimatedTaxCost').val();
    if (salesTax == undefined || salesTax == '') {
        salesTax = "0";
    }
    var freight = $('#EstimatedFreightCost').val();
    if (freight == undefined || freight == '') {
        freight = "0";
    }
    var grandTotal = subtotal + parseFloat(salesTax) + parseFloat(freight);
    $('#Subtotal').html(subtotal.toFixed(2));
    $('#EstimatedTotalCost').html(grandTotal.toFixed(2));
}

function UpdateAllowFinalApproval(formContent) {
    var hasPreexistingApprovalElement = $('#HasPreexistingApproval');
    if (hasPreexistingApprovalElement) {
        if (hasPreexistingApprovalElement.val() === 'False') {
            var checked = $('input.Approval:checked', formContent);
            if (checked.length === 0) {
                $('#HasFinalApproval').prop('checked', false);
                $('#HasFinalApproval').prop('disabled', true);
            }
            else {
                $('#HasFinalApproval').prop('disabled', false);
            }
        }
    }
}

function MonitorAllowFinalApproval(formContent) {
    $('.Approval', formContent).change(function (e) {
        UpdateAllowFinalApproval();
    });
    UpdateAllowFinalApproval();
}

function SetupCostControls(formContent) {
    var taxIncludedCheckbox = $('#IsTaxIncluded', formContent);
    var freightIncludedCheckbox = $('#IsFreightIncluded', formContent);
    if (taxIncludedCheckbox.prop('checked')) {
        $('#EstimatedTaxCost', formContent).val('0.00').attr('readonly', 'readonly').prop('readonly', true);
    }
    taxIncludedCheckbox.change(function () {
        if (taxIncludedCheckbox.prop('checked')) {
            $('#EstimatedTaxCost', formContent).val('0.00').attr('readonly', 'readonly').prop('readonly', true);
        }
        else {
            $('#EstimatedTaxCost', formContent).removeAttr('readonly').prop('readonly', false);
        }
    });
    if (freightIncludedCheckbox.prop('checked')) {
        $('#EstimatedFreightCost', formContent).val('0.00').attr('readonly', 'readonly').prop('readonly', true);
    }
    freightIncludedCheckbox.change(function () {
        if (freightIncludedCheckbox.prop('checked')) {
            $('#EstimatedFreightCost', formContent).val('0.00').attr('readonly', 'readonly').prop('readonly', true);
        }
        else {
            $('#EstimatedFreightCost', formContent).removeAttr('readonly').prop('readonly', false);
        }
    });
}

function SetupDropDownLists(formContent) {
    $("#PurchaseOrderTypeId, #PurchaseOrderAssetClassId", formContent).chosen({
        disable_search_threshold: 100,
        width: "120px"
    });

    $("#VendorId", formContent).chosen({
        no_results_text: "no matches found!",
        width: "200px"
    });
}

function CapitalExpenseLevel2(e) {
    var IsCapitalExpense = $('#IsCapitalExpense').prop('checked');
    var isTwo = $('#IsLevel2MarkedForApproval').prop('checked');
    var isThree = $('#IsLevel3MarkedForApproval').prop('checked');
    var isFour = $('#IsLevel4MarkedForApproval').prop('checked');
    if (IsCapitalExpense == true) {
        if (isFour == undefined && isThree == undefined && isTwo != undefined) {
            //$('#IsLevel2MarkedForApproval').prop('checked', false);
            //$('#IsLevel2MarkedForApproval').prop("disabled", true);
            $('#IsMarkedForFinalApproval').prop('checked', false);
            $('#IsMarkedForFinalApproval').prop("disabled", true);
            return true;
        }
        return false;
    } else if (DenyChecked(this) == false) {
        $('#IsLevel2MarkedForApproval').prop("disabled", false);
        $('#IsMarkedForFinalApproval').prop("disabled", false);
        return false;
    }
};

function DenyChecked(e) {
    var isDenial = $('#IsMarkedForDenial').prop('checked');
    if (isDenial != undefined && isDenial == true) {
        $('input#IsMarkedForFinalApproval').prop('checked', false);
        $('input[id$=MarkedForApproval]').prop('checked', false);
        $('input#IsMarkedForFinalApproval').prop("disabled", true);
        $('input[id$=MarkedForApproval]').prop("disabled", true);
        return true;
    } else {
        $('input#IsMarkedForFinalApproval').prop("disabled", false);
        $('input[id$=MarkedForApproval]').prop("disabled", false);
        return false;
    };
};

function RemoveApprovalEvents(e) {
    $('input#IsMarkedForFinalApproval').off();
    $('input[id$=MarkedForApproval]').off();
};

function AddApprovalEvents(e) {
    $('input#IsMarkedForFinalApproval').on('change', function (e) {
        if (CapitalExpenseLevel2(this) == false) {
            ApprovalRules(this);
            NoneApproved(this);
        };
    });
    $('input[id$=MarkedForApproval]').on('change', function (e) {
        if (CapitalExpenseLevel2(this) == false) {
            NoneApproved(this);
        };
    });
};

function NoneApproved(e) {
    var isFinalChecked = $('#IsMarkedForFinalApproval').prop('checked');
    var isTwo = $('#IsLevel2MarkedForApproval').prop('checked');
    var isThree = $('#IsLevel3MarkedForApproval').prop('checked');
    var isFour = $('#IsLevel4MarkedForApproval').prop('checked');
    var isPreviousApproved = $('#HasPreexistingApproval').val();

    //alert("NoneApproved: isFinalChecked=" + isFinalChecked + ", isTwo=" + isTwo + ", isThree=" + isThree + ", isFour=" + isFour);

    if (isFinalChecked == true && (
            (isFour == false || isFour == undefined) &&
            (isThree == false || isThree == undefined) &&
            (isTwo == false || isTwo == undefined) &&
            (isPreviousApproved == false || isPreviousApproved == undefined))
        ) {
        $('#IsMarkedForFinalApproval').prop('checked', false);
    };
};

function ApprovalRules(e) {

    var isFinalChecked = $('#IsMarkedForFinalApproval').prop('checked');
    var fourChecked = $('#IsLevel4MarkedForApproval').prop('checked');
    var threeChecked = $('#IsLevel3MarkedForApproval').prop('checked');
    var twoChecked = $('#IsLevel2MarkedForApproval').prop('checked');

    if (isFinalChecked == true && fourChecked == false) {
        $('#IsLevel4MarkedForApproval').prop('checked', true);
        fourChecked = true;
    }
    else if (isFinalChecked == true && fourChecked == undefined && threeChecked == false) {
        $('#IsLevel3MarkedForApproval').prop('checked', true);
        threeChecked = true;
    }
    else if (isFinalChecked == true &&
        fourChecked == undefined && threeChecked == undefined && twoChecked == false) {
        $('#IsLevel2MarkedForApproval').prop('checked', true);
        twoChecked = true;
    };
    //alert("ApprovalRules: isFinalChecked="+isFinalChecked+", fourChecked=" + fourChecked + ", threeChecked=" + threeChecked + ", twoChecked=" + twoChecked);
};

function PreparePage() {
    var formContent = $('#formContent');
    var table = $('#purchaseOrderTable').dataTable({
        "bAutoWidth": false,
        "sScrollY": "600px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            {
                "sWidth": "40px"
            },
            {
                "sWidth": "40px"
            },
            {
                "sWidth": "50px"
            },
            {
                "sWidth": "50px"
            },
            {
                "sWidth": "50px"
            },
            {
                "sWidth": "50px"
            },
            {
                "sWidth": "30px"
            },
            {
                "sWidth": "20px",
                "bSortable": false
            },
            {
                "sWidth": "30px",
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
        if (confirm("Are you sure you would like to delete this purchase order?")) {
            $.ajax({
                url: element.attr('href'),
                type: 'post',
                success: function (result) {
                    if (result.Success) {
                        var jqRow = element.closest('tr');
                        var row = jqRow.get(0);
                        var rowIndex = table.fnGetPosition(row);
                        table.fnDeleteRow(rowIndex);
                        if (jqRow.attr('id') == $('#PurchaseOrderId').val() || jqRow.attr('id') == $('#PurchaseOrder_PurchaseOrderId').val()) {
                            formContent.hide();
                        }
                    }
                    SetCountDownTime(1800);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Could not communicate with the server.');
                }
            });
        }
    });

    table.on('click', '.edit-link', function (e) {
        var element = $(this);
        e.preventDefault();
        $.ajax({
            url: element.attr('href'),
            success: function (data) {
                formContent.html(data);
                if ($('#HasFinalApproval', formContent).prop('checked')) {
                    SetReadonly(formContent);
                }
                InitializeDates();
                MonitorAllowFinalApproval(formContent);
                SetupCostControls(formContent);
                SetupDropDownLists(formContent);
                formContent.show();
                $('.ItemDescription', formContent).autogrow({
                    onInitialize: true,
                    fixMinHeight: false
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    table.on('click', '.view-link', function (e) {
        var element = $(this);
        e.preventDefault();
        $.ajax({
            url: element.attr('href'),
            success: function (data) {
                formContent.html(data);
                formContent.show();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    $('#create').click(function (e) {
        var element = $(this);
        e.preventDefault();
        $.ajax({
            url: element.attr('href') + '/' + $("#CurrentCommunity").val(),
            success: function (data) {
                formContent.html(data);
                InitializeDates();
                MonitorAllowFinalApproval(formContent);
                SetupCostControls(formContent);
                SetupDropDownLists(formContent);
                formContent.show();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Could not communicate with the server.');
            }
        });
    });

    var addIndex = 0;

    formContent.on('change', '#IsSpecialProject', function (e) {
        if($('#IsSpecialProject').prop('checked') == true)
        {
            $('#IsEmergency').prop('checked', false);
        }
    });
    formContent.on('change', '#IsEmergency', function (e) {
        if ($('#IsEmergency').prop('checked') == true) {
            $('#IsSpecialProject').prop('checked', false);
        }
    });

    formContent.on('change', '#IsPurchaseOrder', function (e) {
        if ($('#IsPurchaseOrder').prop('checked') == true) {
            $('#IsCapitalExpense').prop('checked', false);
        }
    });
    formContent.on('change', '#IsCapitalExpense', function (e) {
        if ($('#IsCapitalExpense').prop('checked') == true) {
            $('#IsPurchaseOrder').prop('checked', false);
        }
    });

    formContent.on('click', '#purchaseItems .delete-item', function (e) {
        e.preventDefault();
        $(this).closest('tr').remove();
        UpdateOrderTotals(formContent);
    });

    formContent.on('click', '#addItem', function (e) {
        e.preventDefault();
        $.ajax({
            url: path + "PurchaseOrder/CreatePurchaseOrderItem",
            success: function (data, textStatus, jqXHR) {
                var row = $(data);
                AdjustPurchaseOrderItem(row, addIndex);
                addIndex++;
                $('#purchaseItems').append(row);
                $('.ItemDescription', formContent).autogrow({
                    onInitialize: true,
                    fixMinHeight: false
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });
    });

    formContent.on('change', '.OrderQuantity, .EstimatedItemCost', function (e) {
        var element = $(this);
        var row = element.closest('tr');
        var isEstimatedItemCostField = this.className.indexOf("EstimatedItemCost") != -1;
        cost = $(".EstimatedItemCost", row).val();

        var quantity = $('.OrderQuantity', row).val();
        var totalCost = (quantity * cost).toFixed(2);
        $('.TotalCost', row).html(totalCost);
        UpdateOrderTotals();
    });

    formContent.on('change', '#EstimatedTaxCost, #EstimatedFreightCost', function (e) {
        UpdateOrderTotals();
    });

    formContent.on('click', '#clearPurchaseOrder', function (e) {
        e.preventDefault();
        $('input[type=text], textarea, select', formContent).val("");
        $('input[type=checkbox]', formContent).prop('checked', false);
        $('.calculated', formContent).each(function () {
            $(this).html("0.00");
        });
        $("#SpanPurchaseOrderId").text("0");
        $("#VendorId").trigger("chosen:updated");
        $("#VendorAddress").text("");
        $("#VendorEmail").text("");
        $("#VendorFax").text("");
        $("#VendorClass").text("");
        $("#PurchaseOrderTypeId").trigger("chosen:updated");
        $("#PurchaseOrderAssetClassId").trigger("chosen:updated");
        $('#purchaseItems tbody tr').remove();
        $('#purchaseOrderDocuments tbody tr').remove();
        UpdateOrderTotals();
        UpdateAllowFinalApproval();
    });

    formContent.on('change', '#IsMarkedForDenial', function (e) {
        if (this.checked) {
            $('#denialComments').show();
        }
        else {
            $('#denialComments').hide();
        }
    });

    formContent.on('click', '#submitPurchaseOrder', function (e) {
        e.preventDefault();

        $('#purchaseItems tr').each(function (index) {
            AdjustPurchaseOrderItem($(this), index - 1);
        });
        $('#purchaseOrderDocuments tbody tr').each(function (index) {
            docIndex = index;
            AdjustPurchaseOrderDocument($(this));
        });

        var form = $("#SavePurchaseOrderItem", formContent);
        var formHtml = formContent.html();

        form.ajaxSubmit({
            target: '#SavePurchaseOrderItem',
            success: function (data, textStatus, jqXHR, $form) {
                try {
                    var result = $.parseJSON(data);
                    if (result.Success) {
                        window.location.replace(path + "PurchaseOrder/");
                        formContent.hide();
                    }
                    else {
                        formContent.html(formHtml);
                        alert('Failure communicating with the server.');
                    }
                }
                catch (error) {
                    form.html(data);
                    InitializeDates();
                    MonitorAllowFinalApproval(formContent);
                    SetupCostControls(formContent);
                    SetupDropDownLists(formContent);
                    $('.ItemDescription', formContent).autogrow({
                        onInitialize: true,
                        fixMinHeight: false
                    });
                    SetCountDownTime(1800);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });

        return false;
    });

    if ($("#CensusDateInvalid").val() == "1") {
        alert("Error: Please enter a valid Last Census Date (mm/dd/yyyy)");
    }
    if ($("#CensusDateInFuture").val() == "1") {
        alert("Error: You can not have a Census date that is in the future");
    }
    $("#CurrentCommunity").change(function () {
        var community = $('#CurrentCommunity').val();
        if (community != '') {
            $('#CommunitySelectionForm').submit();
        }
    });
    if ($("#ToDateRangeInvalid").val() == "1") {
        alert('Error: Please enter a valid date in the Infection Date Range "To" Field (mm/dd/yyyy)');
    }
    if ($("#FromDateRangeInvalid").val() == "1") {
        alert('Error: Please enter a valid date in the Infection Date Range "From" Field (mm/dd/yyyy)');
    }
    if ($("#ToDateRangeInFuture").val() == "1") {
        alert('Error: Please enter a valid date that is not in the future in the Infection Date Range "To" Field');
    }
    if ($("#FromDateRangeInFuture").val() == "1") {
        alert('Error: Please enter a valid date that is not in the future in the Infection Date Range "From" Field');
    }
    if ($("#FromAfterTo").val() == "1") {
        alert('Error: You can not have the "From" Date after the "To" Date in the Infection Date Range Fields');
    }

    InitializeDates();
    $(".ItemDescription", formContent).autogrow({
        onInitialize: true,
        fixMinHeight: false
    });

    SetupCostControls(formContent);
    SetupDropDownLists(formContent);
    MonitorAllowFinalApproval(formContent);

    formContent.on("change", "#VendorId", function (e) {
        $("#VendorAddress").text("");
        $("#VendorEmail").text("");
        $("#VendorFax").text("");
        $("#VendorClass").text("");
        var vendorId = $(this).val();

        if (vendorId != "") {
            $.ajax({
                url: path + "PurchaseOrderAdmin/GetVendor",
                dataType: "json",
                cache: false,
                type: "GET",
                data: { vendorId: vendorId },
                success: function (result) {
                    $("#VendorAddress").html(result.FullAddressRaw);

                    if (result.Email !== null && result.Email !== undefined)
                        $("#VendorEmail").text(result.Email);

                    if (result.Fax !== null && result.Fax !== undefined)
                        $("#VendorFax").text(result.Fax);

                    if (result.VendorClass !== null && result.VendorClass !== undefined)
                        $("#VendorClass").text(result.VendorClass.Name);
                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
    });

    var docIndex = 0;

    function AdjustPurchaseOrderDocument(row) {
        $('td input', row).add('td textarea', row).each(function () {
            var field = $(this).data('field');
            this.id = 'Documents_' + docIndex + '__' + field;
            this.setAttribute('name', 'Documents[' + docIndex + '].' + field);
        });
        docIndex++;
    }

    formContent.on('click', '#purchaseOrderDocuments .delete-item', function (e) {
        e.preventDefault();
        if (confirm("Do you want to delete this document from the purchase order?")) {
            $(this).closest('tr').remove();
        }
    });

    formContent.on('click', '#purchaseOrderDocuments .delete-item-new', function (e) {
        e.preventDefault();
        $(this).closest('tr').remove();
    });

    formContent.on('click', '#addDocument', function (e) {
        e.preventDefault();
        $.ajax({
            url: path + "PurchaseOrder/CreatePurchaseOrderDocument",
            success: function (data) {
                var row = $(data);
                AdjustPurchaseOrderDocument(row);
                $('#purchaseOrderDocuments').append(row);

                formContent.off("change", "input:file");
                formContent.on("change", "input:file", function (e) {
                    var control = $(this);
                    var id = $(control).attr("id");
                    var filename = $(control).val();

                    $("#purchaseOrderDocuments a").each(function () {
                        if ($(this).text() == filename.substring(filename.lastIndexOf("\\") + 1)) {
                            alert("Another file with the same name has already been uploaded");
                            control.replaceWith(control = control.clone(true));
                        }
                    });
                    $("#purchaseOrderDocuments input:file").each(function () {
                        if ($(this).attr("id") != id && $(this).val() == filename) {
                            alert("Another file with the same name has already been selected");
                            control.replaceWith(control = control.clone(true));
                        }
                    });
                });

                $("button.delete-item", row).attr("class", "delete-item-new")
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });
    });
}