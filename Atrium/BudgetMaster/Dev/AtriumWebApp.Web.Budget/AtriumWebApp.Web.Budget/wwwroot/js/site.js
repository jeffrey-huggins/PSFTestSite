function initSelector(optionSelector, name) {
    var optionCount = $(optionSelector + " option").length;
    var selectedOption = $(optionSelector + " option:selected").attr("rowNum");

    $(optionSelector).on("change", function () {
        var selectedOption = $(optionSelector + " option:selected").attr("rowNum");
        $(name + "Selector").val(selectedOption + " of " + optionCount);
    });

    $(name + "Selector").val(selectedOption + " of " + optionCount);

    $(name + "Selector").on("focus", function () {
        var selectedOption = $(optionSelector + " option:selected").attr("rowNum");
        $(name + "Selector").val(selectedOption);
    });

    $(name + "Selector").on("blur", function () {
        clearSelection();
        var optionCount = $(optionSelector + " option").length;

        var currentOption = $(optionSelector + " option:selected").attr("rowNum");
        var newOption = parseInt($(name + "Selector").val());
        if (isNaN(newOption) || newOption <= 0 || newOption > optionCount) {
            $(name + "Selector").val(currentOption + " of " + optionCount);
        }
        else {
            $(optionSelector + " option[rowNum='" + newOption + "']").prop("selected", true);
            $(name + "Selector").val(newOption + " of " + optionCount);
            $(optionSelector).change();
        }
    });

    $(name + "Back").on("click", function () {
        var selectedOption = parseInt($(optionSelector + " option:selected").attr("rowNum"));
        selectedOption--;
        if (selectedOption > 0) {
            $(name + "Selector").val(selectedOption);
            $(name + "Selector").blur();
        }
    });

    $(name + "Forward").on("click", function () {
        var optionCount = $(optionSelector + " option").length;
        var selectedOption = parseInt($(optionSelector + " option:selected").attr("rowNum"));
        selectedOption++;
        if (selectedOption <= optionCount) {
            $(name + "Selector").val(selectedOption);
            $(name + "Selector").blur();
        }
    });

    $(name + "First").on("click", function () {
        $(name + "Selector").val(1);
        $(name + "Selector").blur();
    });

    $(name + "Last").on("click", function () {
        var optionCount = $(optionSelector + " option").length;
        $(name + "Selector").val(optionCount);
        $(name + "Selector").blur();
    });
}

function clearSelection() {
    if (document.selection && document.selection.empty) {
        document.selection.empty();
    } else if (window.getSelection) {
        var sel = window.getSelection();
        sel.removeAllRanges();
    }
}

$(document).ajaxSend(function () {
    ShowProgress();
});

$(document).ajaxStop(function () {
    HideProgress();
});
