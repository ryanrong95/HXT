(function () {
    var doc = window.document,
		a = {},
		rExtractUri = /(?:http|https|file):\/\/.*?\/.+?.js/,
		isLtIE8 = ('' + doc.querySelector).indexOf('[native code]') === -1;
    //JS获取当前脚本文件的绝对路径
    getCurrAbsPath = function () {
        // FF,Chrome
        if (doc.currentScript) {
            return doc.currentScript.src;
        }

        var stack;
        try {
            a.b();
        }
        catch (e) {
            stack = e.stack || e.stacktrace || e.fileName || e.sourceURL;
        }
        // IE10
        if (stack) {
            var absPath = rExtractUri.exec(stack)[0];
            if (absPath) {
                return absPath;
            }
        }

        // IE5-9
        for (var scripts = doc.scripts,
            i = scripts.length - 1,
            script; script = scripts[i--];) {
            if (script.readyState === 'interactive') {
                // if less than ie 8, must get abs path by getAttribute(src, 4)
                return isLtIE8 ? script.getAttribute('src', 4) : script.src;
            }
        }
    };
    var selfUrl = getCurrAbsPath();//该js的路径
    var selfUrlArr = selfUrl.split("/");//截取该js的路径
    var jsName = selfUrlArr[selfUrlArr.length - 1];//该js的名称
    var indexLocal = selfUrl.indexOf(jsName);////该js的名称的字符串所在位置
    var prexUrl = selfUrl.slice(0, indexLocal);//获取script 外部引用的绝对地址
    //拼接所有的相对地址s
    //debounce.js是节流函数
    var plugins = ['clientCrmPlus', 'contactCrmPlus', 'consigneeCrmPlus', 'standardPartNumberCrmPlus',
        'companyCrmPlus', 'supplierCrmPlus', 'standardBrandCrmPlus', 'tagboxCrmPlus',
        'agentBrandCrmPlus',
        'clientAddressCrmPlus', 'clientContactCrmPlus', 'clientInvoiceCrmPlus', 'clientPayerCrmPlus', 'clientRegisterCrmPlus',
        'companyPayeeCrmPlus', 'companyPayerCrmPlus',
        'supplierPayeeCrmPlus', 'supplierRegisterCrmPlus', 'enterpriseCrmPlus'
    ];
    var html = '';
    for (var index = 0; index < plugins.length; index++) {
        html += '<script src="' + prexUrl + plugins[index] + '.js"></script' + '>'
    }
    document.write(html);
})();

