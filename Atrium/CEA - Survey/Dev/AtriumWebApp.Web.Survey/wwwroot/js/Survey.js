Number.prototype.formatMoney = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

$(document).ready(function () {
    var addIndex = 0;
    var formContent = $('#survey');

    var oTableCitations = $('#CitationsTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "257px",
        "sDom": "rtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            {
                "bVisible": false
            },
            null,
            {
                "bVisible": false
            },
            null,
            {
                "bVisible": false
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '40px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '40px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No citations for the survey"
        }
    });

    $("#CitationsTable").on('click','a.edit', function (e) {
        e.preventDefault();
        var nRowId = $(this).closest('tr').attr("id");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableCitations.fnGetData(nRow);
        $("#EditingId").val(nRowId);
        if (aData[0] == "Federal") {
            $("#NewFederal").click();
            $("#FedDef").val(aData[2]);
            $("#SAS").val(aData[4]);
            $("#Comments").val(aData[6]);
        }
        else if (aData[0] == "State") {
            $("#NewState").click();
            $("#StateDef").val(aData[2]);
            $("#SASNotRequired").val(aData[4]);
            $("#Comments").val(aData[6]);
        }
        else if (aData[0] == "Safety") {
            $("#NewSafety").click();
            $("#SafetyDef").val(aData[2]);
            $("#SASNotRequired").val(aData[4]);
            $("#Waiver").prop("checked", aData[5] == "Yes");
            $("#Comments").val(aData[6]);
        }
    });

    function PreparePayerGroupControls() {
        var firstColumns = $('tr td:first-child', oTableCitations);
        var hasFederal = false;
        firstColumns.each(function (index, element) {
            hasFederal = hasFederal || element.innerHTML == "Federal";
        });
        if (hasFederal) {
            $('.payer-group').hide();
            $('.payer-group-readonly').show();
        }
        else {
            $('.payer-group').show();
            $('.payer-group-readonly').hide();
        }
    }

    $("#CitationsTable a.delete").on('click', function (e) {
        e.preventDefault();
        if (!confirm("Are you sure you want to delete?")) {
            return;
        }
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableCitations.fnGetData(nRow);
        $.post(path + "Survey/DeleteCitation", { citationId: nRowId, citationType: aData[0] }).done(function () {
            oTableCitations.fnDeleteRow(nRow);
            PreparePayerGroupControls();
            
        });
    });

    $('#CurrentSurvey_AtriumPayerGroupCode').change(function () {
        var jqThis = $(this);
        var value = jqThis.val();
        var optionsHtml = $('#FedDefData' + value).html();
        $('#FedDef').html(optionsHtml);
        $('#ReadonlyPayerGroup').text($('option:selected', jqThis).text());
    });

    var oTableCMP = $('#CMPTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "rtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            null,
            null,
            {
                "bVisible": false
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '40px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '40px'
            },
            {
                "bVisible": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No CMPs for the survey"
        }
    });
    var nEditingCMP = null;
    oTableCMP.fnSort([[0, 'desc']]);

    $("#CMPTable").on('click','a.edit', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableCMP.fnGetData(nRow);
        $("#CMPEditingId").val(nRowId);
        if (aData[0] == "Yes") {
            $("#DateCMPFrom").val(aData[2]);
            $("#DateCMPTo").val(aData[3]);
            DailyShow();
        }
        if (aData[1] == "Yes") {
            $("#DateInstance").val(aData[2]);
            InstanceShow();
        }
        $("#CMPAmount").val(aData[5]);
        $("#Daily").prop("checked", aData[0] == "Yes");
        $("#Instance").prop("checked", aData[1] == "Yes");
        $("#Discount").prop("checked", aData[8] == "Yes");
    });

    $("#CMPTable a.delete").on('click', function (e) {
        e.preventDefault();
        if (!confirm("Are you sure you want to delete?")) {
            return;
        }
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.post(path + "Survey/DeleteCMP", { cmpId: nRowId }).done(function () {
            oTableCMP.fnDeleteRow(nRow);
            $("#CMPAmountYTD").val("Refresh page to update");
            if ($("#CMPEditingId").val() == nRowId) {
                $("#clearCMP").click();
            }
            
        });
    });

    var oTableSurvey = $('#SurveyTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "rtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false,
            },
            {
                "bSearchable": false,
                "bSortable": false,
            },
            {
                "bSearchable": false,
                "bSortable": false,
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Surveys for this time period"
        }
    });
    var nEditingSurvey = null;
    oTableSurvey.fnSort([[1, 'asc']]);

    $('#SurveyTable').on('click','a.edit', function (e) {
        e.preventDefault();
        var nRowId = $(this).closest('tr').attr("id");
        var ids = nRowId.split(' ');
        ShowProgress();
        $.post(path + "Survey/ChangeSurveyId", { cycleId: ids[0], surveyId: ids[1] }).done(function () {
            window.location.reload();
        });
    });

    $("#SurveyTable a.delete").on('click', function (e) {
        e.preventDefault();
        if (!confirm("Are you sure you want to delete?")) {
            return;
        }
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var ids = nRowId.split(' ');
        $.post(path + "Survey/DeleteSurvey", { cycleId: ids[0], surveyId: ids[1] }).done(function () {
            oTableSurvey.fnDeleteRow(nRow);
            if ($("#CurrentSurvey_SurveyCycleId").val() == ids[0] && $("#CurrentSurvey_SurveyId").val() == ids[1]) {
                $.post(path + "Survey/ClearSurvey").done(function () {
                    window.location.reload();
                });
            }
        });
    });

    $(function () {
        $("#DateEntry").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#Date2567").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#DateDOP").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#DateExit").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#DateCertain").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#DateTerm").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#DateFollowUp").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#DateDOPStart").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: -textbox.offsetWidth - 178 + 'px'
                });
            }
        });
        $("#DateDOPEnd").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: -textbox.offsetWidth - 178 + 'px'
                });
            }
        });
        $("#DateCMPFrom").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#DateInstance").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#DateCMPTo").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
        $("#occurredRangeTo").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: '5px',
                    marginLeft: '0px'
                });
            }
        });
        $("#occurredRangeFrom").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: '5px',
                    marginLeft: '0px'
                });
            }
        });
    });
    $(function () {
        if ($("#ToDateRangeInvalid").val() == "1") {
            alert('Error: Please enter a valid date in the Incident Date Range "To" Field (mm/dd/yyyy)');
        }
        if ($("#FromDateRangeInvalid").val() == "1") {
            alert('Error: Please enter a valid date in the Incident Date Range "From" Field (mm/dd/yyyy)');
        }
        if ($("#ToDateRangeInFuture").val() == "1") {
            alert('Error: Please enter a valid date that is not in the future in the Incident Date Range "To" Field');
        }
        if ($("#FromDateRangeInFuture").val() == "1") {
            alert('Error: Please enter a valid date that is not in the future in the Incident Date Range "From" Field');
        }
        if ($("#FromAfterTo").val() == "1") {
            alert('Error: You can not have the "From" Date after the "To" Date in the Incident Date Range Fields');
        }
    });
    //Clear Survey
    $("#clearSurvey").click(function () {
        $.post(path + "Survey/ClearSurvey").done(function () {
            window.location.reload();
        });
    });

    //Create Follow Up
    $(".followup").on('click', function (e) {
        e.preventDefault();
        var nRowId = $(this).closest('tr').attr("id");
        var ids = nRowId.split(' ');
        $.post(path + "Survey/CreateFollowUp", { cycleId: ids[0], surveyId: ids[1] }).done(function () {
            window.location.reload();
        });
    });
    $("#CurrentSurvey_DidNotClearFollowUpFlg").click(function () {
        if (!$(this).is(":checked")) {
            return;
        }
        if (!confirm("Would you like to save your current form and create a follow up?")) {
            return;
        }
        $("#CreateFollowUp").val("true");
        $("#submitSurvey").click();
    });
    //Manual Validations [ID, Display Name, isRequired]
    var datesToValidate = [["DateEntry", "Entry Date", true], ["DateExit", "Exit Date", true],
        ["Date2567", "2567 Received", false], ["DateCertain", "Date Certain", false], ["DateFollowUp", "Follow Up Date", false],
        ["DateDOP", "Potential DOP Date", false], ["DateTerm", "Potential Term Date", false], ["DateDOPStart", "DOP Start Date", false],
        ["DateDOPEnd", "DOP End Date", false]];
    var currencyToValidate = [["CurrentSurvey_DOPDailyAmount", "DOP Daily Amount", false], ["CurrentSurvey_StateFineAmount", "State Fine Amount", false]];
    $("#submitSurvey").click(function () {
        var requiredMessage = " is a required field.";
        var invalidMessage = " must be in the format mm/dd/yyyy.";
        var invalidCurrencyMessage = " must be in the format X.XX";
        var submit = true;
        for (var i = 0; i < datesToValidate.length; i++) {
            var date = $("#" + datesToValidate[i][0]).val();
            if (datesToValidate[i][2] && date == "") {
                alert(datesToValidate[i][1] + requiredMessage);
                submit = false;
            }
            if (isInvalidDate(date)) {
                alert(datesToValidate[i][1] + invalidMessage);
                submit = false;
            }
        }
        for (var i = 0; i < currencyToValidate.length; i++) {
            var currency = $("#" + currencyToValidate[i][0]).val();
            if (currencyToValidate[i][2] && currency == "") {
                alert(currencyToValidate[i][1] + requiredMessage);
                submit = false;
            }
            if (isInvalidCurrency(currency)) {
                alert(currencyToValidate[i][1] + invalidCurrencyMessage);
                submit = false;
            }
        }

        var uniqueIdentifier = 'Documents';
        $('#surveyDocuments tbody tr').each(function (index) {
            ApplyUniqueItemIndex($(this), index, uniqueIdentifier);
        });

        if (submit) {
            var survey = $("form#SaveMainSurvey");
            $.ajax({
                url: $(survey).attr('action'),
                type: $(survey).attr('method'),
                data: $(survey).serialize()
            }).done(function (data) {
                $("form#SaveDocumentsSurvey").submit();
            });
        }
    });


    var CMPDatesToValidate = [["DateCMPFrom", "From Date", true], ["DateCMPTo", "To Date", false], ["DateInstance", "Instance Date", true]];
    var CMPCurrencyToValidate = [["CMPAmount", "Amount", true]];
    $("#submitCMP").click(function () {
        var cycleId = $("#CurrentSurvey_SurveyCycleId").val();
        if (cycleId == "") {
            alert("Cannot save a CMP until a survey has been opened or saved.");
            return;
        }
        var requiredMessage = " is a required field.";
        var invalidMessage = " must be in the format mm/dd/yyyy.";
        var invalidCurrencyMessage = " must be in the format X.XX";
        var submit = true;
        for (var i = 0; i < CMPDatesToValidate.length; i++) {
            var box = $("#" + CMPDatesToValidate[i][0]);
            var date = $("#" + CMPDatesToValidate[i][0]).val();
            if (CMPDatesToValidate[i][2] && date == "" && !box.hasClass("hidden")) {
                alert(CMPDatesToValidate[i][1] + requiredMessage);
                submit = false;
            }
            if (isInvalidDate(date)) {
                alert(CMPDatesToValidate[i][1] + invalidMessage);
                submit = false;
            }
        }
        for (var i = 0; i < CMPCurrencyToValidate.length; i++) {
            var currency = $("#" + CMPCurrencyToValidate[i][0]).val();
            if (CMPCurrencyToValidate[i][2] && currency == "") {
                alert(CMPCurrencyToValidate[i][1] + requiredMessage);
                submit = false;
            }
            if (isInvalidCurrency(currency)) {
                alert(CMPCurrencyToValidate[i][1] + invalidCurrencyMessage);
                submit = false;
            }
        }
        if (submit) {
            var form = $("#SaveCMP");
            var data = form.serialize();
            var method = form.attr('method');
            var url = form.attr('action');
            $.ajax({
                url: url,
                data: data,
                type: method,
                success: function (data) {
                    if (data.Success) {
                        var rowData = [
                            $('#Daily').prop('checked') ? 'Yes' : 'No',
                            $('#Instance').prop('checked') ? 'Yes' : 'No',
                            $('#Instance').prop('checked') ? $('#DateInstance').val() : $('#DateCMPFrom').val(),
                            $('#Instance').prop('checked') ? '' : $('#DateCMPTo').val(),
                            Number($('#CMPAmount').val()).formatMoney(2),
                            $('#CMPAmount').val(),
                            '<a href="" class="edit">Edit</a>',
                            '<a href="" class="delete">Delete</a>',
                            $('#Discount').prop('checked') ? 'Yes' : 'No'
                        ];
                        if ($('#CMPEditingId').val()) {
                            oTableCMP.fnUpdate(rowData, $('#' + data.Id).get(0));
                        }
                        else {
                            var added = oTableCMP.fnAddData(rowData);
                            var row = oTableCMP.fnGetNodes(added);
                            $(row).attr('id', data.Id);
                        }
                        $("#clearCMP").click();
                        
                    }
                    else {
                        alert('There was an unexpected error saving the record.  If the happens repeatedly, contact support.');
                    }
                },
                error: function () {
                    alert('Error communicating with the server.  If this happens repeatedly, contact support.');
                }
            });
        }
    });
    $("#clearCMP").click(function () {
        $("#CMPEditingId").val("");
        $("#DateCMPFrom").val("");
        $("#DateCMPTo").val("");
        $("#DateInstance").val("");
        $("#CMPAmount").val("");
        $("#Daily").prop("checked", true);
        $("#Instance").prop("checked", false);
        DailyShow();
    });

    //Flag reveals
    $("#CurrentSurvey_DOPFlg").click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            showDOP();
        }
        else {
            hideDOP();
        }
    });
    $("#CurrentSurvey_StateFineFlg").click(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            showState();
        }
        else {
            hideState();
        }
    });

    if ($("#CurrentSurvey_DOPFlg").is(":checked")) {
        showDOP();
    }
    if ($("#CurrentSurvey_StateFineFlg").is(":checked")) {
        showState();
    }

    //CMP Mutually Exclusive Flags
    $("#Daily").click(function () {
        var checked = $(this).is(":checked");
        $("#Instance").prop("checked", !checked);
        if (checked) {
            DailyShow();
        }
        else {
            InstanceShow();
        }
    });
    $("#Instance").click(function () {
        var checked = $(this).is(":checked");
        $("#Daily").prop("checked", !checked);
        if (!checked) {
            DailyShow();
        }
        else {
            InstanceShow();
        }
    });

    //Citations
    $("#submitCitation").click(function (e) {
        e.preventDefault();
        var cycleId = $("#CurrentSurvey_SurveyCycleId").val();
        if (cycleId == "") {
            alert("Cannot save a Citation until a survey has been opened or saved.");
        }
        else {
            var form = $("#SaveCitation");
            var data = form.serialize();
            var method = form.attr('method');
            var url = form.attr('action');
            $.ajax({
                url: url,
                data: data,
                type: method,
                success: function (data) {
                    if (data.Success) {
                        var rowData;
                        var citationType = $('#CitationType').val();
                        switch (citationType) {
                            case 'Fed':
                                {
                                    var tag = $('#FedDef');
                                    var sas = $('#SAS');
                                    rowData = [
                                        "Federal",
                                        $('option:selected', tag).text(),
                                        tag.val(),
                                        $('option:selected', sas).text(),
                                        sas.val(),
                                        '',
                                        $('#Comments').val(),
                                        '<a href="" class="edit">Edit</a>',
                                        '<a href="" class="delete">Delete</a>'
                                    ];
                                    break;
                                }
                            case 'Safety':
                                {
                                    var tag = $('#SafetyDef');
                                    var sas = $('#SASNotRequired');
                                    rowData = [
                                        'Safety',
                                        $('option:selected', tag).text(),
                                        tag.val(),
                                        $('option:selected', sas).text(),
                                        sas.val(),
                                        $('#Waiver').prop('checked') ? 'Yes' : 'No',
                                        $('#Comments').val(),
                                        '<a href="" class="edit">Edit</a>',
                                        '<a href="" class="delete">Delete</a>'
                                    ];
                                    break;
                                }
                            case 'State':
                                {
                                    var tag = $('#StateDef');
                                    var sas = $('#SASNotRequired');
                                    rowData = [
                                        'State',
                                        $('option:selected', tag).text(),
                                        tag.val(),
                                        $('option:selected', sas).text(),
                                        sas.val(),
                                        '',
                                        $('#Comments').val(),
                                        '<a href="" class="edit">Edit</a>',
                                        '<a href="" class="delete">Delete</a>'
                                    ];
                                    break;
                                }
                        }
                        if ($('#EditingId').val()) {
                            oTableCitations.fnUpdate(rowData, $('#' + data.Id).get(0));
                        }
                        else {
                            var added = oTableCitations.fnAddData(rowData);
                            var row = oTableCitations.fnGetNodes(added);
                            $(row).attr('id', data.Id);
                        }
                        PreparePayerGroupControls();
                        $('#close').click();
                        
                    }
                    else {
                        alert('There was an unexpected error saving the record.  If the happens repeatedly, contact support.');
                    }
                },
                error: function () {
                    alert('Error communicating with the server.  If this happens repeatedly, contact support.');
                }
            });
        }
    });
    $("a#NewFederal").on('click', function (e) {
        e.preventDefault();
        var modal = $('<div />');
        modal.addClass("modal");
        $('body').append(modal);
        $("#CitationTitle").html("Federal Citation Form");
        $("#CitationType").val("Fed");
        $("#FedDef").show().val($("#FedDef option:first").val());
        $("#SAS").show().val($("#SAS option:first").val());
        $(".waiverShow").css("visibility", "hidden");
        $("#Comments").val("");
        var citation = $(".Citation");
        citation.show();
        var top = Math.max($(window).height() / 2 - citation[0].offsetHeight / 2, 0);
        var left = Math.max($(window).width() / 2 - citation[0].offsetWidth / 2, 0);
        citation.css({ top: top, left: left });
    });
    $("a#NewState").on('click', function (e) {
        e.preventDefault();
        var modal = $('<div />');
        modal.addClass("modal");
        $('body').append(modal);
        $("#CitationTitle").html("State Citation Form");
        $("#CitationType").val("State");
        $("#StateDef").show().val($("#StateDef option:first").val());
        $("#SASNotRequired").show().val($("#SASNotRequired option:first").val());
        $(".waiverShow").css("visibility", "hidden");
        $("#Comments").val("");
        var citation = $(".Citation");
        citation.show();
        var top = Math.max($(window).height() / 2 - citation[0].offsetHeight / 2, 0);
        var left = Math.max($(window).width() / 2 - citation[0].offsetWidth / 2, 0);
        citation.css({ top: top, left: left });
    });
    $("a#NewSafety").on('click', function (e) {
        e.preventDefault();
        var modal = $('<div />');
        modal.addClass("modal");
        $('body').append(modal);
        $("#CitationTitle").html("Safety Citation Form");
        $("#CitationType").val("Safety");
        $("#SafetyDef").show().val($("#SafetyDef option:first").val());
        $("#SASNotRequired").show().val($("#SASNotRequired option:first").val());
        $(".waiverShow").css("visibility", "visible");
        $("#Waiver").removeAttr("checked");
        $("#Comments").val("");
        var citation = $(".Citation");
        citation.show();
        var top = Math.max($(window).height() / 2 - citation[0].offsetHeight / 2, 0);
        var left = Math.max($(window).width() / 2 - citation[0].offsetWidth / 2, 0);
        citation.css({ top: top, left: left });
    });
    $(".Citation #close").click(function () {
        $(".modal").hide();
        $(".Citation").hide();
        $("#FedDef").hide();
        $("#SafetyDef").hide();
        $("#StateDef").hide();
        $("#SASNotRequired").hide();
        $("#SAS").hide();
        $("#EditingId").val("");
    });

    $("#SelectedSurveyType").change(function ()
    {
        var selectedSurveyTypeText = $('option:selected', $(this)).text();
        if (selectedSurveyTypeText == 'Complaint') {
            $('.IsUnsubstantianted').removeAttr("disabled");
        }
        else
        {
            $('input.IsUnsubstantianted').prop("checked", false);
            $('.IsUnsubstantianted').attr("disabled", true);
        }
    });

    formContent.on('click', '#addSurveyDocument', function (e) {
        e.preventDefault();
        $.ajax({
            url: path + "/Survey/CreateSurveyDocument",
            success: function (data, textStatus, jqXHR) {
                var row = $(data);
                var uniqueIdentifier = 'Documents';
                ApplyUniqueItemIndex(row, addIndex, uniqueIdentifier);
                addIndex++;

                var documentTable = $('#surveyDocuments');
                $('#surveyDocuments').append(row);
                InitializeDates();

                formContent.off("change", "input:file");
                formContent.on("change", "input:file", function (e) {
                    var control = $(this);
                    var id = $(control).attr("id");
                    var filename = $(control).val();

                    $("#surveyDocuments a").each(function () {
                        if ($(this).text() == filename.substring(filename.lastIndexOf("\\") + 1)) {
                            alert("Another file with the same name has already been uploaded");
                            control.replaceWith(control = control.clone(true));
                        }
                    });
                    $("#surveyDocuments input:file").each(function () {
                        if ($(this).attr("id") != id && $(this).val() == filename) {
                            alert("Another file with the same name has already been selected");
                            control.replaceWith(control = control.clone(true));
                        }
                    });
                });
                $("button.delete-item", row).attr("class", "delete-item")
                ShowTable($("#surveyDocuments"));
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failure communicating with the server.');
            }
        });
    });

    //    $('#SaveDocumentsSurvey').ajaxComplete(function () {
    //alert('SaveDocumentsSurvey: completed');
    //    });

    formContent.on('click', '#surveyDocuments .delete-item', function (e) {
        e.preventDefault();
        $(this).closest('tr').remove();
    });

});

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

