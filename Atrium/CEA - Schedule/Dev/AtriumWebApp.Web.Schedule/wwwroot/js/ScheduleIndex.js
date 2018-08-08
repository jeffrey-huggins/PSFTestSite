/**
 * Protect window.console method calls, e.g. console is not defined on IE
 * unless dev tools are open, and IE doesn't define console.debug
 * 
 * http://stackoverflow.com/questions/3326650/console-is-undefined-error-for-internet-explorer
 * 
 * Chrome 41.0.2272.118: debug,error,info,log,warn,dir,dirxml,table,trace,assert,count,markTimeline,profile,profileEnd,time,timeEnd,timeStamp,timeline,timelineEnd,group,groupCollapsed,groupEnd,clear
 * Firefox 37.0.1: log,info,warn,error,exception,debug,table,trace,dir,group,groupCollapsed,groupEnd,time,timeEnd,profile,profileEnd,assert,count
 * Internet Explorer 11: select,log,info,warn,error,debug,assert,time,timeEnd,timeStamp,group,groupCollapsed,groupEnd,trace,clear,dir,dirxml,count,countReset,cd
 * Safari 6.2.4: debug,error,log,info,warn,clear,dir,dirxml,table,trace,assert,count,profile,profileEnd,time,timeEnd,timeStamp,group,groupCollapsed,groupEnd
 * Opera 28.0.1750.48: debug,error,info,log,warn,dir,dirxml,table,trace,assert,count,markTimeline,profile,profileEnd,time,timeEnd,timeStamp,timeline,timelineEnd,group,groupCollapsed,groupEnd,clear
 */
(function () {
    // Union of Chrome, Firefox, IE, Opera, and Safari console methods
    var methods = ["assert", "cd", "clear", "count", "countReset",
      "debug", "dir", "dirxml", "error", "exception", "group", "groupCollapsed",
      "groupEnd", "info", "log", "markTimeline", "profile", "profileEnd",
      "select", "table", "time", "timeEnd", "timeStamp", "timeline",
      "timelineEnd", "trace", "warn"];
    var length = methods.length;
    var console = (window.console = window.console || {});
    var method;
    var noop = function () { };
    while (length--) {
        method = methods[length];
        // define undefined methods as noops to prevent errors
        if (!console[method])
            console[method] = noop;
    }
})();

/*
 * This is turned OFF until a server side logging solutions needed.
 * The server needs and Event Log (eventVwr) with Security turned ON for installing the App Event Log.
 * 
 * A better solution is have a DB table and log events only when a server side web.config key is flipped.
 * 
 * See window.onerror function for capturing all window errors for this script.
 */
function LogErrorToServer(stackframes, sev, msg) {
    var stacked = stackframes.map(function (sf) {
        return sf.toString() || "";
    }).join('\n');
    var data = {
        severity: encodeURIComponent(sev || 2) || 2,
        message: encodeURIComponent(msg || "") || "",
        stackTrace: stackframes != null ?
                        encodeURIComponent(stacked) : "NULL"
    };
    console.log('LogErrorToServer: msg==' + JSON.stringify(msg || ""));
    console.log('LogErrorToServer: stack==' + JSON.stringify(stacked || ""));
    //alert('LogErrorToServer: ' + JSON.stringify(data || ""));
    $.ajax({
        cache: false,
        type: 'POST',
        dataType: 'html',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        url: path + "Schedule/LogClientError",
        data: data,
        success: function (data, textStatus, jqXHR) {
            
            SetCountDownTime(timeout);
            
        },
        error: function (data, error, errortext) {
            
            alert("logErrorToServer: ERROR: RC=" + data.statusCode + ", RT=" +
                data.statusText + ", error=" + JSON.stringify(error) + ", error.text="
                + errortext + ", data=" + data.responseText);
        }
    }).always(function (e) {
        SetCountDownTime(timeout);
        
        
    });

}

var errback = function (err) { console != null ? console.log(err.message) : alert("Window.Error: err==" + (err || "null")); };

// Auto-log uncaught JS errors
window.onerror = function (msg, file, line, col, error) {
    // callback is called with an Array[StackFrame]
    try {
        if (error == null)
            StackTrace.get()
                .then(function (stackframes) {
                    ConsoleLogCapture(stackframes, msg, file, line, col, error);
                    //LogErrorToServer(stackframes, 1, msg);
                })
                .catch(errback);
        else
            StackTrace.fromError(error || "null")
                .then(function (stackframes) {
                    ConsoleLogCapture(stackframes, msg, file, line, col, error);
                    //LogErrorToServer(stackframes, 1, msg);
                })
                .catch(errback);
    } catch (e) {
        ConsoleLogCapture(null, msg, file, line, col, error);
    }
    return true;
};

