var shortner = {
    init: function() {
        $('#show-code').hide();
        $('#UrlId').focus();
        $('#create-button').on('click', function () {
            shortner.createItem();
        });
        $('#create-new').on('click', function () {
            location.reload();
        });
    },
    createItem: function () {
        var payload = shortner.getObject();
        if (!shortner.isValidUrl(payload.OriginalUri)) {
            shortner.setAlertMessage('Please enter a valid url', 'danger');
            return;
        }
        $.ajax({
            type: 'post',
            url: '/Shortner/Create',
            data: payload,
            success: function (data, status, jqXHR) {
                if (jqXHR.status == "200") {
                    if (data.isSuccess == true)
                        shortner.onSuccess(data);
                    else
                        shortner.setAlertMessage(data.message, 'danger');
                    return;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.status == "200") {
                    return;
                }
                alert('Failed to save the post ' + jqXHR.status + ' ' + jqXHR.statusText);
            },
            dataType: "json"
        });
    },
    onSuccess: function (data) {
        shortner.setAlertMessage(data.message, 'success');
        $('#create-code').hide();
        $('#show-code').show();
    },
    validate: function (payload) {
        return shortner.isValidUrl(payload.OriginalUri);
    },
    setAlertMessage: function (message, type) {
        var banner = $('#alertbanner');
        var textEl = $('#alertmessage');
        banner.removeClass('alert-success');
        banner.removeClass('alert-warning');
        banner.removeClass('alert-danger');
        banner.removeClass('alert-primary');
        banner.addClass('alert-' + type);
        textEl.html(message);
        banner.show().delay(30000).fadeOut();
    },
    getObject: function () {
        return {
            Id: $('#IdText').val(),
            OriginalUri: $('#UrlId').val()
        };
    },
    isValidUrl: function (value) {
        let url;
        try {
            url = new URL(value);
        } catch (_) {
            return false;
        }
        return url.protocol === "http:" || url.protocol === "https:";
    }
}