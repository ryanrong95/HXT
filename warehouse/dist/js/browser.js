/*回调windows事件：name 回调标识; data 数据（字符串或json字符串）*/
function FireEvent(name, data) {
    var event = new MessageEvent(name, { 'view': window, 'bubbles': false, 'cancelable': false, 'data': data });
    document.dispatchEvent(event);
}

function PageEvent(data) {FireEvent("PageEvents", data);
}


PageEvent("{\"name\":\"pageinit\"}");


//var print = {
//    label: function (fileDescript) {
//        FireEvent('print', fileDescript);
//    },
//    template: function (json) {
//        FireEvent('print', fileDescript);
//    }
//};


//export function printLable(data) {

    
//    if (true) {

//    }

//    FireEvent("PageEvents", data);
//}

//print.label();

//function printLable(fileDescript) {

//    if (true) {

//    }

//    FireEvent('printLable', fileDescript);
//}

