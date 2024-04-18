//动态添加tab
function AddTab(parent, title, href, id, height) {
    var content = "";
    if (href) {
        content = '<iframe id="' + id + '" src="' + href + '" style="width:100%;height:' + height + 'px;border:none"></iframe>';
    }
    else {
        content = '未实现';
    }
    $('#' + parent).tabs('add', {
        title: title,
        closable: false,
        content: content,
    });

    $('#' + parent).tabs('select', 0);
}

//刷新tab
function RefreshTab(parent, title, href, id, height) {
    if ($('#' + parent).tabs('exists', title)) {
        $('#' + parent).tabs('close', title);
    }
    AddTab(parent, title, href, id,height);
}

//为Easy-ui的只读输入框设置背景色
function SetReadonlyBgColor(className, color) {
    $('.' + className).each(function () {
        if ($(this).attr('readonly')) {
            if (className == 'easyui-textbox') {
                $(this).textbox('textbox').css('background', color);
            } else if (className == 'easyui-numberbox') {
                $(this).numberbox('textbox').css('background', color);
            } else if (className == 'easyui-combogrid') {
                $(this).combogrid('textbox').css('background', color);
            }
        }
    });
}

//设置只读界面
function InitClientPage() {
    if (window.parent.frames.Source != 'Add' && window.parent.frames.Source != 'Assign' && window.parent.frames.Source != 'Edit') {
        $('#btnSave').hide();
        $('#btnExport').hide();
        $('#btnAdd').hide();
        $('#btnDeclare').hide();
        $('#btnManifest').hide();
        $('#btnReBuild').hide();
        $('.ContainerAdd').hide();
        $('.ItemAdd').hide();
        $('.BatchRemove').hide();
        $('.Deletecontainer').hide();
        $('input[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
        $('textarea[class*=textbox-text]').attr('readonly', true).attr('disabled', true);
        $("a[id^='uploadfile_']").css("display", "none");
        return true;
    }
    return false;
}

//表单验证
function Valid(id) {
    var isValid = $('#' + id).form('enableValidation').form('validate');
    if (!isValid) {
        //$.messager.alert('提示', '请按提示输入数据！');
        return false;
    }
    else {
        return true;
    }
}

//取表单键值对
function FormValues(id) {
    var values = {};
    var $input = $("#" + id).find("input");

    $.each($input, function (index, val) {
        var $this = $(val);
        var id = $this.attr("id");
        if ($this.attr("class") != undefined) {
            //根据不同控件类型，获取value  
            if ($this.attr("class").indexOf("combobox") > 0) {
                values[id] = $('#' + id).combobox('getText');
                values[id + 'ID'] = $('#' + id).combobox('getValues').join(',');
            }
            else if ($this.attr("class").indexOf("datetimebox") > 0) {
                values[id] = $('#' + id).datetimebox('getValue');
            }
            else if ($this.attr("class").indexOf("datebox") > 0 && $this.attr("class").indexOf("validatebox") < 0) {
                values[id] = $('#' + id).datebox('getValue');
            }
            else if ($this.attr("class").indexOf("checkbox") > 0) {
                values[id] = $('#' + id).is(":checked");
            }
            else {
                values[id] = $.trim($this.val().replace(new RegExp("'", "g"), "#39;"));
            }
        }

        //取地址控件
        if ($this.attr("name") != undefined && $this.attr("name").indexOf("Address") >= 0) {
            values[$this.attr("name")] = $this.val();
        }

        if ($this.attr("name") != undefined && $this.attr("type").indexOf("checkbox") >= 0) {
            values[$this.attr("name")] = $this["0"].checked;
        }

    });
    return values;
}

//日期格式化-ryan
Date.prototype.format = function (format) {
    var date = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S+": this.getMilliseconds()
    };
    if (/(y+)/i.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    }
    for (var k in date) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? date[k] : ("00" + date[k]).substr(("" + date[k]).length));
        }
    }
    return format;
}



