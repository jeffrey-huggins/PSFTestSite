var previousUser;
var objectAccessChanged;
var currentObjId;
var accessChanged;
var currentAppId;

function initGeneralAccess() {
    $("#applicationAccess").DataTable({
        "filter": false,
        "autoWidth": true,
        "scrollY": "400px",
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
            "sEmptyTable": "No Records"
        }
    });
    $("#currentCommunitiesContainer").find("table").DataTable({
        "filter": false,
        "autoWidth": true,
        "scrollY": "400px",
        "dom": "frtS",
        "displayLength": -1,
        "columns": [
            null,//Community
            {
                "searchable": false,
                "sortable": false
            },//App Flag
            {
                "searchable": false,
                "sortable": false
            },//Report Flag,
            {
                "searchable": false,
                "sortable": false
            },//Delete Flag
            {
                "searchable": false,
                "sortable": false
            }//exists Flag
        ],
        "language": {
            "emptyTable": "No Records"
        }
    });
    $("#AllDelete").change(function () {
        if ($("#AllDelete").prop("checked")) {
            $(".deleteFlag").change();
        }
        $("#AllDelete").prop("checked", false);
    });
    $("#AllApps").change(function () {
        if ($("#AllApps").prop("checked")) {
            var nonCheckedItems = $(".appFlag:not(:checked)");
            nonCheckedItems.prop("checked", true);
            nonCheckedItems.change();
        }
        if (!$("#AllApps").prop("checked")) {
            var checkedItems = $(".appFlag:checked");
            checkedItems.prop("checked", false);
            checkedItems.change();
        }
    });
    $("#AllReport").change(function () {
        if ($("#AllReport").prop("checked")) {
            var nonCheckedItems = $(".reptFlag:not(:checked)");
            nonCheckedItems.prop("checked", true);
            nonCheckedItems.change();
        }
        if (!$("#AllReport").prop("checked")) {
            var checkedItems = $(".reptFlag:checked");
            checkedItems.prop("checked", false);
            checkedItems.change();
        }
    });
    $(".reptFlag").change(function (event) {
        var row = $(event.currentTarget).closest("tr");
        updateGeneralAccess(row);
        $("#AllDelete").prop("checked", false);
        if (!$(event.currentTarget).prop("checked")) {
            $("#AllReport").prop("checked", false);
        }
    });
    $(".appFlag").change(function (event) {
        var row = $(event.currentTarget).closest("tr");
        updateGeneralAccess(row);
        $("#AllDelete").prop("checked", false);
        if (!$(event.currentTarget).prop("checked")) {
            $("#AllApps").prop("checked", false);
        }
    });
    $(".deleteFlag").change(function (event) {
        $("#AllApps").prop("checked", false);
        $("#AllReport").prop("checked", false);
        var row = $(event.currentTarget).closest("tr");
        if (row.find(".deleteFlag").prop("checked")) {
            row.find(".reptFlag").prop("checked", false);
            row.find(".appFlag").prop("checked", false);
        }
        deleteGeneralAccess(row);
    });
    $("#currentCommunitiesContainer").find("input").change(function () {
        var appTable = $("#applicationAccessContainer").find("table");

        if ($("#currentCommunitiesContainer").find(".reptFlag:checked").length > 0 || $("#currentCommunitiesContainer").find(".appFlag:checked").length) {
            appTable.find(".highlight").find("input").prop("checked", true);
        } else {
            appTable.find(".highlight").find("input").prop("checked", false);
        }
    });
    $("#applicationAccess").find("tr").click(function () {
        $("#applicationAccess").find("tr").removeClass("highlight");
        $("#applicationAccess").find("tr").addClass("borderRow");
        $(this).addClass("highlight");
        $(this).removeClass("borderRow");
        $("#AllApps").prop("checked", false);
        $("#AllDelete").prop("checked", false);
        $("#AllReport").prop("checked", false);
        getCommunities($(this).attr("appId"));
    });
}

function updateGeneralAccess(row) {
    if (!row.is(":visible")) {
        return;
    }
    var userId = $("#UserList").val();
    var reportFlag = row.find(".reptFlag").prop("checked");
    var appFlag = row.find(".appFlag").prop("checked");
    var community = row.attr("commid");
    $.ajax({
        url: path + "UserAdmin/UpdateUserAppAccess",
        type: "POST",
        data: {
            userId: userId,
            appId: currentAppId,
            communityId: community,
            reportFlag: reportFlag,
            appFlag: appFlag
        }
    }).done(function (result) {
        if (result.Success) {
            row.find(".notExists").hide();
            row.find(".exists").show();
            $("#applicationAccess .highlight").find(".exists").show();
            $("#applicationAccess .highlight").find(".notExists").hide();
            
        }
    }).fail(function () {
        alert("Unable to update row, try again");
    });
}

