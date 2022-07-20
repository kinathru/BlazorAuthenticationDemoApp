var validNavigation = false;


function renderjQueryComponents()
{
    $(".timepicker").bind("change paste keyup select focusout", function () {
        var max = parseInt($(this).attr('max'));
        var min = parseInt($(this).attr('min'));
        if ($(this).val() > max) {
            $(this).val(max);
        }
        else if ($(this).val() < min) {
            $(this).val(min);
        }
    });
   
}

function InitBootstrapScript() {
    console.log("Init Bootstrap Script!");
}

