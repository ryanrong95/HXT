/*回调windows事件：name 回调标识; data 数据（字符串或json字符串）*/
function FireEvent(name, data) {

    var event = new MessageEvent(name, { 'view': window, 'bubbles': false, 'cancelable': false, 'data':data ==null ?null : JSON.stringify(data) });
    document.dispatchEvent(event);
} 

export function GetAllPrinterNames() {

    FireEvent("GetAllPrinterNames", null);

    if (!!window['_5BF73BF4121DA1C7FB3BF3CE0C3271CC']) {
        var ___1 = window['_5BF73BF4121DA1C7FB3BF3CE0C3271CC'];
        window['_5BF73BF4121DA1C7FB3BF3CE0C3271CC'] = null;
        return ___1;
    }
}
