/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

/*
form值：
实际sender的val
*/
if (v_RFQ_3_0) {
    var url = document.scripts[document.scripts.length - 1].src;
    var lower = url.toLowerCase();
    var prexUrl = url.substring(0, lower.indexOf('/yahv/') + '/yahv/'.length);
    writeScript('' + prexUrl + '/standard-easyui/v_RFQ_3_0/companyPayee.js');
}
else
//供应商收款人（原受益人）
(function ($) {
    //接口地址
    var AjaxUrl = {
        getCompanyPayee: '/csrmapi/companies/Payee/'
    }
    //编写companyPayee插件
    $.fn.companyPayee = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.companyPayee.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("companyPayee.该插件没有这个方法：" + opt);
                return this;
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.companyPayee.defaults, options);

        return this.each(function () {
            var sender = $(this);

            //var inputCompanyPayeeID = $('#' + sender.attr('textboxname') + 'companyPayeeID');
            //if (inputCompanyPayeeID.length > 0) {
            //    sender.supplierPayee('setCompanyName', options.companyName);
            //    return sender;
            //}

            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'companyPayee_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }
            var formName = sender.prop('name');
            if (!formName) {
                alert('控件name不能为空！2');
            }

            //创建form返回值
            //var inputCompanyPayeeID = $('<input type="hidden" value="' + '' + '" id="' + formName + 'companyPayeeID" name="' + formName + 'companyPayeeID">');
            //alert(inputCompanyPayeeID.length);
            //sender.after(inputCompanyPayeeID);
            //alert([$('#supplierPayercompanyPayeeID').length, formName + 'companyPayeeID', 'supplierPayercompanyPayeeID']);
            //sender.data('inputs', {
            //    inputCompanyPayeeID: inputCompanyPayeeID
            //});
            var initdata = [];
            if (options.companyName) {
                $.ajax({
                    async: false,
                    url: AjaxUrl.getCompanyPayee,
                    dataType: 'json',
                    type: 'GET',
                    data: { name: options.companyName },
                    success: function (json) {
                        initdata = json.Data;
                        sender.data('items', initdata);
                    },
                    error: function (err) {
                        alert('error:' + JSON.stringify(err));
                    }
                });
            }

            sender.combobox({
                valueField: 'ID',
                textField: 'BankAccount',
                formatter: function (row) {
                    var s = '<div class="supplier-payer">' +
                            '<div><em>实际名称:</em>' + row.RealName + '</div>' +
                            '<div><em>开户行:</em>' + row.Bank + '</div>' +
                            '<div><em>开户行地址:</em>' + row.BankAddress + '</div>' +
                            '<div><em>银行账号:</em>' + row.Account + '</div>' +
                            '<div><em>SwiftCode:</em>' + row.SwiftCode + '</div>' +
                            '<div><em>支付方式:</em>' + row.MethordDes + '</div>' +
                            '<div><em>币种:</em>' + row.CurrencyDes + '</div>' +
                            '<div><em>所在地:</em>' + row.DistrictDes + '</div>' +
                            '<div><em>联系人姓名:</em>' + row.Name + '</div>' +
                            '<div><em>联系人电话:</em>' + row.Tel + '</div>' +
                            '<div><em>联系人手机号:</em>' + row.Mobile + '</div>' +
                            '<div><em>联系人邮箱:</em>' + row.Email + '</div>' +
                            '</div> ';
                    return s;
                },
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                onSelect: function (record) {
                    //var oldValue = inputCompanyPayeeID.val();
                    //inputCompanyPayeeID.val(record.id);
                },
                data: initdata,
                editable: false
            });

            //结束
        });
    }

    //供应商插件的默认配置
    $.fn.companyPayee.defaults = {
        companyName: '', //供应商名称
        prompt: '请收款人',
        required: true,
    };

    //供应商插件对外的方法
    $.fn.companyPayee.methods = {
        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
        },
        //获取数据
        setCompanyName: function (jq, name) {
            var sender = $(jq);

            sender.combobox('disable');
            sender.combobox('clear');
            sender.combobox('loadData', []);

            $.ajax({
                url: AjaxUrl.getCompanyPayee,
                dataType: 'json',
                type: 'GET',
                data: { name: name },
                success: function (json) {
                    var data = json.Data;
                    sender.combobox('loadData', data);
                    sender.data('items', data);
                    sender.combobox('enable');
                 

                    //有些数据对不上特殊增加
                    var value = sender.combobox('getValue');
                    if ($.grep(data, function (item) {
                        return item.ID == value;
                    }).length == 0) {
                        sender.combobox('setValue', '');
                    }


                },
                error: function (err) {
                    alert('error:' + JSON.stringify(err));
                }
            });

            return sender;
        },
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            return sender;
        }
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('companyPayee');
})(jQuery)