function ConsoleLogCapture(stackframes, msg, file, line, col, error) {
    var errOut = 'window.onerror: file==' + JSON.stringify(file || "") + "\n" +
        'window.onerror: line, col==' + JSON.stringify(line || "") + ", " + JSON.stringify(col || "") + "\n" +
        'window.onerror: msg==' + JSON.stringify(msg || "") + "\n" +
        'window.onerror: stack==' + JSON.stringify(stackframes || "");
    if (console != null) {
        console.log('window.onerror: ============');
        console.log(errOut);
        console.log('window.onerror: ============');
    } else {
        alert("window.onerror: error==" + (errOut || "null"));
    }
}

function SetScheduleDIVMaxHeight() {
    var windowHeight = $(window).innerHeight();
    // subtract sum of all current heights
    var extraHeight = windowHeight - 665;

    $("[id=scheduleDIV]").css("maxHeight", (295 + extraHeight + "px"));
    $(window).resize(function () {
        var windowHeight = $(window).innerHeight();
        // subtract sum of all current heights
        var extraHeight = windowHeight - 665;

        $("[id=scheduleDIV]").css("maxHeight", (295 + extraHeight + "px"));
    });

}

var timeout = 1800;

function AddSlot(ledgerId, shift, type) {
    
    
    $.ajax({
        url: path + "Schedule/AddSlot",
        type: "post",
        data: {
            shift: shift,
            generalLedgerId: ledgerId
        }
    }).done(function (result) {
        $("#SlotEdit").html(result);
        var slotNbr = $("#slotNbr").val();
        var slotId = $("#slotId").val();
        var newRow = $("#blankRow").html();
        newRow = replaceAll(newRow, "@slot.SlotNbr", slotNbr);
        newRow = replaceAll(newRow, "@type", type);
        newRow = replaceAll(newRow, "@shift", shift);
        newRow = replaceAll(newRow, "@slot.Id", slotId);
        var shiftSlot = $("#" + shift + "\\/_" + type);
        shiftSlot.find("#slotTd").append(newRow);
        $("[slotid='" + slotId +"']").click(EditSlot);
        //CalculateDayHours();
        InitSlotEditModal();
        SetCountDownTime(timeout);
    });
}
function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}
function RemoveSlot(slotId) {
    
    $.ajax({
        url: path + "Schedule/RemoveSlot",
        type: "post",
        data: {
            slotId: slotId
        }
    }).done(function (result) {
        
        var row = $($("[slotid='" + slotId + "']").closest("tr")[0]);
        //var shift = row.closest(".shiftSlotRow")[0].id.split("/")[0];
        //var type = row.closest(".shiftSlotRow")[0].id.split("_")[1];
        
        //var oldEmployeeId = row.find(".slotEmpNm").attr("employeeid");
        //var oldEmployeeName = row.find(".slotEmpNm").text().trim();
        //var oldHourCount = null;
        //var totalOldHours = 0.0;
        //if (oldEmployeeId) {
        //    oldHourCount = parseFloat($("#EMP\\/" + oldEmployeeId).val().trim());
        //}
        //var days = row.find(".day");
        
        //for (var i = 1; i < days.length + 1; i++) {
        //    var oldDayHour = parseFloat($($(days[i - 1]).find("label")[2]).text());
        //    if (isNaN(oldDayHour)) {
        //        oldDayHour = 0;
        //    }
        //    totalOldHours += oldDayHour;
        //    if (oldEmployeeId) {
        //        oldHourCount -= oldDayHour;
        //    }
        //    //subtract old hours from totals
        //    if (oldEmployeeId || oldEmployeeName == "Pending Position") {
        //        $("[id ^='" + shift + "'][id *='" + type + "'][id $='" + (i - 1) + "']").each(function (index, element) {
        //            var oldTotal = parseFloat($(element).text());
        //            oldTotal -= oldDayHour;
        //            $(element).text(oldTotal.toFixed(1));
        //        });

        //        var oldDayTotal = parseFloat(row.closest("#scheduleTableDIV").find("#DayTotal-" + (i - 1)).text());
        //        oldDayTotal -= oldDayHour;
        //        row.closest("#scheduleTableDIV").find("#DayTotal-" + (i - 1)).text(oldDayTotal.toFixed(1));
        //    }
        //}
        ////subtract old hours from employee total
        //if (oldEmployeeId || oldEmployeeName == "Pending Position") {
        //    row.find(".slotEmpNm").attr("employeeid", "");
        //    var oldShiftTotal = parseFloat($("#" + type + "\\/_shift_total_hours_" + shift).text());
        //    oldShiftTotal -= totalOldHours;
        //    $("#" + type + "\\/_shift_total_hours_" + shift).text(oldShiftTotal.toFixed(1));

        //    var employeeTotal = parseFloat($("#EMP\\/" + oldEmployeeId).text());
        //    employeeTotal -= totalOldHours;
        //    $("#EMP\\/" + oldEmployeeId).text(employeeTotal.toFixed(1));
        //    if (employeeTotal > 80) {
        //        $("#EMP\\/" + oldEmployeeId).parent().siblings("span").addClass("redText");
        //    }
        //    else {
        //        $("#EMP\\/" + oldEmployeeId).parent().siblings("span").removeClass("redText");
        //    }

        //    var oldGrandTotal = row.closest("#scheduleTableDIV").find(".TotalHourCount").text();
        //    oldGrandTotal -= totalOldHours;
        //    row.closest("#scheduleTableDIV").find(".TotalHourCount").text(oldGrandTotal.toFixed(1));
        //}
        row.remove();
        //UpdateSummaryHours();
        SetCountDownTime(timeout);
        $("#summaryTable").addClass("NotSaved");
        
    });
}

