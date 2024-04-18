/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

/*
form值：
inputclientInvoiceID name="{formName}clientInvoiceID"
*/
if (v_RFQ_3_0) {
    var url = document.scripts[document.scripts.length - 1].src;
    var lower = url.toLowerCase();
    var prexUrl = url.substring(0, lower.indexOf('/yahv/') + '/yahv/'.length);
    writeScript('' + prexUrl + '/standard-easyui/v_RFQ_3_0/clientInvoice.js');
}
else
//客户收款人（原受益人）
(function ($) {
    //接口地址
    var AjaxUrl = {
        getClientInvoice: '/csrmapi/Clients/Invoices/'
    }

    //编写clientInvoice插件
    $.fn.clientInvoice = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.clientInvoice.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("clientInvoice.该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.clientInvoice.defaults, options);

        return this.each(function () {
            var sender = $(this);

            //var oldinputclientInvoiceID = $('#' + sender.attr('textboxname') + 'clientInvoiceID');
            //if (oldinputclientInvoiceID.length > 0) {
            //    sender.clientInvoice('setClientName', options.clientName);
            //    return sender;
            //}

            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'clientInvoice_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }
            var formName = sender.prop('name');
            if (!formName) {
                alert('clientInvoice.控件name不能为空！2');
            }

            //创建form返回值
            //var inputclientInvoiceID = $('<input type="hidden" value="' + '' + '" id="' + formName + 'clientInvoiceID" name="' + formName + 'clientInvoiceID">');
            //sender.after(inputclientInvoiceID);

            //sender.data('inputs', {
            //    inputclientInvoiceID: inputclientInvoiceID
            //});
            var initdata = [];

            if (options.clientID) {
                $.ajax({
                    async: false,
                    url: AjaxUrl.getClientInvoice,
                    dataType: 'json',
                    type: 'GET',
                    data: { id: options.clientID },
                    success: function (json) {
                        if (json.Code == '200') {
                            initdata = json.Data;
                            sender.data('items', initdata);
                        }
                    },
                    error: function (err) {
                        alert('clientInvoice.error:' + JSON.stringify(err));
                    }
                });
            }

            sender.combobox({
                valueField: 'ID',
                textField: 'EnterpriseName',
                formatter: function (row) {
                    var s = '<div class="supplier-payer">' +
                            '<div><em>抬头:</em>' + row.EnterpriseName + '</div>' +
                            '<div><em>银行:</em>' + row.Bank + '</div>' +
                            '<div><em>帐号:</em>' + row.Account + '</div>' +
                            '<div><em>开户行:</em>' + row.BankAddress + '</div>' +
                            '<div><em>税务码:</em>' + row.TaxperNumber + '</div>' +
                            '</div> ';
                    return s;
                },
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                onSelect: function (record) {
                    //var oldValue = inputclientInvoiceID.val();
                    //inputclientInvoiceID.val(record.id);
                    //sender.combobox('setText');
                },
                data: initdata,
                editable: false
            });

            //结束
        });
    }

    //客户插件的默认配置
    $.fn.clientInvoice.defaults = {
        clientID: '', //客户ID
        clientName: '', //客户名称，暂时补齐做用
        prompt: '请收款人',
        required: false,
    };

    //客户插件对外的方法
    $.fn.clientInvoice.methods = {
        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
        },
        //设置客户名称
        setClientName: function (jq, name) {
            var sender = $(jq);
            sender.combobox('disable');
            sender.combobox('clear');
            sender.combobox('loadData', []);

            $.ajax({
                url: AjaxUrl.getClientInvoice,
                dataType: 'json',
                type: 'GET',
                data: { name: name },
                success: function (json) {
                    initdata = json.Data;
                    sender.combobox('loadData', initdata);
                    sender.data('items', initdata);
                    sender.combobox('enable');
                },
                error: function (err) {
                    alert('clientInvoice.error:' + JSON.stringify(err));
                }
            });



            return sender;
        },
        //设置客户ID
        setClientID: function (jq, id) {
            var sender = $(jq);

            sender.combobox('disable');
            sender.combobox('clear');
            sender.combobox('loadData', []);

            $.ajax({
                url: AjaxUrl.getClientInvoice,
                dataType: 'json',
                type: 'GET',
                data: { id: id },
                success: function (json) {
                    var data = json.Data;
                    sender.combobox('loadData', data);
                    sender.data('items', data);
                    sender.combobox('enable');

                    //清除名字
                    //var name = sender.data('setName');
                    //sender.data('setName', '');
                    //var ogs = $.grep(data, function (item) {
                    //    return item.Name == name;
                    //});
                    //if (ogs.length > 0) {
                    //    sender.combobox('setValue', ogs[0].ID);
                    //}

                },
                error: function (err) {
                    alert('clientInvoice.error:' + JSON.stringify(err));
                }
            });

            return sender;
        },
        //设置值
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            sender.combobox('setText', '');
            return sender;
        },
        //通过名字赋值
        setName: function (jq, text) {
            var sender = $(jq);
            sender.data('setName', text);
            var data = sender.combobox('getData');
            var ogs = $.grep(data, function (item) {
                return item.Name == text;
            });

            if (ogs.length > 0) {
                sender.combobox('setValue', ogs[0].ID);
                sender.combobox('setText', '');
            }
            return sender;
        }


    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('clientInvoice');
})(jQuery)