/* Confirmation Modal */
var targetModal = new bootstrap.Modal(document.getElementById('targetModal', {}))
targetModal.show();

var dismissModal = document.getElementById('dismissModal');
dismissModal.addEventListener("click", () => {
    targetModal.hide();
})

/* Phone number */

const phoneInputField = document.querySelector("#phone");
const phoneInput = window.intlTelInput(phoneInputField, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});

/* Choose File */
$("#files").change(function () {
    filename = this.files[0].name;
    console.log(filename);
    $("#choosenfile").text(filename);
});

/* Password */

function passtoggle() {
    var x = document.getElementById("floatingPassword");
    if (x.type === "password") {
        x.type = "text";
        document.querySelectorAll("i.fa.fa-eye-slash").forEach(i => i.style.display = "none");
        document.querySelectorAll("i.fa.fa-eye").forEach(i => i.style.display = "block");
    }
    else {
        x.type = "password";
        document.querySelectorAll("i.fa.fa-eye-slash").forEach(i => i.style.display = "block");
        document.querySelectorAll("i.fa.fa-eye").forEach(i => i.style.display = "none");
    }
}

$(document).ready(function () {
    $(".t-tab > .position-relative >  .btn > .rounded").click(function () {
        $(".t-tab > .position-relative >  .btn > .rounded").removeClass("activeTab");
        $(this).addClass("activeTab");
    });
    $(".t-filter > .btn").click(function () {
        $(".t-filter > .btn").removeClass("activeFilter");
        $(this).addClass("activeFilter");
    });
});