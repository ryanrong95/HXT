//展示loading
function ajaxLoading() {
    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
    $("<div class=\"datagrid-mask-msg\" style=\"padding:18px 5px 18px 30px;\"></div>").html("<span style=\"position: relative;top: -12px;color:#666;font-size:14px;\">loading</span>").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
}

function ajaxLoadingWithMsg(msg) {
    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
    $("<div class=\"datagrid-mask-msg\" style=\"padding:18px 5px 18px 30px;\"></div>").html("<span style=\"position: relative;top: -12px;color:#666;font-size:14px;\">" + msg + "</span>").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
}


//easyui 只展示文本
$.fn.toText = function () {
    var senders = $(this);
    senders.each(function () {
        var sender = $(this);
        var iclass = sender.prop('class');

        var width = sender.width();
        var height = sender.height();

        if (iclass.indexOf('easyui-combo') >= 0 || iclass.indexOf('easyui-text') >= 0) {
            var easyui = sender.next();
            easyui.hide();
            var text = sender.textbox('getText');
            easyui.after('<span style="width: ' + width + 'px; height: ' + height + 'px; display: block;">' + text + '</span>');
        } else {
            sender.hide();
            var text = sender.val();
            sender.after('<span style="width: ' + width + 'px; height: ' + height + 'px; display: block;">' + text + '</span>');
        }
    });
};

//loading展示结束
function ajaxLoadEnd() {
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
}

$.fn.layout.defaults.border = false;
//$.fn.panel.defaults.border = false;

if ($.fn.linkbutton) {
    $.fn.linkbutton.defaults.height = "22px";
}

//validatebox验证密码一致的扩展方法

$.extend($.fn.validatebox.defaults.rules, {
    equalTo: { validator: function (value, param) { return $(param[0]).val() == value; }, message: '字段不匹配' },
    phoneNum: { //验证手机号  
        validator: function (value, param) {
            return /^0?(13|14|15|17|18|19)[0-9]{9}$/.test(value);
        },
        message: '请输入正确的手机号码。'
    },
    telNum: { //既验证手机号，又验证座机号
        validator: function (value, param) {
            return /(^[0-9-()（）]{7,18}$)|(^0?(13|14|15|17|18|19)[0-9]{9}$)/.test(value);
        },
        message: '请输入正确的电话号码。'
    },
    idcard: {// 验证身份证
        validator: function (value) {
            return /^\d{15}(\d{2}[A-Za-z0-9])?$/i.test(value);
        },
        message: '身份证号码格式不正确'
    }
});

//阻止默认事件
function stopPropagation(e) {
    if (e.stopPropagation)
        e.stopPropagation();
    else
        e.cancelBubble = true;
}

//form统一调用
$(function () {
    var sender = $('form');
    sender.each(function (index, elem) {
        var subber = $(elem);

     

        subber.submit(function () {

            //debugger;

            //subber.linkbutton('disable');
            //subber.prop('disabled', 'disabled');


         
            if (subber.data('submited')) {
                var $ = top.$;
                myAlert = $.messager.alert;
                myAlert('提示', '请不要重复提交！');
            }

            var vaild = subber.form('enableValidation').form('validate');
            if (vaild) {
                subber.data('submited', true);
            }

            return vaild;

            //可以接收事件，未来开发
            //return subber.form('enableValidation').form('validate');

            //alert(vaild);

            //return false;

            //if (subber.form('enableValidation').form('validate')) {
            //    return true;
            //} else {
            //    return false;
            //}
        });
    });
    //帮助王俊丽触发
    $(document.body).bind('click', function () {
        top.document.body.click();
    });

    if (typeof (model) != 'undefined' && model) {
        sender.form('load', model);
    }
});