function deleteGeneralAccess(row) {
    var userId = $("#UserList").val();
    var community = row.attr("commid");
    $.ajax({
        url: path + "UserAdmin/DeleteUserAppAccess",
        type: "POST",
        data: {
            userId: userId,
            appId: currentAppId,
            communityId: community
        }
    }).done(function (result) {
        if (result.Success) {
            row.find(".exists").hide();
            row.find(".notExists").show();
            row.find(".appFlag").prop("checked", false);
            row.find(".deleteFlag").prop("checked", false);
            row.find(".reptFlag").prop("checked", false);
            if ($("#communityTable .exists:visible").length == 0) {
                $("#applicationAccess .highlight").find(".exists").hide();
                $("#applicationAccess .highlight").find(".notExists").show();
            }
        }
    }).fail(function () {
        alert("Unable to delete row, try again");
    });
}

function initObjectAccess() {
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
            "sEmptyTable": "No Records"
        }
    });
    $("#objectAccess").find("tr").click(function () {
        $("#objectAccess").find("tr").removeClass("highlight");
        $("#objectAccess").find("tr").addClass("borderRow");
        $(this).addClass("highlight");
        $(this).removeClass("borderRow");
        $("#AllObjDelete").prop("checked", false);
        $("#AllObjAccess").prop("checked", false);
        getObjectCommunities($(this).attr("objectid"));
    });
    $("#currentObjectCommunitiesContainer").find("table").DataTable({
        "filter": false,
        "autoWidth": true,
        "scrollY": "400px",
        "dom": "frtS",
        "displayLength": -1,
        "columns": [
            null,//Community
            {
                "searchable": false,
                "sortable": false
            },//Access Flag
            {
                "searchable": false,
                "sortable": false
            },//Delete Flag
            {
                "searchable": false,
                "sortable": false
            }//exists Flag
        ],
        "language": {
            "emptyTable": "No Records"
        }
    });
    $("#AllObjDelete").change(function () {
        if ($("#AllObjDelete").prop("checked")) {
            $(".deleteObjFlag").change();
        }
        $("#AllObjDelete").prop("checked", false);
    });
    $("#AllObjAccess").change(function () {
        if ($("#AllObjAccess").prop("checked")) {

            var nonCheckedItems = $("#communityObjTable tr:visible .accessFlag:not(:checked)");
            nonCheckedItems.prop("checked", true);
            nonCheckedItems.change();
        }
        if (!$("#AllObjAccess").prop("checked")) {
            var checkedItems = $("#communityObjTable tr:visible .accessFlag:checked");
            checkedItems.prop("checked", false);
            checkedItems.change();
        }
    });
    $(".accessFlag").change(function (event) {
        var row = $(event.currentTarget).closest("tr");
        updateObjectAccess(row);
        $("#AllObjDelete").prop("checked", false);
        if (!$(event.currentTarget).prop("checked")) {
            $("#AllObjAccess").prop("checked", false);
        }

    });
    $(".deleteObjFlag").change(function (event) {
        $("#AllObjAccess").prop("checked", false);
        var row = $(event.currentTarget).closest("tr");
        deleteObjectAccess(row);
    });
    $("#currentObjectCommunitiesContainer").find("input").change(function () {
        var objTable = $("#objectAccessContainer").find("table");
        if ($("#currentObjectCommunitiesContainer").find(".accessFlag:checked").length > 0) {
            objTable.find(".highlight").find("input").prop("checked", true);
        } else {
            objTable.find(".highlight").find("input").prop("checked", false);
        }
    });
}

function updateObjectAccess(row) {
    if (!row.is(":visible")) {
        return;
    }
    var userId = $("#UserList").val();
    var objId = $("#objectAccessContainer").find(".highlight").attr("objectid");
    var accessFlag = row.find(".accessFlag").prop("checked");
    var community = row.attr("objcommid");
    $.ajax({
        url: path + "UserAdmin/UpdateUserObjectAccess",
        type: "POST",
        data: {
            userId: userId,
            objectId: objId,
            communityId: community,
            accessFlag: accessFlag,
        }
    }).done(function (result) {
        if (result.Success) {
            row.find(".notExists").hide();
            row.find(".exists").show();
            $("#objectAccess .highlight").find(".exists").show();
            $("#objectAccess .highlight").find(".notExists").hide();
        }
    }).fail(function () {
        alert("Unable to update row, try again");
    });
}

