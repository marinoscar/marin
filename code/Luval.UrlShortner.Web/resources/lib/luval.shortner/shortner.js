var shortner = {
    init: function () {
        $('#show-code').hide();
        $('#UrlId').focus();
        $('#create-button').on('click', function () {
            shortner.createItem();
        });
        $('#create-new').on('click', function () {
            location.reload();
        });
        $('#copy-clipboard').on('click', function () {
            shortner.copyTextToClipboard($('#ResultUrl').val());
            shortner.setAlertMessage('Short url has been copied to the clipboard', 'success');
        });
        $('#paste-button').on('click', function () {
            shortner.pasteText();
        });
    },
    pasteText: async function () {
        const text = await navigator.clipboard.readText();
        $('#UrlId').val(text);
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
                    if (data.success == true)
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
        $('#ResultUrl').val(data.url);
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
    },
    copyTextToClipboard: function (text) {
        if (!navigator.clipboard) {
            shortner.fallbackCopyTextToClipboard(text);
            return;
        }
        navigator.clipboard.writeText(text).then(function () {
            console.log('Async: Copying to clipboard was successful!');
        }, function (err) {
            console.error('Async: Could not copy text: ', err);
        });
    },
    fallbackCopyTextToClipboard: function (text) {
        var textArea = document.createElement("textarea");
        textArea.value = text;

        // Avoid scrolling to bottom
        textArea.style.top = "0";
        textArea.style.left = "0";
        textArea.style.position = "fixed";

        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        try {
            var successful = document.execCommand('copy');
            var msg = successful ? 'successful' : 'unsuccessful';
            console.log('Fallback: Copying text command was ' + msg);
        } catch (err) {
            console.error('Fallback: Oops, unable to copy', err);
        }

        document.body.removeChild(textArea);
    }
}