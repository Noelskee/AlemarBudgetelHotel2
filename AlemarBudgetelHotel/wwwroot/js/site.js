// Customer Portal JavaScript

$(document).ready(function () {
    // Initialize tooltips
    $('[data-bs-toggle="tooltip"]').tooltip();

    // Animate cards on load
    $('.room-card').each(function (index) {
        $(this).css('opacity', '0').delay(index * 100).animate({ opacity: 1 }, 500);
    });
});

// Date and time validation
function validateBookingDates() {
    const checkIn = new Date($('#checkInDateTime').val());
    const now = new Date();

    if (checkIn < now) {
        alert('Check-in time cannot be in the past!');
        return false;
    }

    return true;
}

// Calculate total based on duration
function calculateTotal(duration, price3, price12, price24) {
    switch (duration) {
        case 'ThreeHours':
            return price3;
        case 'TwelveHours':
            return price12;
        case 'TwentyFourHours':
            return price24;
        default:
            return 0;
    }
}

// Format currency
function formatCurrency(amount) {
    return '₱' + parseFloat(amount).toLocaleString('en-PH', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

// Phone number formatting
function formatPhoneNumber(input) {
    let value = input.value.replace(/\D/g, '');
    if (value.length > 11) {
        value = value.substr(0, 11);
    }
    input.value = value;
}

// GCash reference validation
function validateGCashReference(reference) {
    return /^\d{13}$/.test(reference);
}

// Confirm booking
function confirmBooking() {
    return confirm('Please confirm that you have completed the GCash payment and entered the correct reference number.');
}
