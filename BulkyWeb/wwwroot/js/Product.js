$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    var dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/product/getall',
            error: function (xhr, error, thrown) {
                console.log("Error loading data: ", error);
            }
        },
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'price', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'categoryName', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
								<a href="/admin/product/upsert?productId=${data}"
								   class="btn btn-primary mx-2">
									<i class="bi bi-pencil-square"></i> Edit
								</a>
								<a href="/admin/product/delete?productId=${data}"
								   class="btn btn-danger mx-2 ">
									<i class="bi bi-trash-fill"></i> Delete
								</a>
							</div>`
                },
                "width" : "25%"
            },


        ]
    });
}

