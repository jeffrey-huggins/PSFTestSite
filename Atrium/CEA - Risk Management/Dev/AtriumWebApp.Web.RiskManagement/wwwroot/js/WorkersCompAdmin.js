var insuranceCode = 0;
var lawFirmCode = 1;
var vocCode = 2;
var tcmCode = 3;
var typeCode = 4;

function initCommunities() {
    //Community Table
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "aaSorting": [],
        "sScrollY": "600px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            null,
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Law Firms"
        }
    });

    $(".checkbox").click(function () {
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "BaseRiskManagement/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "RMW"
            },
            success: function (result) {
                if (result.Success) {
                } else {
                    alert("Community has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $(".checkboxReport").click(function () {
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "BaseRiskManagement/ChangeReportFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "RMW"
            },
            success: function (result) {
                if (result.Success) {
                } else {
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
}

function initInsurance() {
    //Insurance Table methods
    var oTableInsurance = $('#Insurance').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '200px' },
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Insurance Companies"
        }
    });
    var nEditing = null;
    oTableInsurance.fnSort([[1, 'asc']]);

    $("#Insurance").on("click",".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableInsurance.fnGetData(nRow);

        $.ajax({
            url: path + "RMWorkerCompAdmin/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                rmwCode: insuranceCode
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Insurance has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $('#Insurance').on('click', 'a.edit', function (e) {
        e.preventDefault();

        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableInsurance, nEditing);
            editRow(oTableInsurance, nRow);
            nEditing = nRow;

        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowInsurance(oTableInsurance, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableInsurance, nRow);
            nEditing = nRow;

        }
    });
    $('#Insurance').on('click', 'a.delete',function (e) {
        e.preventDefault();

        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            $.ajax({
                url: path + "RMWorkerCompAdmin/DeleteRowInsurance",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: { rowId: nRowId },
                success: function (result) {
                    if (result.Success) {
                        oTableInsurance.fnDeleteRow(nRow);
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
    });
    $('#Insurance').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableInsurance, nRow);
    });

    $("#insuranceTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#insuranceTab form");
        var code = $("#InsuranceNm").val();
        var dataTable = $("#Insurance").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Insurance already exists");
            return false;
        }
        if (!form.valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: "POST",
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    var name = result.data.InsuranceNm;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.InsuranceId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });
}

function initLegalFirm() {
    var oTableLawFirm = $('#LawFirm').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '200px' },
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Law Firms"
        }
    });
    var nEditingLF = null;
    oTableLawFirm.fnSort([[1, 'asc']]);

    $("#LawFirm").on("click",".EnableDisable",function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableLawFirm.fnGetData(nRow);

        $.ajax({
            url: path + "RMWorkerCompAdmin/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                rmwCode: lawFirmCode
            },
            success: function (result) {
                if (result.Success) {
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Law Firm has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $('#LawFirm').on('click', 'a.edit', function (e) {
        e.preventDefault();

        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingLF !== null && nEditingLF != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableLawFirm, nEditingLF);
            editRow(oTableLawFirm, nRow);
            nEditingLF = nRow;
        }
        else if (nEditingLF == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowLawFirm(oTableLawFirm, nEditingLF, nRowId);
            if (success) {
                nEditingLF = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableLawFirm, nRow);
            nEditingLF = nRow;
        }
    });
    $('#LawFirm').on('click', 'a.delete', function (e) {
        e.preventDefault();

        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            $.ajax({
                url: path + "RMWorkerCompAdmin/DeleteRowLawFirm",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: { rowId: nRowId },
                success: function (result) {
                    if (result.Success) {
                        oTableLawFirm.fnDeleteRow(nRow);
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
    });
    $('#LawFirm').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableLawFirm, nRow);
    });

    $("#legalTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#legalTab form");
        var code = $("#LegalFirmNm ").val();
        var dataTable = $("#LawFirm").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Legal firm already exists");
            return false;
        }
        if (!form.valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: "POST",
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    var name = result.data.LegalFirmNm ;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.LegalFirmID;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });
}

