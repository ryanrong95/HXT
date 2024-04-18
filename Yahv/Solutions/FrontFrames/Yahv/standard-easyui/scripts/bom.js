(function ($) {
    String.prototype.getBytesLength = function () {
        var totalLength = 0;
        var charCode;
        for (var i = 0; i < this.length; i++) {
            charCode = this.charCodeAt(i);
            if (charCode < 0x007f) {
                totalLength++;
            } else if ((0x0080 <= charCode) && (charCode <= 0x07ff)) {
                totalLength += 2;
            } else if ((0x0800 <= charCode) && (charCode <= 0xffff)) {
                totalLength += 3;
            } else {
                totalLength += 4;
            }
        }
        return totalLength;
    };

    $.extend($.fn.validatebox.defaults.rules, {
        isNegative: {
            validator: function (value, param) {
                return (value.indexOf("-") == -1 && value > 0);
            },
            message: '数量必须大于0'
        },
        maxLength: {
            validator: function(value, param){
                return (value.getBytesLength() <= param[0])
            },
            message: '长度必须小于等于{0}'
        }
    });
    //公共方法

    //数量说明
    var QuantityRemarkData = [{ 'Description': '未知', 'ID': 0 }, { 'Description': '实际采购', 'ID': 1 }, { 'Description': '季度用量', 'ID': 2 }];
    //渠道要求
    var ChannelRequirementData = [{ 'Description': '不限', 'ID': 0 }, { 'Description': '市场', 'ID': 1 }, { 'Description': '海外', 'ID': 2 }, { 'Description': '高端', 'ID': 4 }, { 'Description': '固定渠道', 'ID': 8 }];
    //货期 
    var TradeTypeData = [{ 'Description': '不限', 'ID': 0 }, { 'Description': '现货', 'ID': 1 }, { 'Description': '期货', 'ID': 2 }];

    //接口地址
    var AjaxUrl = {
        url: "/RFQ/Sale/Boms/Edit.aspx?action=GetReadData"
    }
    //获取数据
    function getData(param, cb) {
        $.post(AjaxUrl.url, { urls: param }, function (data) {

        });
    }
    //生成随机ID
    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }

    //通过Description找ID

    function getIdByDescription(data, Description) {
        var ID;
        for (var i = 0; i < data.length; i++) {
            if (data[i].Description == Description) {
                ID = data[i].ID
            }
        }
        return ID;
    }

    function getIdsByDescriptions(data, Descriptions) {
        var IDs = [];
        var desArr = [];
        if (Descriptions.indexOf(",") == -1 && Descriptions.indexOf("，") == -1) {
            desArr.push(Descriptions);
        } else if (Descriptions.indexOf(",") > 0 && Descriptions.indexOf("，") == -1) {
            desArr = Descriptions.split(",");
        } else if (Descriptions.indexOf("，") > 0 && Descriptions.indexOf(",") == -1) {
            desArr = Descriptions.split("，");
        } else if (Descriptions.indexOf(",") > 0 && Descriptions.indexOf("，") > 0) {
            Descriptions = Descriptions.replace(/，/ig, ',');
            desArr = Descriptions.split(",");
        }
        for (var j = 0; j < desArr.length; j++) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].Description == desArr[j]) {
                    IDs.push(data[i].ID);
                }
            }
        }
        return IDs;
    }

    //编写bom插件
    $.fn.bom = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.bom.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法")
            }
        }
        var options = opt || {};
        var sender = this;
        //根据各种情况，融合options
        var data_options = {};
        var s = $.trim(sender.attr("data-options"));
        if (s) {
            if (s.substring(0, 1) != "{") {
                s = "{" + s + "}";
            }
            data_options = (new Function("return " + s))();
            options = $.extend(true, {}, $.fn.bom.defaults, data_options, opt);
        } else {
            options = $.extend(true, {}, $.fn.bom.defaults, opt);
        }

        if ($(sender).data("input")) {
            $(sender).data("table").remove();
            $(sender).data("input").remove();
        }

        $(sender).data("options", options);
        var table_id = randomID("table");

        /*生成dom节点元素开始*/
        var $table = $("<table class='liebiao bom_liebiao' id='" + table_id + "'></table>");
        var $input = $("<input type='hidden' name='" + options.name + "_rows'></table>");//存储有几条数据
        $(sender).data("input", $input);
        var $tr_th = $("<tr></tr>");
        $th_index = $("<th style='width:30px;'></th>");
        $tr_th.append($th_index);
        //默认的属性值
        var column2 = [
            {
                field: 'Name',
                title: '产品型号',
                required: true,
                width: 160
            },
            {
                field: 'Quantity',
                title: '数量',
                required: true,
                width: 90
            },
            {
                field: 'QuantityRemark',
                title: '数量说明',
                required: false,
                width: 102
            },
            {
                field: 'Manufacturer',
                title: '品牌',
                required: true,
                width: 160
            },
            {
                field: 'MfReplace',
                title: '品牌可替',
                required: false,
                width: 90
            },
            {
                field: 'ChannelRequirement',
                title: '渠道要求',
                required: false,
                width: 120
            },
            {
                field: 'Package',
                title: '封装',
                required: false
            },
            {
                field: 'Batch',
                title: '批次',
                required: false
            },
            {
                field: 'UnitPrice',
                title: '接受价',
                required: false
            },
            {
                field: 'UnitPrice1',
                title: '未税价',
                required: false
            },
            {
                field: 'UnitPrice2',
                title: '含税价',
                required: false
            },
            {
                field: 'TradeType',
                title: '货期',
                required: false,
                width: 68
            },
            {
                field: 'PurchasingCycle',
                title: '采购周期（天）',
                required: false
            },
            {
                field: 'Summary',
                title: '备注',
                required: false,
                width: 100
            }
        ]
        var name_index = [];
        var column;
        if (options.column) {
            column = options.column;
        } else {
            column = column2;
        }

        var saveData = [];//存储所有的值
        $(sender).data("saveData", saveData);

        //循环生成表格表头
        for (var i = 0; i < column.length; i++) {
            var $th;
            if (column[i].required) {
                if (column[i].width) {
                    $th = $("<th field='" + column[i].field + "' style='width:" + column[i].width + "px;'>" + column[i].title + "<span>*</span></th>");
                } else {
                    $th = $("<th field='" + column[i].field + "'>" + column[i].title + "<span>*</span></th>");
                }
            } else {
                if (column[i].width) {
                    $th = $("<th field='" + column[i].field + "' style='width:" + column[i].width + "px;'>" + column[i].title + "</th>");
                } else {
                    $th = $("<th field='" + column[i].field + "'>" + column[i].title + "</th>");
                }
            }
            $tr_th.append($th);

            name_index.push(column[i].field);//每个属性的name值
        }
        $table.append($tr_th);

        function createElement(data, name_index, sender) {
            for (var j = 0; j < data.length; j++) {
                var $tr_td = $("<tr index=" + j + "></tr>");
                var $td_first = $("<td class='first'>" + (j + 1) + "</td>")
                $tr_td.append($td_first);
                var $td_Name = $("<td><input id='" + name_index[0] + '_' + j + "' name='" + name_index[0] + '_' + j + "' class='easyui-standardModel' data-options='width:140,value:\"" + data[j].Name + "\"'/></td>");

                var $td_Quantity = $("<td><input id='" + name_index[1] + '_' + j + "' name='" + name_index[1] + '_' + j + "' class='easyui-numberbox' data-options='width:70,required:true,missingMessage:\"" + column[1].title + "不能为空\",validType:\"isNegative\",value:\"" + data[j].Quantity + "\"'/></td>");

                var $td_QuantityRemark = $("<td><span id='" + name_index[2] + '_' + j + "' class='easyui-radio'  data-options='name:\"" + name_index[2] + "_" + j + "\",data:" + JSON.stringify(QuantityRemarkData) + ",valueField: \"ID\",labelField: \"Description\",checked:" + getIdByDescription(QuantityRemarkData, data[j].QuantityRemark) + "'></span></td>");

                var $td_Manufacturer = $("<td><input id='" + name_index[3] + '_' + j + "' name='" + name_index[3] + '_' + j + "' class='easyui-standardBrand' data-options='width:140,value:\"" + data[j].Manufacturer + "\"'/></td>");

                var $td_MfReplace = null;

                if (data[j].MfReplace == "true") {
                    data[j].MfReplace = true;
                }
                if (data[j].MfReplace == "false") {
                    data[j].MfReplace = false;
                }

                if (data[j].MfReplace === "" || data[j].MfReplace == true) {
                    $td_MfReplace = $("<td><input id='" + name_index[4] + '_' + j + "' class='easyui-switchbutton' checked name='" + name_index[4] + '_' + j + "' data-options='width:70,height:22,onText:\"可替\",offText:\"不可替\",onChange: " + function (x) { $(this).switchbutton("setValue", x); $(this).switchbutton("setValue", x); } + "' /></td>");
                } else if (data[j].MfReplace == false) {
                    $td_MfReplace = $("<td><input id='" + name_index[4] + '_' + j + "' class='easyui-switchbutton' name='" + name_index[4] + '_' + j + "' data-options='value:false,width:70,height:22,onText:\"可替\",offText:\"不可替\",onChange: " + function (x) { $(this).switchbutton("setValue", x); $(this).switchbutton("setValue", x); } + "' /></td>");
                }

                var $td_ChannelRequirement = $("<td><span id='" + name_index[5] + '_' + j + "' class='easyui-checkboxlist'  data-options='name:\"" + name_index[5] + "_" + j + "\",missingMessage:\"" + column[5].title + "不能为空\",data:" + JSON.stringify(ChannelRequirementData) + ",valueField: \"ID\",labelField: \"Description\",checked:" + JSON.stringify(getIdsByDescriptions(ChannelRequirementData, data[j].ChannelRequirement)) + ",onSelect: " + function (element, item) { if (item.ID == 0 && $(element).checkboxlist("checkedValue").indexOf(0) != -1) { $(element).checkboxlist("setCheck", [0]) } else if (item.ID != 0 && $(element).checkboxlist("checkedValue").indexOf(0) != -1) { var list = $(element).checkboxlist("checkedValue").filter(function (it) { return it != 0 }); $(element).checkboxlist("setCheck", list); } } + "'></span></td>");


                var $td_Package = $("<td><input id='" + name_index[6] + '_' + j + "' name='" + name_index[6] + '_' + j + "' class='easyui-textbox' value='" + data[j].Package + "' data-options='width:70, validType:\"maxLength[50]\"'/></td>");
                var $td_Batch = $("<td><input id='" + name_index[7] + '_' + j + "' name='" + name_index[7] + '_' + j + "' class='easyui-textbox' value='" + data[j].Batch + "' data-options='width:70'/></td>");
                
                var $td_UnitPrice = $("<td><input id='" + name_index[8] + '_' + j + "' name='" + name_index[8] + '_' + j + "'/></td>");
                var $td_UnitPrice1 = $("<td><input id='" + name_index[9] + '_' + j + "' name='" + name_index[9] + '_' + j + "'/></td>");
                var $td_UnitPrice2 = $("<td><input id='" + name_index[10] + '_' + j + "' name='" + name_index[10] + '_' + j + "'/></td>");

                var $td_TradeType = $("<td><span id='" + name_index[11] + '_' + j + "' class='easyui-radio'  data-options='name:\"" + name_index[11] + "_" + j + "\",data:" + JSON.stringify(TradeTypeData) + ",valueField: \"ID\",labelField: \"Description\",checked:" + getIdByDescription(TradeTypeData, data[j].TradeType) + "'></span></td>");

                var $td_PurchasingCycle = $("<td><input id='" + name_index[12] + '_' + j + "' name='" + name_index[12] + '_' + j + "' class='easyui-numberbox' value='" + data[j].PurchasingCycle + "' data-options='width:70'/></td>");
                var $td_Summary = $("<td><input id='" + name_index[13] + '_' + j + "' name='" + name_index[13] + '_' + j + "' class='easyui-textbox' value='" + data[j].Summary + "' data-options='width:80'/></td>");

                $tr_td.append($td_Name);
                $tr_td.append($td_Quantity);
                $tr_td.append($td_QuantityRemark);
                $tr_td.append($td_Manufacturer);
                $tr_td.append($td_MfReplace);
                $tr_td.append($td_ChannelRequirement);
                $tr_td.append($td_Package);
                $tr_td.append($td_Batch);
                $tr_td.append($td_UnitPrice);
                $tr_td.append($td_UnitPrice1);
                $tr_td.append($td_UnitPrice2);
                $tr_td.append($td_TradeType);
                $tr_td.append($td_PurchasingCycle);
                $tr_td.append($td_Summary);
                $table.append($tr_td);
            }
            $(sender).after($table);
            $.parser.parse('#' + table_id);
            for (var n = 0; n < data.length; n++) {
                if (options.currency != 'CNY') {
                    $('#' + name_index[8] + '_' + n).numberbox2({
                        width: 70,
                        target2: '#' + name_index[9] + '_' + n,
                        target3: '#' + name_index[10] + '_' + n,
                        currency: options.currency,
                        rateType: options.rateType,
                        required: false,
                        missingMessage: column[8].title + "不能为空",
                        precision: 5,
                        groupSeparator: ",",
                        value: data[n].UnitPrice,
                        isNeedERate: true,
                        isNeedVATRate: true,
                        finish: function () {
                            $('#' + name_index[9] + '_' + n).numberbox2({
                                width: 70,
                                target1: '#' + name_index[8] + '_' + n,
                                target3: '#' + name_index[10] + '_' + n,
                                currency: "CNY",
                                rateType: options.rateType,
                                readonly: true,
                                value: data[n].UnitPrice1,
                                required: false,
                                missingMessage: column[9].title + "不能为空",
                                precision: 5,
                                groupSeparator: ",",
                                finish: function () {
                                    $('#' + name_index[10] + '_' + n).numberbox2({
                                        width: 70,
                                        target1: '#' + name_index[8] + '_' + n,
                                        target2: '#' + name_index[9] + '_' + n,
                                        currency: "CNY",
                                        rateType: options.rateType,
                                        readonly: true,
                                        value: data[n].UnitPrice2,
                                        required: false,
                                        missingMessage: column[10].title + "不能为空",
                                        precision: 5,
                                        groupSeparator: ",",
                                        finish: function () { }
                                    })
                                }
                            })
                        }
                    })
                } else {
                    $('#' + name_index[8] + '_' + n).numberbox2({
                        width: 70,
                        target2: '#' + name_index[9] + '_' + n,
                        target3: '#' + name_index[10] + '_' + n,
                        currency: options.currency,
                        rateType: options.rateType,
                        required: false,
                        missingMessage: column[8].title + "不能为空",
                        precision: 5,
                        groupSeparator: ",",
                        value: data[n].UnitPrice,
                        isNeedERate: true,
                        isNeedVATRate: true,
                        finish: function () {
                            $('#' + name_index[9] + '_' + n).numberbox2({
                                width: 70,
                                target1: '#' + name_index[8] + '_' + n,
                                target3: '#' + name_index[10] + '_' + n,
                                currency: "CNY",
                                rateType: options.rateType,
                                value: data[n].UnitPrice1,
                                required: false,
                                missingMessage: column[9].title + "不能为空",
                                precision: 5,
                                groupSeparator: ",",
                                finish: function () {
                                    $('#' + name_index[10] + '_' + n).numberbox2({
                                        width: 70,
                                        target1: '#' + name_index[8] + '_' + n,
                                        target2: '#' + name_index[9] + '_' + n,
                                        currency: "CNY",
                                        rateType: options.rateType,
                                        value: data[n].UnitPrice2,
                                        required: false,
                                        missingMessage: column[10].title + "不能为空",
                                        precision: 5,
                                        groupSeparator: ",",
                                        finish: function () { }
                                    })
                                }
                            })
                        }
                    })
                }
                
            }
            //渲染完成后，loading结束
            ajaxLoadEnd();
        }

        //调用创建元素
        function doCreateElement(data, name_index, sender) {
            if (data.length > 0) {
                $(sender).data("saveData", data);
                $(sender).data("input").val(data.length);
                createElement(data, name_index, sender);
            } else {
                $(sender).data("saveData", []);
                $(sender).data("input").val(0);
            }
            $(sender).data("table", $table);
            $(sender).data("table").after($(sender).data("input"));
            $(sender).data("tableId", table_id);
            $(sender).hide();
        }

        /*生成dom节点元素结束*/

        var data;
        if (options.data) {
            data = options.data;
            doCreateElement(data, name_index, sender);
        } else {
            getData(options.param, function (data) {
                data = data;
                doCreateElement(data, name_index, sender);
            })
        }
    }

    //bom控件的默认配置
    $.fn.bom.defaults = {
        name: "",
        column: null,
        url: null,
        data: null,
        param: null,
        currency: 'CNY',
        rateType:30
    };

    //bom控件对外的方法
    $.fn.bom.methods = {
        //获取bom控件options
        options: function (jq) {
            return $(jq).data("options");
        },
        //获取数据
        getData: function (jq) {
            var data = $(jq).data("saveData");
            var table_id = $(jq).data("tableId");
            var data2 = null;
            if (data && data.length > 0) {
                data2 = [];
                for (var i = 0; i < data.length; i++) {
                    var m = {};
                    for (var val in data[i]) {
                        m[val] = $("#" + val + "_" + i).val();
                        if (val == 'QuantityRemark') {
                            var QuantityRemark = $("#" + val + "_" + i).radio("checkeditem").Description;
                            m[val] = QuantityRemark;
                        }
                        if (val == 'MfReplace') {
                            var MfReplace = $("#" + val + "_" + i).val();
                            if (MfReplace == "on" || MfReplace == "true") {
                                m[val] = true;
                            } else if (MfReplace == "off" || MfReplace == "false") {
                                m[val] = false;
                            }
                        }
                        
                        if (val == 'ChannelRequirement') {
                            var ChannelRequirementArr = $("#" + val + "_" + i).checkboxlist("checkeditem");
                            var ChannelRequirementArr2 = [];
                            for (var n = 0; n < ChannelRequirementArr.length; n++) {
                                ChannelRequirementArr2.push(ChannelRequirementArr[n].Description)
                            }
                            var ChannelRequirementArr3 = ChannelRequirementArr2.join(",");
                            m[val] = ChannelRequirementArr3;
                        }
                        if (val == 'TradeType') {
                            var TradeType = $("#" + val + "_" + i).radio("checkeditem").Description;
                            m[val] = TradeType;
                        }
                    }
                    data2.push(m);
                }
            }
            return data2;
        },
    };
    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('bom');
})(jQuery)