function deleteObjectAccess(row) {
    var userId = $("#UserList").val();
    var community = row.attr("objcommid");
    var objId = $("#objectAccessContainer").find(".highlight").attr("objectid");
    $.ajax({
        url: path + "UserAdmin/DeleteUserObjectAccess",
        type: "POST",
        data: {
            userId: userId,
            objectId: objId,
            communityId: community
        }
    }).done(function (result) {
        if (result.Success) {
            row.find(".exists").hide();
            row.find(".notExists").show();
            row.find(".accessFlag").prop("checked", false);
            row.find(".deleteObjFlag").prop("checked", false);
            if ($("#communityObjTable .exists:visible").length == 0) {
                $("#objectAccessContainer .highlight").find(".exists").hide();
                $("#objectAccessContainer .highlight").find(".notExists").show();
            }
        }
    }).fail(function () {
        alert("Unable to delete row, try again");
    });
}

function initAdminAccess() {
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
            null,//Application Group
            null,//Is Admin
            null//Delete
        ],
        "language": {
            "sEmptyTable": "No Records"
        }
    });
    $("#adminAccess").find(".deleteAdmin").click(function () {
        var row = $(this).closest("tr");
        deleteAdminAccess(row);
    });
    $("#adminAccess").find(".isAdmin").click(function () {
        var row = $(this).closest("tr");
        updateAdminAccess(row);
    });
}

function updateAdminAccess(row) {
    if (!row.is(":visible")) {
        return;
    }
    var userId = $("#UserList").val();
    var isAdmin = row.find(".isAdmin").prop("checked");
    var appId = row.attr("adminappid");
    $.ajax({
        url: path + "UserAdmin/UpdateUserAdminAccess",
        type: "POST",
        data: {
            userId: userId,
            appId: appId,
            isAdmin: isAdmin
        }
    }).done(function (result) {
        if (result.Success) {
            row.find(".notExists").hide();
            row.find(".exists").show();
        }
    }).fail(function () {
        alert("Unable to update row, try again");
    });
}

function deleteAdminAccess(row) {
    var userId = $("#UserList").val();
    var appId = row.attr("adminappid");
    $.ajax({
        url: path + "UserAdmin/DeleteUserAdminAccess",
        type: "POST",
        data: {
            userId: userId,
            appId: appId
        }
    }).done(function (result) {
        if (result.Success) {
            row.find(".exists").hide();
            row.find(".notExists").show();
            row.find(".isAdmin").prop("checked", false);
            row.find(".deleteAdmin").prop("checked", false);
        }
    }).fail(function () {
        alert("Unable to delete row, try again");
    });
}

function resetPage() {
    window.ShowProgress();
    objectAccessChanged = {};
    accessChanged = {};
    createObjectcommunitySelector();
    getAdminAccess();
    $.when(getApplicationAccess(), getObjectAccess()).then(function () {
        var appTable = $("#applicationAccessContainer");
        var objTable = $("#objectAccessContainer");
        if (appTable.find(".highlight").length > 0) {
            var appId = appTable.find(".highlight").attr("appid");
            getCommunities(appId);
        }
        else {
            $("#communitySection").hide();
        }
        if (objTable.find(".highlight").length > 0) {
            var objId = objTable.find(".highlight").attr("objectid");
            getObjectCommunities(objId);
        }
        else {
            $("#objectCommunitySection").hide();
        }
        window.HideProgress();
    });
}

