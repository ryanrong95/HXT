/*回调windows事件：name 回调标识; data 数据（字符串或json字符串）*/
function FireEvent(name, data) {

    var event = new MessageEvent(name, { 'view': window, 'bubbles': false, 'cancelable': false, 'data':data ==null ?null : JSON.stringify(data) });
    document.dispatchEvent(event);
} 
