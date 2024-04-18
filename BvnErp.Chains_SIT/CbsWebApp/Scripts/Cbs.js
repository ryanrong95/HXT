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

//扩展easyui表单的验证  
$.extend($.fn.validatebox.defaults.rules, {

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