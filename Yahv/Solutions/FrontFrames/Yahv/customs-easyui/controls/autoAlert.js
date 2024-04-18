$(function () {
    var context = '[context]';
    var sign = '[sign]'.toLowerCase();

    if (typeof (postBack) == 'undefined') {
        $.timeouts.alert({
            position: "TC",
            timeout: 1500,
            msg: context,
            type: sign,//info,error,info,warning
        })
    } else {
        $.timeouts.alert({
            position: "TC",
            timeout: 1500,
            msg: context,
            type: sign,//info,error,info,warning
        })
        if (typeof (postBack) != 'undefined' && postBack) {
            postBack();
        }
    }
});