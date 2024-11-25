// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const numberInputs = document.querySelectorAll('input[type="number"]');

function selectAllText(event) {
    event.target.select();
}

numberInputs.forEach(input => {
    input.addEventListener('focus', selectAllText);
    input.addEventListener('click', selectAllText);
});


function showToast(type, message) {
    // Create a new toast element
    var toastElement = $('<div class="toast" role="alert" aria-live="assertive" aria-atomic="true">' +
        '<div class="toast-header">' +
        '<strong class="me-auto" id="toastTitle">' + (type.toUpperCase()) + '</strong>' +
        '<button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>' +
        '</div>' +
        '<div class="toast-body" id="toastBody">' + message + '</div>' +
        '</div>');

    // Set the background color based on the type
    toastElement.removeClass('bg-success bg-danger bg-warning');
    if (type === 'success') {
        toastElement.addClass('bg-success text-white');
    } else if (type === 'warning') {
        toastElement.addClass('bg-warning text-dark');
    } else {
        toastElement.addClass('bg-danger text-white');
    }

    // Append the new toast to the container
    $('.toast-container').append(toastElement);

    // Show the toast
    var toast = new bootstrap.Toast(toastElement, {
        delay: 20000
    });
    toast.show();
}

/*///// BootstrapTable functions /////*/
function TotalFormatter(data, footerValue) {
    var field = this.field; // Get the field we are summing
    var total = data.reduce(function (sum, row) {
        return sum + (+row[field]); // Convert value to number and sum up
    }, 0);
    return Math.floor(total * 100) / 100;
}

/*///// Select2 Init /////*/
$(document).ready(function () {
    $('.single-select').select2({
        theme: "bootstrap-5",
        placeholder: "Select...",
        allowClear: false
    });

    $('.multi-select').select2({
        theme: "bootstrap-5",
        placeholder: "Select...",
        allowClear: true,
        closeOnSelect: false
    });
});