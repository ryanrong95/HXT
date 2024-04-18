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

//验证手机号
function ValidHKMobile(str) {
    var reg = /^\d{1,20}$/;
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
//验证上级页面
$(function () {
    pushHistory();
    window.addEventListener("popstate", function (e) {
        axios({
            url: "/Home/CheckSuperiorPage",
            method: "post",
            data: {},
        }).then(function (response) {
            if (response.data.type == "error") {
                location.href = "http://wl.net.cn/";
            } else {
                window.history.back(-1);
            }
        }).catch(function (error) {

        });
    }, false);
    function pushHistory() {
        var state = {
            title: "title",
            url: "#"
        };
        window.history.pushState(state, "title", "");
    }
});
/*
       三个参数
       file：一个是文件(类型是图片格式)，
       w：一个是文件压缩的后宽度，宽度越小，字节越小
       objDiv：一个是容器或者回调函数
       photoCompress()
        */
function photoCompress(file, w, objDiv) {
    var ready = new FileReader();
    /*开始读取指定的Blob对象或File对象中的内容. 当读取操作完成时,readyState属性的值会成为DONE,如果设置了onloadend事件处理程序,则调用之.同时,result属性中将包含一个data: URL格式的字符串以表示所读取文件的内容.*/
    ready.readAsDataURL(file);
    ready.onload = function () {
        var re = this.result;
        canvasDataURL(re, w, objDiv)
    }
}
function canvasDataURL(path, obj, callback) {
    var img = new Image();
    img.src = path;
    img.onload = function () {
        var that = this;
        // 默认按比例压缩
        var w = that.width,
            h = that.height,
            scale = w / h;
        w = obj.width || w;
        h = obj.height || (w / scale);
        var quality = 0.7;  // 默认图片质量为0.7
        //生成canvas
        var canvas = document.createElement('canvas');
        var ctx = canvas.getContext('2d');
        // 创建属性节点
        var anw = document.createAttribute("width");
        anw.nodeValue = w;
        var anh = document.createAttribute("height");
        anh.nodeValue = h;
        canvas.setAttributeNode(anw);
        canvas.setAttributeNode(anh);
        ctx.drawImage(that, 0, 0, w, h);
        // 图像质量
        if (obj.quality && obj.quality <= 1 && obj.quality > 0) {
            quality = obj.quality;
        }
        // quality值越小，所绘制出的图像越模糊
        var base64 = canvas.toDataURL('image/jpeg', quality);
        // 回调函数返回base64的值
        callback(base64);
    }
}
/**
 * 将以base64的图片url数据转换为Blob
 * @param urlData
 *            用url方式表示的base64图片数据
 */
function convertBase64UrlToBlob(urlData) {
    var arr = urlData.split(','), mime = arr[0].match(/:(.*?);/)[1],
        bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }
    return new Blob([u8arr], { type: mime });
}