function EditSlot(e) {
    
    globalEvent = e;
    var slotId = e.currentTarget.attributes["slotid"].value;

    $.ajax({
        url: path + "Schedule/EditSlot",
        type: "post",
        data: {
            slotId: slotId
        }
    }).done(function (result) {
        $("#SlotEdit").html(result);
        CalculateDayHours();
        InitSlotEditModal();
    })


}

function CloseEditSlotWindow() {
    $('.ui-dialog-content').dialog('close');
}

function GetDailyValues(type) {
    var values = "";
    for (var i = 1; i <= 14; i++) {
        var date = $("#date" + i).val();
        var typeValue = $("#" + type + i).val();
        values += date + ";" + typeValue + ",";
    }
    return values;
}

function SaveSlotEdit() {
    $('#SlotEdit').hide();
    CloseEditSlotWindow();
    
    var slotId = $("#slotId").val();
    var slotNbr = $("#slotNbr").val();
    var startShifts = GetDailyValues("startShift");
    var endShifts = GetDailyValues("endShift");
    var rooms = GetDailyValues("Room");
    var hours = GetDailyValues("hourCount");
    var employee = $("#Employees").val();
    var employeeDisplay = $("#Employees option:selected").text();
    $.ajax({
        cache: false,
        type: 'POST',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        dataType: 'html', //'html' 'json'
        url: path + "Schedule/SaveSlotEdit",
        data: {
            slotId: slotId,
            employeeId: employee,
            startShifts: startShifts,
            endShifts: endShifts,
            rooms: rooms,
            hourCount: hours
        },
        success: function (data, textStatus, jqXHR) {

            var row = $($("[slotid='" + slotId + "']").closest("tr")[0]);
            //test = row;
            var shift = row.closest(".shiftSlotRow")[0].id.split("/")[0];
            var type = row.closest(".shiftSlotRow")[0].id.split("_")[1];

            var oldEmployeeId = row.find(".slotEmpNm").attr("employeeid");
            var oldEmployeeName = row.find(".slotEmpNm").text().trim();
            
            var newEmployeeId = employee;
            var oldHourCount = null;
            var totalOldHours = 0.0;
            if (oldEmployeeId) {
                oldHourCount = parseFloat($("#EMP\\/" + oldEmployeeId).val().trim());
            }
            var newHourTotal = 0.0;
            row.find(".slotEmpNm").text(employeeDisplay);
            if (employeeDisplay != "Unassigned") {
                row.removeClass("highlight");
            } else {
                row.addClass("highlight");
            }
            var days = row.find(".day");

            for (var i = 1; i < days.length + 1; i++) {
                var room = $("#Room" + i + " option:selected").text();
                var startShift = $("#startShift" + i + " option:selected");//.text();
                var endShift = $("#endShift" + i + " option:selected").text();
                var hourCount = parseFloat($("#hourCount" + i).val());
                //var oldDayHour = parseFloat($($(days[i - 1]).find("label")[2]).text());
                //if (isNaN(oldDayHour)) {
                //    oldDayHour = 0;
                //}
                //totalOldHours += oldDayHour;
                //if (oldEmployeeId) {
                //    oldHourCount -= oldDayHour;
                //}
                if (isNaN(hourCount)) {
                    hourCount = 0;
                }
                //newHourTotal += hourCount;
                var startShiftText = startShift.text();
                if (startShift.val() < 0) {
                    startShiftText = $("#hourMap-" + startShift.val()).text();
                    hourCount = startShiftText;
                }
                $($(days[i - 1]).find("label")[0]).text(startShiftText);
                $($(days[i - 1]).find("label")[1]).text(endShift);
                $($(days[i - 1]).find("label")[2]).text(hourCount);
                $($(days[i - 1]).find("label")[3]).text(room);
            //    if (hourCount == startShiftText) {
            //        hourCount = 0;
            //    }
            //    //subtract old hours from totals
            //    if (oldEmployeeId || oldEmployeeName == "Pending Position") {
            //        $("[id ^='" + shift + "'][id *='"+type+"'][id $='" + (i - 1) + "']").each(function (index, element) {
            //            var oldTotal = parseFloat($(element).text());
            //            oldTotal -= oldDayHour;
            //            $(element).text(oldTotal.toFixed(1));
            //        });

            //        var oldDayTotal = parseFloat(row.closest("#scheduleTableDIV").find("#DayTotal-" + (i - 1)).text());
            //        oldDayTotal -= oldDayHour;
            //        row.closest("#scheduleTableDIV").find("#DayTotal-" + (i - 1)).text(oldDayTotal.toFixed(1));
            //    }
            //    //add new hours to totals
            //    if (newEmployeeId > 0 || employeeDisplay == "Pending Position") {
            //        $("[id ^='" + shift + "'][id *='" + type + "'][id $='" + (i - 1) + "']").each(function (index, element) {
            //            var oldTotal = parseFloat($(element).text());
            //            oldTotal += hourCount;
            //            $(element).text(oldTotal.toFixed(1));
            //        });

            //        var oldDayTotal = parseFloat(row.closest("#scheduleTableDIV").find("#DayTotal-" + (i - 1)).text());
            //        oldDayTotal += hourCount;
            //        row.closest("#scheduleTableDIV").find("#DayTotal-" + (i - 1)).text(oldDayTotal.toFixed(1));
            //    }
            }
            ////subtract old hours from employee total
            //if (oldEmployeeId || oldEmployeeName == "Pending Position") {
            //    row.find(".slotEmpNm").attr("employeeid", "");
            //    var oldShiftTotal = parseFloat($("#" + type + "\\/_shift_total_hours_" + shift).text());
            //    oldShiftTotal -= totalOldHours;
            //    $("#" + type + "\\/_shift_total_hours_" + shift).text(oldShiftTotal.toFixed(1));

            //    var employeeTotal = parseFloat($("#EMP\\/" + oldEmployeeId).text());
            //    employeeTotal -= totalOldHours;
            //    $("#EMP\\/" + oldEmployeeId).text(employeeTotal.toFixed(1));
            //    if (employeeTotal > 80) {
            //        $("#EMP\\/" + oldEmployeeId).parent().siblings("span").addClass("redText");
            //    }
            //    else {
            //        $("#EMP\\/" + oldEmployeeId).parent().siblings("span").removeClass("redText");
            //    }

            //    var oldGrandTotal = row.closest("#scheduleTableDIV").find(".TotalHourCount").text();
            //    oldGrandTotal -= totalOldHours;
            //    row.closest("#scheduleTableDIV").find(".TotalHourCount").text(oldGrandTotal.toFixed(1));
            //}
            ////add new hours to employee total
            //if (newEmployeeId > 0 || employeeDisplay == "Pending Position") {
            //    row.find(".slotEmpNm").attr("employeeid", newEmployeeId);
            //    var oldShiftTotal = parseFloat($("#" + type + "\\/_shift_total_hours_" + shift).text());
            //    oldShiftTotal += newHourTotal;
            //    $("#" + type + "\\/_shift_total_hours_" + shift).text(oldShiftTotal.toFixed(1));

            //    var employeeTotal = parseFloat($("#EMP\\/" + newEmployeeId).text());
            //    employeeTotal += newHourTotal;
            //    $("#EMP\\/" + newEmployeeId).text(employeeTotal.toFixed(1));
            //    if (employeeTotal > 80) {
            //        $("#EMP\\/" + newEmployeeId).parent().siblings("span").addClass("redText");
            //    }
            //    else {
            //        $("#EMP\\/" + newEmployeeId).parent().siblings("span").removeClass("redText");
            //    }
            //    var oldGrandTotal = parseFloat(row.closest("#scheduleTableDIV").find(".TotalHourCount").text());
            //    oldGrandTotal += newHourTotal;
            //    row.closest("#scheduleTableDIV").find(".TotalHourCount").text(oldGrandTotal.toFixed(1));
            //}
            //UpdateSummaryHours();
            $("#summaryTable").addClass("NotSaved");
            
            SetCountDownTime(timeout);
        },
        error: function (data, error, errortext) {
            
            document.getElementById('SlotEdit').innerHTML = "";
            $('#SlotEdit').hide();
        }
    }).always(function (e) {
        $('#SlotEdit').hide();
        
    });
}


