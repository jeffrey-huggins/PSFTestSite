function woundNoteLoaded() {
    $.validator.setDefaults({ ignore: [] });
    $("#saveWoundNoteForm").removeData("validator");
    $("#saveWoundNoteForm").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#saveWoundNoteForm"));
    $("#saveWoundNoteForm").validate();

    $("#createNote").hide();

    if ($("#notedID").val() == "0") {
        $("#NotedDate").val("");
    }
    else {
        $("#PressureWoundStageId").prop("disabled", true);
    }
    $("#Drainage").on("click", function () {
        $("#DrainageDesc").toggle($(this).is(":checked"));
        if ($(this).is(":checked")) {
            $("#DrainageDesc").rules("add", "required");

        } else {
            $("#DrainageDesc").rules("remove", "required");

        }

    });
    var woundType = $("#WoundType").val();
    if (woundType == "pressure") {
        $(".pressureWound").show();
        $("#LengthNbr").rules("add", "required");
        $("#WidthNbr").rules("add", "required");
        $("#PressureWoundStageId").rules("add", "required");
    }
    else if (woundType == "composite") {
        $(".compositeWound").show();

    }
    $(".isDate").datepicker({
        onSelect: function () {
            //ie fix for date picker re-opening after selection in modal windows
            $(this).parent().focus();
        }
    });

    $("#PressureWoundStageId").on("change", function () {
        var stage = $(this).val();
        if (stage == "1" || stage == "" || stage == "5") {
            $("#DepthNbr").val("");
            $(".depthHide").hide();
            $("#DepthNbr").rules("remove", "required");
        }
        else {
            $(".depthHide").show();
            $("#DepthNbr").rules("add", "required");

        }

    });
    $("#PressureWoundStageId").change();

}

function woundSubmitSuccess(result) {
    if (result.Success) {
        if ($("#saveWoundNoteForm").length > 0) {
            var noteForm = $("#saveWoundNoteForm");
            var SOCEvent = result.ID;
            $("#SOCEventId").val(SOCEvent);
            $("#Wound_SOCEventId").val(SOCEvent);
            noteForm.ajaxSubmit({
                iframe: true,
                dataType: 'json',
                success: function (noteResult) {
                    if (noteResult.Success) {
                        $("#modal").dialog("close");
                        updateTable();
                    }

                }
            });

        } else {
            $("#modal").dialog("close");
            updateTable();
        }
    }
    else {
        alert("Error: Event not found in database");
    }
}

$(document).ready(function () {
    loadNotesList();

    var socEventId = $("#Wound_SOCEventId").val();

    if (socEventId != "0") {

        $("#Wound_Site").prop("disabled", true);
        $("#Wound_CompositeWoundDescribeId").prop("disabled", true);
        $("#Wound_AdmittedWithFlg").prop("disabled", true);
    }
    else {
        $("#OccurredDate").val("");
        $("#NotesArea").load(path + "StandardsOfCare/EditWoundNote?eventId=" + socEventId, woundNoteLoaded);
    }
    $(".isDate").datepicker({
        onSelect: function () {
            //ie fix for date picker re-opening after selection in modal windows
            $(this).parent().focus();
        }
    }).on("change", function (ev) {
        $(this).valid();
    });

    $("#submitWound").on("click", function (e) {
        e.preventDefault();
        var form = $("#SaveWound");
        if (!form.valid()) {
            return;

        }
        if ($("#saveWoundNoteForm").length > 0) {
            if (!$("#saveWoundNoteForm").valid()) {
                return;
            }
        }
        form.ajaxSubmit({
            iframe: true,
            dataType: 'json',
            success: woundSubmitSuccess,
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

    $("#clearWound").on("click", function () {
        var eventId = $("#Wound_SOCEventId").val();
        $("#NotesArea").load(path + "StandardsOfCare/EditWoundNote?socEventId=" + eventId, woundNoteLoaded);
    });

    $("#createNote").on("click", function () {
        var eventId = $("#Wound_SOCEventId").val();
        $("#NotesArea").load(path + "StandardsOfCare/EditWoundNote?socEventId=" + eventId, woundNoteLoaded);
    })

    $("#Wound_UnavoidableFlg").on("click", function () {
        if ($(this).is(":checked")) {
            $("#File_Document").removeAttr("disabled");
        } else {
            $("#File_Document").attr("disabled", true);
        }
    });
    $("#Wound_AffectedByOther").on("click", function () {
        $("#Wound_AffectedByOtherDescription").toggle($(this).is(":checked"));
    });

    $.validator.setDefaults({ ignore: [] });
    $("#SaveWound").removeData("validator");
    $("#SaveWound").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#SaveWound"));
    $("#SaveWound").validate();
    $("#SaveWound").show();
})

function loadNotesList() {
    var eventId = $("#Wound_SOCEventId").val();
    $("#NotesListing").load(path + "StandardsOfCare/WoundNoteListing?eventId=" + eventId, function () {
        $("#woundNoteTable").dataTable({
            "bFilter": false,
            "bAutoWidth": false,
            //"sScrollY": "200px",
            "sDom": "frtS",
            "iDisplayLength": -1

        });
        $("#woundNoteTable").on("click", ".edit", function (e) {
            e.preventDefault();
            $("#NotesArea").load(this.href, woundNoteLoaded);
        });
        $("#woundNoteTable").on("click", ".copy", function (e) {
            e.preventDefault();
            $("#NotesArea").load(this.href, function () {
                $("#notedID").val("0");
                $("#NotedDate").val("");
                $("#LengthNbr").val("");
                $("#WidthNbr").val("");
                $("#DepthNbr").val("");
                $("#Drainage").prop("checked", false);
                $("#DrainageDesc").val("");
                $("#DrainageDesc").hide();
                $("#Treatment").val("");
                $("#LabDiagnostic").val("");
                woundNoteLoaded();
            });
        });
        $("#woundNoteTable").on("click", ".delete", function (e) {
            e.preventDefault();
            var row = $(this).closest("tr");
            if ($("#woundNoteTable").find("tbody tr").length == 1) {
                alert("Cannot delete this row. One Noted Record must remain for each Standards of Care entry.");
                return;
            }
            $.ajax({
                url: this.href,
                method: "post",
                success: function (result) {
                    if (result.Success) {
                        row.remove();

                    }
                    else {
                        alert(result.data);

                    }

                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }

            });
        });
    });

}