$("#Residents").change(function () {
    if ($("#Residents option:selected").val() == "") {
        return;
    }
    var chosenResident = $("#Residents option:selected").val();
    $("#hiddenDDLSubmit").val(chosenResident);
});