function PrepareCommunityAndPayPeriodSelectors() {
    $("#CurrentCommunity").off('change');
    $("#CurrentCommunity").on('change', function (e) {
        $("#employeeJobInfo").load(path + "Schedule/GetEmployeeJobs?communityId=" + $(this).val(), function () {
            
        });
        $('#PayPeriod').datepicker('setDate', null);
        //UpdateDatePicker(this);
        CurrentPayPeriodDates(this);
        CommunityAndWeekSelected(e);
    });
    $("#PayPeriod").off('change');
    $("#PayPeriod").on('change', function (e) {
        e.preventDefault();
        CommunityAndWeekSelected(e);
    });
    $("#PayPeriod").off('click');
    $("#PayPeriod").on('click', function (e) {
        e.preventDefault();
        $("#PayPeriod").datepicker().css("display", "");
        if (PayPeriodDates == undefined || PayPeriodDates == null) {
            CurrentPayPeriodDates(this);
            $("#PayPeriod").datepicker("destroy");
            $("#PayPeriod").datepicker("refresh");
        }
    });
}

function CurrentPayPeriodDates(e) {
    $('#PayPeriod').prop('disabled', true);
    $.ajax({
        cache: false,
        type: 'GET',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        dataType: 'json', //'html' XOR 'json'
        url: path + "Schedule/CurrentPayPeriodDates",
        data: {
            communityId: $("#CurrentCommunity").val()
        },
        success: function (data, textStatus, jqXHR) {
            $("#PayPeriodWarning").hide();
            $('#PayPeriod').removeAttr('disabled');
            var parsed = data;
            if (parsed.Success && parsed.PayPeriodBeginDates != undefined && parsed.PayPeriodBeginDates.length > 0) {
                PayPeriodDates = parsed.PayPeriodBeginDates.split(',');
                $('#PayPeriod').datepicker({
                    beforeShowDay: function (a) {
                        var f = $.datepicker.formatDate('yymmdd', a);
                        if ($.inArray(f, PayPeriodDates) > -1) {
                            return [true];
                        } else {
                            return [false, ""];
                        }
                    },
                    showOnFocus: function (a) { true; }
                });
                InitializeDates();
                
                
                return PayPeriodDates;
            }
            else {
                PayPeriodDates = null;
                $('#PayPeriod').datepicker({
                    beforeShowDay: function (a) {
                        var f = $.datepicker.formatDate('yymmdd', a);
                        if ($.inArray(f, PayPeriodDates) > -1) {
                            return [true];
                        } else {
                            return [false, ""];
                        }
                    },
                    showOnFocus: function (a) { true; }
                });
                $('#PayPeriod').prop('disabled', true);
                $("#PayPeriodWarning").show();
            }
        },
        error: function (data, error, errortext) {
            alert("CurrentPayPeriodDates: ERROR: data=" + data.responseText + ", RC=" + data.statusCode + ", RT=" + data.statusText + ", error=" + JSON.stringify(error) + ", error.text=" + errortext);
            //alert("Warning: Error updating Schedule. Please try again.");
        }
    }).always(function (e) {
        
    });
};