//获取地址参数
function queryString(name, url) {
    var result;
    if (url == undefined) {
        result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    } else {
        result = url.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    }
    if (result == null || result.length < 1) {
        return "";
    }
    return result[1];
}
//控制tooltip的显示样式
if ($.fn.tooltip) {
    $.fn.tooltip.defaults.onShow = function () { $(this).tooltip('tip').css({ backgroundColor: '#fb4d4d', borderColor: '#fb4d4d', color: '#fff' }); };
}
if ($.fn.validatebox) {
    $.fn.validatebox.defaults.tipOptions.onShow = function () { $(this).tooltip('tip').css({ backgroundColor: '#fb4d4d', borderColor: '#fb4d4d', color: '#fff' }); };
}
//easyui 控件默认宽高
if ($.fn.accordion) {
    $.fn.accordion.defaults.width = "auto";
    $.fn.accordion.defaults.height = "auto";
}

if ($.fn.calendar) {
    $.fn.calendar.defaults.width = 120;
    $.fn.calendar.defaults.height = 120;
}

if ($.fn.checkbox) {
    $.fn.checkbox.defaults.width = 16;
    $.fn.checkbox.defaults.height = 16;
}

if ($.fn.combo) {
    $.fn.combo.defaults.panelWidth = null;
    $.fn.combo.defaults.panelHeight = 300;
    $.fn.combo.defaults.panelMinWidth = null;
    $.fn.combo.defaults.panelMaxWidth = null;
    $.fn.combo.defaults.panelMinHeight = null;
    $.fn.combo.defaults.panelMaxHeight = null;
}

if ($.fn.combobox) {
    $.fn.combobox.defaults.width = "120px";
    $.fn.combobox.defaults.height = "22px";
    $.fn.combobox.defaults.panelMinHeight = 30;
    $.fn.combobox.defaults.panelHeight = "auto";
    $.fn.combobox.defaults.panelMaxHeight = 200;
}
if ($.fn.combotree) {
    $.fn.combotree.defaults.width = "120px";
    $.fn.combotree.defaults.height = "22px";
    $.fn.combotree.defaults.panelMinWidth = "120px";
    $.fn.combotree.defaults.panelWidth = "auto";
    $.fn.combotree.defaults.panelMinHeight = 30;
    $.fn.combotree.defaults.panelHeight = "auto";
    $.fn.combotree.defaults.panelMaxHeight = 200;
}

if ($.fn.datebox) {
    $.fn.datebox.defaults.width = 120;
    $.fn.datebox.defaults.panelWidth = 250;
    $.fn.datebox.defaults.panelHeight = "auto";
    $.fn.datebox.defaults.height = "22px";
}

if ($.fn.linkbutton) {
    $.fn.linkbutton.defaults.height = "22px";
}

if ($.fn.layout) {
    $.fn.layout.paneldefaults.minWidth = 10;
    $.fn.layout.paneldefaults.minHeight = 10;
    $.fn.layout.paneldefaults.maxWidth = 10000;
    $.fn.layout.paneldefaults.maxHeight = 10000;
}

if ($.fn.menu) {
    $.fn.menu.defaults.minWidth = 150;
    $.fn.menu.defaults.itemHeight = 32;
}

if ($.fn.messager) {
    $.fn.messager.defaults.width = 300;
    $.fn.messager.defaults.height = "auto";
    $.fn.messager.defaults.minHeight = 150;
}

if ($.fn.panel) {
    $.fn.panel.defaults.width = "auto";
    $.fn.panel.defaults.height = "auto";
}

if ($.fn.progressbar) {
    $.fn.progressbar.defaults.width = "auto";
    $.fn.progressbar.defaults.height = 22;
}

if ($.fn.propertygrid) {
    $.fn.propertygrid.defaults.groupHeight = 28;
    $.fn.propertygrid.defaults.expanderWidth = 20;
    $.fn.propertygrid.defaults.frozenColumns = [[{ field: "f", width: 20, resizable: false }]];
    $.fn.propertygrid.defaults.columns = [[{ field: "name", title: "Name", width: 100, sortable: true }, { field: "value", title: "Value", width: 100, resizable: false }]];
}

if ($.fn.radiobutton) {
    $.fn.radiobutton.defaults.width = 14;
    $.fn.radiobutton.defaults.height = 14;
}