function initVOC() {
    //VOC Table methods
    var oTableVOC = $('#VOC').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '200px' },
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No VOCs"
        }
    });
    var nEditingVOC = null;
    oTableVOC.fnSort([[1, 'asc']]);

    $("#VOC").on("click",".EnableDisable", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableVOC.fnGetData(nRow);

        $.ajax({
            url: path + "RMWorkerCompAdmin/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                rmwCode: vocCode
            },
            success: function (result) {
                if (result.Success) {
                } else {
                    checkbox.prop("checked", !checked);
                    alert("VOC has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $('#VOC').on('click', 'a.edit', function (e) {
        e.preventDefault();

        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingVOC !== null && nEditingVOC != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableVOC, nEditingVOC);
            editRow(oTableVOC, nRow);
            nEditingVOC = nRow;
        }
        else if (nEditingVOC == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowVOC(oTableVOC, nEditingVOC, nRowId);
            if (success) {
                nEditingVOC = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableVOC, nRow);
            nEditingVOC = nRow;
        }
    });
    $('#VOC').on('click', 'a.delete', function (e) {
        e.preventDefault();

        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            $.ajax({
                url: path + "RMWorkerCompAdmin/DeleteRowVOC",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: { rowId: nRowId },
                success: function (result) {
                    if (result.Success) {
                        oTableVOC.fnDeleteRow(nRow);
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
    });
    $('#VOC').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableVOC, nRow);
    });

    $("#vocTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#vocTab form");
        var code = $("#VOCRehabName").val();
        var dataTable = $("#VOC").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("VOC Rehab already exists");
            return false;
        }
        if (!form.valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: "POST",
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    var name = result.data.VOCRehabName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.VOCRehabID;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

}

function initTCM() {
    //TCM Table methods
    var oTableTCM = $('#TCM').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '200px' },
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No TCMs"
        }
    });
    var nEditingTCM = null;
    oTableTCM.fnSort([[1, 'asc']]);

    $("#TCM").on('click','.EnableDisable',function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableTCM.fnGetData(nRow);

        $.ajax({
            url: path + "RMWorkerCompAdmin/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                rmwCode: tcmCode
            },
            success: function (result) {
                if (result.Success) {
                } else {
                    checkbox.prop("checked", !checked);
                    alert("TCM has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $('#TCM').on('click', 'a.edit', function (e) {
        e.preventDefault();

        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingTCM !== null && nEditingTCM != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableTCM, nEditingTCM);
            editRow(oTableTCM, nRow);
            nEditingTCM = nRow;
        }
        else if (nEditingTCM == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowTCM(oTableTCM, nEditingTCM, nRowId);
            if (success) {
                nEditingTCM = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableTCM, nRow);
            nEditingTCM = nRow;
        }
    });
    $('#TCM').on('click','a.delete', function (e) {
        e.preventDefault();

        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            $.ajax({
                url: path + "RMWorkerCompAdmin/DeleteRowTCM",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: { rowId: nRowId },
                success: function (result) {
                    if (result.Success) {
                        oTableTCM.fnDeleteRow(nRow);
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
    });
    $('#TCM').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableTCM, nRow);
    });

    $("#tcmTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#tcmTab form");
        var code = $("#TCMName").val();
        var dataTable = $("#TCM").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("TCM already exists");
            return false;
        }
        if (!form.valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: "POST",
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    var name = result.data.TCMName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.TCMId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });
}

function initClaims() {
    //Insurance Table methods
    var oTableType = $('#ClaimTypes').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "100px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "aoColumns": [
            { "sWidth": '200px' },
            null,
            null,
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            },
            {
                "bSearchable": false,
                "bSortable": false,
                "sWidth": '20px'
            }
        ],
        "oLanguage": {
            "sEmptyTable": "No Claim Types"
        }
    });
    var nEditingType = null;
    oTableType.fnSort([[1, 'asc']]);

    $("#ClaimTypes").on('click','.EnableDisable',function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableType.fnGetData(nRow);

        $.ajax({
            url: path + "RMWorkerCompAdmin/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                rmwCode: typeCode
            },
            success: function (result) {
                if (result.Success) {
                } else {
                    checkbox.prop("checked", !checked);
                    alert("Claim Type has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $('#ClaimTypes').on('click', 'a.edit', function (e) {
        e.preventDefault();

        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingType !== null && nEditingType != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableType, nEditingType);
            editRow(oTableType, nRow);
            nEditingType = nRow;

        }
        else if (nEditingType == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowType(oTableType, nEditingType, nRowId);
            if (success) {
                nEditingType = null;
            }

        }
        else {
            /* No row currently being edited */
            editRow(oTableType, nRow);
            nEditingType = nRow;

        }
    });
    $('#ClaimTypes').on('click', 'a.delete', function (e) {
        e.preventDefault();

        var delConfirm = confirm("Are you sure you want to delete?");
        if (delConfirm == true) {
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            $.ajax({
                url: path + "RMWorkerCompAdmin/DeleteRowType",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: { rowId: nRowId },
                success: function (result) {
                    if (result.Success) {
                        oTableType.fnDeleteRow(nRow);
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
    });
    $('#ClaimTypes').on('click', 'a.cancel', function (e) {
        e.preventDefault();

        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableType, nRow);
    });

    $("#claimTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#claimTab form");
        var code = $("#ClaimTypeDesc").val();
        var dataTable = $("#ClaimTypes").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Claim type already exists");
            return false;
        }
        if (!form.valid()) {
            return false;
        }
        $.ajax({
            url: form.attr("action"),
            dataType: "json",
            type: "POST",
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    var name = result.data.ClaimTypeDesc;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.ClaimTypeId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisable", "EnableDisable", enabled),
                        '<a class="edit" href="">Edit</a>',
                        '<a class="delete" href="">Delete</a>'
                    ]);
                    var newRow = dataTable.fnSettings().aoData[addId[0]].nTr;
                    newRow.setAttribute('id', id);
                    form[0].reset();
                }
                else {
                    alert(result.message);
                }
            },
            error: function (data, error, errortext) {
                alert("Server is not responding. Please reload page and try again");
            }
        });
    });

}

