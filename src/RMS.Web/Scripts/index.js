
$(document).ready(function () {
    
    $('#datatab tfoot th').each(function () {
        $(this).html('<input type="text" />');
    });

    var oTable = $('#requestsdatatable').DataTable({
        "serverSide": true,
        "ajax": {
            "type": "POST",
            "url": '/Home/DataHandler',
            "contentType": 'application/json; charset=utf-8',
            'data': function (data) { return data = JSON.stringify(data); }
        },
        "dom": 'frtiS',
        "scrollY": 500,
        "scrollX": true,
        "scrollCollapse": true,
        "scroller": {
            loadingIndicator: false
        },
        "processing": true,
        "paging": true,
        "sPaginationType": "full_numbers",
        "deferRender": true,
        "columns": [
            { "data": "Name" },
            { "data": "Description" },
            {
                "data": "RaisedDate",
                "render": function (data) {
                    var ticks = parseInt(data.replace("/Date(", "").replace(")/", ""), 10);
                    var date = new Date(ticks);
                    var month = date.getMonth() + 1;
                    return date.getDate() + "-" + (month.length > 1 ? month : "0" + month) + "-" + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes();
                }
            },
            {
                "data": "DueDate",
                "render": function (data) {
                    var ticks = parseInt(data.replace("/Date(", "").replace(")/", ""), 10);
                    var date = new Date(ticks);
                    var month = date.getMonth() + 1;
                    return date.getDate() + "-" + (month.length > 1 ? month : "0" + month) + "-" + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes();
                }
            },
            {
                "data": "StatusName",
                "render": function (data) {
                    return '<span class="badge">' + data + '</span>'
                }
            },
            {// this is Actions Column 
                mRender: function (data, type, row) {
                    var linkDownload = '<a href="/Home/Download?id=-1" class="openDialog btn btn-default btn-primary"><i class="fa fa-download"></i></a>';
                    linkDownload = linkDownload.replace("-1", row.Id);

                    return linkDownload;
                }
            },
            {// this is Actions Column 
                mRender: function (data, type, row) {
                    var linkDetails = '<a href="/Home/Details?id=-1" class="openDialog btn btn-default btn-info"><i class="fa fa-eye"></i></a>';
                    linkDetails = linkDetails.replace("-1", row.Id);

                    var linkEdit = '<a href="/Home/Edit?id=-1" class="openDialog btn btn-default btn-warning"><i class="fa fa-pencil"></i></a>';
                    linkEdit = linkEdit.replace("-1", row.Id);

                    var linkDelete = '<a href="/Home/Delete?id=-1" class="deleteDialog btn btn-default btn-danger"><i class="fa fa-trash-o"></i></a>';
                    linkDelete = linkDelete.replace("-1", row.Id);

                    return linkDetails + " | " + linkEdit + " | " + linkDelete;
                }
            }
        ],
        "order": [0, "asc"]

    });

    oTable.columns().every(function () {
        var that = this;

        $('input', this.footer()).on('keyup change', function () {
            that
                .search(this.value)
                .draw();
        });
    });

});
