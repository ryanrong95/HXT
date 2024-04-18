//Date对象扩展
if (!Array.prototype.map) {
    Array.prototype.map = function (callback) {
        for (var i = 0; i < this.length; i++) {
            this[i] = callback(this[i], i);
        }
        return this;
    }
}
if (!Array.prototype.filter) {
    Array.prototype.filter = function (callback) {
        var arry = [];
        for (var i = 0; i < this.length; i++) {
            if (callback(this[i])) {
                arry.push(this[i]);
            }
        }
        return arry;
    }
}
Date.prototype.toDateStr = function () {
    var arry = [];
    arry.push(this.getFullYear());
    arry.push(this.getMonth() + 1);
    arry.push(this.getDate());
    return arry.map(function (i) { return i < 10 ? '0' + i : i; }).join('-');
}
Date.prototype.toDateTimeStr = function () {
    var arry = new Array();
    arry.push(this.getHours());
    arry.push(this.getMinutes());
    arry.push(this.getSeconds());
    return this.toDateStr() + ' ' + arry.map(function (i) { return i < 10 ? '0' + i : i; }).join(':');
}

/*以下两个扩展用于枚举的title和值转换,需要配合页面输出js对象
示例:(10).getName(obj);'正常'.getIndex(obj);
*/
Number.prototype.getName = function (obj) {
    return obj[this.toString()];
};
String.prototype.getIndex = function (obj) {
    return obj[this];
};
if (!window.JSON) {
    window.JSON = {
        parse: function (str) {
            return eval('(' + str + ')');
        },
        stringify: function (obj) {
            return "{}";
        }
    }
}
window.getQueryString = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return "";
}
