$(document).ready(function () {
        var restraintContextId = 9;

        var oTableRest = $('#restraint').dataTable({
            "bFilter": false,
            "bAutoWidth": false,
            "sScrollY": "200px",
            "sDom": "frtS",
            "iDisplayLength": -1,
            "aoColumns": [
                {
                    "sWidth": '400px'
                },
                null,
                {
                    "bSortable": false
                },
                {
                    "bSortable": false
                },
                {
                    "bSortable": false
                }
            ],
            "oLanguage": {
                "sEmptyTable": "No Restraints"
            }
        });
        oTableRest.fnSort([[1, 'asc']]);
        var nEditing = null;

        $("#restraint .EnableDisable").click(function () {
            var checkbox = $(this);
            var checked = $(this).is(":checked");
            var nRow = $(this).parents('tr')[0];
            var aData = oTableRest.fnGetData(nRow);

            $.ajax({
                url: path + "JSON/ChangeDataFlg",
                dataType: "json",
                cache: false,
                type: 'POST',
                data: {
                    description: aData[0],
                    dFlag: checked,
                    appCode: "SOC",
                    socCode: restraintContextId
                },
                success: function (result) {
                    if (result.Success) {
                        var message = aData[0] + " has been successfully ";
                        if (checked)
                            message += "enabled.";
                        else
                            message += "disabled.";
                        alert(message);
                    } else {
                        checkbox.prop("checked", !checked);
                        alert("Restraint has failed to be enabled/disabled. Please try again " +
                            "and if you see this message repeatedly the server may be down.");
                    }

                }
            });
        });

        $("#restraint a.delete").live("click", function (e) {
            e.preventDefault();
            var nRow = $(this).parents('tr')[0];
            var nRowId = $(this).closest('tr').attr("id");
            var aData = oTableRest.fnGetData(nRow);

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
                    appCode: "SOC",
                    socCode: restraintContextId
                },
                success: function (result) {
                    if (result.Success) {
                        oTableRest.fnDeleteRow(nRow);
                    } else {
                        alert("Restraint has failed to be deleted. Please try again " +
                            "and if you see this message repeatedly the server may be down.");
                    }

                }
            });
        });

        //Edit
        $('#restraint a.edit').live('click', function (e) {
            e.preventDefault();
            /* Get the row as a parent of the link that was clicked on */
            var nRow = $(this).parents('tr')[0];
            if (nEditing !== null && nEditing != nRow) {
                /* A different row is being edited - the edit should be cancelled and this row edited */
                restoreRow(oTableRest, nEditing);
                editRow(oTableRest, nRow);
                nEditing = nRow;

            }
            else if (nEditing == nRow && this.innerHTML == "Save") {
                /* This row is being edited and should be saved */
                var nRowId = $(this).closest('tr').attr("id");
                var success = saveRowRest(oTableRest, nEditing, nRowId);
                if (success) {
                    nEditing = null;
                }

            }
            else {
                /* No row currently being edited */
                editRow(oTableRest, nRow);
                nEditing = nRow;

            }
        });
        $('#restraint a.cancel').live('click', function (e) {
            e.preventDefault();

            var nRow = $(this).parents('tr')[0];
            restoreRow(oTableRest, nRow);
        });

        function restoreRow(oTable, nRow) {
            var aData = oTable.fnGetData(nRow);
            var jqTds = $('>td', nRow);

            for (var i = 0, iLen = jqTds.length ; i < iLen ; i++) {
                oTable.fnUpdate(aData[i], nRow, i, false);
            }
        }
        function editRow(oTable, nRow) {
            var aData = oTable.fnGetData(nRow);
            var jqTds = $('>td', nRow);
            jqTds[0].innerHTML = '<input type="text" size="45" value="' + aData[0] + '"/>';
            jqTds[1].innerHTML = '<input type="text" size="3" value="' + aData[1] + '"/>';
            jqTds[3].innerHTML = '<a class="edit" href="">Save</a> <a class="cancel" href="">Cancel</a>';
        }

        function saveRowRest(oTable, nRow, nRowId) {
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
                    appCode: "SOC",
                    socCode: restraintContextId
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

    });