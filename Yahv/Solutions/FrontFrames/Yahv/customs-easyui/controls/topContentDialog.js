$(function () {

    var topid = '[topid]';

    var current = top.$.myDialogFuse.options;

    var options = current[current.length - 1];

    options.sumbitDialog = function () {
        $('[type="submit"]').click();
    };


    //alert(options.title);

    if (document.title.length > 0) {
        options.title = document.title;
    }

    var ox = parseInt(options.width);
    var oy = parseInt(options.height);

    ox = ox <= 100 ? ox * top.$(window).width() / 100 : ox;
    oy = oy <= 100 ? oy * top.$(window).height() / 100 : oy;

    var x = Math.abs(top.$(window).width() - ox) / 2;
    var y = Math.abs(top.$(window).height() - oy) / 2;

    options.left = x + "px";
    options.top = y + "px";

    $(window).focus();

    $('#' + topid).dialog(options);

    $('*').keyup(function (e) {
        if (e.keyCode == 27) {
            $('#' + topid).dialog('close');
        }
        e.stopPropagation();
        return false;
    });
});


