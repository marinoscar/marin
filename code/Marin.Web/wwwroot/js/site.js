// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var utils = {
    isNull: function (o) {
        return o === null || o === undefined;
    },
    isNullOrEmpty: function (s) {
        return utils.isNull(s) || s === '';
    }
}