function InitializeDates() {
    $(".isDate").datepicker({
        beforeShow: function (textbox, instance) {
            instance.dpDiv.css({
                marginTop: (-textbox.offsetHeight) + 'px',
                marginLeft: textbox.offsetWidth + 'px'
            });
        }
    });
};

function DeleteSchedule() {
    if (confirm("Are you sure you want to delete this schedule?")) {
        
        var payPeriod = $("#PayPeriod_Id").val();
        $.ajax({
            url: path + "Schedule/RemoveSchedule",
            type: "post",
            data: {
                payPeriodId: payPeriod
            }
        }).done(function (result) {
            
            SetCountDownTime(timeout);
            $("#submitScheduleViewModel").hide();
            $("#deleteScheduleViewModel").hide();
            $("#textOpenShift").attr("disabled", "disabled");
            $("#scheduleInfo").html("");
        })
    }
}

function DoesScheduleForWeekExist(e) {
    
    //DoesScheduleForWeekExist
    $.ajax({
        url: path + "Schedule/DoesScheduleForWeekExist",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            communityId: $("#CurrentCommunity").val(),
            payPeriod: $("#PayPeriod").val()
        },
        success: function (data, textStatus, jqXHR) {
            if (!data.ScheduleWeekExist) {
                CopySchedule();
            } else {
                GetSchedule();
            };
        },
        error: function (data, error, errortext) {
            
            alert(data.responseText);
        }
    });
};

function InitSlotEditModal() {
    var $dialog = $("#SlotEdit");
    var width = $dialog.width();
    var height = $dialog.height();
    //$dialog.empty();
    $dialog.dialog({
        //title: "Warning on SAVE - Employee not selected",
        dialogClass: "",
        closeOnEscape: true,
        modal: true,
        open: function (event, ui) {
            
            $(".ui-dialog").css("width", "0px");
            $(".ui-dialog").css("height", "0px");
            $(".ui-dialog").css("top", "-10px");
            $(".ui-dialog").css("left", "-10px")
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        },
        close: function (event, ui) {
            
            
        }
    });
    $dialog.draggable({ disabled: false });
    //$dialog.find(".ui-resizable-handle").hide();
    $dialog.dialog("open");

    var top = ((Math.max($(window).height()) / 2) - ($dialog.height() / 2));
    var left = ((Math.max($(window).width()) / 2) - ($dialog.width() / 2));

    $dialog.css({ top: top, left: left });
}

