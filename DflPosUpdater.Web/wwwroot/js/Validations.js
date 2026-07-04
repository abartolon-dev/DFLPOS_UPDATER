function ValidateLetters(fieldValue, fieldName) {
    const letterRegex = /^[a-zA-ZáÁéÉíÍóÓúÚñÑüÜ\s]+$/;

    if (!letterRegex.test(fieldValue)) {

        toastr.error("Debe contener solo letras.");
        document.getElementById(fieldName).value = '';
        document.getElementById(fieldName).focus();
    }
    return true;
}

function ValidateRfc(fieldValue, fieldName) {
    const rfcRegex = /^([A-ZÑ&]{3,4}) ?(?:- ?)?(\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])) ?(?:- ?)?([A-Z\d]{2})([A\d])$/;

    if (!rfcRegex.test(fieldValue)) {

        toastr.error("Formato de RFC incorrecto.");
        document.getElementById(fieldName).value = '';
        document.getElementById(fieldName).focus();
    }
    return true;
}

function ValidateCurp(fieldValue, fieldName) {
    const curpRegex = /^([A-Z&]|[a-z&]{1})([AEIOU]|[aeiou]{1})([A-Z&]|[a-z&]{1})([A-Z&]|[a-z&]{1})([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])([HM]|[hm]{1})([AS|as|BC|bc|BS|bs|CC|cc|CS|cs|CH|ch|CL|cl|CM|cm|DF|df|DG|dg|GT|gt|GR|gr|HG|hg|JC|jc|MC|mc|MN|mn|MS|ms|NT|nt|NL|nl|OC|oc|PL|pl|QT|qt|QR|qr|SP|sp|SL|sl|SR|sr|TC|tc|TS|ts|TL|tl|VZ|vz|YN|yn|ZS|zs|NE|ne]{2})([^A|a|E|e|I|i|O|o|U|u]{1})([^A|a|E|e|I|i|O|o|U|u]{1})([^A|a|E|e|I|i|O|o|U|u]{1})([0-9]{2})$/;

    if (!curpRegex.test(fieldValue)) {


        toastr.error("Formato de CURP incorrecto.");
        document.getElementById(fieldName).value = '';
        document.getElementById(fieldName).focus();
    }
    return true;
}

function ValidateNumbers(fieldValue, fieldName) {
    const numberRegex = /^([0-9])*$/;

    if (!numberRegex.test(fieldValue)) {

        toastr.error("Debe contener solo números.");
        document.getElementById(fieldName).value = '';
        document.getElementById(fieldName).focus();
    }
    return true;
}

function ValidateDates(StartDate, EndDate) {
    var dateEnd = stringToDate(EndDate);
    var dateStart = stringToDate(StartDate);

    if (dateEnd < dateStart) {
        toastr.error('Fecha Final no puede ser menor a Inicial.');
        return false;
    }

    return true;
}

function AvoidLineBreak() { //Evitar "Enters" en inputs
    var key = event.keyCode;
    if (key === 13)
        event.preventDefault();
}

function AvoidLineBreakWithCount(input, minimum, maximum) { //Evitar "Enters" en inputs
    var key = event.keyCode;
    var id_input = input.getAttribute('id');
    var input_value = (event.target.value.trim().length).toString();
    if (key === 13)
        event.preventDefault();
    var myCount = document.getElementById("the-current-count-" + id_input);
    if (myCount) {
        var icon_current = "";
        if (parseInt(input_value) >= minimum)
            icon_current = "<i class='fa fa-check-circle'></i> ";
        document.getElementById("the-current-count-" + id_input).innerHTML = icon_current + input_value + " / " + maximum;
        $("the-current-count-" + id_input).addClass("extra-bold");
        if (parseInt(input_value) >= minimum && parseInt(input_value) <= (maximum - minimum)) {
            myCount.style.color = "green";
            myCount.style.fontWeight = "1000";
        } else if (parseInt(input_value) >= (maximum - minimum)) {
            myCount.style.color = "red";
            myCount.style.fontWeight = "1200";
        } else {
            myCount.style.color = "black";
            myCount.style.fontWeight = "500";
        }
    }
}