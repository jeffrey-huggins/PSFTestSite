function initGeneralAccess() {
    $("#applicationAccess").DataTable({
        "filter": false,
        "scrollY": "400px",
        "autoWidth": true,
        "dom": "frtS",
        "displayLength": -1,
        "sorting": [[2, "asc"]],
        "columns": [
            null,//Application Name
            null,//Application Code
            null,//Application Group
            null
        ],
        "language": {
            "emptyTable": "No Records"
        }
    });
    $("#appUsers").DataTable({
        "filter": false,
        "autoWidth": true,
        "scrollY": "400px",
        "dom": "frtS",
        "displayLength": -1,
        "language": {
            "emptyTable": "No users have access to this app"
        }
    });
    $("#applicationAccess").find("tr").click(function (event) {
        var element = event.target;
        if (!element) {
            element = event.srcElement;
        }
        if (element.localName == "input") {
            return;
        }
        $("#applicationAccess").find("tr").removeClass("highlight");
        $("#applicationAccess").find("tr").addClass("borderRow");
        $(this).addClass("highlight");
        $(this).removeClass("borderRow");
        getApplicationUserAccess($(this).attr("appId"));
    });
    $(".appAccess").change(function (event) {
        var allowAccess = $(this).prop("checked");
        var id = $(this).closest("tr").attr("appId");
        setApplicationUserAccess(id, allowAccess);
    });
}

function initObjectAccess() {
    $("#objectUsers").DataTable({
        "filter": false,
        "autoWidth": true,
        "scrollY": "400px",
        "dom": "frtS",
        "displayLength": -1,
        "language": {
            "sEmptyTable": "No users have access to this object"
        }
    });
    $("#objectAccess").DataTable({
        "filter": false,
        "autoWidth": true,
        "scrollY": "400px",
        "dom": "frtS",
        "displayLength": -1,
        "sorting": [[2, "asc"], [1, "asc"]],
        "columns": [
            null,//Object Desc
            null,//Object Code
            null,//Application Name
            null
        ],
        "language": {
            "emptyTable": "No Records"
        }
    });

    $("#objectAccess").find("tr").click(function () {
        var element = event.target;
        if (!element) {
            element = event.srcElement;
        }
        if (element.localName == "input") {
            return;
        }
        $("#objectAccess").find("tr").removeClass("highlight");
        $("#objectAccess").find("tr").addClass("borderRow");
        $(this).addClass("highlight");
        $(this).removeClass("borderRow");
        getObjectUserAccess($(this).attr("objectid"));
    });

    $(".objectAccess").change(function (event) {
        var allowAccess = $(this).prop("checked");
        var id = $(this).closest("tr").attr("objectid");
        setObjectUserAccess(id, allowAccess);
    });
}

function initAdminAccess() {

    $("#adminUsers").DataTable({
        "filter": false,
        "autoWidth": true,
        "scrollY": "400px",
        "dom": "frtS",
        "displayLength": -1,
        "language": {
            "sEmptyTable": "No admins are setup for this app"
        }
    });
    $("#adminAccess").DataTable({
        "filter": false,
        "autoWidth": true,
        "scrollY": "400px",
        "dom": "frtS",
        "displayLength": -1,
        "sorting": [[2, "asc"]],
        "columns": [
            null,//Application Name
            null,//Application Code
            null//Application Group
        ],
        "language": {
            "emptyTable": "No Records"
        }
    });
    $("#adminAccess").find("tr").click(function () {
        var element = event.target;
        if (!element) {
            element = event.srcElement;
        }
        if (element.localName == "input") {
            return;
        }
        $("#adminAccess").find("tr").removeClass("highlight");
        $("#adminAccess").find("tr").addClass("borderRow");
        $(this).addClass("highlight");
        $(this).removeClass("borderRow");
        getAdminList($(this).attr("adminappid"));
    });
}

function initCommunityStatus() {
    $("#communityTable").DataTable({
        "filter": false,
        "autoWidth": true,
        "scrollY": "400px",
        "dom": "frtS",
        "displayLength": -1,
        "language": {
            "emptyTable": "No communities exist"
        }
    });

    $(".allDataEntry").on("change", function () {
        $("#communityTable").find(".dataFlag").prop("checked", $(".allDataEntry").prop("checked"));
        $("#communityTable").find(".dataFlag").change();
    });

    $(".allReport").on("change", function () {
        $("#communityTable").find(".reportFlag").prop("checked", $(".allReport").prop("checked"));
        $("#communityTable").find(".reportFlag").change();
    });

    $("#communityTable").on("change","input",function () {
        var row = $(this).closest("tr");
        var appId = row[0].id;
        var commId = $("#applicationStatus option:selected").val();
        var dataFlag = row.find(".dataFlag").prop("checked");
        var reportFlag = row.find(".reportFlag").prop("checked");
        $.ajax({
            url: path + "AccessAdmin/SetCommunityInfo",
            type: "POST",
            data: {
                communityId: commId,
                applicationId: appId,
                dataFlag: dataFlag,
                reportFlag: reportFlag
            }
        }).done(function (result) {
            if (result.Success) {
                row.find(".notExists").hide();
                row.find(".exists").show();
            }
            else {
                alert("Unable to set flags, server may be down, try again");
            }
        }).fail(function () {
            alert("Unable to set flags, server may be down, try again");
        });

    });

    $("#applicationStatus").on("change", function () {
        var id = $("#applicationStatus option:selected").val();
        getApplicationCommunityInfo(id);
    });
    var id = $("#applicationStatus option:selected").val();
    getApplicationCommunityInfo(id);
}

