// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var consoleSite = {
    jsonPayloadEditor: "",
    jsonResponseEditor: "",
    jsonParameterEditor: "",
    jsonHeadersEditor: "",

    initEditor: function (element, views, initialJson) {
        var container = document.getElementById(element);
        var options = {
            mode: views[0],
            modes: views
        };
        var editor = new JSONEditor(container, options);
        editor.set(initialJson);
        return editor;
    },
    initPayloadEditor: function () {
        consoleSite.jsonPayloadEditor = consoleSite.initEditor('jsonPayload', ['code', 'form', 'text', 'tree', 'view', 'preview'], {});
    },
    initParameterEditor: function () {
        consoleSite.jsonParameterEditor = consoleSite.initEditor('jsonParamters', ['code','text', 'tree'], { paramterKey: "parameterValue" });
    },
    initHeadersEditor: function () {
        consoleSite.jsonHeadersEditor = consoleSite.initEditor('jsonHeaders', ['code', 'text', 'tree'], { "Content-Type": "application/json" });
    },
    initResponseEditor: function (element) {
        consoleSite.jsonResponseEditor = consoleSite.initEditor('jsonResponse', ['view', 'text', 'preview'], {});
    },
    getAppUrl: function () {
        return location.protocol + '//' + location.host;
    },
    initPage: function () {
        $('#request').val(consoleSite.getAppUrl());
        consoleSite.initPayloadForm();
        consoleSite.initTabClick();
        consoleSite.initPayloadEditor();
        consoleSite.initParameterEditor();
        consoleSite.initHeadersEditor();
        consoleSite.initResponseEditor();
    },
    initPayloadForm: function () {
        $('#make-request').on('click', function () {
            var json = consoleSite.jsonPayloadEditor.get();
            var url = $('#request').val();
            var method = $('#method').val();
            $.ajax({
                type: method,
                url: url,
                data: json,
                success: function (data, status, jqXHR) {
                    if (jqXHR.status == "200")
                        consoleSite.jsonResponseEditor.set(data);
                    else {
                        var json = { status: jqXHR.status, statusText: jqXHR.statusText };
                        consoleSite.responseEditor.set(json);
                    }
                    consoleSite.setSelectedTab('response');

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var json = { status: jqXHR.status, statusText: jqXHR.statusText };
                    consoleSite.jsonResponseEditor.set(json);
                    consoleSite.setSelectedTab('response');
                },
                dataType: "json"
            });
        });
    },
    initTabClick: function () {
        $('a[data-tab]').on('click', function () {
            consoleSite.setSelectedTab($(this).data('tab'));
        });
    },
    setSelectedTab: function (tab) {
        $('a[data-tab]').removeClass('active');
        $('a[data-tab="' + tab + '"]').addClass('active');
        $('div[data-editor]').addClass('d-none');
        if (tab == 'body')
            $('#payload-tab').removeClass('d-none');
        if (tab == 'parameters')
            $('#parameters-tab').removeClass('d-none');
        if (tab == 'headers')
            $('#headers-tab').removeClass('d-none');
        if (tab == 'response')
            $('#response-tab').removeClass('d-none');
    }
}
