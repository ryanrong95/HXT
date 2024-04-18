/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

/*
form提交 用id(不填写随机数)+'ForJson'
*/

//文件上传第五版
//上传文件陈翰修改版
(function ($) {
    //获取当前
    var url = document.scripts[document.scripts.length - 1].src;
    //加载：jqueryform.js
    if (typeof ($.fn.ajaxSubmit) == 'undefined' || !$.fn.ajaxSubmit) {
        var lower = url.toLowerCase();
        var prexUrl = url.substring(0, lower.indexOf('/yahv/') + '/yahv/'.length);
        var script = '<script src="' + prexUrl + 'jquery-easyui-extension/jqueryform.js"></script' + '>';
        document.write(script);
    }


    //扩展方法from提交的时候验证
    $.extend($.fn.validatebox.defaults.rules, {
        IsUploaded: {
            validator: function (value, param) {

                var sender_id = param[0];
                //var file = $('#' + sender_id + '_iframe')[0].window.document.getElementById(sender_id + '_file');
                var file = $.fileUploaders[sender_id];
                var fileslength = file[0].files.length;

                //var fileslength = $('#' + sender_id + '_iframe').contents().find('input[type="file"]')[0].files.length;
                return fileslength > 0;
            },
            message: '必须上传文件'
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
        'docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',			    //MS Word文档
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
        'bmp': 'image/bmp',					        //bmp
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
        'xml': 'application/xml,text/xml',	        //可扩展标记语言
        'zip': 'aplication/zip'				        //压缩档案
    }

    //验证文件大小
    function validSize(file, maxLength) {
        var msg = null;
        if (file.size > maxLength) {
            msg = '文件：' + file.name + ',' + "超过" + (maxLength / 1024 / 1024) + "MB"
        } else if (file.size == 0) {
            msg = '文件：' + file.name + ',' + "文件为空，不可上传"
        }
        return msg;
    }

    //判断文件类型
    function validExtension(file, extensions) {
        extension = file.name.substring(file.name.lastIndexOf('.') + 1).toLowerCase();

        if (!fileFormat[extension]) {
            return "文件类型不属于" + extensions;
        }
        var currentFormat = fileFormat[extension].split(',');
        var flag = false;
        for (var i = 0; i < extensions.length; i++) {
            for (var j = 0; j < currentFormat.length; j++) {

                console.log([extensions[i], currentFormat[j]]);
                if (extensions[i] == currentFormat[j]
                    || currentFormat[j] == fileFormat[extensions[i].replace('.', '')]) {
                    flag = true;
                    break;
                }
            }
        }
        if (!flag) {
            return "文件类型不属于" + extensions;
        }
        return null;
    }

    $.fn.fileUploader = function (opt, param) {
        var sender = this;
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.fileUploader.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.fileUploader.defaults, opt);

        return this.each(function () {
            var sender = $(this);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'file_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }

            //创建进度条
            var progressbar;
            if (options.progressbarTarget) {
                $(options.progressbarTarget).append('<div id="' + sender_id + '_progressbar"  data-options="value:0" style="width:100%; height:14px;"></div>');
                progressbar = $('#' + sender_id + '_progressbar');
                progressbar.progressbar({ value: 0 });
                progressbar.hide();
            }


            //创建返回值
            var formJson = $('<input type="hidden" name="' + sender_id + 'ForJson">');
            $('form').append(formJson);

            //创建ajaxSubmit iframe
            var iframe = $("<iframe id='" + sender_id + "_iframe' name='" + sender_id + "_iframe' style='width:0;height:0;border:none;'></iframe>");//iframe页面文件上传
            $(document.body).append(iframe);


            var accepts = $.map(options.accept, function (item) {
                var value = fileFormat[item] || item;
                //if (!value) {
                //    alert('没有提供指定的类型：' + item);
                //}
                return value;
            });

            //alert(options.accept.join(','));

            var file = $('<input type="file" id="' + sender_id + '_file" name="' + sender_id + '_file" ' + (options.multiple ? 'multiple="multiple"' : '') + ' required="' + options.required + '"' + ' accept="' + accepts + '" value="文件上传"/>');

            if (!$.fileUploaders) {
                $.fileUploaders = {};
            }
            $.fileUploaders[sender_id] = file;


            var iframeform = $('<form action="" method="post" enctype="multipart/form-data"></form>');
            iframeform.append(file);
            $('#' + sender_id + '_iframe')[0].innerHTML = $('<div>').append(iframeform).html();
            //alert($('#' + sender_id + '_iframe')[0].innerHTML);
            //iframe.contents().find('body').append('asdfasdfasdf');
            //iframe.contents().find('body').append(iframeform);
            //alert(iframe.contents().first().html());

            sender.linkbutton({
                onClick: function () {
                    //alert(iframe.contents().find('body').find('#' + sender_id + '_file').length);
                    //.click();
                    file.click();
                }
            });

            var ajaxData = $.extend(true, {}, options.beforeData);
            //设定上传类型
            if (options.type) {
                ajaxData.type = options.type;
                //alert(options.type);
            }

            file.change(function () {

                var massages = [];
                var files = file.prop('files');
                //浏览器验证,按照文档上来说：目前的都支持09年以后
                if (typeof (files) == 'undefined' || !files) {
                    alert('浏览器不支持文档大小验证');
                }

                if (options.limit && files.length > options.limit) {
                    $.messager.alert('Warning', '问价个数不能超过：' + options.limit);
                    return;
                }

                if (files.length > 0) {
                    for (var index = 0; index < files.length; index++) {
                        var item = files[index];
                        var msg = validSize(item, options.maxlength);
                        if (msg) {
                            massages.push(msg);
                        }
                        if (options.accept && options.accept.length > 0) {

                            msg = validExtension(item, options.accept);
                            if (msg) {
                                massages.push(msg);
                            }
                        }
                    }
                }
                else {
                    return;
                }

                if (massages.length > 0) {
                    $.messager.alert('Warning', massages.join('<br/>'));
                    return;
                }



                //ajax提交
                iframeform.ajaxSubmit({
                    url: options.url,
                    type: "post",
                    data: ajaxData,
                    beforeSend: function () {
                        if (progressbar) {
                            progressbar.progressbar('setValue', 0);
                            progressbar.show();
                        }
                    },
                    uploadProgress: function (event, position, total, percentComplete) {
                        var percentVal = percentComplete;
                        progressbar.progressbar('setValue', percentComplete);
                    },
                    success: function (data) {//提交成功后执行的回调函数
                        if (data.length > 0) {
                            if (progressbar) {
                                progressbar.progressbar('setValue', 100);
                                setTimeout(function () {
                                    progressbar.hide();
                                }, 1000)
                            }

                            $.timeouts.alert({
                                position: "TC",
                                msg: "上传成功",
                                type: "success",
                                timeout: 1000
                            });
                            if (options.successTarget) {
                                var msgr = $(options.successTarget);

                                var ul = $("<ul></ul>");
                                for (var index = 0; index < data.length; index++) {
                                    var item = data[index];
                                    var li = $("<li><a href='" + item.CallUrl + "' target='_blank'><i class='iconfont icon-wenjian'></i><em>" + item.FileName + "</em> </a></li>");
                                    //王辉建议提出：删除

                                    ul.append(li);
                                }
                                msgr.html(ul);
                            }

                            //王辉提出建议：每次上传的数据需要保留
                            //目前单会话这样的开发方式实属困难
                            //需要改变一种开发方式

                            //保存本地form
                            try {
                                formJson.val(JSON.stringify(data));
                            } catch (e) {
                                alert('浏览器不支持：' + JSON.stringify);
                            }

                            if (options.success) {
                                options.success(data);
                            }
                        }
                    },
                    error: function (data, XMLHttpRequest, textStatus, errorThrown) {                 //提交失败执行的函数
                        alert(JSON.stringify([data, XMLHttpRequest, textStatus, errorThrown]));

                        $.timeouts.alert({
                            position: "TC",
                            msg: "上传失败",
                            type: "error"
                        })
                    },
                    dataType: null,　　　　　　　         //服务器返回数据类型
                    timeout: 6000 　　　　　 　           //设置请求时间，超过该时间后，自动退出请求，单位(毫秒)。　　
                });
            });

            if (options.required) {
                //增加验证
                var validatebox = $('<input style="height:0px;width:0px;border:none;" value="1" />');
                sender.after(validatebox);
                validatebox.validatebox({
                    required: options.required,
                    //missingMessage: '上传文件不能为空',
                    validType: 'IsUploaded["' + sender_id + '"]'
                });
            }
        });


    }

    $.fn.fileUploader.defaults = {
        required: true,
        url: '?uploader_g={9674920C-0544-45E7-A428-B8015719C9DB}',
        type: '', //类型，用于保存的首地址
        multiple: true,
        accept: [],//文件类型 ,固定只接受数组
        saveType: null,//保存类型（PI,或者其他）
        maxlength: 5 * 1024 * 1024,//文件大小
        limit: 5,//限制上传个数
        progressbarTarget: '', //进度条显示目标
        successTarget: '',//成功消息显示目标
        multiple: false,//多上穿
        beforeData: function () {//上传前可以获取动态参数
            return {};
        },
        success: function (data) {

        }
    };
    $.fn.fileUploader.methods = {
        //保留
        test: function (jq) {
            var sender = $(jq);
            returnsender;
        }
    };
    $.parser.plugins.push('fileUploader');
})(jQuery)
