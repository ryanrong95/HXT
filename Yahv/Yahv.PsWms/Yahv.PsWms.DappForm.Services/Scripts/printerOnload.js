function FireEvent(name, data) {
    var event = new MessageEvent(name, { 'view': window, 'bubbles': false, 'cancelable': false, 'data': data });
    document.dispatchEvent(event);
}

window.onload = function () {
    if (!!window['correct']) {
        window['correct']();
    }
    setTimeout(function () {
        FireEvent('Print', 'data');
    }, 0);
};