﻿
$(function () {
    'use strict';

    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload({
        autoUpload: false
    });

    // Load existing files:
    //    $.getJSON($('#fileupload form').prop('action'), function (files) {
    //        var fu = $('#fileupload').data('fileupload');
    //        fu._adjustMaxNumberOfFiles(-files.length);
    //        fu._renderDownload(files)
    //            .appendTo($('#fileupload .files'))
    //            .fadeIn(function () {
    //                // Fix for IE7 and lower:
    //                $(this).show();
    //            });
    //    });

    $('#fileupload').bind('fileuploaddone', function (e, data) {
        if (data.jqXHR.responseText || data.result) {
            var fu = $('#fileupload').data('fileupload');
            var JSONjQueryObject = (data.jqXHR.responseText) ? jQuery.parseJSON(data.jqXHR.responseText) : data.result;
            fu._adjustMaxNumberOfFiles(JSONjQueryObject.files.length);
            //                debugger;
            fu._renderDownload(JSONjQueryObject.files)
                .appendTo($('#fileupload .files'))
                .fadeIn(function () {
                    // Fix for IE7 and lower:
                    $(this).show();
                });
        }
    });

    // Open download dialogs via iframes,
    // to prevent aborting current uploads:
    $('#fileupload .files a:not([target^=_blank])').live('click', function (e) {
        e.preventDefault();
        $('<iframe style="display:none;"></iframe>')
            .prop('src', this.href)
            .appendTo('body');
    });

});