function CommunityAndWeekSelected(e) {
    if ($("#CurrentCommunity").val().length > 0 &&
        $("#CurrentCommunity").val() != -1 &&
        $("#PayPeriod").val().length > 0) {
        $('#textOpenShift').show();
        $('#submitScheduleViewModel').hide();
        $('#deleteScheduleViewModel').hide();
        $("#textOpenShift").hide();
        $('#coreTable').hide();

        // ExistingPayPeriod
        e.stopImmediatePropagation();
        DoesScheduleForWeekExist();
    } else if ($("#CurrentCommunity").val().length > 0 &&
        $("#CurrentCommunity").val() != -1 &&
        $("#PayPeriod").val().length == 0) {
        $('#textOpenShift').show();
        $('#submitScheduleViewModel').hide();
        $('#deleteScheduleViewModel').hide();
        $("#textOpenShift").hide();
        $('copyPreviousPayPeriod').hide();
        $('#coreTable').hide();
    } else {
        $('#textOpenShift').hide();
        $('#submitScheduleViewModel').hide();
        $('#deleteScheduleViewModel').hide();
        $("#textOpenShift").hide();
        $('copyPreviousPayPeriod').hide();
        $('#coreTable').hide();
    }
}

function UpdateSummaryHours() {
    //AddEmployeeHours("RN");
    //AddEmployeeHours("LPN");
    //AddEmployeeHours("CNA");
    //var RNLPNSumHours = AddSummaryHours(["RN", "LPN"]);
    //var RNLPNHtml = "<tr id='RN_LPN'><td><div class='EmployeeTotalHoursDIV'>";
    //RNLPNHtml += "<label>Total RN,LPN Hours:</label><label class='rightAndPadded' id='SummaryTotalHours_LPN'>";
    //RNLPNHtml += RNLPNSumHours.toFixed(1) + "</label></td></tr>";
    //$("#RN_LPN").remove();
    //$("#LPN").after(RNLPNHtml);

    //var CNASumHours = AddSummaryHours(["RN", "LPN", "CNA"]);
    //$("#RN_LPN_CNA").remove();
    //var CNAHtml = "<tr id='RN_LPN_CNA'><td><div class='EmployeeTotalHoursDIV'>";
    //CNAHtml += "<label>Total RN,LPN,CNA Hours:</label><label class='rightAndPadded' id='SummaryTotalHours_CPA'>";
    //CNAHtml += CNASumHours.toFixed(1) + "</label></td></tr>";
    //$("#CNA").after(CNAHtml);
    SetPPD();
};

function AddEmployeeHours(tabName) {
    var total = 0.0;
    $.each($("#Employees_" + tabName).find("[id^='EMP']"), function (index, value) {
        total += parseFloat($(value).text());
    });
    $("#SummaryTotalHours_" + tabName).text(total.toFixed(1));
    return total;
}

function AddSummaryHours(tabs) {
    var total = 0;
    for (var i = 0; i < tabs.length; i++) {
        total += parseFloat($("#PPDSummaryTotalHours_" + tabs[i]).text());
    }
    return total;
}

function CalculatePPD(tabs) {
    var censusValue = $("[name='payerGroup.AvgDailyCensusCnt']").val().replace(/[^0-9.]/g, '');
    $("[name='payerGroup.AvgDailyCensusCnt']").val(censusValue);
    var census = parseFloat($("[name='payerGroup.AvgDailyCensusCnt']").val()) * 14;

    var budget = 0;
    for (var i = 0; i < tabs.length; i++) {
        var budgetValue = $("[name='" + tabs[i] + "GenLedger.HourPPDCnt']").val();
        $("[name='" + tabs[i] + "GenLedger.HourPPDCnt']").val(budgetValue);
        budget += parseFloat($("[name='" + tabs[i] + "GenLedger.HourPPDCnt']").val());
        //var budgetHours = (census * budget) / 2;
    }
    if (tabs.length == 1) {
        var budgetHours = (census * budget) / 2;
        if (isNaN(budgetHours) || !isFinite(budgetHours)) {
            budgetHours = 0;
        }
        $("#totalHours_" + tabs[0]).text(budgetHours.toFixed(1));
    }
    if (isNaN(budget) || !isFinite(budget)) {
        budget = 0;
    }
    $("#summaryExpectedPPD_" + tabs.join("_")).text(budget.toFixed(2));
    var actualPPD = AddSummaryHours(tabs) / census;
    if (isNaN(actualPPD) || !isFinite(actualPPD)) {
        actualPPD = 0;
    }
    $("#summaryActualPPD_" + tabs.join("_")).text(actualPPD.toFixed(2));

}

function SetPPD() {
    CalculatePPD(["RN"]);
    CalculatePPD(["LPN"]);
    CalculatePPD(["CNA"]);
    CalculatePPD(["RN", "LPN"]);
    CalculatePPD(["RN", "LPN", "CNA"]);
}