function isInvalidDate(value) {
    return value != "" && !ValidateDate(value);
}
function isInvalidCurrency(value) {
    return value != "" && !ValidateCurrency(value);
}

function showDOP() {
    $(".dopShow").css("visibility", "visible");
    $("#DateDOPStart").removeAttr("disabled");
    $("#DateDOPEnd").removeAttr("disabled");
    $("#CurrentSurvey_DOPDailyAmount").removeAttr("disabled");
}
function hideDOP() {
    $(".dopShow").css("visibility", "hidden");
    $("#DateDOPStart").attr("disabled", "disabled");
    $("#DateDOPEnd").attr("disabled", "disabled");
    $("#CurrentSurvey_DOPDailyAmount").attr("disabled", "disabled");
}

function showState() {
    $(".stateShow").css("visibility", "visible");
    $("#CurrentSurvey_StateFineAmount").removeAttr("disabled");
}
function hideState() {
    $(".stateShow").css("visibility", "hidden");
    $("#CurrentSurvey_StateFineAmount").attr("disabled", "disabled");
}

function DailyShow() {
    $(".dailyShow").show();
    $(".dailyShow input").removeClass("hidden");
    $(".instanceShow").hide();
    $(".instanceShow input").addClass("hidden");
}

function InstanceShow() {
    $(".instanceShow").show();
    $(".instanceShow input").removeClass("hidden");
    $(".dailyShow").hide();
    $(".dailyShow input").addClass("hidden");
}