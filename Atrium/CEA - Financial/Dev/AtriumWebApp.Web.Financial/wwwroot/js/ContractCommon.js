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

function ApplyUniqueItemIndex(row, index, uniqueIdentifier) {
    $('td input', row).add('td textarea', row).each(function () {
        var field = $(this).data('field');
        this.id = uniqueIdentifier + '_' + index + '__' + field;
        this.setAttribute('name', uniqueIdentifier + '[' + index + '].' + field);
    });

    $('td select', row).each(function () {
        var field = $(this).data('field');
        var prefix = uniqueIdentifier + '_' + index + '__';

        if (field == 'AddressTypes') {
            var selectedValue = $(this).val();
            $("#" + prefix + "AddressTypeId").val(selectedValue);
        }

        if (field == 'States') {
            var selectedValue = $(this).val();
            $("#" + prefix + "StateId").val(selectedValue);
        }

        if (field == 'ProviderContactTypes') {
            var selectedValue = $(this).val();
            $("#" + prefix + "ProviderContactTypeId").val(selectedValue);
        }

        if (field == 'SelectedRenewal') {
            var selectedValue = $(this).val();
            $("#" + prefix + "ContractRenewalId").val(selectedValue);
        }

        if (field == 'SelectedTerminationNotice') {
            var selectedValue = $(this).val();
            $("#" + prefix + "ContractTerminationNoticeId").val(selectedValue);
        }

        if (field == 'SelectedPaymentTerm') {
            var selectedValue = $(this).val();
            $("#" + prefix + "ContractPaymentTermId").val(selectedValue);
        }

        if (field == 'SelectedCommunity') {
            var selectedValue = $(this).val();
            this.setAttribute('name', uniqueIdentifier + '[' + index + '].' + field);
            $("#" + prefix + "CommunityId").val(selectedValue);
        }
    });
}

function HideTableIfNoRows(tableElement) {
    var rowCount = $("> tbody > tr", tableElement).length;

    if (rowCount == 0)
        tableElement.hide();
}

function ShowTable(tableElement) {
    tableElement.fadeIn(1000);
}
