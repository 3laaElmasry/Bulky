var dataTable;
$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/Order/getall',
            error: function (xhr, error, thrown) {
                console.log("Error loading data: ", error);
            }
        },
        "columns": [
            { data: 'id', "width": "4%" },
            { data: 'applicationUser.name', "width": "15%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "15%" },
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
                "width": "25%"
            },


        ]
    });
}

