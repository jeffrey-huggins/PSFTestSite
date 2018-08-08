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

function inProgress(id) {
    var inProgress = false;
    $("#" + id).find("input[type='text'], select").each(function (index, element) {
        if ($(element).val() != '') {
            inProgress = true;
            return;
        }
    });

    if ($("#" + id).find("input").is(":checked")) {
        inProgress = true;
    }
    return inProgress;
}

$.validator.addMethod("item_required", function (value, element) {
    if (inProgress("PPDSection") || inProgress("questionaireSection") || inProgress("BMATSection")) {
        return true;
    }
    else {
        return false;

    }
}, "PPD, Questionnaire or BMAT must be completed.");


$.validator.addMethod("resolved_greater_initial", function (value, element) {
    var resolvedDate = getMomentDate(value);
    var initialDate = getMomentDate($("#OccurredDate").val());
    return this.optional(element) || (initialDate.diff(resolvedDate) <= 0);

}, "Resolved Date cannot occur before initial date.");

$.validator.addMethod("no_future_date", function (value, element) {
    var validation = checkDate(value);
    return this.optional(element) || (validation == "ok");

}, "Date cannot be in the future and should take the format mm/dd/yyyy.");

$.validator.addMethod("no_future_datetime", function (value, element) {
    var validation = checkDateTime(value);
    return this.optional(element) || (validation == "ok");

}, "Date cannot be in the future and should take the format mm/dd/yyyy hh:mm am/pm.");

$.validator.addMethod("date_before_incident", function (value, element) {
    var selectedDate = getMomentDateTime(value);
    var incidentDate = getMomentDateTime($("#Event_IncidentDateTime").val());
    return this.optional(element) || (incidentDate.diff(selectedDate) <= 0);
}, "Date cannot occur before incident date");

$.validator.addMethod("date_before_incident_time", function (value, element) {
    var selectedDate = getMomentDate(value);
    var incidentDate = getMomentDate($("#Event_IncidentDateTime").val().split(" ")[0]);
    return this.optional(element) || (incidentDate.diff(selectedDate) <= 0);
}, "Date cannot occur before incident date");

$.validator.addMethod("intervention_required", function (value, element) {
    return ($("#interventionTable").find(":checked").length > 0);
}, "At least one Intervention is required");

$.validator.addMethod("treatment_required", function (value, element) {
    return ($("#treatmentTable").find(":checked").length > 0);
}, "At least one Treatment is required");