function TimeHalfHour(time) {
    if (time[2] == "3") {
        time = time.replaceAt(2, "5");
    }
    return parseFloat(time);
}

String.prototype.replaceAt = function (index, replacement) {
    return this.substr(0, index) + replacement + this.substr(index + replacement.length);
}

function CopySchedule() {
    var existingPayPeriodDate = $("#PayPeriod").val();
    
    $(".modal").hide();
    $.ajax({
        url: path + "Schedule/CopyScheduleFromMaster",
        type: "post",
        data: {
            communityId: $("#CurrentCommunity").val(),
            dateOfWeek: existingPayPeriodDate
        }
    }).done(function (result) {
        if (result.success) {
            alert("No schedule currently exists for this pay period.  Copying from master.");
            GetSchedule();
        } else {
            console.log(result);
            alert(result.responseText);
            
        }
        
    }).fail(function (result) {
        console.log(result);
        alert(result);
    });
}

function SaveSchedule(selectedTab, scrollPos) {

    
    var payGroupId = $("#payerGroup_Id").val();
    var avgCensus = $("#payerGroup_AvgDailyCensusCnt").val();
    var ppdString = "";
    $("[id$='GenLedger_HourPPDCnt']").each(function (index, ppd) {
        var ppdType = ppd.id.replace("GenLedger_HourPPDCnt", "");
        var ppdValue = ppd.value;
        ppdString += ppdValue + ";" + ppdType + ",";
    });
    $(".modal").hide();
    $.ajax({
        url: path + "Schedule/SaveSchedule",
        type: "post",
        data: {
            payerGroupId: payGroupId,
            dailyCensus: avgCensus,
            ppdInfo: ppdString
        }
    })
    .done(function (result) {
        selectedTab = $("#tabs").tabs("option", "active");
        SetCountDownTime(timeout);
        GetSchedule(selectedTab,scrollPos);
    }).fail(function (result) {
        alert(result);
        
    });
}

function GetSchedule(selectedTab, scrollPos) {
    
    $(".modal").hide();
    $.ajax({
        url: path + "Schedule/GetPayPeriodSchedule",
        type: "post",
        data: {
            communityId: $("#CurrentCommunity").val(),
            payPeriod: $("#PayPeriod").val()
        }
    })
        .done(function (result) {
            $("#scheduleInfo").html(result);
            $("#summaryTable").removeClass("NotSaved");
            //UpdateSummaryHours();
            $("#tabs").tabs({
                activate: function () {
                    if ($("#tabs").tabs("option", "active") == 0) {
                        $("#tabsNursesPPD").show();
                    } else {
                        $("#tabsNursesPPD").hide();
                    }
                    if ($("#tabs").tabs("option", "active") == 1) {
                        $("#tabsCNAPPD").show();
                    } else {
                        $("#tabsCNAPPD").hide();
                    }
                }

            });
            if (selectedTab) {
                $("#tabs").tabs("option", "active", selectedTab);
            }
            if ($("#IsDisabled").val().toLowerCase() != "true") {
                $('#submitScheduleViewModel').show();
                $('#deleteScheduleViewModel').show();
                $(".btnEditSlot").click(EditSlot);
                $("#textOpenShift").show();
                $("#textOpenShift").removeAttr("disabled");
            }
            SetScheduleDIVMaxHeight();
            $('#coreTable').show();
            
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus);
            console.log(errorThrown);
            //alert(result);

        });
}

function CalculateDayHours() {
    for (var i = 1; i <= 14; i++) {

        var startShift = TimeHalfHour($("#startShift" + i).val());
        if (isNaN(startShift) || startShift < 0) {
            $("#endShift" + i).val("None");
            $("#endShift" + i).prop("disabled", true);
            $("#hourCount" + i).val(0);
        }
        else {
            $("#endShift" + i).prop("disabled", false);
            var endShift = TimeHalfHour($("#endShift" + i).val());
            if (!isNaN(endShift)) {
                var hours = (endShift - startShift) / 100;
                if (endShift <= startShift) {
                    hours = 24 - ((startShift - endShift) / 100);
                }

                if (hours >= 4.5) {
                    hours -= 0.5;
                }
                $("#hourCount" + i).val(hours);
            }
        }

    }
}

function ToggleSendTextNotification(e) {
    var arySelectedEmployees = $('#employee_Selected:checked').closest('.textOpenShiftEmployee');
    var textAreaMsg = $('#Message').val();
    if (textAreaMsg === '' || (arySelectedEmployees) == null || $(arySelectedEmployees).length == 0) {
        $('#btnTextOpenShift').prop('disabled', 'disabled');
    } else {
        $('#btnTextOpenShift').prop('disabled', '');
    };
}