//扩展easyui表单的验证  
$.extend($.fn.validatebox.defaults.rules, {
    //验证汉子  
    CHS: {
        validator: function (value) {
            return /^[\u0391-\uFFE5]+$/.test(value);
        },
        message: '只能输入汉字'
    },
    NoCHS: {
        ///[\u4E00-\u9FA5]/g
        validator: function (value) {
            //if (/[\u4E00-\u9FA5]/.test(value)) {
            //    return false;
            //}
            if (value.length > 32) {
                return false;
            }
            return !new RegExp(/[\u4E00-\u9FA5]/).test(value);
        },
        message: '不能输入汉字且长度不超过32'
    },
    // 验证数字
    numbercheck: {
        validator: function (value) {
            return /^[0-9]+$/.test(value);
        },
        message: "只能输入数字"
    },
    //验证公司名称包括括号（  
    companyname: {
        validator: function (value) {
            return /^[\u0391-\uFFE5]+$/.test(value);
        },
        message: '请输入正确公司名称'
    },
    //移动手机号码验证  
    mobile: {//value值为文本框中的值  
        validator: function (value) {
            var reg = /^1[3|4|5|6|7|8|9]\d{9}$/;
            return reg.test(value);
        },
        message: '手机号码格式不正确'
    },
    //验证身份证号
    idnumber: {
        validator: function (value) {
            // 身份证号码为15位或者18位，15位时全为数字，18位前17位为数字，最后一位是校验位，可能为数字或字符X  
            var reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
            return reg.test(value);
        },
        message: '请输入正确的证件号码.'
    },
    //国内邮编验证  
    zipcode: {
        validator: function (value) {
            var reg = /^[1-9]\d{5}$/;
            return reg.test(value);
        },
        message: '邮编必须是非0开始的6位数字.'
    },
    bankaccount: {
        validator: function (value) {
            var reg = /^[-a-zA-Z0-9()]+$/;
            return reg.test(value);
            //return new RegExp("^[-a-zA-Z0-9()]+$").test(value)
        },
        message: '银行账号只能包含字母、数字和"-"'
    },
    //验证代理费率
    agencyrate: {
        validator: function (value) {
            var reg = /^(1|0(\.\d{1,4})?)$/;
            return reg.test(value);
        },
        message: '请输入正确代理费率.'
    },
    //4位小数
    exchangerate: {
        validator: function (value) {
            var reg = /^([1-9]\d{0,15}|0)(\.\d{1,4})?$/;
            return reg.test(value);
        },
        message: '汇率为四位小数'
    },
    supplierEname: {
        validator: function (value) {
            return new RegExp("^[-,.() \'a-zA-Z0-9()]+$").test(value)
        },
        message: '供应商英文名称只能包含字母、数字和"-(,.\'"'
    },
    supplierBankName: {
        validator: function (value) {
            return new RegExp("^[-,.() \'a-zA-Z0-9()]+$").test(value)
        },
        message: '银行名称只能包含字母、数字和"-(,.\'"'
    },
    supplierAccountAddr: {
        validator: function (value) {
            return new RegExp("^[-,.() #/\'a-zA-Z0-9()]+$").test(value)
        },
        message: '地址只能包含字母、数字和"-(,.#/\'"'
    },
    //用户账号验证(只能包括 _ 数字 字母)   
    account: {//param的值为[]中值  
        validator: function (value, param) {
            if (value.length < param[0] || value.length > param[1]) {
                $.fn.validatebox.defaults.rules.account.message = '用户名长度必须在' + param[0] + '至' + param[1] + '范围';
                return false;
            } else {
                if (!/^[\w]+$/.test(value)) {
                    $.fn.validatebox.defaults.rules.account.message = '用户名只能数字、字母、下划线组成.';
                    return false;
                } else {
                    return true;
                }
            }
        }, message: ''
    },
    // filebox验证文件大小的规则函数
    // 如：validType : ['fileSize[3,"MB"]']
    fileSize: {
        validator: function (value, array) {
            var size = array[0];
            var unit = array[1];
            if (!size || isNaN(size) || size == 0) {
                $.error('验证文件大小的值不能为 "' + size + '"');
            } else if (!unit) {
                $.error('请指定验证文件大小的单位');
            }
            var index = -1;
            var unitArr = new Array("bytes", "kb", "mb", "gb", "tb", "pb", "eb", "zb", "yb");
            for (var i = 0; i < unitArr.length; i++) {
                if (unitArr[i] == unit.toLowerCase()) {
                    index = i;
                    break;
                }
            }
            if (index == -1) {
                $.error('请指定正确的验证文件大小的单位：["bytes", "kb", "mb", "gb", "tb", "pb", "eb", "zb", "yb"]');
            }
            // 转换为bytes公式
            var formula = 1;
            while (index > 0) {
                formula = formula * 1024;
                index--;
            }
            // this为页面上能看到文件名称的文本框，而非真实的file
            // $(this).next()是file元素
            return $(this).next().get(0).files[0].size < parseFloat(size) * formula;
        },
        message: '文件大小必须小于 {0}{1}'
    },
    //下拉框输入验证
    comboBoxEditValid: {
        validator: function (value, param) {
            var $combobox = $("#" + param[0]);
            if (value) {
                if ($combobox.combobox('getValue') == $combobox.combobox('getText'))
                    return false;
                return true;
            }
            return false;
        },
        message: '请选择下拉框选项，不要直接使用输入内容'
    },
    //验证件数
    packNo: {
        validator: function (value) {
            var max = 2147483647;
            var min = 1;
            if (value < min) {
                $.fn.validatebox.defaults.rules.packNo.message = '件数不能小于1';
                return false;
            }
            if (value > max) {
                $.fn.validatebox.defaults.rules.packNo.message = '输入数字超出限制';
                return false;
            }
            return true;
        },
        message: ''
    },
    //验证流水号：输入只能是字母或数字或两者
    seqNo: {
        validator: function (value) {
            if (value.length > 50) {
                return false;
            }
            return new RegExp("[a-zA-Z0-9]+$").test(value);
        },
        message: '流水号只能包含字母、数字, 且长度不超过50'
    },
    //验证税务名称：* + 开票名称 + * + 报关品名，如：*光电子器件*发光二极管
    taxName: {
        validator: function (value) {
            var reg = /^\*.*\*.*[^\*]$/;
            return reg.test(value);
        },
        message: '税务名称格式要求：* + 开票名称 + * + 报关品名，如：*光电子器件*发光二极管'
    },
    //验证运输批次号：输入只能是11位数字
    voyNo: {
        validator: function (value) {
            var reg = /^(\d{13})$/;
            return reg.test(value);
        },
        message: '运输批次号必须是13位数字'
    },
});

