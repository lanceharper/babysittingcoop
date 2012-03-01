﻿$(function () {
    $("#babysitting-list").jqGrid({
        url: config.listUrl,
        datatype: 'json',
        mtype: 'POST',
        colNames: ['', 'Name', 'Provided Points', 'Receiver Points', 'Total Points'],
        colModel: [
                    { name: 'Id', index: 'Id', align: 'left', sortable: false, hidden: true },
                    { name: 'Name', index: 'Name', align: 'center' },
                    { name: 'ProvidedPoints', index: 'ProvidedPoints', align: 'right' },
                    { name: 'ReceiverPoints', index: 'ReceiverPoints', align: 'right' },
                    { name: 'TotalPoints', index: 'TotalPoints', align: 'right' }
                ],
        autowidth: true,
        caption: '',
        pager: $("#babysitting-pager"),
        rowNum: '20',
        viewrecords: true,
        height: '100%',
        sortname: 'TotalPoints',
        sortorder: "desc",
        postData: {

        }
    });

});
    