function initPage() {
    initGeneralAccess();
    initObjectAccess();
    initAdminAccess();
    $("#userAutoList").focus(function () {
        previousUser = $("#userAutoList").val();
        $("#userAutoList").val("");
    });
    $("#userAutoList").blur(function () {
        if ($("#userAutoList").val() == "") {
            $("#userAutoList").val(previousUser);
        }

    });
    $("#tabs").tabs({
        activate: function (event, ui) {
            var dataTables = ui.newPanel.find(".dataTable").filter(function () {
                return this.id != "";
            });
            for (i = 0; i < dataTables.length; i++) {
                var dataTable = $("#" + dataTables[i].id).dataTable();
                dataTable.fnAdjustColumnSizing();
            }

            if (ui.newTab.find("a").attr("href") == "#Object") {
                resetPage();
            }
        }
    });
    $(".ui-tabs-nav").css("display", "inline-block");
    $("#userAutoList").autocomplete({
        source: userList,
        select: function (event, ui) {
            event.preventDefault();
            objectAccessChanged = null;
            accessChanged = null;
            $("#UserList").val(ui.item.value);
            $("#userAutoList").val(ui.item.label);

            resetPage();

            
        }
    });
    getAdminAccess();
    getApplicationAccess();
    getObjectAccess();
   //getObjectAccess
}
function getAdminAccess() {
    window.ShowProgress();
    $("#adminAccess").find("input").prop("checked", false);
    var id = $("#UserList").val();
    $.ajax({
        url: path + "UserAdmin/GetAdminAccess",
        type: "POST",
        data: {
            userId: id
        }
    }).done(function (result) {
        for (var i = 0; i < result.length; i++) {
            var appId = result[i];
            $("#adminapplication" + appId).find(".isAdmin").prop("checked", true);
        }
    }).always(function () {
        window.HideProgress();
    });
}
function getApplicationAccess() {
    window.ShowProgress();
    accessChanged = {};
    $("#objectAccess tbody tr").hide();
    $("#applicationAccess").find(".exists").hide();
    $("#applicationAccess").find(".notExists").show();
    $("#applicationAccess").find("input").prop("checked", false);

    var id = $("#UserList").val();
    return $.ajax({
        url: path + "UserAdmin/GetUserAppAccess",
        type: "POST",
        data: {
            id: id
        }
    }).done(function (result) {
        for (var i = 0; i < result.length; i++) {
            var appId = result[i];
            $("#objectAccess [appid='" + appId + "']").show();
            $("#application" + appId).find("input").prop("checked", true);
            $("#application" + appId).find(".notExists").hide();
            $("#application" + appId).find(".exists").show();
        }
    }).always(function () {
        window.HideProgress();
    });
}

function getObjectAccess() {
    window.ShowProgress();
    objectAccessChanged = {};
    $("#objectAccess").find("input").prop("checked", false);
    $("#objectAccess").find(".notExists").show();
    $("#objectAccess").find(".exists").hide();
    var id = $("#UserList").val();
    return $.ajax({
        url: path + "UserAdmin/GetUserObjectAccess",
        type: "POST",
        data: {
            userId: id
        }
    }).done(function (result) {
        for (var i = 0; i < result.length; i++) {
            var objectId = result[i];
            $("#object" + objectId).find("input").prop("checked", true);
            $("#object" + objectId).find(".notExists").hide();
            $("#object" + objectId).find(".exists").show();
        }

    }).always(function () {
        window.HideProgress();
    });

}

function getObjectCommunities(objId) {
    var communityList = objectAccessChanged["'" + objId + "'"];
    var appId = $("#object" + objId).attr("appid");
    showObjectCommunities(appId);
    $("#currentObjectCommunitiesContainer").find(".notExists").show();
    $("#currentObjectCommunitiesContainer").find(".exists").hide();
    if (communityList) {
        createObjectcommunitySelector();

        for (var i = 0; i < communityList.length; i++) {
            var communityInfo = communityList[i];
            var commId = communityInfo.CommunityId;
            var enabledFlag = communityInfo.EnabledFlag;
            var deleteFlag = communityInfo.DeleteFlag;
            var rowExists = communityInfo.RowExists;
            var row = $("[objcommid='" + commId + "'");
            row.find(".accessFlag").prop("checked", enabledFlag);
            row.find(".deleteObjFlag").prop("checked", deleteFlag);
            if (rowExists) {
                row.find(".notExists").hide();
                row.find(".exists").show();
            }
            else {
                row.find(".notExists").show();
                row.find(".exists").hide();
            }

            row.find(".rowExists").prop("checked", rowExists);
        }
    }
    else {
        var userId = $("#UserList").val();
        window.ShowProgress();
        $.ajax({
            url: path + "UserAdmin/GetUserObjectCommunityAccess",
            type: "POST",
            data: {
                userId: userId,
                objectId: objId
            }
        }).done(function (result) {
            createObjectcommunitySelector();
            for (var i = 0; i < result.length; i++) {
                var communityInfo = result[i];
                var appId = result[i].CommunityId;
                var row = $("[objcommid='" + appId + "'");
                row.find(".accessFlag").prop("checked", result[i].EnabledFlag);
                row.find(".deleteObjFlag").prop("checked", result[i].DeleteFlag);
                row.find(".rowExists").prop("checked", result[i].RowExists);
                if (result[i].RowExists) {
                    row.find(".notExists").hide();
                    row.find(".exists").show();
                }
                else {
                    row.find(".notExists").show();
                    row.find(".exists").hide();
                }
            }
        }).always(function () {
            window.HideProgress();
        });
    }
    $("#objectCommunitySection").show();
    $("#communityObjTable").DataTable().columns.adjust();
    currentObjId = objId;
}

