/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" />

(function ($) {
    var api = {
        isShowRecord: '/rfqapi/Records/IsShowList'
    };

    $.fn.rfqUserRecord = function (opt, param) {
        //如果options为string，则是方法调用
        if (typeof opt == 'string') {
            var method = $.fn.rfqUserRecord.methods[opt];
            if (method) {
                return method(this, param);//如果有该方法就调用该方法
            } else {
                alert("该插件没有这个方法");
            }
        }

        var options = opt || {};
        options = $.extend(true, {}, $.fn.rfqUserRecord.default, options);

        if (!options.other) {
            //alert('不存在对方名称或是ID!');//也可以选择不 alert ，这里按照标准做
            return;
        }

        return this.each(function () {
            var sender = $(this);
            //创建按钮
            var button = $('<a>');
            sender.hide().after(button);
            button.linkbutton({
                text: options.buttonText,
            });

            $.get(api.isShowRecord, {
                other: options.other
            }, function (result) {
                if (!result.success) {
                    button.linkbutton('disable');
                    button.remove();
                    return;
                }

                //click
                button.click(function () {
                    $.myDialog({
                        title: "备案详情",
                        url: '/RFQ/RecordManagement/FilesList.aspx?id=' + result.data,
                        width: "60%",
                        height: "50%",
                        isHaveOk: false
                    });
                });
            });
        });
    }

    $.fn.rfqUserRecord.default = {
        buttonText: '备案详情',
        other: null //ClientID,对方ID，或是名字
    };

    //将自定义的插件加入 easyui 的插件组
    $.parser.plugins.push('rfqUserRecord');
})(jQuery);