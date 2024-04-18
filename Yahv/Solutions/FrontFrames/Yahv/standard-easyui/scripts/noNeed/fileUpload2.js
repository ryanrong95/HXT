//文件上传第二版
(function ($) {
    //验证文件大小
    function validSize(file, maxLength) {
        var msg = null;
        if (file.size > maxLength) {
            msg = "超过" + (maxLength / 1024) + "kb"
        }
        return msg;
    }
    //判断文件类型
    function validExtension(file, extensions) {
        var msg = null;
        var arry = extensions.split('|');
        extension = file.name.substring(file.name.lastIndexOf('.') + 1).toLowerCase();
        if (arry.filter(function (item) { return extension == item; }).length == 0) {
            msg = "文件类型不是" + extensions;
        }
        return msg;
    }
    //删除文件
    function removeFile(e, sender, $input) {
        $(e.target).parent().remove();
        var removetext = $(e.target).parent().find("em").text();
        var textArr = $(sender).filebox("getText").split(",");
        var textArr2 = [];
        count = 0;
        for (var i = 0; i < textArr.length; i++) {
            if (textArr[i] != removetext) {
                textArr2.push(textArr[i]);
            } else {
                count = i;
            }
        }
        var texts3 = textArr2.join(",")
        $(sender).filebox("setText", texts3);

        var files=$(sender).data("saveFiles");
        var files2 = [];
        for (var j = 0; j < files.length; j++) {
            if (removetext != files[j].name) {
                files2.push(files[j]);
            }
        }
        var formdata = new FormData();
        for (var j = 0; j < files2.length; j++) {
            formdata.append("name", files2[j]);
        }
        $input.val(formdata.getAll("name"));
        console.log(formdata.getAll("name"))
        $(sender).data("saveFiles", formdata.getAll("name"));
    }

    //添加文件到页面中
    function addFileLi($input, sender, files, $ul, options) {
        var flag = false;
        $ul.html(null);
        for (var i = 0; i < files.length; i++) {
            flag = false;
            var $span = $("<span></span>");
            var $close = $("<i class='iconfont icon-close fr'></i>");
            var $li = $("<li><i class='iconfont icon-wenjian'></i><em>" + files[i].name + "</em> (" + files[i].size / 1024 + "kb) </li>");
            $li.append($close);
            $li.append($span);
            $close.on("click", function (e) {
                removeFile(e, sender, $input);
            })
            var maxTip = null;
            var acceptTip = null;
            if (options.maxlength) {
                maxTip = validSize(files[i], options.maxlength);
            }
            if (options.accept) {
                acceptTip = validExtension(files[i], options.accept);
            }
            var textTip = null;
            if (maxTip && acceptTip) {
                textTip = maxTip + "," + acceptTip;
            } else if (maxTip && !acceptTip) {
                textTip = maxTip;
            } else if (acceptTip && !maxTip) {
                textTip = acceptTip;
            } else if (!maxTip && !acceptTip) {
                textTip = "";
                flag = true;
            }
            $span.text(textTip);
            $ul.append($li);
        }
        return flag;
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
        $(sender).data("isCanSubmit", false);//存储options的值
        $(sender).data("saveFiles", null);

        //为页面添加dom元素---开始
        var $ul = $("<ul class='fileUl'></ul>");
        $(sender).after($ul);
        var $progress = $("<div style='margin-top:5px;'></div>");
        $(sender).data("progress", $progress);
        $(sender).after($progress);
        $input = $("<input type='hidden' name='xx'/>");
        $(sender).after($input);
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
                maxLength: 5000
            })
            $progress.hide();
            $(sender).filebox({
                buttonText: options.buttonText != null ? options.buttonText : "选择文件",
                width: options.width != null ? options.width : 200,
                multiple: options.multiple != null ? options.multiple : true,
                accept: options.accept != null ? options.accept : 'png',
                maxlength: options.maxlength != null ? options.maxlength : 50000,
                onChange: function (newValue, oldValue) {
                    var files = $(sender).filebox("files");
                    console.log(files);
                    $(sender).data("saveFiles", files);
                    var formdata = new FormData();
                    for (var j = 0; j < files.length; j++) {
                        formdata.append("name", files[j]);
                    }
                    $input.val(formdata.getAll("name"));
                    $(sender).data("saveFiles", formdata.getAll("name"));
                    $(sender).data("isCanSubmit", addFileLi($input, sender, files, $ul, options));
                }
            })

        }
        init.call(this);
    }

    $.fn.fileUpload.defaults = {
        buttonText: "选择文件",
        width: 170,
        multiple: true,
        accept: 'png',
        maxlength: 50000
    };

    $.fn.fileUpload.methods = {
        options: function (jq) {
            return $(jq).data("options");//返回options
        },
        submit: function () {

        }
    };
    $.parser.plugins.push('fileUpload');
})(jQuery)


//上传文件
//1.单文件：
//2.多文件：
//3.文件列表
//4.上传进度条