$(document).ready(function () {
    $("#tabs").tabs();
    initCommunities();
    initInsurance();
    initLegalFirm();
    initVOC();
    initTCM();
    initClaims();

    $("#tabs").tabs({
        activate: function (event, ui) {
            var dataTables = ui.newPanel.find(".dataTable").filter(function () {
                return this.id != "";
            });
            for (i = 0; i < dataTables.length; i++) {
                var dataTable = $("#" + dataTables[i].id).dataTable();
                dataTable.fnAdjustColumnSizing();
            }
        }
    });

});

function restoreRow(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);

    for (var i = 0, iLen = jqTds.length; i < iLen; i++) {
        oTable.fnUpdate(aData[i], nRow, i, false);
    }
}

function editRow(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    jqTds[0].innerHTML = '<input type="text" size="30" value="' + aData[0] + '"/>';
    jqTds[1].innerHTML = '<input type="text" size="3" value="' + aData[1] + '"/>';
    jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function saveRowInsurance(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);
    var jqSelect = $('select', nRow);

    $.ajax({
        url: path + "RMWorkerCompAdmin/EditRowInsurance",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            insurance: jqInputs[0].value,
            order: jqInputs[1].value
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });

    //oTable.fnStandingRedraw();
    return true;
}

function saveRowLawFirm(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "RMWorkerCompAdmin/EditRowLawFirm",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            lawFirm: jqInputs[0].value,
            order: jqInputs[1].value
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });

    //oTable.fnStandingRedraw();
    return true;
}

function saveRowVOC(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "RMWorkerCompAdmin/EditRowVOC",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            voc: jqInputs[0].value,
            order: jqInputs[1].value
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });

    //oTable.fnStandingRedraw();
    return true;
}

function saveRowTCM(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "RMWorkerCompAdmin/EditRowTCM",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            tcm: jqInputs[0].value,
            order: jqInputs[1].value
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });

    //oTable.fnStandingRedraw();
    return true;
}

function saveRowType(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "RMWorkerCompAdmin/EditRowType",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            type: jqInputs[0].value,
            order: jqInputs[1].value
        },
        success: function (result) {
            if (result.Success) {
                oTable.fnUpdate(jqInputs[0].value, nRow, 0, false);
                oTable.fnUpdate(jqInputs[1].value, nRow, 1, false);
                oTable.fnUpdate('<a class="edit" href="">Edit</a>', nRow, 3, false);
            }
        },
        error: function (data, error, errortext) {
            alert("Server is not responding. Please reload page and try again");
        }

    });

    //oTable.fnStandingRedraw();
    return true;
}

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});

function alreadyExist(code, dataTable, index) {
    if (!index) {
        index = 0;
    }
    var dataSet = dataTable.dataTable().fnGetData();
    for (var i = 0; i < dataSet.length; i++) {
        var existingCode = dataSet[i][index];
        if (existingCode.toLowerCase() === code.toLowerCase()) {
            return true;
        }
    }
    return false;
}

function createCheckboxString(id, cssClass, name, value) {
    if (value) {
        var checked = "checked";
    }
    var html = '<input class="' + cssClass + '" id="' + id + '" name="' + name + '" type="checkbox" value="' + value + '" ' + checked + '>';
    html += '<input name="' + name + '" type="hidden" value="' + value + '">';
    return html;
}