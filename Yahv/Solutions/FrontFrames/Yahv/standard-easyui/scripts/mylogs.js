// -----------------------------------------------------------------------
/*
    日志控件

    1.页面定义div容器:
    <div id="logContainer" title="日志记录" class="easyui-panel">
    </div>

    2.后台Json数据: 需要包含CreateDate和Summary
    var json = new JSingle<dynamic>()
    {
        code = 200,
        success = true,
        data = logs.ToList().Select(log => new
        {
            log.ID,
            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"),
            log.Summary
        })
    };

    3.控件初始化示例:
    $.ajax({
        url: '/Api/Controller/GetLogs',
        type: 'get',
        data: {
            paramter: 'LM5000SD-6/NOPB',
        },
        dataType: 'json',
        success: function (res) {
            if (res.code == '100') {
                $('#logContainer').append('<div style="margin:10px"><p style="margin:5px">无日志记录</p></div>');
            } else if (res.code == '200') {
                $('#logContainer').mylogs({
                    data: res.data
                });
            } else if (res.code == '300') {
                console.log("接口异常");
            }
        },
        error: function (data) {

        }
    });
*/
// -----------------------------------------------------------------------

(function ($) {

    //插件扩展
    $.fn.mylogs = function (options) {
        var settings = $.extend({}, $.fn.mylogs.defaults, options);
        var data = settings.data;

        if (data.length > 0) {
            this.append('<div style="margin:10px">');
            //日志信息
            for (var index = 0; index < data.length; index++) {
                var createDate = new Date(data[index].CreateDate).format('yyyy-MM-dd hh:mm');
                var summary = data[index].Summary;
                this.append('<p style="margin:5px">' + createDate + ' ' + summary + '</p>');
            }
            this.append('</div>');
        }

        return this;
    }

    //默认参数
    $.fn.mylogs.defaults = {
        data: [],
    };

})(jQuery);

//日期格式化
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