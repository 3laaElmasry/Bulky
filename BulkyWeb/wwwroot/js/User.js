var dataTable;
$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/User/getall',
            delay: 5000, // Poll every 5 seconds
            error: function (xhr, error, thrown) {
                console.log("Error loading data: ", error);
            }
        },
        pageLength: 10,
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'companyName', "width": "15%" },
            { data: 'role', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
								<a href="/admin/user/edit?userId=${data}"
								   class="btn btn-primary mx-2">
									<i class="bi bi-pencil-square"></i> Edit
								</a>
								
							</div>`
                },
                "width": "25%"
            },


        ]
    });
}

