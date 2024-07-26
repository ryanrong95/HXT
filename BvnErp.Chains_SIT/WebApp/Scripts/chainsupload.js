//
//控件初始化
//$('#BusinessLicense').chainsupload({
//    required: true,
//    multiple: false,
//    validType: ['fileSize[10,"MB"]'],
//    buttonText: '选择',
//    buttonAlign: 'right',
//    prompt: '请选择图片或PDF类型的文件',
//    accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
//});
//
//控件赋值
//此处只是将附件地址绑定页面<a>标签，可查看;
//重新选择文件相当于编辑保存，否则等效于附件无更改(后台需验证file.ContentLength != 0);
//$('#BusinessLicense').chainsupload("setValue", ClientInfoData.File);
//File:{Name:"a.txt",URL:"http://bib.com/a/a.txt"}
//
//

; (function () {
    //定义Chainsupload的构造函数
    var Chainsupload = function (ele, opt) {
        this.$element = ele,
            this.fid = ele.attr('id'),
            this.hasMD5 = [],
            this.fileurl = "",
            this.fileList = [],
            this.defaults = {
                required: true,
                multiple: false,
                validType: ['fileSize[1,"MB"]'],
                buttonText: '选择',
                buttonAlign: 'right',
                prompt: '请选择图片或PDF类型的文件',
                accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
            },
            this.options = $.extend({}, this.defaults, opt)
    }

    function generateDomForUpload() {
        var fid = this.$element.attr('id');
        var width = this.$element.css('width');
        this.$element.attr('display', 'none');

        if ($('#chainsuploadcss_' + fid).length < 1) {
            var $wrap = $('<span id="chainsuploadcss_' + fid + '" class="chainsuploadcss"></span>');

            var $spanBL = $('<span id="spanBL_' + fid + '"></span>');
            var $fileinput = $('<input class="easyui-filebox" style="width: ' + width + '" />');
            //$fileinput.attr('id', fid);
            $fileinput.attr('name', fid);
            $spanBL.append($fileinput);

            var $spanBLUrl = $('<span id="spanBLUrl_' + fid + '"></span>');
            var $showA = $('<a id="urlBL_' + fid + '" href="javascript:void(0);"></a>');
            $showA.attr('style', "color: #0081d5; cursor: pointer; margin: 0 8px; font: 12px/1.2 Arial,Verdana,'微软雅黑','宋体';");
            $spanBLUrl.append($showA);

            var $spanBtn = $('<span id="spanBtn_' + fid + '"></span>');
            $spanBtn.append($('<a class="easyui-linkbutton" style="margin: 0 5px;" id="uploadfile_' + fid + '">上传</a>'));
            $spanBtn.append($('<a class="easyui-linkbutton" style="margin: 0 5px;" id="showfile_' + fid + '">查看</a>'));
            $wrap.append($spanBL).append($spanBLUrl).append($spanBtn);

            this.$element.parent().append($wrap);

            //渲染按钮
            $.parser.parse($spanBtn);
        }
    }

    function setValue($this, file) {
        var fid = $this.$element.attr('id');

        //组装查看附件dom
        if ($('#restartDialog').length < 1) {
            var $div = $('<div id="restartDialog" class="easyui-window" title="查看附件" style="width: 70%; height: 600px;"></div>');
            $div.attr('data-options', "iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true,fitColumns:true");
            $div.append('<img id="fsidImg" style="width: 100%; height: 100%" />');
            $div.append('<iframe id="fsidPdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>');
            $('body form').append($('<div id="showFileWin"></div>').append($div));
            $.parser.parse($('#showFileWin'));
        }

        //点击上传按钮
        $("#uploadfile_" + fid).on("click", function () {
            $('#spanBL_' + fid + ' label').click();
        });

        //点击查看按钮
        $("#showfile_" + fid).on("click", function () {
            var url = file.Url.toLowerCase();
            if (url.toLowerCase().indexOf('pdf') > 0) {
                $('#fsidPdf').attr('src', file.Url);
                $('#fsidImg').css("display", "none");
                $("#restartDialog").window('open').window('center');
            }
            else if (url.toLowerCase().indexOf('doc') > 0 || url.toLowerCase().indexOf('docx') > 0) {
                //如果是word文档，则下载查看
                let a = document.createElement('a');
                a.href = file.Url;
                a.download = "";
                a.click();
            }
            else {
                $('#fsidImg').attr('src', file.Url);
                $('#fsidPdf').css("display", "none");
                $("#restartDialog").window('open').window('center');
            }

        });

        $('#spanBL_' + fid).css("display", "none");
        $('#spanBLUrl_' + fid).css("display", "unset");
        $('#showfile_' + fid).css("display", "inline-block");
        $('#uploadfile_' + fid).css("display", "inline-block");

        $("input[textboxname=" + fid + "]").filebox({
            required: false,
            //prompt: file.Name
        });
        $('#urlBL_' + fid).append(file.Name);
    };

    //定义Beautifier的方法
    Chainsupload.prototype = {
        build: function () {
            var $this = this;
            generateDomForUpload.call(this);
            //$("body").append(this.$dialogFile);

            $("input[name=" + $this.fid + "]").filebox({
                required: this.options.required,
                multiple: false,
                validType: this.options.validType,
                buttonText: this.options.buttonText,
                buttonAlign: this.options.buttonAlign,
                prompt: this.options.prompt,
                accept: this.options.accept,
                onChange: function (newValue, oldValue) {
                    $('#spanBL_' + $this.fid).css("display", "unset");
                    $('#spanBLUrl_' + $this.fid).css("display", "none");
                    $("#uploadfile_" + $this.fid).css("display", "none");
                    $("#showfile_" + $this.fid).css("display", "none");

                    //验证文件类型
                    var type = $this.options.accept.join();
                    type = type.replace(new RegExp("image/", "g"), "").replace(new RegExp("application/", "g"), "")
                    type = type.replace('msword', 'doc').replace('vnd.openxmlformats-officedocument.wordprocessingml.document', 'docx');
                    var point = newValue.lastIndexOf(".");
                    var houzhui = newValue.substr(point + 1);
                    if (type.indexOf(houzhui.toLowerCase()) < 0) {
                        $("input[textboxname=" + $this.fid + "]").filebox('setValue', '');
                        $("input[textboxname=" + $this.fid + "]").filebox({
                            required: $this.options.required,
                        });
                        $.messager.alert('消息', "请选择" + type + "格式的文件！", function () {
                            return false;
                        });
                        return false;
                    }
                    //回调自定义方法
                    if ($this.options.onChange != undefined) {
                        $this.options.onChange.call(newValue, oldValue);
                    }
                }
            });

            $('#spanBLUrl_' + $this.fid).css("display", "none");
            $('#spanBL_' + $this.fid).css("display", "unset");
            $('#showfile_' + $this.fid).css("display", "none");
            $('#uploadfile_' + $this.fid).css("display", "none");
        },
        setValue: function (file) {
            var $this = this;
            if (file != null) {
                setValue($this, file);
            }
        },
        resetOption: function () {
            var $this = this;
            $("input[textboxname=" + $this.fid + "]").filebox({
                required: this.options.required,
                multiple: false,
                validType: this.options.validType,
                buttonText: this.options.buttonText,
                buttonAlign: this.options.buttonAlign,
                prompt: this.options.prompt,
                accept: this.options.accept,
                onChange: function (newValue, oldValue) {
                    $('#spanBL_' + $this.fid).css("display", "unset");
                    $('#spanBLUrl_' + $this.fid).css("display", "none");
                    $("#uploadfile_" + $this.fid).css("display", "none");
                    $("#showfile_" + $this.fid).css("display", "none");

                    //验证文件类型
                    var type = $this.options.accept.join();
                    type = type.replace(new RegExp("image/", "g"), "").replace(new RegExp("application/", "g"), "")
                    var point = newValue.lastIndexOf(".");
                    var houzhui = newValue.substr(point + 1);
                    if (type.indexOf(houzhui) < 0) {
                        $("input[textboxname=" + $this.fid + "]").filebox('setValue', '');
                        $.messager.alert('消息', "请选择" + type + "格式的文件！", function () {
                            return false;
                        });
                        return false;
                    }
                    //回调自定义方法
                    if ($this.options.onChange != undefined) {
                        $this.options.onChange.call(newValue, oldValue);
                    }
                }
            });
        },
        resetRequired: function (isRequired) {
            var $this = this;
            $this.options.required = isRequired;
            $("input[textboxname=" + $this.fid + "]").filebox('textbox').validatebox('options').required = isRequired;
        }
    }

    //在插件中使用chainsupload对象
    $.fn.chainsupload = function (options, param) {
        this.each(function () {
            var $this = $(this);
            if (typeof options == "string") {
                var chainsupload = $this.data("chainsupload");

                if (!chainsupload) {
                    //如果实体不存在
                    throw "swtextInst isn't initialized";
                } else if (!chainsupload[options]) {
                    throw options + " does not support";
                } else if (typeof chainsupload[options] != "function") {
                    throw options + " is not a function";
                }
                //调用方法
                chainsupload[options].call(chainsupload, param);
            } else {
                //是否已经初始化
                if ($this.next().find('input[class^=easyui-filebox]').length > 0) {
                    //初始化过了，重新option
                    var chainsupload = new Chainsupload($this, options);
                    chainsupload.resetOption();
                } else {
                    //初始化控件
                    var chainsupload = new Chainsupload($this, options);
                    chainsupload.build();

                    //将控件实例放入H5属性中
                    //$this.parent(".text-box").data("textBox", chainsupload);
                    $this.data("chainsupload", chainsupload);
                }
            }
        });
        return this;
    }
})();