function showObjectCommunities(appId) {
    var userId = $("#UserList").val();
    $.ajax({
        url: path + "UserAdmin/GetUserCommunityAccess",
        type: "POST",
        data: {
            userId: userId,
            appId: appId
        }
    }).done(function (result) {
        $("#communityObjTable tbody tr").hide();
        for (var i = 0; i < result.length; i++) {
            var appId = result[i].CommunityId;
            var access = result[i].AppFlg;
            if (access) {
                $("#communityObjTable [objcommid='" + appId + "']").show();
            }
        }
    });
}

function createObjectcommunitySelector() {
    $("#communityObjTable").find("input").prop("checked", false);

}

function getCommunities(appId) {
    var communityList = accessChanged["'" + appId + "'"];

    $.ajax({
        url: path + "UserAdmin/GetCommunitiesForApp",
        type: "POST",
        data: {
            appId: appId
        }
    }).done(function (result) {
        $("#communityTable tr").hide();
        $("#communityTable tr").removeClass("highlight");
        for (var i = 0; i < result.CommunityIdList.length; i++) {
            var app = result.CommunityIdList[i];
            $("[commId='" + app + "']").show();
        }
        $("#communityTable").DataTable().columns.adjust();
    });

    $("#currentCommunitiesContainer").find(".notExists").show();
    $("#currentCommunitiesContainer").find(".exists").hide();
    if (communityList) {
        createCommunitySelector();

        for (var i = 0; i < communityList.length; i++) {
            var communityInfo = communityList[i];
            var commId = communityInfo.CommunityId;
            var appFlag = communityInfo.AppFlg;
            var reportFlag = communityInfo.ReportFlg;
            var deleteFlag = communityInfo.DeleteFlg;
            var rowExists = communityInfo.RowExists;
            var row = $("[commId='" + commId + "'");
            row.find(".appFlag").prop("checked", appFlag);
            row.find(".reptFlag").prop("checked", reportFlag);
            row.find(".deleteFlag").prop("checked", deleteFlag);
            row.find(".rowExists").prop("checked", rowExists);
            if (rowExists) {
                row.find(".notExists").hide();
                row.find(".exists").show();
            }
            else {
                row.find(".notExists").show();
                row.find(".exists").hide();
            }
        }
    }
    else {
        var userId = $("#UserList").val();
        window.ShowProgress();
        $.ajax({
            url: path + "UserAdmin/GetUserCommunityAccess",
            type: "POST",
            data: {
                userId: userId,
                appId: appId
            }
        }).done(function (result) {
            createCommunitySelector();
            for (var i = 0; i < result.length; i++) {
                var communityInfo = result[i];
                var appId = result[i].CommunityId;
                var row = $("[commId='" + appId + "']");
                if (!row.is(":visible")) {
                    row.addClass("highlight");
                    row.show();
                }
                row.find(".appFlag").prop("checked", result[i].AppFlg);
                row.find(".reptFlag").prop("checked", result[i].ReportFlg);
                row.find(".deleteFlag").prop("checked", result[i].DeleteFlg);
                row.find(".rowExists").prop("checked", result[i].RowExists);
                if (result[i].RowExists) {
                    row.find(".notExists").hide();
                    row.find(".exists").show();
                }
                else {
                    row.find(".notExists").show();
                    row.find(".exists").hide();
                }
            }
        }).fail(function () {
            alert("Error retrieving user's community access");

            }).always(function () {
                
            window.HideProgress();
        });
    }
    $("#communitySection").show();
    $("#communityTable").DataTable().columns.adjust();
    currentAppId = appId;

}

function createCommunitySelector() {
    $("#communityTable").find("input").prop("checked", false);
}

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxStop(function () {
    HideProgress();
});

initPage();