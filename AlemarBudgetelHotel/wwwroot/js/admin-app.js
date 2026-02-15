// Admin Panel JavaScript

$(document).ready(function () {
    // Initialize DataTables if present
    if ($.fn.DataTable) {
        $('.data-table').DataTable({
            pageLength: 25,
            order: [[0, 'desc']]
        });
    }

    // Initialize tooltips
    $('[data-bs-toggle="tooltip"]').tooltip();

    // Auto-refresh dashboard every 30 seconds
    if (window.location.pathname === '/Admin') {
        setInterval(function () {
            location.reload();
        }, 30000);
    }
});

// Dashboard Charts
function createRevenueChart(canvasId, labels, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Revenue (₱)',
                data: data,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });
}

function createOccupancyChart(canvasId, occupied, available) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Occupied', 'Available'],
            datasets: [{
                data: [occupied, available],
                backgroundColor: ['#dc3545', '#28a745']
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });
}

// Confirm actions
function confirmCheckIn(reservationId) {
    return confirm('Check in this guest now?');
}

function confirmCheckOut(reservationId) {
    return confirm('Check out this guest? This action cannot be undone.');
}

function confirmCancel(reservationId) {
    return confirm('Cancel this reservation? This action cannot be undone.');
}

function confirmDelete(itemType, itemId) {
    return confirm(`Delete this ${itemType}? This action cannot be undone.`);
}

// Room status update
function updateRoomStatus(roomId, status) {
    if (confirm(`Change room status to ${status}?`)) {
        $.post('/Admin/UpdateRoomStatus', { roomId: roomId, status: status }, function (response) {
            if (response.success) {
                location.reload();
            } else {
                alert('Failed to update room status');
            }
        });
    }
}

// Print report
function printReport() {
    window.print();
}

// Export to Excel
function exportToExcel(tableId, filename) {
    const table = document.getElementById(tableId);
    const html = table.outerHTML;
    const url = 'data:application/vnd.ms-excel,' + encodeURIComponent(html);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename + '.xls';
    link.click();
}
