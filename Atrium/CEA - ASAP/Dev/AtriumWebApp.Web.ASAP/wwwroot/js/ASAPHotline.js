$(document).ready(function () {
    var oTable = $('#ASAPs').dataTable({
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            null, //Policy #
            null, //Date
            {
                "sWidth": '250px' //Caller
            },
            {
                "visible": false //Contact Info
            },
            {
                "visible": false //Complaint Id
            },
            {
                "visible": false //Complaint
            },
            {
                "visible": false //Investigation
            },
            {
                "visible": false //Action
            },
            {
                "visible": false //Summary
            },
            {
                "bSortable": false,
                "sWidth": "40px"
            },
            {
                "bSortable": false,
                "sWidth": "40px"
            }
        ],
        "language": {
            "emptyTable": "No records within this range."
        }
    });
    oTable.fnSort([[0, 'desc']]);

    $("#ASAPs a.edit").on("click", function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTable.fnGetData(nRow);

        $("#Date").val(aData[1]);
        $("#NumberId").val(nRowId);
        $("#ComplaintTypes").val(aData[4]);
        $("#CallerName").val(aData[2]);
        $("#CallerNumber").val(aData[3]);
        $("#Complaint").val(aData[5]);
        $("#Investigation").val(aData[6]);
        $("#Action").val(aData[7]);
        $("#Summary").val(aData[8]);
        $("#EditingId").val(nRowId);

        //Gray out checkboxes for email
        //$("#Contacts input").attr("disabled", "disabled");

        //get documents for ASAPCall
        $.ajax({
            url: path + "ASAPHotline/GetASAPCallDocuments",
            type: "POST",
            dataType: "json",
            data: { asapCallId: nRowId },
            success: function (result) {
                var template = $("#file-link-template");
                var fileSelect = $(".file-selection");
                var documentSection = $("#asap-documents-section");
                //clear list of links
                documentSection.find(".file-link").each(function (index, element) {
                    element.outerHTML = ""; //I am pretty sure there is a better way to do this....
                });

                $(result).each(function (index, element) {
                    //get template copy
                    var newItem = $(template[0].outerHTML);
                    newItem.removeClass("hidden");
                    newItem.prop("id", "file-link-" + element.ASAPCallDocumentId);
                    //fill in values
                    var link = newItem.find("a");
                    link.prop("href", path + "ASAPHotline/" + template.find("a").data("action") + "/" + element.ASAPCallDocumentId);
                    link[0].innerHTML = element.DocumentFileName;
                    //doesn't work in IE --> //link.prop("text", element.DocumentFileName);
                    newItem.find("#ASAPCallDocumentId").val(element.ASAPCallDocumentId);

                    //attach to DOM
                    $(".file-selection").first().before(newItem);
                });

            },
            error: function (data, error, errortext) {
                alert(errortext);
            }
        });
    });

    $("#ASAPs a.delete").on("click", function (e) {
        e.preventDefault();

        if (!confirm("Are you sure you want to delete?"))
            return;

        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        $.ajax({
            url: path + "ASAPHotline/DeleteRow",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: { rowId: nRowId },
            success: function (result) {
                if (result.Success) {
                    oTable.fnDeleteRow(nRow);
                    if ($("#EditingId").val() == nRowId) {
                        $("#clearASAP").click();
                    }
                }
                else {
                    alert("Error: Event not found in database");
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $("#asap-documents-section").on("click", ".file-link .delete-item", function (e) {
        var section = $(e.target).parents(".file-link");
        section.addClass("hidden delete-flagged");

    });

    var oTableContact = $('#Contacts').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            {
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No contacts listed"
        }
    });
    oTableContact.fnSort([[0, 'desc']]);

    $("#Contacts #ContactCheckBox").click(function () {
        var nRowId = $(this).closest('tr').attr("id");
        var checked = $(this).is(":checked");

        $.post(path + "ASAPHotline/EmailContact", {
            rowId: nRowId,
            check: checked
        });
        //$.ajax({
        //    url: "../ASAPHotline/EmailContact",
        //    dataType: "json",
        //    cache: false,
        //    type: 'POST',
        //    data: {
        //        rowId: nRowId,
        //        checked: checked
        //    },
        //    success: function (result) {

        //    },
        //    error: function (data, error, errortext) {
        //        alert("Server is not responding. Please reload page and try again");
        //    }
        //});
    });

    $(function () {
        $("#occurredRangeTo").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: '0px',
                    marginLeft: '0px'
                });
            }
        });
        $("#occurredRangeFrom").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: '0px',
                    marginLeft: '0px'
                });
            }
        });
        $(".isDate").datepicker({
            beforeShow: function (textbox, instance) {
                instance.dpDiv.css({
                    marginTop: (-textbox.offsetHeight) + 'px',
                    marginLeft: textbox.offsetWidth + 'px'
                });
            }
        });
    });
    var validator = $("#SaveASAP").validate({
        rules: {
            Date: {
                required: true,
                date: true
            },
            ComplaintTypes: {
                required: true
            },
            CallerName: {
                required: true
            },
            CallerNumber: {
                required: true
            },
            Complaint: {
                required: true
            }
        }
    });



    $(function () {
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
    });

    $("#submitASAP").click(function () {
        if ($("#SaveASAP").valid()) {
            //delete file links flagged for deletion
            setTimeout(function () {
                $("#asap-documents-section .file-link.delete-flagged").each(function (index, element) {
                    var item = $(element);
                    $.ajax({
                        url: path + "ASAPHotline/DeleteASAPCallDocument",
                        type: "POST",
                        dataType: "json",
                        data: { documentId: item.find("#ASAPCallDocumentId").val() }
                    });
                });

            }, 200);

            //then submit
            $("#SaveASAP").submit();
        }
    });

    $("#clearASAP").click(function () {
        validator.resetForm();

        $("#SaveASAP textarea").val("");
        $("#SaveASAP input").val("");
        $("#ComplaintTypes").val("");
        $("#Contacts input").prop("checked", false);
        //$("#Contacts input").removeAttr("disabled");
        $("#asap-documents-section").find(".file-link").each(function (index, element) {
            element.outerHTML = ""; //I am pretty sure there is a better way to do this....
        });
        $.post("../ASAPHotline/ClearEmailContacts");
    });
});