$(function () {
    var context = '[context]';
    var title = '[title]';
    var sign = '[sign]'.toLowerCase();
    var isClose = eval('[isClose]'.toLocaleLowerCase());
    var method = '[method]'.toLowerCase();
    //var url = '[url]';

    var myAlert;

    var $ = top.$;
    myAlert = $.messager.alert;

    function getArgs(strParame) {
        var args = new Object();
        var query = location.search.substring(1); // Get query string
        var pairs = query.split("&"); // Break at ampersand
        for (var i = 0; i < pairs.length; i++) {
            var pos = pairs[i].indexOf('='); // Look for "name=value"
            if (pos == -1) continue; // If not found, skip
            var argname = pairs[i].substring(0, pos); // Extract the name
            var value = pairs[i].substring(pos + 1); // Extract the value
            value = decodeURIComponent(value); // Decode it, if needed
            args[argname] = value; // Store as a property
        }
        return args[strParame]; // Return the object
    }

    //alert(getArgs('pagemodeiframeid'));


    if (sign == 'none' && typeof (postBack) == 'undefined') {
        myAlert(title, context);
    } else {
        myAlert(title, context, sign, function () {

          
            if (isClose && method) {
                if (method == 'dialog') {
                    $.myDialog.close();
                } else {
                    $.myWindow.close();
                }
            }
            else {
                if (typeof (postBack) != 'undefined' && postBack) {
                    postBack();
                }
            }
        });
    }
});

