/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />



//企业联系人
(function ($) {
    //接口地址
    var AjaxUrl = {
        getContact: '/crmplus2api/Enterprises/Contacts'
    }
    window.contactEnterpriseID;
    function loadData(cb) {
        $.ajax({
            url: AjaxUrl.getContact,
            dataType: 'json',
            type: 'GET',
            data: { enterpriseID: window.contactEnterpriseID },
            success: function (json) {
                if (cb) {
                    cb(json)
                }
            },
            error: function (err) {
                alert('contactCrmPlus.error:' + JSON.stringify(err));
            }
        });
    }
    //编写contactCrmPlus插件
    $.fn.contactCrmPlus = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.contactCrmPlus.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("contactCrmPlus.该插件没有这个方法：" + opt)
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.contactCrmPlus.defaults, options);

        return this.each(function () {
            var sender = $(this);

            var oldinputContactID = $('#' + sender.attr('textboxname') + 'ID');
            if (oldinputContactID.length > 0) {
                sender.contactCrmPlus('setName', options.Name);
                return sender;
            }

            //保存设置类型
            sender.data('options', options);

            var sender_id = sender.prop('id');
            if (!sender_id) {
                sender_id = 'Contact_sender_' + Math.random().toString().substring(2);
                sender.prop('id', sender_id);
            }
            var formName = sender.prop('name');
            if (!formName) {
                alert('contactCrmPlus.控件name不能为空！2');
            }



            var initdata = [];
            if (options.isAdd) {
                var addButton = $('<a>');
                addButton.click(function () {
                    if (!window.contactEnterpriseID) {
                        alert('无企业');
                        return;
                    }

                    //var olds = sender.combobox('getData');
                    //alert(JSON.stringify(olds));

                    $.myDialogFuse({
                        title: '新增联系人',
                        url: '/CrmPlus/Uss/Contacts/Edit.aspx?id=' + window.contactEnterpriseID,
                        width: '730px',
                        height: '450px',
                        onClose: function () {
                            loadData(function (data) {

                                var lenOld = sender.combobox('getData').length;

                                if (data.length > 0) {
                                    sender.combobox('loadData', data);
                                    sender.data('items', data);
                                    //sender.combobox('enable');
                                    //sender.combobox('value', data[0].ID);
                                }

                                if (lenOld != data.length) {
                                    sender.combobox('setValue', data[data.length - 1].ID);
                                }
                            });
                        }
                    });
                });
                sender.after(addButton);
                addButton.linkbutton({
                    iconCls: 'icon-yg-add',
                    text: '新增',

                });

            }
            sender.combobox({
                valueField: 'ID',
                textField: 'Name',
                formatter: function (row) {
                    var s = '<div>' +
                            '<div><em>姓名:</em>' + row.Name + '</div>' +
                            '<div><em>电话:</em>' + row.Tel + '</div>' +
                            '<div><em>手机号:</em>' + row.Mobile + '</div>' +
                            '<div><em>邮箱:</em>' + row.Email + '</div>' +
                            '</div> ';
                    return s;
                },
                prompt: options.prompt,
                required: options.required,
                value: options.value,
                onSelect: function (record) {
                },
                data: initdata,
                editable: false,

            });


        });
    }

    //企业插件的默认配置
    $.fn.contactCrmPlus.defaults = {
        //value: '', //企业ID
        prompt: '联系人',
        required: false,
        isAdd: false //新增
    };

    //企业插件对外的方法
    $.fn.contactCrmPlus.methods = {
        //获取数据
        getData: function (jq) {
            var sender = $(jq);
            return sender.data('items');
        },

        //设置企业ID
        setEnterpriseID: function (jq, id) {
            var sender = $(jq);

            //sender.combobox('disable');
            sender.combobox('clear');
            sender.combobox('loadData', []);
            window.contactEnterpriseID = id;
            loadData(function (data) {
                if (data.length > 0) {
                    sender.combobox('loadData', data);
                    sender.data('items', data);
                    //sender.combobox('enable');
                    sender.combobox('setValue', data[0].ID);
                }
            })
            return sender;
        },
        //param:{ enterpriseID: '企业ID', contactID: '联系人ID' })
        //企业联系人加载出来后再选中
        loadSetValue: function (jq, param) {
            var sender = $(jq);
            //sender.combobox('disable');
            sender.combobox('clear');
            sender.combobox('loadData', []);
            window.contactEnterpriseID = param.enterpriseID;
            loadData(function (data) {
                sender.combobox('loadData', data);
                sender.data('items', data);
                //sender.combobox('enable');
                var selector = null;
                $.map(data, function (item, index) {
                    if (param.contactID == item.ID) {
                        selector = item;
                    }
                });
                selector == null ? alert('联系人不存在') : sender.combobox('setValue', param.contactID)
            })

            return sender;
        },

        //设置值,数据已加载后，可直接只用
        setValue: function (jq, value) {
            var sender = $(jq);
            sender.combobox('setValue', value);
            return sender;
        },

    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('contactCrmPlus');
})(jQuery)