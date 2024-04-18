//扩展方法from提交的时候验证输入值是否为下拉列表的值
(function () {
	$.extend($.fn.combobox.defaults.rules, {
		OnlySelectDropValue: {
			validator: function (value, param) {
				var data = $("#" + param[0]).combobox('getData');
				var flag = false;
				if (data.length) {
					for (var i = 0; i < data.length; i++) {
						if (value == data[i].Name) {
							flag = true;
							return flag;
						} else {
							flag = false;
						}
					}
				}
				return flag;
			},
			message: '只能选择下拉列表的数据'
		}
	});
	var doc = window.document,
		a = {},
		rExtractUri = /(?:http|https|file):\/\/.*?\/.+?.js/,
		isLtIE8 = ('' + doc.querySelector).indexOf('[native code]') === -1;
	//JS获取当前脚本文件的绝对路径
	window.getCurrAbsPath = function () {
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
	var selfUrl = window.getCurrAbsPath();//该js的路径
	var selfUrlArr = selfUrl.split("/");//截取该js的路径
	var jsName = selfUrlArr[selfUrlArr.length - 1];//该js的名称
	var indexLocal = selfUrl.indexOf(jsName);////该js的名称的字符串所在位置
	var prexUrl = selfUrl.slice(0, indexLocal);//获取script 外部引用的绝对地址
	//拼接所有的相对地址
	//debounce.js是节流函数
	var plugins = ['ChinaAreaData', 'ChinaArea'];
	var html = '';
	for (var index = 0; index < plugins.length; index++) {
		html += '<script src="' + prexUrl + plugins[index] + '.js"></script' + '>'
	}
	document.write(html);
})();