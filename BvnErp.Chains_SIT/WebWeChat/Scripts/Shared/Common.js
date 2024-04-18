//公共方法

//正则验证方法

//验证身份证
function ValidIDNumber(str) {
    // 身份证号码为15位或者18位，15位时全为数字，18位前17位为数字，最后一位是校验位，可能为数字或字符X  
    var reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
    if (reg.test(str) === false) {
        return false;
    }
    return true;
}

//验证手机号
function ValidMobile(str) {
    var reg = /^1[3|4|5|7|8|9][0-9]\d{8}$/;
    if (reg.test(str) === false) {
        return false;
    }
    return true;
}

//验证整数
function ValidNumber(str) {
    var reg = /^[0-9]*$/;
    if (reg.test(str) === false) {
        return false;
    }
    return true;
}

//验证整数和小数
function ValidDecimal(str) {
    var reg = /^[0-9]+([.]{1}[0-9]+){0,1}$/;
    if (reg.test(str) === false) {
        return false;
    }
    return true;
}

//验证邮箱
function ValidMail(str) {
    var reg = /^[A-Za-z\d]+([-_.][A-Za-z\d]+)*@([A-Za-z\d]+[-.])+[A-Za-z\d]{2,4}$/;
    if (reg.test(str) === false) {
        return false;
    }
    return true;
}

//验证密码
function ValidPassword(str) {
    var reg = /^(?![A-Z]+$)(?![a-z]+$)(?!\d+$)\S{6,12}$/;
    if (reg.test(str) === false) {
        return false;
    }
    return true;
}
//日期加月份
Date.prototype.addMonth = function (addMonth) {
    var y = this.getFullYear();
    var m = this.getMonth();
    var nextY = y;
    var nextM = m;
    //如果当前月+要加上的月>11 这里之所以用11是因为 js的月份从0开始
    if (m > 11) {
        nextY = y + 1;
        nextM = parseInt(m + addMonth) - 11;

    } else {
        nextM = this.getMonth() + addMonth

    }
    var daysInNextMonth = Date.daysInMonth(nextY, nextM);
    var day = this.getDate();
    if (day > daysInNextMonth) {
        day = daysInNextMonth;

    }
    return new Date(nextY, nextM, day);

};
Date.daysInMonth = function (year, month) {
    if (month == 1) {
        if (year % 4 == 0 && year % 100 != 0)
            return 29;
        else
            return 28;

    } else if ((month <= 6 && month % 2 == 0) || (month = 6 && month % 2 == 1))
        return 31;
    else
        return 30;
};
Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(), //day
        "H+": this.getHours(), //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3), //quarter
        "S": this.getMilliseconds() //millisecond
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}
 