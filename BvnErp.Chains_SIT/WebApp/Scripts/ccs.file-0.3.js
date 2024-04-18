// -----------------------------------------------------------------------
// TODO：待补充说明
// -----------------------------------------------------------------------
; (function ($) {
    //定义构造函数
    function CcsFile($element, options) {
        this.$element = $element,
        this.defaults = {
            icon: '../App_Themes/xp/images/wenjian.png',
            data: [],
            required: false,
            multiple: false,
            validType: ['fileSize[500,"KB"]'],
            accept: ['image/jpg', 'image/bmp', 'image/jpeg', 'image/gif', 'image/png', 'application/pdf'],
            uploadprompt: '仅限图片或pdf格式的文件,并且不超过500kb\n',
            exportprompt: '导出pdf格式的文件，交给客户盖章后上传'
        },
        this.options = $.extend({}, this.defaults, options)
    }

    //添加方法
    CcsFile.prototype = {
        init: function () {
            var $this = this;
            generateDom($this);
            initViewFile($this);
            initUploadFile($this);

            $('a[name="ccsFileRemove"]').on('click', function () {
                var $this = $(this);
                $.messager.confirm('确认', '请您再次确认是否删除所选文件！', function (success) {
                    if (success) {
                        $this.parents('.ccsFileContent').remove();
                    }
                });
            });
        },
        getFiles: function () {
            var files = [];
            $.each($('.ccsFileContent'), function (index, val) {
                files.push({ id: $(val).data('id'), name: $(val).data('name') });
            });
            return files;
        }
    }

    //在插件中使用ccsfile对象
    $.fn.ccsfile = function (options, param) {
        var $this = this;
        var ccsfile = $this.data("ccsfile");

        if (typeof (options) == 'string') {
            //验证调用是否合法
            if (!ccsfile) {
                throw "ccsfile instance isn't initialized";
            } else if (!ccsfile[options]) {
                throw options + " does not support";
            } else if (typeof ccsfile[options] != "function") {
                throw options + " is not a function";
            }

            return ccsfile[options].call(ccsfile, param);
        };

        if (!ccsfile) {
            ccsfile = new CcsFile($this, options);
            ccsfile.init();

            $this.data("ccsfile", ccsfile);
        }
        return $this;
    }

    function generateDom($this) {
        var options = $this.options;
        var data = options.data;
        var $element = $this.$element;
        var fid = $element.attr('id');

        var $html = '<div style="margin:10px"><div>';
        if (data.length > 0) {
            //文件信息
            for (var index = 0; index < data.length; index++) {
                $html += appendFile($this, data[index]);
            }
        }

        //导出、上传
        $html += '<div style="margin-top: 10px;">' +
                    '<span style="margin-left: 5px;">' +
                        '<a href="#" id="' + fid + '_exportfile" class="easyui-linkbutton" data-options="iconCls:\'icon-ok\'">导出</a>' +
                    '</span>' +
                    '<span style="margin-left: 10px">' +
                        '<input id="' + fid + '_uploadfile" name="' + fid + '_uploadfile" class="easyui-filebox" style="width: 54px; height: 26px" data-options="buttonText: \'上传\',buttonIcon:\'icon-add\'"/>' +
                    '</span>' +
                '</div>';
        //提示信息
        $html += '<div style="margin-top: 10px">' +
                    '<p style="margin: 5px">' + options.exportprompt + '</p>' +
                    '<p style="margin: 5px">' + options.uploadprompt + '</p>' +
                '</div>' +
            '</div>';

        //添加内容
        $element.append($html);
        //样式渲染
        $.parser.parse($element);
    }

    //查看文件
    function viewFile() {
    }

    //移除文件
    function removeFile() {

    }

    //添加文件
    function appendFile($this, file) {
        var options = $this.options;

        var $html = '<div class="ccsFileContent" data-id="' + file.ID + '" data-name="' + file.Name + '" data-fileformat="' + file.FileFormat + '" data-virtualpath="' + file.VirtualPath + '" data-url="' + file.Url + '">' +
                        '<table>' +
                            '<tr>' +
                                '<td rowspan="2">' +
                                    '<img style="margin-left: 5px" src="' + options.icon + '" /></td>' +
                                '<td><span style="margin-left: 5px;">' + file.Name + '</span></td>' +
                            '</tr>' +
                            '<tr>' +
                                '<td>' +
                                    '<a name="ccsFileView" href="#"><span style="color: cornflowerblue; margin-left: 5px;">预览</span></a>' +
                                    '<a name="ccsFileRemove" href="#"><span style="color: cornflowerblue; margin-left: 10px;">删除</span></a>' +
                                '</td>' +
                            '</tr>' +
                        '</table>' +
                    '</div>';
        return $html;
    }

    function initViewFile($this) {
        var fid = $this.$element.attr('id');
        var data = $this.options.data;

        //组装查看附件dom
        if ($('#restartDialog').length < 1) {
            var $div = $('<div id="restartDialog" class="easyui-window" title="查看附件" style="width: 1000px; height: 600px;"></div>');
            $div.attr('data-options', "iconCls:'pag-list',modal:true,collapsible:false,minimizable:false,maximizable:true,resizable:true,closed:true,fitColumns:true");
            $div.append('<img id="fsidImg" style="width: 100%; height: 100%" />');
            $div.append('<iframe id="fsidPdf" src="" width="100%" height="99%" frameborder="0" scroll="no"></iframe>');
            $('body').append($('<div id="showFileWin"></div>').append($div));
            $.parser.parse($('#showFileWin'));
        }

        //点击查看按钮
        $('#' + fid + '_viewfile').on("click", function () {
            if (data[0].Url.toLowerCase().indexOf('pdf') > 0) {
                $('#fsidPdf').attr('src', data[0].Url);
                $('#fsidImg').css("display", "none");
                $("#restartDialog").window('open').window('center');
            }
            else if (data[0].Url.toLowerCase().indexOf('doc') > 0 || data[0].Url.toLowerCase().indexOf('docx') > 0) {
                //如果是word文档，则下载查看
                let a = document.createElement('a');
                a.href = data[0].Url;
                a.download = "";
                a.click();
            }
            else {
                $('#fsidImg').attr('src', data[0].Url);
                $('#fsidPdf').css("display", "none");
                $("#restartDialog").window('open').window('center');
            }
        });
    }

    function initUploadFile($this) {
        var options = $this.options;
        var fid = $this.$element.attr('id');

        $('input[textboxname="' + fid + '_uploadfile"]').filebox({
            required: options.required,
            multiple: options.multiple,
            validType: options.validType,
            accept: options.accept,
            onChange: function (newValue, oldValue) {
                //验证文件大小
                if ($(this).next().attr("class").indexOf("textbox-invalid") > 0) {
                    $.messager.alert('提示', '文件大小不能超过500kb！');
                    return;
                }

                //验证文件类型
                var type = options.accept.join();
                type = type.replace(new RegExp("image/", "g"), "").replace(new RegExp("application/", "g"), "")
                type = type.replace('msword', 'doc').replace('vnd.openxmlformats-officedocument.wordprocessingml.document', 'docx');
                var point = newValue.lastIndexOf(".");
                var ext = newValue.substr(point + 1);
                if (type.indexOf(ext.toLowerCase()) < 0) {
                    $.messager.alert('消息', "请选择" + type + "格式的文件！");
                    return;
                }
            }
        });
    }
})(jQuery);