$(document).ready(function () {
    var eventId = $("#SOCEventId").val();

    if (eventId == "0") {
        $("#OccurredDate").val("");
        $("#editNote").load(path + "StandardsOfCare/EditRestraintNote?eventId=0",restraintNoteLoaded);
    }
    $(".isDate").datepicker().on("change", function (ev) {
        $(this).valid();
    });

    loadNoteList();
    $("#saveEvent").on("click", function (e) {
        e.preventDefault();
        var form = $("#SaveGenericForm");
        if (!form.valid()) {
            return;

        }
        if ($("#saveRestraintNoteForm").length > 0) {
            if (!$("#saveRestraintNoteForm").valid()) {
                return;
            }
        }
        form.ajaxSubmit({
            success: restraintSaveSuccess,
            dataType: "json",
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }

        });
    });
    $("#SaveGenericForm").show();
});

function restraintNoteLoaded() {
    $("#closeNote").on("click", function () {
        $("#editNote").html("");

    });
    if ($("#notedID").val() == "0") {
        $("#AttemptedReducedDate").val("");
        $("#closeNote").hide();
    }

    if ($("#DiagnosisSupportsRestraintFlg").is(":checked")) {
        $("#DiagnosisSupportsRestraintDesc").show();

    }

    $("#DiagnosisSupportsRestraintFlg").on("click", function () {
        $("#DiagnosisSupportsRestraintDesc").toggle($(this).is(":checked"));
        if ($(this).is(":checked")) {
            $("#DiagnosisSupportsRestraintDesc").rules("add", "required");

        } else {
            $("#DiagnosisSupportsRestraintDesc").rules("remove", "required");

        }

    });

    $(".isDate").datepicker({
        onSelect: function () {
            //ie fix for date picker re-opening after selection in modal windows
            $(this).parent().focus();
        }
    });

    $.validator.addMethod("reducedateexists", function (value, element) {
        var reducedDate = $("#AttemptedReducedDate").val();
        var id = $("#notedID").val();
        var count = $(".reduceDate").filter(function () {
            var noteId = $(this).closest("tr").attr("restraintid");
            if (noteId == id) {
                return false;
            }
            if (reducedDate == $(this).text()) {
                return true;
            }
            if ($(this).text() == "") {
                return false;
            }
            return reducedDate.indexOf($(this).text()) != -1;

        }).length;
        
        return (count < 1);

    }, "Cannot have two entries with the same Date Reduced. Please change the date.");

    $.validator.setDefaults({ ignore: [] });
    $("#saveRestraintNoteForm").removeData("validator");
    $("#saveRestraintNoteForm").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#saveRestraintNoteForm"));
    $("#saveRestraintNoteForm").validate();
}

function loadNoteList() {
    var eventId = $("#SOCEventId").val();
    $("#restraintNoteList").load(path + "StandardsOfCare/ListRestraintNotes?eventId=" + eventId, function () {
        $("#restraintNoteTable").dataTable({
            "bFilter": false,
            "bAutoWidth": false,
            //"sScrollY": "200px",
            "sDom": "frtS",
            "iDisplayLength": -1

        });
        $("#restraintNoteTable").on("click", ".edit", function (e) {
            e.preventDefault();
            $("#editNote").load(this.href, restraintNoteLoaded);
        });
        $("#restraintNoteTable").on("click", ".copy", function (e) {
            e.preventDefault();
            $("#editNote").load(this.href, function () {
                $("#notedID").val("0");
                restraintNoteLoaded();
            });
        });
        $("#restraintNoteTable").on("click", ".delete", function (e) {
            e.preventDefault();
            var row = $(this).closest("tr");
            if ($("#restraintNoteTable").find("tbody tr").length == 1) {
                alert("Cannot delete this row. One Noted Record must remain for each Standards of Care entry.");
                return;
            }
            $.ajax({
                url: this.href,
                method: "post",
                success: function (result) {
                    if (result.Success) {
                        loadNoteList();
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

function restraintSaveSuccess(result) {
    if (result.Success) {
        if ($("#saveRestraintNoteForm").length > 0) {
            var noteForm = $("#saveRestraintNoteForm");
            var socEvent = result.id;
            $("#SaveGenericForm").find("#SOCEventId").val(socEvent);
            $("#saveRestraintNoteForm").find("#SOCEventId").val(socEvent);
            noteForm.ajaxSubmit({
                iframe: true,
                dataType: 'json',
                success: function (noteResult) {
                    if (noteResult.Success) {
                        $("#modal").dialog("close");
                        updateTable();
                    }
                    else {
                        alert(noteResult.data);
                    }
                },
                error: function (data, error, errortext) {
                    alert("Server is not responding. Please reload page and try again");
                }
            });
        }
        else {
            $("#modal").dialog("close");
            updateTable();
        }
    }
    else {
        alert("Error: Event not found in database");
    }
}




$.validator.setDefaults({ ignore: [] });
$("#SaveGenericForm").removeData("validator");
$("#SaveGenericForm").removeData("unobtrusiveValidation");
$.validator.unobtrusive.parse($("#SaveGenericForm"));
$("#SaveGenericForm").validate();