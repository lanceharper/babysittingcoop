$(function () {
    $("#recommendation-list").jqGrid({
        url: config.listUrl,
        datatype: 'json',
        mtype: 'POST',
        colNames: ['', 'Name', 'Babysitting Jobs Provided'],
        colModel: [
                    { name: 'Id', index: 'Id', align: 'left', sortable: false, hidden: true },
                    { name: 'Name', index: 'Name', align: 'center', sortable: false },
                    { name: 'ProvidedCount', index: 'ProvidedCount', align: 'right', sortable: false }
                ],
        autowidth: true,
        caption: '',
        pager: $("#recommendation-pager"),
        pgbuttons: false,
        pgtext: null,
        rowNum: '20',
        viewrecords: true,
        height: '100%',
        sortname: 'ProvidedCount',
        sortorder: "desc",
        postData: {
            id: function () { return $("#babysitters").val(); }
        }
    });

    $("#babysitters").change(function () {

        $("#recommendation-list").trigger("reloadGrid");
    });
});