function InitSendTextNofication(TextOpenShiftModal) {

    $(document).on('click', '#employee_Selected', function (e) {
        ToggleSendTextNotification(this);
    });
    $(document).on('input', '#Message', function (e) {
        ToggleSendTextNotification(this);
    });

    $(document).on('click', '#btnTextOpenShift', function (e) {
        var arySelectedEmployeeIds = [];
        var arySelectedEmployees = $('#employee_Selected:checked').closest('.textOpenShiftEmployee');
        if ($(arySelectedEmployees) != null && $(arySelectedEmployees).length > 0) {
            $.each($(arySelectedEmployees), function (i, elem) {
                arySelectedEmployeeIds.push($(elem).prop('id'));
            });
        }
        var communityName = $("#CommunityName").val();
        var message = $('#Message').val();
        var empIds = arySelectedEmployeeIds.join('~');
        //alert('btnTextOpenShift: arySelectedEmployeeIds.join(~)=' + arySelectedEmployeeIds.join('~'));

        $.ajax({
            url: path + "Schedule/UpdateTextOpenShift",
            data: {
                communityName: communityName,
                employeeIds: empIds,
                message: message
            },
            success: function (data, textStatus, jqXHR, $form) {
                try {
                    var result = $.parseJSON(data);
                    //alert('btnTextOpenShift: result=' + JSON.stringify(result));
                    if (result.Success) {
                        alert("Notifications were sent.");
                        $(".modal").hide();
                        TextOpenShiftModal.hide();
                    }
                } catch (error) { }
            },
            error: function (data, error, errortext) {
                alert(errortext + ": " + $(data.responseText).closest("title").text());
                //alert("Could not communicate with the server.");
            }
        });
    });

    //TextOpenShiftEmployeeTable_wrapper
    $(document).on('change', 'input#jobType_Selected', function (e) {
        var aryUnSelectedRoles = $('#jobType_Selected:not(:checked)').siblings('input#jobType_JobTypeName');
        if ($(aryUnSelectedRoles) != null && $(aryUnSelectedRoles).length > 0) {
            $.each($(aryUnSelectedRoles), function (i, elem) {
                $('[id$=' + $(elem).val() + '].textOpenShiftEmployee').find('input.selectEmployee').prop('checked', false);
                $('[id$=' + $(elem).val() + '].textOpenShiftEmployee').hide();
            });
        }
        var arySelectedRoles = $('#jobType_Selected:checked').siblings('input#jobType_JobTypeName');
        if ($(arySelectedRoles) != null && $(arySelectedRoles).length > 0) {
            $.each($(arySelectedRoles), function (i, elem) {
                $('[id$=' + $(elem).val() + '].textOpenShiftEmployee').show();
            });
        }
        ToggleSendTextNotification(this);
    });

    $('#textOpenShift').on('click', function (e) {
        e.preventDefault();
        var communityId = $("#CurrentCommunity").val();
        var payPeriod = $("#PayPeriod").val();
        $.ajax({
            url: path + "Schedule/EditTextOpenShift",
            data: {
                communityId: communityId,
                payPeriod: payPeriod
            },
            success: function (data) {
                var modal = $('<div />');
                modal.addClass("modal");
                $('body').append(modal);
                TextOpenShiftModal.html(data);
                ToggleSendTextNotification(this);
                TextOpenShiftModal.show();
                var top = Math.max($(window).height() / 2 - TextOpenShiftModal[0].offsetHeight / 2, 0);
                var left = Math.max($(window).width() / 2 - TextOpenShiftModal[0].offsetWidth / 2, 0);
                TextOpenShiftModal.css({ top: top, left: left });
                textOpenShiftEmployeeTable = $('#TextOpenShiftJobTypeTable', TextOpenShiftModal).dataTable({
                    "bFilter": false,
                    "bAutoWidth": false,
                    "sScrollY": "70px",
                    "sDom": "frtS",
                    "iDisplayLength": -1,
                    "aoColumns": [
                        {
                            "sWidth": "100px"
                        },
                        {
                            "sWidth": "50px",
                            "bSortable": false
                        }
                    ],
                    "oLanguage": {
                        "sEmptyTable": "No Employees"
                    }
                });
                //var textOpenShiftEmployees= $(TextOpenShiftModal).find('.textOpenShiftEmployee');
                textOpenShiftEmployeeTable = $('#TextOpenShiftEmployeeTable', TextOpenShiftModal).dataTable({
                    "bFilter": false,
                    "bAutoWidth": false,
                    "sScrollY": "220px",
                    "sDom": "frtS",
                    "iDisplayLength": -1,
                    "aoColumns": [
                        {
                            "sWidth": "100px"
                        },
                        {
                            "sWidth": "50px",
                            "bSortable": false
                        }
                    ],
                    "oLanguage": {
                        "sEmptyTable": "No Employees"
                    }
                });
            },
            error: function () {
                alert("Could not communicate with the server.");
            }
        });
    });

    TextOpenShiftModal.on('click', '#close', function () {
        $(".modal").hide();
        TextOpenShiftModal.hide();
    });
}
