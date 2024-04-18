//文件上传第五版
(function ($) {
    //扩展方法from提交的时候验证
    $.extend($.fn.validatebox.defaults.rules, {
        limitNum: {
            validator: function (value) {
                var options = $(this).prev().data("options");
                var fileslength = $(this).next().contents().find('input[type="file"]')[0].files.length;
                var limitFlag = true;
                if (options.type == null && options.limit != null && (options.limit < fileslength)) {
                    limitFlag = false;
                }
                return limitFlag;
            },
            message: '文件数量超出'
        },
        fileFormat: {
            validator: function (value) {
                var options = $(this).prev().data("options");
                var files = $(this).next().contents().find('input[type="file"]')[0].files;
                var $ul = $(this).prev().data("ul");
                var isCanSubmit = addFileLi(files, $ul, options, false)
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
    //文件格式
    var fileFormat = {
        '3gpp': 'audio/3gpp, video/3gpp',	        //3GPP音频/视频
        'ac3': 'audio/ac3',					        //AC3音频
        'asf': 'allpication/vnd.ms-asf',		    //高级流格式
        'au': 'audio/basic',					    //AU音频
        'css': 'text/css',					        //css层叠样式表
        'csv': 'text/csv',					        //csv逗号分隔值
        'doc': 'application/msword',			    //MS Word文档
        'dot': 'application/msword',			    //MS Word模板
        'dtd': 'application/xml-dtd',		        //文件类型定义
        'dwg': 'image/vnd.dwg',				        //AutoCAD绘图数据库
        'dxf': 'image/vnd.dxf',				        //AutoCAD图形交换格式
        'gif': 'image/gif',					        //图形交换格式
        'htm': 'text/html',					        //超文本标记语言
        'html': 'text/html',					    //超文本标记语言
        'jp2': 'image/jp2',					        //JPEG-2000
        'jpe': 'image/jpeg',					    //JPEG
        'jpeg': 'image/jpeg',				        //JPEG
        'jpg': 'image/jpeg',					    //JPEG
        'js': 'text/javascript',				    //JavaScript
        'json': 'application/json',			        //json	
        'mp2': 'audio/mpeg, video/mpeg',		    //MPEG音频/视频流，第二层
        'mp3': 'audio/mpeg',					    //MPEG音频流，第三层
        'mp4': 'audio/mp4, video/mp4',		        //MPEG-4音频/视频
        'mpeg': 'video/mpeg',				        //MPEG视频流，第二层
        'mpg': 'video/mpeg',					    //MPEG视频流，第二层
        'mpp': 'application/vnd.ms-project',        //MS项目项目
        'ogg': 'application/ogg, audio/ogg',	    //Ogg Vorbis
        'pdf': 'application/pdf',			        //可移植文档格式
        'png': 'image/png',				            //便携式网络图形
        'pot': 'application/vnd.ms-powerpoint',     //MS PowerPoint模板
        'pps': 'application/vnd.ms-powerpoint',     //MS PowerPoint幻灯片
        'ppt': 'application/vnd.ms-powerpoint',     //MS PowerPoint演示文稿
        'rtf': 'application/rtf, text/rtf',         //富文本格式
        'svf': 'image/vnd.svf',				        //简单的矢量格式
        'tif': 'image/tiff',					    //标记图像格式文件
        'tiff': 'image/tiff',				        //标记图像格式文件
        'txt': 'text/plain',					    //纯文本
        'wdb': 'application/vnd.ms-works',          //MS Works数据库
        'wps': 'application/vnd.ms-works',          //Works文本文档
        'xhtml': 'application/xhtml+xml',	        //可扩展的超文本标记语言
        'xlc': 'application/vnd.ms-excel',	        //MS Excel图表
        'xlm': 'application/vnd.ms-excel',          //MS Excel宏
        'xls': 'application/vnd.ms-excel',	        //MS Excel电子表格
        'xlsx': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        'xlt': 'application/vnd.ms-excel',	        //MS Excel模板
        'xlw': 'application/vnd.ms-excel',	        //MS Excel工作区
        'xml': 'text/xml, application/xml',	        //可扩展标记语言
        'zip': 'aplication/zip'				        //压缩档案
    }

    //获取cookie
    function getToken(cookiename) {
        var strcookie = document.cookie;//获取cookie字符串
        var arrcookie = strcookie.split("; ");//分割
        for (var i = 0; i < arrcookie.length; i++) {
            var arr = arrcookie[i].split("=");
            if (arr[0] == cookiename) {
                return arr[1];
            }
        }
        return "";
    }

    //验证文件大小
    function validSize(file, maxLength) {
        var msg = null;
        if (file.size > maxLength) {
            msg = "超过" + (maxLength / 1024 / 1024) + "MB"
        } else if (file.size == 0) {
            msg = "文件为空，不可上传"
        }
        return msg;
    }

    //判断文件类型
    function validExtension(file, extensions) {
        var msg = null;
        var arry = extensions.split(',');
        extension = file.name.substring(file.name.lastIndexOf('.') + 1).toLowerCase();
        var filecorrectFormat = fileFormat[extension].split(',');
        var flag = false;
        for (var i = 0; i < arry.length; i++) {
            for (var j = 0; j < filecorrectFormat.length; j++) {
                if (arry[i] == filecorrectFormat[j]) {
                    flag = true;
                    break;
                }
            }
        }
        if (!flag) {
            msg = "文件类型不属于" + extensions;
        }
        return msg;
    }

    //添加文件到页面中
    function addFileLi(files, $ul, options, isAddDom) {
        var flag = false;
        if (isAddDom) {
            $ul.html(null);
        }
        for (var i = 0; i < files.length; i++) {
            flag = false;
            var $span, $li;
            if (isAddDom) {
                $span = $("<span></span>");
                $li = $("<li><a><i class='iconfont icon-wenjian'></i><em>" + files[i].name + "</em> (" + files[i].size / 1024 + "kb) </a></li>");
                $li.append($span);
            }
            var maxTip = null;      //文件大小提示
            var acceptTip = null;   //文件格式提示
            if (options.maxlength && options.required) {
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
            if (isAddDom) {
                $span.text(textTip);
                $ul.append($li);
            }
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

        if ($(sender).data("progress")) {
            return;
        }

        $(sender).data("options", options);//存储options的值

        var id1 = randomID('fileuploadbox');//sender的id值
        if (!$(sender).attr("id")) {
            $(sender).attr("id", id1);
        }
        var name = randomID('fileupload');//隐藏input的name
        var id2 = randomID(id1 + '_iframe');
        var name3 = randomID(id1 + '_file');

        //为页面添加dom元素---开始
        var $ul = $("<ul class='fileUl'></ul>");//展示上传的数据
        var mtarget = $(options.target);
        if (mtarget.length > 0) {
            mtarget.append($ul);
        } else {
            $(sender).after($ul);
        }


        var $progress = $("<div style='margin-top:5px;' id='p'></div>");//展示上传的进程

        if (mtarget.length > 0) {
            mtarget.append($progress);
        } else {
            $(sender).after($progress);
        }

        //$(sender).after($progress);

        $iframe = $("<iframe id='" + id2 + "' name='" + id2 + "' style='width:0;height:0;border:none;'></iframe>");//iframe页面文件上传
        $(sender).after($iframe);
        $(sender).data('iframe', $iframe);
        var $form = '<form action="" method="post" enctype="multipart/form-data"><input type="file" name="' + name3 + '" value="文件上传"/></form>';

        if (options.type == "img") {
            $(sender).attr("title", "点击更改图片")
            options.accept = 'image/gif,image/png,image/jpeg';
        }
        if (options.accept) {
            $form = '<form action="" method="post" enctype ="multipart/form-data"><input type="file" name="' + name3 + '" accept="' + options.accept + '" value="文件上传"/></form>';
        }
        $(sender).data('form', $form);
        $iframe.contents().find("body").append($form);

        $img = $("<img  class='filebox_img hide'/>")
        $(sender).before($img);
        $(sender).data('img', $img);

        $input = $("<input type='text' name=" + name + "  style='width:0;height:0;border:none;'/>");//隐藏的input存储值
        $(sender).after($input);

        //为页面添加dom元素---结束

        //存储数据开始
        $(sender).data("submitResult", false);//存储上传结果
        $(sender).data("saveFiles", null);//保存上传数据

        $(sender).data("ul", $ul);
        $(sender).data("progress", $progress);
        $(sender).data("input", $input);
        $(sender).data("iframe", $iframe);
        //存储数据结束

        var param = {
            url: $(sender).data("options").url || "/Tests/FileUpload.aspx",　　 //form提交数据的地址
            type: "post",　　　                 //form提交的方式(method:post/get)
            data: $(sender).data("options").data || {
                token: getToken("ydcx_Yahv.Erp"),
                mainID: 'test',
                type: 'test'
            },
            beforeSend: function () {
                $(sender).data("progress").show();
                $(sender).data("progress").progressbar('setValue', 0);
                //上传数据之前暴露事件onUnloadBefore
                if ($(sender).data("options").onUnloadBefore) {
                    $(sender).data("options").onUnloadBefore();
                }
            },
            uploadProgress: function (event, position, total, percentComplete) {
                var percentVal = percentComplete;
                $(sender).data("progress").progressbar('setValue', percentComplete);
            },
            success: function (data) {//提交成功后执行的回调函数
                if (data.length) {
                    $(sender).data("FilesUploadAfterPath", data);
                    $.timeouts.alert({
                        position: "TC",
                        msg: "上传成功",
                        type: "success",
                        timeout: 1000
                    })
                    if ($(sender).data("options").type == "img") {
                        $img.addClass("show");
                        $img.attr("src", data[0]);
                    }
                    $(sender).data("progress").progressbar('setValue', 100);
                    setTimeout(function () {
                        $(sender).data("progress").hide();
                    }, 1000)

                    $(sender).data("submitResult", true);

                    $(sender).data("ul").find('li').each(function (i, v) {
                        $(v).find('a').attr('href', data[i]);
                        $(v).find('a').attr('target', '_blank');
                    })

                    //上传成功暴露事件onUnloadSuccess
                    if ($(sender).data("options").onUnloadSuccess) {
                        $(sender).data("options").onUnloadSuccess(data);
                    }
                }
            },
            error: function (err) {                 //提交失败执行的函数
                $(sender).data("FilesUploadAfterPath", null);
                console.log(err);
                $.timeouts.alert({
                    position: "TC",
                    msg: "上传失败",
                    type: "error"
                })
                $(sender).data("submitResult", false);
            },
            dataType: null,　　　　　　　         //服务器返回数据类型
            timeout: 6000 　　　　　 　           //设置请求时间，超过该时间后，自动退出请求，单位(毫秒)。　　
        }

        //点击上传文件事件
        function clickFun() {
            //在已经设置图片的情况下
            if ($(sender).data('noneedvailed')) {
                $(sender).data("input").validatebox({
                    validType: ["limitNum", "fileFormat", "isSubmit['" + $(sender).attr("id") + "']"],
                    validateOnBlur: true
                })
            }
            if ($(sender).data("iframe").contents().find("input").length == 0) {
                $(sender).data('iframe').contents().find("body").append($(sender).data('form'));
            }
            //为input[type=file]的添加多选属性
            if ($(sender).data("options").multiple && $(sender).data("options").multiple == true && $(sender).data("options").type == null && (!$(sender).data("options").limit || $(sender).data("options").limit > 1)) {
                $(sender).data("iframe").contents().find("input").attr("multiple", "multiple");
            }
            //解绑change事件
            $(sender).data("iframe").contents().find("input").unbind("change");
            //绑定change事件
            $(sender).data("iframe").contents().find("input").bind("change", function (event) {
                var event = event || window.event;
                var target = event.target || event.srcElement; //获取document 对象的引用 
                var files = target.files;
                var length = target.files.length;
                var limitFlag = true;
                if ($(sender).data("options").limit) {
                    if (length > $(sender).data("options").limit) {
                        limitFlag = false;
                        $.timeouts.alert({
                            position: "TC",
                            msg: '文件超出' + $(sender).data("options").limit + '个，无法上传！',
                            type: "warning",
                        })
                    }
                }
                var files2 = [];
                var formData = new FormData();
                if (files) {
                    for (var i = 0; i < files.length; i++) {
                        files2.push(files[i]);
                    }
                }
                $(sender).data("saveFiles", files2);
                $(sender).data("input").val(files2);
                //在页面上展示input上传数据，并且返回格式是否正确
                var flag = addFileLi(files, $(sender).data("ul"), $(sender).data("options"), true);
                $(sender).data("input").blur();
                //如果文件有数据并且可以上传，则执行上传
                if (files.length && limitFlag && flag) {
                    $(sender).data("iframe").contents().find("form").ajaxSubmit(param);
                }
            })

            //触发iframe的input[type=file]的点击事件
            return $(sender).data("iframe").contents().find("input").click();
        }

        //初始化页面
        var init = function () {
            //初始化存储数据的input
            $(sender).data("input").validatebox({
                required: $(sender).data("options").required,
                missingMessage: '上传文件不能为空',
                validType: ["limitNum", "fileFormat", "isSubmit['" + $(sender).attr("id") + "']"],
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
            if ($(sender).data("options").type == null) {
                if ($(sender).text() == null || $(sender).text() == '') {
                    $(sender).text("文件上传");
                }
                $(sender).linkbutton({
                    iconCls: $(sender).data("options").iconCls,
                    onClick: function () {
                        clickFun();
                    }
                });
            } else if ($(sender).data("options").type == 'img') {
                $(sender).text();
                $(sender).addClass("iconfont icon-uploadimg filebox_bg");
                $(sender).click(function () {
                    clickFun();
                })
            }
        }
        init.call(this);
    }

    $.fn.fileUpload.defaults = {
        required: true,
        iconCls: null,
        url: null,
        type: null,
        multiple: true,
        accept: null,//文件类型
        saveType: null,//保存类型（PI,或者其他）
        data: {
            session: Math.random() + '' + Math.random(),
            token: getToken("ydcx_Yahv.Erp"),
            mainID: 'test',
            type: "test"
        },
        maxlength: 5 * 1024 * 1024,//文件大小
        limit: null,//限制上传个数
        target: ''
    };
    $.fn.fileUpload.methods = {
        //参数options
        options: function (jq) {
            return $(jq).data("options");
        },
        //文件上传数据
        files: function (jq) {
            return $(jq).data("saveFiles");
        },
        getFilesUploadAfterPath: function (jq) {
            var paths = null;
            if ($(jq).data("FilesUploadAfterPath")) {
                paths = $(jq).data("FilesUploadAfterPath")
            }
            return paths;
        },
        setFile: function (jq, param) {
            if ($(jq).data('options').type == 'img') {
                $($(jq).data('img')).attr('src', param.src);
                $($(jq).data('img')).addClass("show");
            }

            $span = $("<span></span>");
            $li = $("<li><a href='" + param.src + "' target='_blank'><i class='iconfont icon-wenjian'></i><em>" + param.name + "</em> </a></li>");
            $li.append($span);
            $(jq).data('ul').append($li);

            $(jq).data('noneedvailed', true);
            $(jq).data("FilesUploadAfterPath", [param]);
            $($(jq).data("input")).validatebox({
                required: false,
                validType: null
            })
        }
    };
    $.parser.plugins.push('fileUpload');
})(jQuery)
