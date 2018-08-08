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
    
    $.ajax({
        cache: false,
        type: 'POST',
        dataType: 'html',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        url: path + "ScheduleTemplate/LogClientError",
        data: data,
        success: function (data, textStatus, jqXHR) {
            
            SetCountDownTime(timeout);
            $('div.loading').hide();
        },
        error: function (data, error, errortext) {
            
            alert("logErrorToServer: ERROR: RC=" + data.statusCode + ", RT=" +
                data.statusText + ", error=" + JSON.stringify(error) + ", error.text="
                + errortext + ", data=" + data.responseText);
        }
    }).always(function (e) {
        SetCountDownTime(timeout);
        
        $('div.loading').hide();
    });

}

var errback = function (err) { console != null ? console.log(err.message) : alert("Window.Error: err==" + (err || "null")); };
var timeout = 1800;
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

function AddSlot(ledgerId, shift, type) {

    
    $.ajax({
        url: path + "ScheduleTemplate/AddSlot",
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
        $("[slotid='" + slotId + "']").click(EditSlot);
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
        url: path + "ScheduleTemplate/RemoveSlot",
        type: "post",
        data: {
            slotId: slotId
        }
    }).done(function (result) {
        SetCountDownTime(timeout);
        
        var row = $($("[slotid='" + slotId + "']").closest("tr")[0]);
        row.remove();
        $("#summaryTable").addClass("NotSaved");
        
    });
}

function CreateEmptySchedule() {
    //CreateEmpty
    
    $.ajax({
        url: path + "ScheduleTemplate/CreateEmpty",
        type: "post",
        data: {
            communityId: $("#CurrentCommunity").val()
        }
    }).done(function (result) {
        GetSchedule();

    });
}

function EditSlot(e) {
    
    globalEvent = e;
    var slotId = e.currentTarget.attributes["slotid"].value;

    $.ajax({
        url: path + "ScheduleTemplate/EditSlot",
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
        url: path + "ScheduleTemplate/SaveSlotEdit",
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
                if (isNaN(hourCount)) {
                    hourCount = 0;
                }
                var startShiftText = startShift.text();
                if (startShift.val() < 0) {
                    startShiftText = $("#hourMap-" + startShift.val()).text();
                    hourCount = startShiftText;
                }
                $($(days[i - 1]).find("label")[0]).text(startShiftText);
                $($(days[i - 1]).find("label")[1]).text(endShift);
                $($(days[i - 1]).find("label")[2]).text(hourCount);
                $($(days[i - 1]).find("label")[3]).text(room);
            }
            $("#summaryTable").addClass("NotSaved");
            
        },
        error: function (data, error, errortext) {
            
            document.getElementById('SlotEdit').innerHTML = "";
            $('#SlotEdit').hide();
        }
    }).always(function (e) {
        $('#SlotEdit').hide();
        
        $('div.loading').hide();
    });
}

function CurrentPayPeriodDates(e) {
    $('#PayPeriod').prop('disabled', true);
    $.ajax({
        cache: false,
        type: 'GET',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        dataType: 'json',
        url: path + "ScheduleTemplate/CurrentPayPeriodDates",
        data: {
            communityId: $("#CurrentCommunity").val()
        },
        success: function (data, textStatus, jqXHR) {
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
        },
        error: function (data, error, errortext) {
            alert("CurrentPayPeriodDates: ERROR: data=" + data.responseText + ", RC=" + data.statusCode + ", RT=" + data.statusText + ", error=" + JSON.stringify(error) + ", error.text=" + errortext);
        }
    }).always(function (e) {
        $('#PayPeriod').prop('disabled', true);
        $('#PayPeriod').removeAttr('disabled');
        
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
            url: path + "ScheduleTemplate/DeleteMasterTemplate",
            type: "post",
            data: {
                payPeriodId: payPeriod
            }
        }).done(function (result) {
            SetCountDownTime(timeout);
            
            $("#submitScheduleViewModel").hide();
            $("#deleteScheduleViewModel").hide();
            $("#scheduleInfo").html("");
        })
    }
}

