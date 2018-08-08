function setupNoteSection() {
    var requestId = $("#Request_Requestid").val();
    $("#editNote").load(path + "RMMedicalRecords/EditOrCreateRecordNote?requestid=" + requestId, setupNoteForm);
    $("#noteTableSection").load(path + "RMMedicalRecords/RMMRecordNotesList?requestid=" + requestId, setupNoteList);
}
function setupNoteForm() {
    $("#submitNote").on("click", function (e) {
        e.preventDefault();
        $.ajax({
            data: $("#editNote *").serialize(),
            url: path + "RMMedicalRecords/SaveRecordNote",
            success: function (result) {
                if (result.success) {
                    setupNoteSection();
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
}
function setupNoteList() {
    var oTableNotes = $('#notesTable').dataTable({
        "sDom": "rtS",
        "bAutoWidth": false,
        "iDisplayLength": -1,
        "aoColumns": [
            {
                "sWidth": '40px'
            },
            {
                "sWidth": '40px'
            },
            {
                "sWidth": '75px'
            },
            {
                "sWidth": '30px',
                "bSortable": false
            },
            {
                "sWidth": '30px',
                "bSortable": false
            }

        ],
        "oLanguage": {
            "sEmptyTable": "No notes for the claim"
        }
    });
    oTableNotes.fnSort([[1, 'desc']]);
    $("#notesTable").on("click", ".edit", function (e) {
        e.preventDefault();
        $("#editNote").load(this.href, setupNoteForm);
    });
    $("#notesTable").on("click", ".delete", function (e) {
        e.preventDefault();
        $.ajax({
            url: this.href,
            success: function (result) {
                if (result.success) {
                    setupNoteSection();
                }
            }
        })
    });
}


function refreshDocList() {
    var requestId = $("#Request_Requestid").val();
    $("#documentListSection").load(path + "RMMedicalRecords/DocumentList?requestid=" + requestId, function () {
        $(".isDate").datepicker();
    });

}
function afterRecordLoaded() {
    $(".isDate").datepicker();
    var requestId = $("#Request_Requestid").val();
    if (requestId == "") {
        $("#Request_OpenDate").val("");
    }
    else {
        if ($("#Request_LawSuitFiledFlg").is(":checked")) {
            $("#showLawsuit").show();
            
        }
        setupNoteSection();
    }
    $("#saveRecord").on("click", function (e) {
        e.preventDefault();
        var form = $("#formMRR");
        if (!form.valid()) {
            return false;
        }
        form.ajaxSubmit({
            method: "post",
            success: function (result) {
                if (result.success) {
                    loadRecordsRequest();
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
    $("#clearRecord").on("click", loadRecordsRequest);
    $("#newDocument").on("change", function () {

        if ($("#newDocument")[0].files.length == 0) {
            return;
        }
        var files = $("#newDocument")[0].files;
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            formData.append("Document", file);
            formData.append("Requestid", requestId);
        }
        $.ajax({
            url: path + "RMMedicalRecords/SaveDocument",
            type: "POST",
            processData: false,
            contentType: false,
            dataType: "json",
            data: formData,
            success: function (result) {
                if (result.Success) {
                    $("#newDocument").replaceWith($("#newDocument").val('').clone(true));
                    refreshDocList();
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        })
    });
    $("#documentListSection").on("click", ".deleteFile", function (e) {
        e.preventDefault();
        $.ajax(this.href, {
            success: function (result) {
                if (result.success) {
                    refreshDocList();
                }
            }
        })

    });
    $("#showLawsuit").on("click", function (e) {
        e.preventDefault();
        showLawsuit();

    })
    $("#Request_LawSuitFiledFlg").on("change", function () {
        if ($(this).is(":checked")) {
            showLawsuit();
            $("#showLawsuit").show();
        }
        else {
            $("#showLawsuit").hide();

        }

    });
    $("#saveLawsuit").on("click", function () {
        $("#modal").dialog("close");

    });
    $("#clearLawsuit").on("click", function () {
        $("#modal input[type='text']").val("");
        $("#modal textarea").val("");
        $("#Request_DeathClaim").prop("checked", false);
    });
}
function showLawsuit() {
    $("#modal").dialog({
        width: 600,
        height: 500,
        modal: true,
        close: function () {
            $(this).dialog('destroy');

        }
    });

}

function loadRecordsRequest() {
    var resident = $("#Residents option:selected").val();
    if (resident != "") {
        var url = path + "RMMedicalRecords/RMMRecordList?patientId=" + resident;
        $("#recordList").load(url, function () {
            $("#recordTable").dataTable({
                "sScrollY": "300px",
                "sDom": "frtS",
                "bFilter": false,
                "iDisplayLength": -1,
                "oLanguage": {
                    "sEmptyTable": "No record requests for the patient"
                }
            });
            $("#recordTable").on("click", ".edit", function (e) {
                e.preventDefault();
                $("#editForm").load(this.href, afterRecordLoaded);
            });
            $("#recordTable").on("click", ".delete", function (e) {
                e.preventDefault();
                $.ajax({
                    url: this.href,
                    success: function(result){
                        if (result.Success) {
                            loadRecordsRequest();
                        }
                    }
                })
            });
        });
        $("#selectedResidentView").load(path + "RMMedicalRecords/PatientInfo?patientId=" + resident, function () {
            //"Standards Of Care for " + SOCCurrentResidentName;
            $("#title").text("Medical Records Request for " + $("#residentName").text());
        });
        $("#editForm").load(path + "RMMedicalRecords/EditOrCreateRMMRecord?patientId=" + resident, afterRecordLoaded);
        $("#measuresSelection").show();
        $(".instruction").hide();

    }

}

$("#sideBar").on("change", "#Residents", function () {
    loadRecordsRequest();
});

$("#sideBar").on("change", "#Communities", function () {
    $("#editForm").html("");
    $("#recordList").html("");
    $(".instruction").show();
});

$("#sideBar").on("click", "#lookbackUpdate", function (e) {
    e.preventDefault();
    var lookbackDate = moment($("#LookbackDate").val(), 'MM/DD/YYYY', true);
    if (!lookbackDate.isValid()) {
        lookbackDate = moment($("#LookbackDate").val(), 'M/DD/YYYY', true);
    }

    if (!lookbackDate.isValid()) {
        alert("Error: Please enter a valid Last Census Date (mm/dd/yyyy)");
    }
    else if (moment().diff(lookbackDate) < 0) {
        alert("Error: You can not have a Census date that is in the future");
    }
    else {
        ShowProgress();
        $("#SideDDL").ajaxSubmit({
            url: path + "RMMedicalRecords/UpdateSideBar",
            type: "POST",
            success: function (result) {
                $("#sideBar").html(result);
                initSidebar();
            }
        });
    }

});

$("#sideBar").on("change", "#Communities", function () {
    ShowProgress();
    $("#SideDDL").ajaxSubmit({
        url: path + "RMMedicalRecords/UpdateSideBar",
        type: "POST",
        success: function (result) {
            $("#sideBar").html(result);
            initSidebar();
        }

    });
});

function initSidebar() {
    $("#LookbackDate").datepicker({
        beforeShow: function (textbox, instance) {
            instance.dpDiv.css({
                marginTop: '10px',
                marginLeft: '0px'
            });
        }
    });
    $("#Residents").val("");
    $("#title").text("Medical Records Request");
    $(".instruction").show();
    $("#window").html("");

}