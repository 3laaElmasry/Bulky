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
            { data: 'author', "width": "20%" },
            { data: 'categoryName', "width": "15%" }

        ]
    });
}

