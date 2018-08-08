//Context Id's for Ajax calls
var siteContextId = 1;
var symptomContextId = 2;
var diagnosisContextId = 0;
var precautionContextId = 3;
var organismContextId = 5;
var antibioticContextId = 6;

function initSites() {
    //Sites
    var oTableSites = $('#Sites').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Sites"
        }
    });
    oTableSites.fnSort([[1, 'asc']]);
    var nEditing = null;

    $("#Sites").on("click",".EnableDisableSite",function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableSites.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "IFC",
                contextId: siteContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Site has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    //Edit
    $('#Sites').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditing !== null && nEditing != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableSites, nEditing);
            editRow(oTableSites, nRow, 60);
            nEditing = nRow;
        }
        else if (nEditing == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowSite(oTableSites, nEditing, nRowId);
            if (success) {
                nEditing = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableSites, nRow, 60);
            nEditing = nRow;
        }
    });

    $('#Sites').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableSites, nRow);
    });

    $("#Sites").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableSites.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "JSON/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "IFC",
                contextId: siteContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableSites.fnDeleteRow(nRow);
                } else {
                    alert("Site has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    $("#siteTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#siteTab form");
        var code = $("#NewSite_PatientIFCSiteName").val();
        var dataTable = $("#Sites").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Site already exists");
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
                    var name = result.data.PatientIFCSiteName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.PatientIFCSiteId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisableSite", "EnableDisable", enabled),
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

function initOrganism() {
    //Organisms
    var oTableOrganisms = $('#Organisms').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Organisms"
        }
    });
    oTableOrganisms.fnSort([[1, 'asc']]);
    var nEditingOrganisms = null;

    $("#Organisms").on("click", ".EnableDisableOrganism", function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableOrganisms.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "IFC",
                contextId: organismContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Organism has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    //Edit
    $('#Organisms').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingOrganisms !== null && nEditingOrganisms != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableOrganisms, nEditingOrganisms);
            editRow(oTableOrganisms, nRow, 64);
            nEditingOrganisms = nRow;
        }
        else if (nEditingOrganisms == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowOrganism(oTableOrganisms, nEditingOrganisms, nRowId);
            if (success) {
                nEditingOrganisms = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableOrganisms, nRow, 64);
            nEditingOrganisms = nRow;
        }
    });

    $('#Organisms').on('click', "a.cancel", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableOrganisms, nRow);
    });

    $("#Organisms").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableOrganisms.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "JSON/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "IFC",
                contextId: organismContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableOrganisms.fnDeleteRow(nRow);
                } else {
                    alert("Organism has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    $("#organismTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#organismTab form");
        var code = $("#NewOrganism_PatientIFCOrganismName").val();
        var dataTable = $("#Organisms").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Organism already exists");
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
                    var name = result.data.PatientIFCOrganismName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.PatientIFCOrganismId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisableOrganism", "EnableDisable", enabled),
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

function initSymptoms() {
    //Symptoms
    var oTableSymptoms = $('#Symptoms').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Symptoms"
        }
    });
    oTableSymptoms.fnSort([[1, 'asc']]);
    var nEditingSymptom = null;

    $("#Symptoms").on("click",".EnableDisableSymptom",function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableSymptoms.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "IFC",
                contextId: symptomContextId
            },
            success: function (result) {
                if (result.Success) {
                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Symptom has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    //Edit
    $('#Symptoms').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingSymptom !== null && nEditingSymptom != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableSymptoms, nEditingSymptom);
            editRow(oTableSymptoms, nRow, 60);
            nEditingSymptom = nRow;
        }
        else if (nEditingSymptom == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowSymptom(oTableSymptoms, nEditingSymptom, nRowId);
            if (success) {
                nEditingSymptom = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableSymptoms, nRow, 60);
            nEditingSymptom = nRow;
        }
    });

    $('#Symptoms').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableSymptoms, nRow);
    });

    $("#Symptoms").on("click", 'a.delete', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableSymptoms.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "JSON/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "IFC",
                contextId: symptomContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableSymptoms.fnDeleteRow(nRow);
                } else {
                    alert("Symptom has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });
    $("#symptomTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#symptomTab form");
        var code = $("#NewSymptom_PatientIFCSymptomName").val();
        var dataTable = $("#Symptoms").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Symptom already exists");
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
                    var name = result.data.PatientIFCSymptomName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.PatientIFCSymptomId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisableSymptom", "EnableDisable", enabled),
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

function initDiagnoses() {
    //Diagnoses Table
    var oTableDiagnoses = $('#Diagnoses').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Diagnoses"
        }
    });
    oTableDiagnoses.fnSort([[1, 'asc']]);
    var nEditingDiagnosis = null;

    $("#Diagnoses").on("click",".EnableDisableDiagnosis",function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableDiagnoses.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "IFC",
                contextId: diagnosisContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Diagnosis has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    //Edit
    $('#Diagnoses').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingDiagnosis !== null && nEditingDiagnosis != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableDiagnoses, nEditingDiagnosis);
            editRow(oTableDiagnoses, nRow, 60);
            nEditingDiagnosis = nRow;
        }
        else if (nEditingDiagnosis == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowDiagnosis(oTableDiagnoses, nEditingDiagnosis, nRowId);
            if (success) {
                nEditingDiagnosis = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableDiagnoses, nRow, 60);
            nEditingDiagnosis = nRow;
        }
    });

    $('#Diagnoses').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableDiagnoses, nRow);
    });

    $("#Diagnoses").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableDiagnoses.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "JSON/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "IFC",
                contextId: diagnosisContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableDiagnoses.fnDeleteRow(nRow);
                } else {
                    alert("Diagnosis has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });
    $("#diagnosisTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#diagnosisTab form");
        var code = $("#NewDiagnosis_PatientIFCDiagnosisName").val();
        var dataTable = $("#Diagnoses").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Diagnosis already exists");
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
                    var name = result.data.PatientIFCDiagnosisName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.PatientIFCDiagnosisId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisableDiagnosis", "EnableDisable", enabled),
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

function initPrecautions() {
    //Precautions
    var oTablePrecautions = $('#Precautions').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Precautions"
        }
    });
    oTablePrecautions.fnSort([[1, 'asc']]);
    var nEditingPrecaution = null;

    $("#Precautions").on("click",".EnableDisablePrecaution",function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTablePrecautions.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "IFC",
                contextId: precautionContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Precaution has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    //Edit
    $('#Precautions').on('click', 'a.edit', function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingPrecaution !== null && nEditingPrecaution != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTablePrecautions, nEditingPrecaution);
            editRow(oTablePrecautions, nRow, 60);
            nEditingPrecaution = nRow;
        }
        else if (nEditingPrecaution == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowPrecaution(oTablePrecautions, nEditingPrecaution, nRowId);
            if (success) {
                nEditingPrecaution = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTablePrecautions, nRow, 60);
            nEditingPrecaution = nRow;
        }
    });

    $('#Precautions').on('click', 'a.cancel', function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTablePrecautions, nRow);
    });

    $("#Precautions").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTablePrecautions.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "JSON/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "IFC",
                contextId: precautionContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTablePrecautions.fnDeleteRow(nRow);
                } else {
                    alert("Precaution has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });
    $("#precautionTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#precautionTab form");
        var code = $("#NewPrecaution_PatientIFCTypeOfPrecautionName").val();
        var dataTable = $("#Precautions").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Precaution already exists");
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
                    var name = result.data.PatientIFCTypeOfPrecautionName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.PatientIFCTypeOfPrecautionId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisablePrecaution", "EnableDisable", enabled),
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

function initAntibiotics() {
    //Antibiotics
    var oTableAntibiotics = $('#Antibiotics').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "200px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Antibiotics"
        }
    });
    oTableAntibiotics.fnSort([[1, 'asc']]);
    var nEditingAntibiotic = null;

    $("#Antibiotics").on("click",".EnableDisableAntibiotic",function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var nRow = $(this).parents('tr')[0];
        var aData = oTableAntibiotics.fnGetData(nRow);

        $.ajax({
            url: path + "JSON/ChangeDataFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                description: aData[0],
                dFlag: checked,
                appCode: "IFC",
                contextId: antibioticContextId
            },
            success: function (result) {
                if (result.Success) {

                }
                else {
                    checkbox.prop("checked", !checked);
                    alert("Antibiotic has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    //Edit
    $('#Antibiotics').on('click', "a.edit", function (e) {
        e.preventDefault();
        /* Get the row as a parent of the link that was clicked on */
        var nRow = $(this).parents('tr')[0];
        if (nEditingAntibiotic !== null && nEditingAntibiotic != nRow) {
            /* A different row is being edited - the edit should be cancelled and this row edited */
            restoreRow(oTableAntibiotics, nEditingAntibiotic);
            editRow(oTableAntibiotics, nRow, 60);
            nEditingAntibiotic = nRow;
        }
        else if (nEditingAntibiotic == nRow && this.innerHTML == "Save") {
            /* This row is being edited and should be saved */
            var nRowId = $(this).closest('tr').attr("id");
            var success = saveRowAntibiotic(oTableAntibiotics, nEditingAntibiotic, nRowId);
            if (success) {
                nEditingAntibiotic = null;
            }
        }
        else {
            /* No row currently being edited */
            editRow(oTableAntibiotics, nRow, 60);
            nEditingAntibiotic = nRow;
        }
    });

    $('#Antibiotics').on('click', "a.cancel", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        restoreRow(oTableAntibiotics, nRow);
    });

    $("#Antibiotics").on("click", "a.delete", function (e) {
        e.preventDefault();
        var nRow = $(this).parents('tr')[0];
        var nRowId = $(this).closest('tr').attr("id");
        var aData = oTableAntibiotics.fnGetData(nRow);

        var delConfirm = confirm("Are you sure you want to delete?");
        if (!delConfirm)
            return;

        $.ajax({
            url: path + "JSON/DeleteRowAdmin",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                rowId: nRowId,
                appCode: "IFC",
                contextId: antibioticContextId
            },
            success: function (result) {
                if (result.Success) {
                    oTableAntibiotics.fnDeleteRow(nRow);
                } else {
                    alert("Antibiotic has failed to be deleted. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }
            }
        });
    });

    $("#antibioticTab").find("[type='submit']").click(function (e) {
        e.preventDefault();
        var form = $("#antibioticTab form");
        var code = $("#NewPrecaution_PatientIFCTypeOfPrecautionName").val();
        var dataTable = $("#Antibiotics").dataTable();
        if (alreadyExist(code, dataTable)) {
            alert("Antibiotic already exists");
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
                    var name = result.data.PatientIFCAntibioticName;
                    var sort = result.data.SortOrder;
                    var enabled = result.data.DataEntryFlg;
                    var id = result.data.PatientIFCAntibioticId;

                    var addId = dataTable.fnAddData([
                        name,
                        sort,
                        createCheckboxString("EnableDisable", "EnableDisableAntibiotic", "EnableDisable", enabled),
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

function initCommunities() {
    //Community Table
    var oTableCom = $('#CommunityTable').dataTable({
        "bFilter": false,
        "bAutoWidth": false,
        "sScrollY": "600px",
        "sDom": "frtS",
        "iDisplayLength": -1,
        "oLanguage": {
            "sEmptyTable": "No Communities"
        }
    });
}

function initShared() {

    $(".checkbox").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "Base/ChangeDataFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "IFC"
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Community has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $(".checkboxReport").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var comId = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();

        $.ajax({
            url: path + "Base/ChangeReportFlgCom",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: comId,
                dFlag: checked,
                appCode: "IFC"
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });

    $(".checkboxPayer").click(function () {
        var checkbox = $(this);
        var checked = $(this).is(":checked");
        var payer = $(this).attr("id");
        var comName = $(this).parents("tr").children(":first-child").text();
        var nRowId = $(this).closest('tr').attr("id");

        $.ajax({
            url: path + "Base/ChangePayerIncludeFlg",
            dataType: "json",
            cache: false,
            type: 'POST',
            data: {
                community: nRowId,
                dFlag: checked,
                appCode: "IFC",
                payer: payer
            },
            success: function (result) {
                if (result.Success) {

                } else {
                    checkbox.prop("checked", !checked);
                    alert("Community report flag has failed to be enabled/disabled. Please try again " +
                        "and if you see this message repeatedly the server may be down.");
                }

            }
        });
    });
}

function initLimits() {
    //Limits
    $("#symptomsLimit .updateLimit").click(function () {
        $.post(path + "JSON/UpdateLimit", { max: $("#Limits_SymptomMax").val(), contextId: symptomContextId }).done(function () {
            alert("Symptom limit has been successfully updated.");
        });
    });
    $("#diagnosesLimit .updateLimit").click(function () {
        $.post(path + "JSON/UpdateLimit", { max: $("#Limits_DiagnosisMax").val(), contextId: diagnosisContextId }).done(function () {
            alert("Diagnosis limit has been successfully updated.");
        });
    });
    $("#precautionsLimit .updateLimit").click(function () {
        $.post(path + "JSON/UpdateLimit", { max: $("#Limits_TypeOfPrecautionMax").val(), contextId: precautionContextId }).done(function () {
            alert("Type of Precaution limit has been successfully updated.");
        });
    });
    $("#antibioticsLimit .updateLimit").click(function () {
        $.post(path + "JSON/UpdateLimit", { max: $("#Limits_AntibioticMax").val(), contextId: antibioticContextId }).done(function () {
            alert("Antibiotic limit has been successfully updated.");
        });
    });
    //DB - Per Rick 2015/06/15 - Using DB values for Organism data requirements
    $("#organismsLimit .updateLimit").click(function () {
        $.post(path + "JSON/UpdateLimit", { max: $("#Limits_OrganismMax").val(), contextId: organismContextId }).done(function () {
            alert("Organism limit has been successfully updated.");
        });
    });

    $("#symptomsLimit .Required").click(function () {
        var checked = $("#symptomsLimit .Required").is(":checked");
        $.post(path + "JSON/ChangeRequiredFlgIFC", { dFlag: checked, contextId: symptomContextId }).done(function () {

        });
    });
    $("#diagnosesLimit .Required").click(function () {
        var checked = $("#diagnosesLimit .Required").is(":checked");
        $.post(path + "JSON/ChangeRequiredFlgIFC", { dFlag: checked, contextId: diagnosisContextId }).done(function () {

        });
    });
    $("#precautionsLimit .Required").click(function () {
        var checked = $("#precautionsLimit .Required").is(":checked");
        $.post(path + "JSON/ChangeRequiredFlgIFC", { dFlag: checked, contextId: precautionContextId }).done(function () {

        });
    });
    $("#antibioticsLimit .Required").click(function () {
        var checked = $("#antibioticsLimit .Required").is(":checked");
        $.post(path + "JSON/ChangeRequiredFlgIFC", { dFlag: checked, contextId: antibioticContextId }).done(function () {

        });
    });
    $("#organismsLimit .Required").click(function () {
        var checked = $("#organismsLimit .Required").is(":checked");
        $.post(path + "JSON/ChangeRequiredFlgIFC", { dFlag: checked, contextId: organismContextId }).done(function () {

        });
    });
}

$(document).ready(function () {
    $("#tabs").tabs();

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

    initSites();
    initOrganism();
    initSymptoms();
    initDiagnoses();
    initPrecautions();
    initAntibiotics();
    initCommunities();
    initShared();
    initLimits();
    
    $("#SaveLookback").validate({
        rules: {
            LookbackDays: {
                required: true,
                number: true
            }
        }
    });
});

function saveRowSite(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value,
            appCode: "IFC",
            contextId: siteContextId
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
}
function saveRowOrganism(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value,
            appCode: "IFC",
            contextId: organismContextId
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
}

function saveRowSymptom(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value,
            appCode: "IFC",
            contextId: symptomContextId
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
}

function saveRowDiagnosis(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value,
            appCode: "IFC",
            contextId: diagnosisContextId
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
}

function saveRowPrecaution(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value,
            appCode: "IFC",
            contextId: precautionContextId
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
}

function saveRowAntibiotic(oTable, nRow, nRowId) {
    var jqInputs = $('input', nRow);

    $.ajax({
        url: path + "JSON/EditRowNameOrder",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            rowId: nRowId,
            description: jqInputs[0].value,
            order: jqInputs[1].value,
            appCode: "IFC",
            contextId: antibioticContextId
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
}

//Shared
function restoreRow(oTable, nRow) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    for (var i = 0, iLen = jqTds.length; i < iLen; i++) {
        oTable.fnUpdate(aData[i], nRow, i, false);
    }
}

function editRow(oTable, nRow, maxLength) {
    var aData = oTable.fnGetData(nRow);
    var jqTds = $('>td', nRow);
    if (maxLength)
        jqTds[0].innerHTML = '<input type="text" value="' + aData[0] + '" maxlength="' + maxLength + '"/>';
    else
        jqTds[0].innerHTML = '<input type="text" value="' + aData[0] + '"/>';
    jqTds[1].innerHTML = '<input type="text" value="' + aData[1] + '" maxlength="4" size="1" />';
    jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
}

function createCheckboxString(id, cssClass, name, value) {
    if (value) {
        var checked = "checked";
    }
    var html = '<input class="' + cssClass + '" id="' + id + '" name="' + name + '" type="checkbox" value="' + value + '" ' + checked + '>';
    html += '<input name="' + name + '" type="hidden" value="' + value + '">';
    return html;
}

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

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});