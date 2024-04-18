//文件上传第三版
(function ($) {
    //扩展方法from提交的时候验证输入值是否为下拉列表的值
    $.extend($.fn.validatebox.defaults.rules, {
        fileFormat: {
            validator: function (value, param) {
                var flag = $("#" + param).attr("flag");
                if (flag == "false") {
                    flag = false;
                } else if (flag == "true") {
                    flag = true;
                }
                return flag;
            },
            message: '文件格式错误'
        },
        isSubmit: {
            validator: function (value, param) {
                var submitResult = $("#" + param).data('submitResult');
                return submitResult;
            },
            message: '文件上传失败'
        }
    });
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

        var files = $(sender).data("saveFiles");
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
            //var $close = $("<i class='iconfont icon-close fr'></i>");
            var $li = $("<li><i class='iconfont icon-wenjian'></i><em>" + files[i].name + "</em> (" + files[i].size / 1024 + "kb) </li>");
            //$li.append($close);
            $li.append($span);
            //$close.on("click", function (e) {
            //    removeFile(e, sender, $input);
            //})
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

    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
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

        $(sender).data("options", options);//存储options的值
        $(sender).data("submitResult", false);//存储上传结果
        $(sender).data("saveFiles", null);//保存上传数据

        var id1 = randomID('fileuploadbox');
        if (!$(sender).attr("id")) {
            $(sender).attr("id", id1);
        }
        var id2 = randomID('fileupload');
        //为页面添加dom元素---开始
        var $ul = $("<ul class='fileUl'></ul>");
        $(sender).after($ul);
        var $progress = $("<div style='margin-top:5px;' id='p'></div>");
        $(sender).data("progress", $progress);
        $(sender).after($progress);
        $iframe = $("<iframe src='fileupload2.html' style='width:0;height:0;border:none;'></iframe>")
        $input = $("<input type='text' id=" + id2 + " name=" + id2 + "  style='width:0;height:0;border:none;'/>");
        $input.attr("flag", false)
        $(sender).after($input);
        $(sender).after($iframe);
        //为页面添加dom元素---结束

        var object = {
            url: options.url != null ? options.url : '/',　　　　　　//form提交数据的地址
            type: "post",　　　  //form提交的方式(method:post/get)
            beforeSerialize: function () { }, //序列化提交数据之前的回调函数
            beforeSubmit: function () { },　　//提交前执行的回调函数
            success: function () {//提交成功后执行的回调函数
                $.timeouts.alert({
                    position: "TC",
                    msg: "上传成功",
                    type: "success",
                })
                $(sender).data("submitResult", true);
            },
            error: function () {//提交失败执行的函数
                $.timeouts.alert({
                    position: "TC",
                    msg: "上传失败",
                    type: "error",
                })
                $(sender).data("submitResult", false);
            },
            dataType: null,　　　　　　　//服务器返回数据类型
            clearForm: true,　　　　　　 //提交成功后是否清空表单中的字段值
            restForm: true,　　　　　　  //提交成功后是否重置表单中的字段值，即恢复到页面加载时的状态
            timeout: 6000 　　　　　 　  //设置请求时间，超过该时间后，自动退出请求，单位(毫秒)。　　
        }

        //进度条增长
        function start() {
            $($progress).show();
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
            //初始化存储数据的input
            $input.validatebox({
                required: true,
                missingMessage: '上传文件不能为空',
                validType: ["fileFormat['" + $input.attr("id") + "']", "isSubmit['" + $(sender).attr("id") + "']"],
            })
            //初始化进度条
            $progress.progressbar({
                value: 0,
                width: 200,
                height: "14px"
            })
            $progress.hide();
            $(sender).linkbutton({
                onClick: function () {
                    //为input[type=file]的添加多选属性
                    if (options.multiple && options.multiple == true) {
                        $iframe.contents().find("input").attr("multiple", "multiple");
                    }
                    //解绑change事件
                    $iframe.contents().find("input").unbind("change");
                    //绑定change事件
                    $iframe.contents().find("input").bind("change", function (event) {
                        var files = event.target.files;
                        console.log(files);
                        var files2 = [];
                        if (files) {
                            for (var i = 0; i < files.length; i++) {
                                files2.push(files[i]);
                            }
                        }
                        $(sender).data("saveFiles", files2);
                        $input.val(files2);
                        console.log($input.val());
                        var flag = addFileLi($input, sender, files, $ul, options);
                        $input.attr('flag', flag);
                        console.log($input.attr('flag'));
                        if (files.length && flag) {
                            $iframe.contents().find("form").ajaxSubmit(object);
                            start();
                        }
                    })
                    return $iframe.contents().find("input").click();
                }
            });
        }
        init.call(this);
    }

    $.fn.fileUpload.defaults = {
        multiple: true,
        accept: 'png',
        url: '/Tests/FileUpload.aspx',
        maxlength: 500000
    };

    $.fn.fileUpload.methods = {
        options: function (jq) {
            return $(jq).data("options");//返回options
        }
    };
    $.parser.plugins.push('fileUpload');
})(jQuery)

