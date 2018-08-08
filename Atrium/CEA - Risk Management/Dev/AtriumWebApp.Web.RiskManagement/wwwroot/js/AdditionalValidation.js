function getMomentDate(dateString) {
    var lookbackDate = moment(dateString, 'MM/DD/YYYY', true);
    if (!lookbackDate.isValid()) {
        lookbackDate = moment(dateString, 'M/DD/YYYY', true);
    }
    if (!lookbackDate.isValid()) {
        lookbackDate = moment(dateString, 'MM/D/YYYY', true);
    }
    if (!lookbackDate.isValid()) {
        lookbackDate = moment(dateString, 'M/D/YYYY', true);
    }
    return lookbackDate;

}

function getMomentDateTime(dateString) {
    var lookbackDate = moment(dateString, 'MM/DD/YYYY hh:mm a', true);
    if (!lookbackDate.isValid()) {
        lookbackDate = moment(dateString, 'M/DD/YYYY hh:mm a', true);
    }
    if (!lookbackDate.isValid()) {
        lookbackDate = moment(dateString, 'MM/D/YYYY hh:mm a', true);
    }
    if (!lookbackDate.isValid()) {
        lookbackDate = moment(dateString, 'M/D/YYYY hh:mm a', true);
    }
    return lookbackDate;

}

function checkDateTime(dateString) {
    var lookbackDate = getMomentDateTime(dateString);
    if (!lookbackDate.isValid()) {
        return "invalid";

    }
    else if (moment().diff(lookbackDate) < 0) {
        return "infuture";
    }
    return "ok";
}

function checkDate(dateString) {
    var lookbackDate = getMomentDate(dateString);

    if (!lookbackDate.isValid()) {
        return "invalid";

    }
    else if (moment().diff(lookbackDate) < 0) {
        return "infuture";
    }
    return "ok";
}

$.validator.addMethod("date",
    function (value, element) {
        var check = checkDate(value);
        return this.optional(element) || (check != "invalid");
    },
    "Please enter a valid date (mm/dd/yyyy)");
// Validate for 2 decimal for money
$.validator.addMethod("money", function (value, element) {
    return this.optional(element) || /^(-)?(\d+)(\.\d{2})?$/.test(value);
}, "Must be in US currency format");

$.validator.addMethod("date-future", function (value, element) {
    var check = checkDate(value);
        return this.optional(element) || (check === "ok");
    },
    "Date cannot be in the future"
);

$.validator.addMethod("date-before", function (value, element) {
    var otherDateSelector = $(element).attr("date-before-field");
    if (otherDateSelector == "") {
        console.error("date-before rule requires an id be supplied to date-before-field attribute.");
        return true;
    }
    var otherDateText = $(otherDateSelector).val();
    if (otherDateText === "") {
        return true;
    }
    var thisDate = getMomentDate(value);
    var otherDate = getMomentDate(otherDateText);

    return thisDate <= otherDate;
}, function (param, element) {
    var errorMessage = $(element).attr("date-before-message");
    return errorMessage;
});