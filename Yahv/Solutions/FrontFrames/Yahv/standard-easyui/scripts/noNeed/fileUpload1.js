////文件上传第一版
(function ($) {
    //验证文件大小
    function validSize(files, maxLength) {
        if (!maxLength || isNaN(maxLength)) {
            return true;
        }
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            if (file.size > maxLength) {
                $.timeouts.alert({ type: "warning", msg: file.name + '超过' + (maxLength / 1024) + "kb",position:"TC"})
                return false;
            }
        }
        return true;
    }
    //判断文件类型
    function validExtension(files, extensions) {
        if (!extensions) {
            return true;
        }
        var arry = extensions.split('|');
        for (var i = 0; i < files.length; i++) {
            var file = files[i], extension = file.name.substring(file.name.lastIndexOf('.') + 1).toLowerCase();
            if (arry.filter(function (item) { return extension == item; }).length == 0) {
                $.timeouts.alert({ type: "error", msg: file.name + '不是' + extensions, position: "TC" })
                return false;
            }
        }
        return true;
    }

    $.fn.fileUpload = function (opt, param) {
        var sender = this;
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.fileUpload.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }
        var options = opt || {};
        var data_options = {};
        var s = $.trim(sender.attr("data-options"));
        //如果html配置了data-options
        if (s) {
            if (s.substring(0, 1) != "{") {
                s = "{" + s + "}";
            }
            data_options = (new Function("return " + s))();
            //$.extend(true, {}, $.fn.fileUpload.defaults, data_options, opt);第一个参数为true，则是深度合并对象
            options = $.extend(true, {}, $.fn.fileUpload.defaults, data_options, opt);
        } else {
            options = $.extend(true, {}, $.fn.fileUpload.defaults, opt);
        }

        //如果已经初始化，就不需要再进行初始化了
        //if ($(sender).data("progress")) {
        //    //show();
        //    return;
        //}
        $(sender).data("options", options);//存储options的值

        //为页面添加dom元素---开始
        var $progress = $("<div style='margin-top:5px;'></div>");
        $(sender).data("progress", $progress);
        $(sender).after($progress);
        //为页面添加dom元素---结束

        //进度条增长
        function start() {
            var value = $progress.progressbar('getValue');
            if (value < 100) {
                value += Math.floor(Math.random() * 10);
                $progress.progressbar('setValue', value);
                setTimeout(arguments.callee, 200);
            } else {
                $($progress).hide();
                $progress.progressbar('setValue', 0);
            }
        };
        //初始化工具栏
        var init = function () {
            //初始化进度条
            $progress.progressbar({
                width: 200,
                height: "14px",
                maxLength:5000
            })
            $progress.hide();
            $(sender).filebox({
                buttonText: "选择文件",
                width: 200,
                multiple: true,
                accept:'.png',
                onChange: function (newValue, oldValue) {
                    $($progress).show();
                    start();
                    var files = $(sender).filebox("files");
                    console.log(files);
                    var validResult = validSize(files, 5000);
                    var extension = validExtension(files, 'png');
                    if (validResult && extension) {
                        $.timeouts.alert({ type: "success", msg: "文件可以上传啦", position: "TC" })
                    }
                }
            })
            
        }
        init.call(this);
    }

    $.fn.fileUpload.defaults = {
    };

    $.fn.fileUpload.methods = {
        options: function (jq) {
            return $(jq).data("options");//返回options
        }
    };
    $.parser.plugins.push('fileUpload');
})(jQuery)


//上传文件
//1.单文件：
//2.多文件：
//3.文件列表
//4.上传进度条