function getApplicationCommunityInfo(id) {

    $.ajax({
        url: path + "AccessAdmin/GetCommunityInfo",
        type: "POST",
        data: {
            communityId: id
        }
    }).done(function (result) {
        $(".allDataEntry").prop("checked", false);
        $(".allReport").prop("checked", false);
        $("#communityTable").find(".notExists").show();
        $("#communityTable").find(".exists").hide();
        $("#communityTable").find(".dataFlag").prop("checked", false);
        $("#communityTable").find(".reportFlag").prop("checked", false);
        for (var i = 0; i < result.length; i++) {
            var applicationId = result[i].ApplicationId;
            var dataEntryFlag = result[i].DataEntryFlg;
            var reportFlag = result[i].ReportFlg;
            var row = $("#communityTable").find("#" + applicationId);
            row.find(".notExists").hide();
            row.find(".exists").show();
            row.find(".dataFlag").prop("checked", dataEntryFlag);
            row.find(".reportFlag").prop("checked", reportFlag);
        }
    });
}

function initPage() {
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
    initGeneralAccess();
    initObjectAccess();
    initAdminAccess();
    initCommunityStatus();
    $(".ui-tabs-nav").css("display", "inline-block");

}
var test;
function getAdminList(appId) {
    $.ajax({
        url: path + "AccessAdmin/GetAdminAccess",
        type: "POST",
        data: {
            appId: appId
        }
    }).done(function (result) {
        var table = $("#adminUsers").DataTable();
        table.clear();
        var accessCollection = [];
        for (var i = 0; i < result.length; i++) {
            var access = [];
            access.push(result[i].UserDisplayName);
            access.push(result[i].AccountName);
            access.push("<a href='" + path + "UserAdmin?id=" + result[i].UserId + "' target='_blank'>Manage</a>");
            accessCollection.push(access);

        }
        table.rows.add(accessCollection);
        table.draw();
        table.columns.adjust();
    });
}

function getObjectUserAccess(objId) {
    
    $.ajax({
        url: path + "AccessAdmin/GetObjectAccess",
        type: "POST",
        data: {
            objectId: objId
        }
    }).done(function (result) {
        var table = $("#objectUsers").DataTable();
        table.clear();
        var accessCollection = [];
        for (var i = 0; i < result.length; i++) {
            var access = [];
            access.push(result[i].UserDisplayName);
            access.push(result[i].AccountName);
            access.push("<a href='" + path + "UserAdmin?id=" + result[i].UserId + "' target='_blank'>Manage</a>");
            accessCollection.push(access);

        }
        table.rows.add(accessCollection);
        table.draw();
        table.columns.adjust();
        
    });
}
function setObjectUserAccess(objId, allow) {
    
    $.ajax({
        url: path + "AccessAdmin/SetObjectAccess",
        type: "POST",
        data: {
            objectId: objId,
            allow: allow
        }
    }).done(function (result) {
        if (result.Success) {

        }
        else {
            alert("Error setting access, the server may be down, try again.");
        }
        
    });
}
function setApplicationUserAccess(appId, allow) {
    
    $.ajax({
        url: path + "AccessAdmin/SetAppAccess",
        type: "POST",
        data: { appId: appId, allow: allow }
    }).done(function (result) {
        if (result.Success) {
        }
        else {
            alert("Error setting access, the server may be down, try again.");
        }
        
    });
}

function getApplicationUserAccess(appId) {
    $.ajax({
        url: path + "AccessAdmin/GetAppAccess",
        method: "POST",
        data: { appId: appId }
    }).done(function (result) {
        var table = $("#appUsers").DataTable();
        table.clear();
        var accessCollection = [];
        for (var i = 0; i < result.length; i++) {
            var access = [];
            access.push(result[i].UserDisplayName);
            access.push(result[i].AccountName);
            access.push("<a href='" + path + "UserAdmin?id=" + result[i].UserId + "' target='_blank'>Manage</a>");
            accessCollection.push(access);

        }
        table.rows.add(accessCollection);
        table.draw();
        
    });
}


$(document).ajaxSend(function () {

    ShowProgress();
});

$(document).ajaxComplete(function () {
    HideProgress();
});

initPage();