/** 
         * 使用方法: 
         * 开启:MaskUtil.mask(); 
         * 关闭:MaskUtil.unmask(); 
         * 
         * MaskUtil.mask('其它提示文字...'); 
         */
var MaskUtil = (function () {
    var $mask, $maskMsg;
    var defMsg = '正在处理，请稍待。。。';
    function init() {
        if (!$mask) {
            $mask = $("<div class=\"datagrid-mask mymask\"></div>").appendTo("body");
        }
        if (!$maskMsg) {
            $maskMsg = $("<div class=\"datagrid-mask-msg mymask\">" + defMsg + "</div>")
                .appendTo("body").css({ 'font-size': '12px' });
        }
        $mask.css({ width: "100%", height: $(document).height() });
        var scrollTop = $(document.body).scrollTop();
        $maskMsg.css({
            left: ($(document.body).outerWidth(true) - 190) / 2
            , top: 300
        });
    }
    return {
        mask: function (msg) {
            init();
            $mask.show();
            $maskMsg.html(msg || defMsg).show();
        }
        , unmask: function () {
            $mask.hide();
            $maskMsg.hide();
        }
    }
}());

/*
       三个参数
       file：一个是文件(类型是图片格式)，
       obj：一个是文件压缩的后宽度，宽度越小，字节越小
       callback：一个是容器或者回调函数
       photoCompress()
        */
function photoCompress(file, obj, callback) {
    var ready = new FileReader();
    /*开始读取指定的Blob对象或File对象中的内容. 当读取操作完成时,readyState属性的值会成为DONE,如果设置了onloadend事件处理程序,则调用之.同时,result属性中将包含一个data: URL格式的字符串以表示所读取文件的内容.*/
    ready.readAsDataURL(file);
    ready.onload = function () {
        var re = this.result;
        canvasDataURL(re, file.name, obj, callback)
    }
}
function canvasDataURL(path, fileName, obj, callback) {
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
        callback(base64, fileName);
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

/**
 * datagrid 刷新行样式插件
 */
(function ($) {
    function setStyle(target, rowIndex) {
        var opts = $.data(target, 'datagrid').options;
        var tr = opts.finder.getTr(target, rowIndex, 'body');
        if (!tr.length) { return; }
        var rowData = opts.finder.getRow(target, rowIndex);
        var cs = _getRowStyle(rowIndex);
        var style = cs.s;
        var cls = 'datagrid-row ' + (rowIndex % 2 && opts.striped ? 'datagrid-row-alt ' : ' ') + cs.c;
        var cls12 = (tr.hasClass('datagrid-row-checked') ? ' datagrid-row-checked' : '') +
                    (tr.hasClass('datagrid-row-selected') ? ' datagrid-row-selected' : '');
        tr.attr('style', style).attr('class', cls + cls12);

        function getStyleValue(css) {
            var classValue = '';
            var styleValue = '';
            if (typeof css == 'string') {
                styleValue = css;
            } else if (css) {
                classValue = css['class'] || '';
                styleValue = css['style'] || '';
            }
            return { c: classValue, s: styleValue };
        }
        function _getRowStyle(rowIndex) {
            var css = opts.rowStyler ? opts.rowStyler.call(target, rowIndex, rowData) : '';
            return getStyleValue(css);
        }
    }
    $.extend($.fn.datagrid.methods, {
        refreshRowStyle: function (jq, index) {
            return jq.each(function () {
                setStyle(this, index);
            });
        }
    });
})(jQuery);

//json 对象转 querystring
function parseParams(data) {
    try {
        var tempArr = [];
        for (var i in data) {
            var key = encodeURIComponent(i);
            var value = encodeURIComponent(data[i]);
            tempArr.push(key + '=' + value);
        }
        var urlParamsStr = tempArr.join('&');
        return urlParamsStr;
    } catch (err) {
        return '';
    }
}