if ($.fn.resizable) {
    $.fn.resizable.defaults.minWidth = 10;
    $.fn.resizable.defaults.minHeight = 10;
    $.fn.resizable.defaults.maxWidth = 10000;
    $.fn.resizable.defaults.maxHeight = 10000;
}

if ($.fn.sidemenu) {
    $.fn.sidemenu.defaults.width = 200;
    $.fn.sidemenu.defaults.height = "auto";
}

if ($.fn.slider) {
    $.fn.slider.defaults.width = "auto";
    $.fn.slider.defaults.height = "auto";
}


if ($.fn.switchbutton) {
    $.fn.switchbutton.defaults.handleWidth = "auto";
    $.fn.switchbutton.defaults.width = 60;
    $.fn.switchbutton.defaults.height = 30;
    $.fn.switchbutton.defaults.labelWidth = "auto";
}

if ($.fn.tabs) {
    $.fn.tabs.defaults.width = "auto";
    $.fn.tabs.defaults.height = "auto";
    $.fn.tabs.defaults.headerWidth = 150;
    $.fn.tabs.defaults.tabWidth = "auto";
    $.fn.tabs.defaults.tabHeight = 32;
}

if ($.fn.textbox) {
    $.fn.textbox.defaults.width = "120px";
    $.fn.textbox.defaults.height = "22px";
    $.fn.textbox.defaults.iconWidth = 26;
    $.fn.textbox.defaults.labelWidth = "auto";

    $.fn.textbox.defaults.isKeydown = false;
    $.fn.textbox.defaults.target = null;
    $.fn.textbox.defaults.inputEvents.keydown = function (e) {
        if (e.keyCode == 13) {
            var t = $(e.data.target);
            t.textbox("setValue", t.textbox("getText"));
            if (t.textbox("options").isKeydown) {
                $("#btnSearch").trigger("click");
            }
        }
    };
}
if ($.fn.passwordbox) {
    $.fn.passwordbox.defaults.width = "120px";
    $.fn.passwordbox.defaults.height = "22px";
}

if ($.fn.numberbox) {
    $.fn.numberbox.defaults.width = "120px";
    $.fn.numberbox.defaults.height = "22px";
}



//验证开始时间小于结束时间 
$.extend($.fn.validatebox.defaults.rules, {
    end: {
        validator: function (value, param) {
            var startDate = $("#start").val();
            var startTmp = new Date(startDate.replace(/-/g, "/"));
            var endTmp = new Date(value.replace(/-/g, "/"));
            return startTmp <= endTmp;
        },
        message: '结束时间要大于开始时间！'
    }
})

if ($.fn.datagrid) {
    // 新增行背景色
    $.fn.datagrid.defaults.rowStyler = function (index, row) {
        if (row.isOpen) {
            return 'background-color:#efbc4b;';
        }
    }
    //单元格点击
    $.fn.datagrid.defaults.onClickCell = function (index, field) {
        if (field == 'show') {
            $(this).myDatagrid('rowExpand', index);
            //var rowData = window.grid.datagrid('getRows')[index];
            //if (rowData.opened) {
            //    var changesRow = window.grid.datagrid('getChanges', 'inserted');
            //    rows = $.map(changesRow, function (value, index) {
            //        if (value.InquiryID == rowData.ID) {
            //            return value;
            //        }
            //    });
            //    for (var i = 0; i < rows.length; i++) {
            //        window.grid.datagrid('deleteRow', rows[i].index);
            //    }
            //    rowData.opened = false;
            //}
            //else {
            //    $.post('?action=getQuotes', { inquiryid: rowData.ID }, function (data) {
            //        if (data.data.length > 0) {
            //            var idx = index;
            //            for (var i = data.data.length - 1; i >= 0; i--) {
            //                var insertrow = data.data[i];
            //                insertrow.index = idx + 1;
            //                insertrow.isOpen = true;
            //                insertrow.inquiryStatus = rowData.Status;
            //                insertrow.inquiryQuoteStatus = rowData.QuoteStatus;
            //                window.grid.datagrid('insertRow', {
            //                    index: insertrow.index,
            //                    row: insertrow
            //                });
            //            }
            //            $.parser.parse('.easyui-formatted');
            //            rowData.opened = true;
            //        }
            //    });
            //}
        }
    }
}




