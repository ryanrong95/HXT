//通用的调用接口获取数据的方法
function getDataFun(url, data, ProcessFun, error) {
    $.ajax({
        url: url,
        type: 'get',
        data: data,
        dataType: 'json',
        success: function (data) {
            if (data.code == "100") {
                if (ProcessFun.noDataFun) {
                    ProcessFun.noDataFun(data);
                }
            } else if (data.code == "200") {
                if (ProcessFun.success) {
                    ProcessFun.success(data);
                }
            } else if (data.code == "300") {
                if (ProcessFun.exceptionFun) {
                    ProcessFun.exceptionFun(data);
                }
                console.log("接口异常");
            }
        },
        error: function (errorMsg) {
            console.log(errorMsg);
            if (error) {
                error(errorMsg);
            }
        }
    });
}

//通用的调用接口提交数据的方法
function postDataFun(url, data, ProcessFun, error) {
    $.ajax({
        url: url,
        type: 'post',
        data: data,
        dataType: 'json',
        crossDomain: true,
        success: function (data) {
            if (data.code == "100") {
                if (ProcessFun.noDataFun) {
                    ProcessFun.noDataFun(data);
                }
            } else if (data.code == "200") {
                if (ProcessFun.success) {
                    ProcessFun.success(data);
                }
            } else if (data.code == "300") {
                if (ProcessFun.exceptionFun) {
                    ProcessFun.exceptionFun(data);
                }
                console.log("接口异常");
            }
        },
        error: function (errorMsg) {
            console.log(errorMsg);
            if (error) {
                error(errorMsg);
            }
        }
    });
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
            }
            else if ($this.attr("class").indexOf("combogrid") > 0) {
                values[id] = $('#' + id).combogrid('getText');
            }
            else if ($this.attr("class").indexOf("datetimebox") > 0) {
                values[id] = $('#' + id).datetimebox('getValue');
            }
            else if ($this.attr("class").indexOf("datebox") > 0 && $this.attr("class").indexOf("validatebox") < 0) {
                values[id] = $('#' + id).datebox('getValue');
            }
            else if ($this.attr("class").indexOf("checkbox") > 0) {
                values[id] = $('#' + id).checkbox('options').checked;
            }
            else {
                values[id] = $.trim($this.val().replace(new RegExp("'", "g"), "#39;"));
            }
        }
    });
    return values;
}

//为Easy-ui的只读输入框设置背景色
function setReadonlyBgColor(className, color) {
    $('.' + className).each(function () {
        if ($(this).attr('readonly')) {
            switch (className) {
                case 'easyui-textbox':
                    $(this).textbox('textbox').css('background', color);
                    break;
                case 'easyui-numberbox':
                    $(this).numberbox('textbox').css('background', color);
                    break;
                case 'easyui-combogrid':
                    $(this).combogrid('textbox').css('background', color);
                    break;
            }
        }
    });
}

//判断输入的文本是否为浮点数 
function IsNotFloat(text) {
    var len = text.length;
    var dotNum = 0;
    if (text.length == 0) {
        return true;
    }

    for (var i = 0; i < len; i++) {
        var oneNum = text.substring(i, i + 1);
        if (oneNum == ".") {
            dotNum++;
        }
        if (((oneNum < "0" || oneNum > "9") && oneNum != ".") || dotNum > 1) {
            return true;
        }
    }
    if (len > 1 && text.substring(0, 1) == "0") {
        if (text.substring(1, 2) != ".") {
            return true;
        }
    }
    return false;
}