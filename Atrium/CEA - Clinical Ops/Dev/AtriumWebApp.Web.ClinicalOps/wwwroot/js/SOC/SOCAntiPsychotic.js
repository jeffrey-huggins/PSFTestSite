function editAntiNote(e) {
    e.preventDefault();
    $("#note").load(this.href, antipsychoticNoteLoaded);
}

function copyAntiNote(e) {
    e.preventDefault();
    $("#note").load(this.href, function () {
        $("#notedID").val("0");
        $("#NotedDate").val("");
        antipsychoticNoteLoaded();
    });
}

function deleteAntiNote(e) {
    e.preventDefault();
    var row = $(this).closest("tr");
    if ($("#antiTable").find("tbody tr").length == 1) {
        alert("Cannot delete this row. One Noted Record must remain for each Standards of Care entry.");
        return;
    }
    $.ajax({
        url: this.href,
        method: "post",
        success: function (result) {
            if (result.Success) {
                loadNotesLists();
            }
            else {
                alert(result.data);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }
    });
}

function antipsychoticNoteLoaded() {
    $("#noteCreation").hide();
    
    $(".isDate").datepicker({
        onSelect: function () {
            //ie fix for date picker re-opening after selection in modal windows
            $(this).parent().focus();
        }}).on("change", function (ev) {
        $(this).valid();
        });
    
    if ($("#notedID").val() == "0") {
        $("#NotedDate").val("");

    }
    $("#closeNote").on("click", function () {
        $("#note").html("");
        $("#noteCreation").show();
    });

    $.validator.setDefaults({ ignore: [] });
    $("#antiPsychoticNote").removeData("validator");
    $("#antiPsychoticNote").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#antiPsychoticNote"));
    $("#antiPsychoticNote").validate();

}

function loadNotesLists() {
    var socEventId = $("#SOCEventId").val();
    $("#NotesArea").load(path + "StandardsOfCare/AntiPsychoticNoteListing?eventId=" + socEventId, function () {
        var oTableAnti = $('#antiTable').dataTable({
            "bFilter": false,
            "bAutoWidth": false,
            //"sScrollY": "200px",
            "sDom": "frtS",
            "iDisplayLength": -1,
            "aoColumns": [
                null,
                null,
                {
                    "bSearchable": false,
                    "bSortable": false
                },
                {
                    "bSearchable": false,
                    "bSortable": false
                },
                {
                    "bSearchable": false,
                    "bSortable": false
                }
            ],
            "oLanguage": {
                "sEmptyTable": "No antipsychotic dates recorded"
            }
        });
        oTableAnti.fnSort([[0, 'desc']]);

        $("#antiTable").on('click', 'a.edit', editAntiNote);

        $("#antiTable").on('click', 'a.copy', copyAntiNote);

        $("#antiTable").on("click", ".delete", deleteAntiNote);
    });

}

$(document).ready(function () {
    var socEventId = $("#SOCEventId").val();
    loadNotesLists();

    if (socEventId != "0") {
        $("#AntiPsychoticMedicationId").prop("disabled", true)
    }
    else {
        $("#OccurredDate").val("");
        createNote();
    }
    if ($("#AntiPsychoticDiagnosisId").val() == "4") {
        $("#OtherDiagnosisDetail").show();
    }
    else {
        $("#OtherDiagnosisDetail").hide();
    }
    $(".isDate").datepicker({
        onSelect: function () {
            //ie fix for date picker re-opening after selection in modal windows
            $(this).parent().focus();
        }
    }).on("change", function (ev) {
        $(this).valid();
    });

    $("#saveAnti").on("click", function (e) {
        e.preventDefault();
        var form = $("#SaveAntipsychotic");
        if (!form.valid()) {
            return;
        }
        if ($("#antiPsychoticNote").length > 0) {
            if (!$("#antiPsychoticNote").valid()) {
                return;
            }
        }
        form.ajaxSubmit({
            iframe: true,
            dataType: 'json',
            success: saveSuccessAntipsychotic,
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }

        });
    });

    $("#AntiPsychoticDiagnosisId").on("change", function () {
        if ($(this).val() == "4") {
            $("#OtherDiagnosisDetail").show();
        }
        else {
            $("#OtherDiagnosisDetail").hide();
            $("#OtherDiagnosisDetail").val("");
        }

    });

    $("#noteCreation").on("click", function (e) { e.preventDefault(); createNote(); });

    $("#clearAnti").click(function () {
        $("#NotedDate, #ReducedDate, #LastAimsTestDate, #Recomendation, #TargetedBehavior").val("");
        if ($("#AntiPsychoticMedicationId").attr("disabled") == null) {
            $("#AntiPsychoticMedicationId").val("");
        }
    });

    $.validator.addMethod("SOC_NoteDateExists", function (value, element) {
        var noteDate = getMomentDate(value);
        var valid = true;
        $(".noteDate").each(function (index, element) {
            var oldNoteDate = getMomentDate($(element).text());
            if (oldNoteDate.diff(noteDate) == 0) {
                valid = false;
            }
        });
        return valid;
    }, "A note with that date already exists, select another date.");

    $.validator.setDefaults({ ignore: [] });
    $("#SaveAntipsychotic").removeData("validator");
    $("#SaveAntipsychotic").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($("#SaveAntipsychotic"));
    $("#SaveAntipsychotic").validate();

    $("#SaveAntipsychotic").show();

});

function saveSuccessAntipsychotic(result) {
    if (result.Success) {
        $("#SOCEventId").val(result.id);
        if ($("#antiPsychoticNote").length > 0) {
            $("#antiPsychoticNote").find("#SOCEventId").val(result.id);
            saveAntipsychoticNote();
        }
        else {
            $("#modal").dialog("close");
        }

        updateTable();
    }
    else {
        alert("Error: Event not found in database");
    }

}

function saveAntipsychoticNote() {
    $("#antiPsychoticNote").ajaxSubmit({
        iframe: true,
        dataType: 'json',
        success: function (noteResult) {
            if (noteResult.Success) {
                loadNotesLists();
                $("#modal").dialog("close");
            }
            else {
                alert("Error: Event not found in database");
            }

        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });
}

function createNote() {
    var socEventId = $("#SOCEventId").val();
    $("#note").load(path + "StandardsOfCare/EditAntiPsychoticNote?eventId=" + socEventId, function () {
        $("#closeNote").hide();
        antipsychoticNoteLoaded();
    });
}



