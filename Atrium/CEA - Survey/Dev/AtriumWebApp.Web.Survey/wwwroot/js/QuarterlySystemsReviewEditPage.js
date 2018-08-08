function PrepareBackButton(backUrl) {
    $('#back').on('click', function (e) {
        e.preventDefault();
        ShowProgress();
        window.location.href = backUrl;
    });
}
function PrepareResidentTable() {
    $('#residents').dataTable({
        "bAutoWidth": false,
        "sDom": "rt",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            null,
            {
                "bSortable": false
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No records within this range."
        },
        "aaSorting": [[1, "asc"]]
    });
}