$(document).ready(function () {
    var oTable = $('#HDwindow-table').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "400px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '70px' }, //Resident
            {
                "sWidth": '25px' //Admit Source
            },
            {
                "sWidth": '50px' //Discharge Date
            },
            {
                "bVisible": false //Discharge
            },
            {
                "sWidth": '50px' //Resident Status
            },
            {
                "sWidth": '10px' //Planned
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '120px' //Diagnosis
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '120px'  //Additional Info
            },
            {
                "bSearchable": false,
                "bSortable": false,  //Edit
                "sWidth": '40px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No entries for date range selected"
        }
    });
    var nEditing = null;
    oTable.fnSort([[2, 'desc']]);
    oTable.fnAdjustColumnSizing();
    $("#HDwindow-table_length").css("display", "none");

    $('#HDwindow-table').on('click','a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTable, nEditing);
            editRow(oTable, nRow, nRowId);
            nEditing = nRow;
        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            //var nRowId = $(this).closest('tr').attr("id");
            var success = saveRow(oTable, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }
        }
        else {
            /* No row currently being edited */
            //editRow(oTable, nRow);
            //var nRowId = $(this).closest('tr').attr("id");
            editRow(oTable, nRow, nRowId);
            nEditing = nRow;
        }
    });
    $('#HDwindow-table').on('click','a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTable, nRow);
    });
    function restoreRow(oTable, nRow) {
        var aData = oTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);
        for (var i = 0, iLen = jqTds.length + 1; i < iLen; i++) {
            oTable.fnUpdate(aData[i], nRow, i, false);
        }
        //oTable.fnStandingRedraw();
    }
    function editRow(oTable, nRow, nRowId) {
        var aData = oTable.fnGetData(nRow);
        var jqTds = $('>td', nRow);

        var Planned = '<input type="checkbox" id="editPlanned" />';
        var PlannedFlg = aData[5] == "Yes";
        jqTds[4].innerHTML = Planned;
        $("#editPlanned").prop("checked", PlannedFlg);

        var ERDischarge = '<select id="editERDischarge">';
        ERDischarge = ERDischarge + $("#ERDischargeReasons").html().toString();
        //if selected ERDischargeReason isn't included, add the option
        if ($('.ERDischargeReasonIsIncluded', jqTds[5]).val().toLowerCase() === 'false') {
            var erText = $('.ERDischargeReason', jqTds[5]).text().trim();
            var erValue = $('.ERDischargeReasonId', jqTds[5]).val();
            ERDischarge = ERDischarge + '<option value="' + erValue + '">' + erText + '</option>';
        }
        ERDischarge = ERDischarge + '</select>';
        $('.ERDischargeReason', jqTds[5]).hide();
        $('.EditERDischargeReason', jqTds[5]).html(ERDischarge);
        $("#editERDischarge").val($('.ERDischargeReasonId', jqTds[5]).val());

        var HDischarge = '<select id="editHDischarge">';
        HDischarge = HDischarge + $("#HospitalDischargeReasons").html().toString();
        //if selected HDischarge isn't included, add the option
        if ($('.HospitalDischargeReasonIsIncluded', jqTds[5]).val().toLowerCase() === 'false') {
            var hospitalDischargeText = $('.HospitalDischargeReason', jqTds[5]).text().trim();
            var hospitalDischargeValue = $('.HospitalDischargeReasonId', jqTds[5]).val();
            HDischarge = HDischarge + '<option value="' + hospitalDischargeValue + '">' + hospitalDischargeText + '</option>';
        }
        HDischarge = HDischarge + '</select>';
        $('.HospitalDischargeReason', jqTds[5]).hide();
        $('.EditHospitalDischargeReason', jqTds[5]).html(HDischarge);
        $("#editHDischarge").val($('.HospitalDischargeReasonId', jqTds[5]).val());

        var DNReturn = '<select id="editDNReturn">';
        DNReturn = DNReturn + $("#DidNotReturnReasons").html().toString();
        DNReturn = DNReturn + '</select>';
        $('.DidNotReturnReason', jqTds[6]).hide();
        $('.EditDidNotReturnReason', jqTds[6]).html(DNReturn);
        $("#editDNReturn").val($('.DidNotReturnReasonId', jqTds[6]).val());

        var Hospital = '<select id="editHospital">';
        Hospital = Hospital + $("#Hospitals").html().toString();
        //if selected Hospital isn't included, add the option
        if ($('.HospitalIsIncluded', jqTds[6]).val().toLowerCase() === 'false') {
            var hospitalText = $('.Hospital', jqTds[6]).text().trim();
            var hospitalValue = $('.HospitalId', jqTds[6]).val();
            Hospital = Hospital + '<option value="' + hospitalValue + '">' + hospitalText + '</option>';
        }
        Hospital = Hospital + '</select>';
        $('.Hospital', jqTds[6]).hide();
        $('.EditHospital', jqTds[6]).html(Hospital);
        $("#editHospital").val($('.HospitalId', jqTds[6]).val());

        if ($('.HospitalDischargeReasonId', jqTds[5]).val() === "-2") {
            $("#editDNReturn").removeAttr("disabled");
        }
        else {
            $("#editDNReturn").attr("disabled", "disabled");
            $("#editDNReturn").val("-1");
        }
        $("#editHDischarge").change(function () {
            var currentVal = $("#editHDischarge").val();
            if (currentVal === "-2") {
                $("#editDNReturn").removeAttr("disabled");
            }
            else {
                $("#editDNReturn").attr("disabled", "disabled");
                $("#editDNReturn").val("-1");
            }
        });
        jqTds[7].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
    }
    function saveRow(oTable, nRow, nRowId) {
        var data = {
            rowId: nRowId,
            cDate: oTable.fnGetData(nRow)[2],
            Planned: $("#editPlanned", nRow).is(":checked"),
            ERDischarge: $("#editERDischarge", nRow).val(),
            HDischarge: $("#editHDischarge", nRow).val(),
            Hospital: $("#editHospital", nRow).val(),
            DNRReason: $("#editDNReturn", nRow).val()
        };

        $.ajax({
            url: path + "HospitalDischarge/EditRow",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: data,
            success: function (result) {
                if (result.Success) {
                    $('.ERDischargeReasonId', nRow).val(data.ERDischarge);
                    $('.ERDischargeReason', nRow).text(result.ERReason).show();
                    $('.EditERDischargeReason', nRow).html('');
                    $('.HospitalDischargeReasonId', nRow).val(data.HDischarge);
                    $('.HospitalDischargeReason', nRow).text(result.HDReason).show();
                    $('.EditHospitalDischargeReason', nRow).html('');
                    oTable.fnUpdate($('.Diagnosis', nRow).html(), nRow, 6, false);

                    $('.HospitalId', nRow).val(data.Hospital);
                    $('.Hospital', nRow).text(result.Hospital).show();
                    $('.EditHospital', nRow).html('');
                    $('.DidNotReturnReasonId', nRow).val(data.DNRReason);
                    $('.DidNotReturnReason', nRow).text(result.DNRReason).show();
                    $('.EditDidNotReturnReason', nRow).html('');
                    oTable.fnUpdate($('.AdditionalInfo', nRow).html(), nRow, 7, false);

                    oTable.fnUpdate(data.Planned ? "Yes" : "No", nRow, 5, false);
                    oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 8, false);
                }
                else if (result.Reason == 1) {
                    alert("Error: Event not found in database");
                    restoreRow(oTable, nRow);
                }
                else if (result.Reason == 0) {
                    alert("Error: Unsuccessful save to the database");
                    restoreRow(oTable, nRow);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload the page and try again");
            }
        });
        //oTable.fnStandingRedraw();
        return true;
    }

    $("#occurredRangeTo,#occurredRangeFrom").datepicker({
        beforeShow: function (textbox, instance) {
            instance.dpDiv.css({
                marginTop: '5px',
                marginLeft: '0px'
            });
        }
    });

    $(function () {
        if ($("#ToDateRangeInvalid").val() == "1") {
            alert('Error: Please enter a valid date in the Discharge Date Range "To" Field (mm/dd/yyyy)');
        }
        if ($("#FromDateRangeInvalid").val() == "1") {
            alert('Error: Please enter a valid date in the Discharge Date Range "From" Field (mm/dd/yyyy)');
        }
        if ($("#ToDateRangeInFuture").val() == "1") {
            alert('Error: Please enter a valid date that is not in the future in the Discharge Date Range "To" Field');
        }
        if ($("#FromDateRangeInFuture").val() == "1") {
            alert('Error: Please enter a valid date that is not in the future in the Discharge Date Range "From" Field');
        }
        if ($("#FromAfterTo").val() == "1") {
            alert('Error: You can not have the "From" Date after the "To" Date in the Discharge Date Range Fields');
        }
    });
});