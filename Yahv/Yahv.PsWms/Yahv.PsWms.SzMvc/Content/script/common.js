//公共方法

//正则验证方法

//验证身份证
function ValidIDNumber(str) {
    // 身份证号码为15位或者18位，15位时全为数字，18位前17位为数字，最后一位是校验位，可能为数字或字符X,香港身份证

    var reg = /^(\d{15}$)|(\d{18}$)|(\d{17}(\d|X|x)$)|(([A-Z]\d{6,10}(\(\w{1}\))?)$)/;
    if (reg.test(str) === false) {
        return false;
    }
    return true;
}

//验证手机号
function ValidMobile(str) {
    var reg = /^[1][3-8]\d{9}$|([6|9])\d{7}$|[0][9]\d{8}$|6\d{5}$/;
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
function ValidPassword(callback, str) {
    var reg1 = /^([a-zA-Z]|\d|[@!#$%^&*.~+=]){8,32}$/;
    var reg2 = /^(?![a-zA-Z]+$)(?!\d+$)(?![@!#$%^&*.~+=]+$)\S/;
    if (!reg1.test(str)) {
        callback(new Error("请输入8-32位的字母、数字与字符[@!#$%^&*.~+=]"));
        return false;
    } else if (!reg2.test(str)) {
        callback(new Error("密码不能全部为字母、数字或特殊字符"));
        return false;
    }
    return true;
}
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
//通用页面跳转
function commonSkip(id, url, aa, active) {
    var arr = [id, aa, active];
    var stuAnswerArr = JSON.stringify(arr);//转成json字符串
    //JS模拟post提交
    var REVQForm = document.createElement("form");
    REVQForm.setAttribute("method", 'POST');
    REVQForm.setAttribute("action", url);
    REVQForm.innerHTML = "";
    var stuAnswerInput = document.createElement("input");
    stuAnswerInput.setAttribute("type", "hidden");
    stuAnswerInput.setAttribute("name", 'para');
    stuAnswerInput.setAttribute("value", stuAnswerArr);
    REVQForm.appendChild(stuAnswerInput);
    document.body.appendChild(REVQForm);
    REVQForm.submit();
}
function commonSkipUrl(id, url, lasturl) {
    var arr = [id, lasturl];
    var stuAnswerArr = JSON.stringify(arr);//转成json字符串
    //JS模拟post提交
    var REVQForm = document.createElement("form");
    REVQForm.setAttribute("method", 'POST');
    REVQForm.setAttribute("action", url);
    REVQForm.innerHTML = "";
    var stuAnswerInput = document.createElement("input");
    stuAnswerInput.setAttribute("type", "hidden");
    stuAnswerInput.setAttribute("name", 'para');
    stuAnswerInput.setAttribute("value", stuAnswerArr);
    REVQForm.appendChild(stuAnswerInput);
    document.body.appendChild(REVQForm);
    REVQForm.submit();
}
//加法 
function accAdd(arg1, arg2) {
    var r1, r2, m;
    try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
    try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
    m = Math.pow(10, Math.max(r1, r2))
    return (arg1 * m + arg2 * m) / m
}
//减法 
function subtr(arg1, arg2) {
    var r1, r2, m, n;
    try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
    try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
    m = Math.pow(10, Math.max(r1, r2));
    n = (r1 >= r2) ? r1 : r2;
    return ((arg1 * m - arg2 * m) / m).toFixed(n);
}
//文件上传
function UploadFile(fileOption, url, callback) {
    var file = fileOption.file;
    var size = file.size / 1024;
    var imgArr = ["image/jpg", "image/bmp", "image/jpeg", "image/gif", "image/png"];
    let config = {
        headers: { 'Content-Type': 'multipart/form-data' }
    };
    var result = {
        type: false,
        msg: "",
        file: "",
    }; //返回值
    if (imgArr.indexOf(file.type) > -1 && size > 500) { //大于500kb的图片压缩
        photoCompress(file, { quality: 0.8 }, function (base64Codes) {
            var bl = convertBase64UrlToBlob(base64Codes);
            var form = new FormData(); // FormData 对象
            form.append("file", bl, "file_" + Date.parse(new Date()) + ".jpg"); // 文件对象
            //添加请求头
            axios.post(url, form, config)
                .then(function (response) {
                    if (response.type == "error") {
                        result.type = false;
                        result.msg = response.msg;
                        callback(result);
                    }
                    else {
                        result.type = true;
                        result.file = response.data.data;
                        callback(result);
                    }
                });
        });
    } else if (imgArr.indexOf(file.type) <= -1 && size > 1024 * 3) {
        result.type = false;
        result.msg = "上传的文件大小不能大于3M";
        callback(result);
    } else {
        var form = new FormData(); // FormData 对象
        form.append("file", file); // 文件对象
        //添加请求头
        axios.post(url, form, config)
            .then(function (response) {
                if (response.type == "error") {
                    result.type = false;
                    result.msg = response.msg;
                    callback(result);
                }
                else {
                    result.type = true;
                    result.file = response.data.data;
                    callback(result);
                }
            });
    }
}

window.onload = function () {

    /**
     *对Date的扩展，将 Date 转化为指定格式的String
     *月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符，
     *年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)
     *例子：
     *(new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423
     *(new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18
     */
    Date.prototype.format = function (fmt) {
        var o = {
            "M+": this.getMonth() + 1, //月份
            "d+": this.getDate(), //日
            "h+": this.getHours(), //小时
            "m+": this.getMinutes(), //分
            "s+": this.getSeconds(), //秒
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度
            "S": this.getMilliseconds() //毫秒
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    }

};

//通用页面跳转
//function commonSkip(id, url) {
//    //JS模拟post提交
//    var REVQForm = document.createElement("form");
//    REVQForm.setAttribute("method", 'POST');
//    REVQForm.setAttribute("action", url);
//    REVQForm.innerHTML = "";
//    var stuAnswerInput = document.createElement("input");
//    stuAnswerInput.setAttribute("type", "hidden");
//    stuAnswerInput.setAttribute("name", 'para');
//    stuAnswerInput.setAttribute("value", id);
//    REVQForm.appendChild(stuAnswerInput);
//    document.body.appendChild(REVQForm);
//    REVQForm.submit();
//}

// 获得当前时间戳
function timestamp() {
    return Date.parse(new Date());
}