// 人民币未税含税换算
// invoiceType 开票类型 4 不开票 1 普票 2 专票
// price 未税价
// taxPrice 含税价
//未税转含税公式： 专用发票：未税金额*1.13  普通发票：未税金额*1.03
var taxRate1 = 1.03; // 普票税率
var taxRate2 = 1.13; // 专票税率
var taxRate4 = 1; // 不开票税率
function ConvertToTaxPrice(invoiceType, price) {
    if (invoiceType == 1) {
        return toDecimal(price * taxRate1, 5);
    }
    else if (invoiceType == 2) {
        return toDecimal(price * taxRate2, 5);
    }
    else {
        return toDecimal(price * taxRate4, 5);
    }
}
// 人民币未税含税换算
// invoiceType 开票类型 1 不开票 2 普票 3 专票
// price 未税价
// taxPrice 含税价
// 含税转未税公式： 专用发票：含税金额/1.13  普通发票：含税金额/1.03
function ConvertToPrice(invoiceType, taxPrice) {
    if (invoiceType == 1) {
        return toDecimal(taxPrice / taxRate1, 5);
    }
    else if (invoiceType == 2) {
        return toDecimal(taxPrice / taxRate2, 5);
    }
    else {
        return toDecimal(taxPrice / taxRate4, 5);
    }
}

//功能：将浮点数四舍五入，取小数点后5位 
function toDecimal(price, x) {
    var f = parseFloat(price);
    if (isNaN(f)) {
        return;
    }
    f = Math.round(price * Math.pow(10, x)) / Math.pow(10, x);
    return f;
}

//onLoadSuccess时为datagird的某个单元格添加tooltip提示的方法
function tipFun(columnName, _grid, cb, select) {
    var tableTd = $('div.datagrid-body td[field="' + columnName + '"]'); //columnName是列名
    if (select != null) {
        tableTd = tableTd[select];
        var $this = $(tableTd);
        var index = $this.parent('tr').attr('datagrid-row-index');
        var currentRow = _grid;//_grid为数据
        var currentRowHtml = cb(currentRow);
        var content = '<div style="color:#fff;max-width:700px;word-break: break-all; word-wrap: break-word;">' + currentRowHtml + '</div>';
        $this.tooltip({ position: 'bottom', content: content, onShow: function () { $(this).tooltip('tip').css({ backgroundColor: '#666', borderColor: '#666' }); } });
    } else {
        tableTd.each(function () {
            var $this = $(this);
            var index = $this.parent('tr').attr('datagrid-row-index');
            if (index == undefined) {
                return;
            }
            var currentRow = _grid.datagrid('getRows')[index];
            var currentRowHtml = cb(currentRow);
            var content = '<div style="color:#fff;max-width:700px;word-break: break-all; word-wrap: break-word;">' + currentRowHtml + '</div>';
            $this.tooltip({ position: 'bottom', content: content, onShow: function () { $(this).tooltip('tip').css({ backgroundColor: '#666', borderColor: '#666' }); } });
        });
    }
}

//生成tip提示的方法
function create_tip(data) {
    var result = "";
    for (var key in data) {
        result += "<div>" + key + "：" + data[key] + "</div>";
    }
    return result;
}

