//文件上传第四版
(function ($) {
    //扩展方法from提交的时候验证
    $.extend($.fn.validatebox.defaults.rules, {
        fileFormat: {
            validator: function (value, param) {
                var isCanSubmit = $("#" + param).data('isCanSubmit');
                return isCanSubmit;
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

    //添加文件到页面中
    function addFileLi($input, sender, files, $ul, options) {
        var flag = false;
        $ul.html(null);
        for (var i = 0; i < files.length; i++) {
            flag = false;
            var $span = $("<span></span>");
            var $li = $("<li><i class='iconfont icon-wenjian'></i><em>" + files[i].name + "</em> (" + files[i].size / 1024 + "kb) </li>");
            $li.append($span);
            var maxTip = null;      //文件大小提示
            var acceptTip = null;   //文件格式提示
            if (options.maxlength) {
                maxTip = validSize(files[i], options.maxlength);
            }
            if (options.accept) {
                acceptTip = validExtension(files[i], options.accept);
            }
            var textTip = null;//文件大小和格式综合提示
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
        return flag;//返回是否可以提交
    }

    //随机色生成id值
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
            options = $.extend(true, {}, $.fn.fileUpload.defaults, data_options, opt);
        } else {
            //如果html没有配置了data-options
            options = $.extend(true, {}, $.fn.fileUpload.defaults, opt);
        }

        var id1 = randomID('fileuploadbox');//sender的id值
        if (!$(sender).attr("id")) {
            $(sender).attr("id", id1);
        }
        var name = randomID('fileupload');//隐藏input的name
        var id2 = randomID(id1 + '_fileuploadbox');
        //为页面添加dom元素---开始
        var $ul = $("<ul class='fileUl'></ul>");//展示上传的数据
        $(sender).after($ul);

        var $progress = $("<div style='margin-top:5px;' id='p'></div>");//展示上传的进程
        $(sender).after($progress);

        $iframe = $("<iframe id='" + id2 + "' name='" + id2 + "' style='width:0;height:0;border:none;'></iframe>");//iframe页面文件上传
        $(sender).after($iframe);
        var $form = '<form action="" method="post"><input type="file" name="name" value="文件上传"/></form>';
        $iframe.contents().find("body").append($form);

        $input = $("<input type='text' name=" + name + "  style='width:0;height:0;border:none;'/>");//隐藏的input存储值
        $(sender).after($input);

        //为页面添加dom元素---结束

        //存储数据开始
        $(sender).data("options", options);//存储options的值
        $(sender).data("isCanSubmit", false);//存储是否可以上传
        $(sender).data("submitResult", false);//存储上传结果
        $(sender).data("saveFiles", null);//保存上传数据

        $(sender).data("ul", $ul);
        $(sender).data("progress", $progress);
        $(sender).data("input", $input);
        $(sender).data("iframe", $iframe);
        //存储数据结束

        var options2 = $(sender).data("options");
        //文件提交的参数
        var param = {
            url: options2.url != null ? options2.url : '/',　　　　//form提交数据的地址
            type: "post",　　　                                 //form提交的方式(method:post/get)
            beforeSerialize: function () { },                   //序列化提交数据之前的回调函数
            beforeSubmit: function () { },　　                  //提交前执行的回调函数
            success: function () {                              //提交成功后执行的回调函数
                $.timeouts.alert({
                    position: "TC",
                    msg: "上传成功",
                    type: "success",
                })
                $(sender).data("submitResult", true);
                $(sender).data("input").blur();
            },
            error: function () {                                 //提交失败执行的函数
                $.timeouts.alert({
                    position: "TC",
                    msg: "上传失败",
                    type: "error",
                })
                $(sender).data("submitResult", false);
                $(sender).data("input").blur();
            },
            dataType: null,　　　　　　　                         //服务器返回数据类型
            clearForm: true,　　　　　　                          //提交成功后是否清空表单中的字段值
            restForm: true,　　　　　　                           //提交成功后是否重置表单中的字段值，即恢复到页面加载时的状态
            timeout: 6000 　　　　　 　                           //设置请求时间，超过该时间后，自动退出请求，单位(毫秒)。　　
        }
        //上传进度条增长
        function start() {
            $(sender).data("progress").show();
            var value = $(sender).data("progress").progressbar('getValue');
            if (value < 100) {
                value += Math.floor(Math.random() * 10);
                $(sender).data("progress").progressbar('setValue', value);
                setTimeout(arguments.callee, 200);
            } else {
                $(sender).data("progress").hide();
                $(sender).data("progress").progressbar('setValue', 0);
            }
        };

        //初始化页面
        var init = function () {
            //初始化存储数据的input
            $(sender).data("input").validatebox({
                required: true,
                missingMessage: '上传文件不能为空',
                validType: ["fileFormat['" + $(sender).attr("id") + "']", "isSubmit['" + $(sender).attr("id") + "']"],
                validateOnBlur: true
            })
            //初始化进度条
            $(sender).data("progress").progressbar({
                value: 0,
                width: 200,
                height: "14px"
            })
            $(sender).data("progress").hide();
            //初始化sender
            $(sender).linkbutton({
                onClick: function () {
                    //为input[type=file]的添加多选属性
                    if (options2.multiple && options2.multiple == true) {
                        $iframe.contents().find("input").attr("multiple", "multiple");
                    }
                    //解绑change事件
                    $(sender).data("iframe").contents().find("input").unbind("change");
                    //绑定change事件
                    $(sender).data("iframe").contents().find("input").bind("change", function (event) {
                        var files = event.target.files;
                        var files2 = [];
                        if (files) {
                            for (var i = 0; i < files.length; i++) {
                                files2.push(files[i]);
                            }
                        }
                        $(sender).data("saveFiles", files2);
                        $(sender).data("input").val(files2);
                        //在页面上展示input上传数据，并且返回格式是否正确
                        var flag = addFileLi($(sender).data("input"), sender, files, $(sender).data("ul"), options2);
                        $(sender).data("isCanSubmit", flag);//存储文件格式是否正确即是否可以上传
                        $(sender).data("input").blur();
                        //如果文件有数据并且可以上传，则执行上传
                        if (files.length && flag) {
                            $(sender).data("iframe").contents().find("form").ajaxSubmit(param);
                            start();
                        }
                    })
                    //触发iframe的input[type=file]的点击事件
                    return $(sender).data("iframe").contents().find("input").click();
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
        //参数options
        options: function (jq) {
            return $(jq).data("options");
        },
        //文件上传数据
        files: function (jq) {
            return $(jq).data("saveFiles");
        }
    };
    $.parser.plugins.push('fileUpload');
})(jQuery)