function ExistingPayPeriodDates(e) {
    $.ajax({
        cache: false,
        type: 'GET',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        dataType: 'json', //'html' XOR 'json'
        url: path + "ScheduleTemplate/ExistingPayPeriodDates",
        data: {
            communityId: $("#CurrentCommunity").val()
        },
        success: function (data, textStatus, jqXHR) {
            var parsed = data;
            if (parsed.Success && parsed.ExistingPayPeriodBeginDates != undefined && parsed.ExistingPayPeriodBeginDates.length > 0) {
                existingPayPeriodDates = parsed.ExistingPayPeriodBeginDates.split(',');
                if (existingPayPeriodDates != null && existingPayPeriodDates != undefined && existingPayPeriodDates.length > 0) {
                    var day = moment($("#PayPeriod").val(), "MM/DD/YYYY").format("YYYYMMDD");
                    
                    //alert("ExistingPayPeriodDates: $.inArray("+day+ ", " +existingPayPeriodDates+")=" +$.inArray(day, existingPayPeriodDates));
                    if ($.inArray(day, existingPayPeriodDates) > -1) {
                        $(".modal").hide();
                        $('#copyPreviousPayPeriodModal').hide();

                        //EditSchedule(e);
                    } else {
                        $('#ExistingPayPeriod').datepicker({
                            beforeShowDay: function (a) {
                                var f = $.datepicker.formatDate('yymmdd', a);
                                if ($.inArray(f, existingPayPeriodDates) > -1) {
                                    return [true];
                                } else {
                                    return [false, ""];
                                }
                            },
                            showOnFocus: function (a) { true; }
                        });
                        InitializeDates();
                        // modal for ExistingPayPeriod
                        var tableCopyPreviousPayPeriodModal = $('#tableCopyPreviousPayPeriodModal');
                        var copyPreviousPayPeriodModal = $('#copyPreviousPayPeriodModal');
                        $("#payPeriodComm").text($("#CurrentCommunity option:selected").text());
                        //var modal = $('<div />');
                        //modal.addClass("modal");
                        //$('body').append(modal);
                        copyPreviousPayPeriodModal.show();
                        //var top = (Math.max($(window).height() / 2) - (copyPreviousPayPeriodModal[0].offsetHeight / 2, 0));
                        //var left = (Math.max($(window).width() / 2) -(copyPreviousPayPeriodModal[0].offsetWidth / 2, 0));
                        var top = ((Math.max($(window).height()) / 2) - (copyPreviousPayPeriodModal.height() / 2));
                        var left = ((Math.max($(window).width()) / 2) - (copyPreviousPayPeriodModal.width() / 2));

                        tableCopyPreviousPayPeriodModal.css({ top: (top - 50), left: (left + 5) });
                        copyPreviousPayPeriodModal.css({ top: (top - 50), left: left });

                        $('#ExistingPayPeriod').on('change', function (e) {
                            if ($('#ExistingPayPeriod').val() != "") {
                                CopySchedule();
                            }
                            
                            $("#ExistingPayPeriod").val("");
                        });

                        $('#btnContinueDefaultSchedule').on('click', function () {
                            $('#copyPreviousPayPeriodModal').hide();
                            
                            CreateEmptySchedule();
                            
                        });
                        $('#closeCopyPreviousPayPeriodModal').on('click', function (e) {
                            $('#copyPreviousPayPeriodModal').hide();
                        });
                    }

                } else {
                    //alert("ExistingPayPeriodDates: No previous dates found. inside");
                }
                return existingPayPeriodDates;
            } else {
                alert("No recent schedules found, loading blank.");
                
                CreateEmptySchedule();
            }
            
        },
        error: function (data, error, errortext) {
            
            alert("ExistingPayPeriodDates: ERROR: data=" + data.responseText + ", RC=" + data.statusCode + ", RT=" + data.statusText + ", error=" + JSON.stringify(error) + ", error.text=" + errortext);
        }
    });
};

function CopySchedule() {
    $('#copyPreviousPayPeriodModal').hide();
    var existingPayPeriodDate = $("#ExistingPayPeriod").val();
    
    $(".modal").hide();
    $.ajax({
        url: path + "ScheduleTemplate/CopyExistingSchedule",
        type: "post",
        data: {
            communityId: $("#CurrentCommunity").val(),
            payPeriodDate: existingPayPeriodDate
        }
    })
    .done(function (result) {
        $(".modal").hide();
        GetSchedule();

    });
}


function DoesScheduleExist(e) {
    $('#copyPreviousPayPeriodModal').hide();
    $("#submitScheduleViewModel").hide();
    $("#deleteScheduleViewModel").hide();
    $('#coreTable').hide();
    $("#employeeJobInfo").load(path + "Schedule/GetEmployeeJobs?communityId=" + $("#CurrentCommunity").val(), function () {

    });

    //DoesScheduleForWeekExist
    $.ajax({
        url: path + "ScheduleTemplate/DoesScheduleForCommunityExist",
        dataType: "json",
        cache: false,
        type: 'POST',
        data: {
            communityId: $("#CurrentCommunity").val()
        },
        success: function (data, textStatus, jqXHR) {
            var parsed = data;
            if (parsed.Success && parsed.ScheduleWeekExist < 1) {
                
                ExistingPayPeriodDates();

            } else {
                $(".modal").hide();
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
            $('div.loading').hide();
            
            $(".ui-dialog").css("width", "0px");
            $(".ui-dialog").css("height", "0px");
            $(".ui-dialog").css("top", "-10px");
            $(".ui-dialog").css("left", "-10px")
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        },
        close: function (event, ui) {
            $('div.loading').hide();
            
        }
    });
    $dialog.draggable({ disabled: false });
    //$dialog.find(".ui-resizable-handle").hide();
    $dialog.dialog("open");

    var top = ((Math.max($(window).height()) / 2) - ($dialog.height() / 2));
    var left = ((Math.max($(window).width()) / 2) - ($dialog.width() / 2));

    $dialog.css({ top: top, left: left });
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
        $("#totalHours_" + tabs[0]).text(budgetHours.toFixed(1));
    }
    $("#summaryExpectedPPD_" + tabs.join("_")).text(budget.toFixed(2));
    var actualPPD = AddSummaryHours(tabs) / census;
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

function SaveSchedule() {
    
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
        url: path + "ScheduleTemplate/SaveTemplateSchedule",
        type: "post",
        data: {
            payerGroupId: payGroupId,
            dailyCensus: avgCensus,
            ppdInfo: ppdString
        }
    })
    .done(function (result) {
        SetCountDownTime(timeout);
        selectedTab = $("#tabs").tabs("option", "active");
        GetSchedule(selectedTab);
    }).fail(function (result,text,code) {
        alert(text);
        
    });
}

function GetSchedule(selectedTab, scrollPos) {

    $.ajax({
        url: path + "ScheduleTemplate/GetPayPeriodScheduleTemplate",
        type: "post",
        data: {
            communityId: $("#CurrentCommunity").val()
        }
    })
    .done(function (result) {
        $("#scheduleInfo").html(result);
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
        SetCountDownTime(timeout);
        $(".modal").hide();
        //UpdateSummaryHours();
        $(".btnEditSlot").click(EditSlot);
        $('#coreTable').show();
        SetScheduleDIVMaxHeight();
        $("#submitScheduleViewModel").show();
        $("#deleteScheduleViewModel").show();
        
    });
}