//生成tip提示的方法
function create_tip2(data) {
    var result = "";
    for (var key in data) {
        result += "<div style='margin-left:30px'>" + key + "：" + data[key] + "</div>";
    }
    return result;
}
//生成客户提示
function createClientTip(grid, type, select) {
    tipFun('Client', grid, function (currentRow) {
        //type=='noContact' 联系人不能看
        //type=='hideNameAndnoContact' 联系人不能看，客户公司名称******
        //type=='all' 都能看

        if (!currentRow['Client']) {
            return false;
        }

        var jsonData = {
            "公司名称": currentRow['Client'].Name,
            "类型": currentRow['Client'].AreaTypeDes,
            "性质": currentRow['Client'].NatureDes,
            "等级": currentRow['Client'].GradeDes,
            "VIP等级": currentRow['Client'].VipDes,
        }
        if (type == 'noContact') {
            jsonData = jsonData;
        } else if (type == 'hideNameAndnoContact') {
            jsonData["公司名称"] = "******";
        } else if (type == 'all') {
            var contactMsg;
            if (!currentRow['ContactMobile']) {
                contactMsg = currentRow['Contact'];
            } else {
                contactMsg = currentRow['Contact'] + "(" + currentRow['ContactMobile'] + ")";
            }
            jsonData["联系人"] = contactMsg;
        }
        return create_tip(jsonData);
    }, select);
}
//生成渠道提示
function createChannelTip(grid, type, select) {
    tipFun('ChannelRequirement', grid, function (currentRow) {
        //type=='all' 都能看
        //type=='hideName' 渠道供应商名称******
        if (!currentRow['ChannelRequirement']) {
            return false;
        }
        var jsonData = {
            "供应商名称": currentRow['ChannelRequirement'].Name,
            //"大赢家编号": currentRow['ChannelRequirement'].DyjCode,
            "是否是原产": currentRow['ChannelRequirement'].IsFactory ? '是' : '否',
            "类型": currentRow['ChannelRequirement'].TypeDes,
            "性质": currentRow['ChannelRequirement'].NatureDes,
            "等级": currentRow['ChannelRequirement'].GradeDes,
            "税号": currentRow['ChannelRequirement'].TaxperNumber,
            "额度": '人民币' + currentRow['ChannelRequirement'].Price,
            "账期": currentRow['ChannelRequirement'].RepayCycle + '天',
        }
        if (type == 'all') {
            jsonData = jsonData;
        } else if (type == 'hideName') {
            jsonData["供应商名称"] = "******";
        }
        return create_tip(jsonData);
    }, select);
}

//生成渠道提示
function createSupplierTip(grid, type, select) {
    tipFun('SupplierInfo', grid, function (currentRow) {
        if (!currentRow['SupplierInfo']) {
            return "该供应商没有特色信息!";
        }
        var manudata = {};
        var namedata = {};
        var a = currentRow['SupplierInfo'][0].品牌;
        var b = currentRow['SupplierInfo'][0].型号;
        for (var i = 0; i < a.length; i++) {
            manudata.品牌名称 = a[i].名称;
            manudata.品牌备注 = a[i].备注;
        }
        var manutip = create_tip2(manudata);
        for (var i = 0; i < b.length; i++) {
            namedata.型号名称 = b[i].名称;
            namedata.型号备注 = b[i].备注;
        }
        var nametip = create_tip2(namedata);
        var jsonData = {
            "特色品牌": manutip,
            "特色型号": nametip,
        }
        return create_tip(jsonData);
    }, select);
}

var HtmlUtil = {
    /*1.用浏览器内部转换器实现html转码*/
    htmlEncode: function (html) {
        //1.首先动态创建一个容器标签元素，如DIV
        var temp = document.createElement("div");
        //2.然后将要转换的字符串设置为这个元素的innerText(ie支持)或者textContent(火狐，google支持)
        (temp.textContent != undefined) ? (temp.textContent = html) : (temp.innerText = html);
        //3.最后返回这个元素的innerHTML，即得到经过HTML编码转换的字符串了
        var output = temp.innerHTML;
        temp = null;
        return output;
    },
    /*2.用浏览器内部转换器实现html解码*/
    htmlDecode: function (text) {
        //1.首先动态创建一个容器标签元素，如DIV
        var temp = document.createElement("div");
        //2.然后将要转换的字符串设置为这个元素的innerHTML(ie，火狐，google都支持)
        temp.innerHTML = text;
        //3.最后返回这个元素的innerText(ie支持)或者textContent(火狐，google支持)，即得到经过HTML解码的字符串了。
        var output = temp.innerText || temp.textContent;
        temp = null;
        return output;
    }
};

/**
* easyui Datagrid百分比显示宽度
*/
function fixWidth(percent) {
    return document.body.clientWidth * percent / 100;//根据自身情况更改
}