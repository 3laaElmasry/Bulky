var dataTable;
$(document).ready(function () {
    // Extract status from URL query string
    var urlParams = new URLSearchParams(window.location.search);
    var status = urlParams.get('status') || 'all'; // Default to empty string if status is not provided
    loadDataTable(status);
});
function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/Order/getall?status=' + status,
            delay: 5000, // Poll every 5 seconds
            error: function (xhr, error, thrown) {
                console.log("Error loading data: ", error);
            }
        },
        pageLength: 10,
        "columns": [
            { data: 'id', "width": "4%" },
            { data: 'applicationUser.name', "width": "20%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
								<a href="/admin/Order/details?orderId=${data}"
								   class="btn btn-primary mx-2">
									<i class="bi bi-pencil-square"></i>
								</a>
								`
                },
                "width": "10%"
            },


        ]
    });
}

