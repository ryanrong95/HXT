// -----------------------------------------------------------------------
// 日志控件
//
// 1.页面定义div容器:
// <div id="ccslogContainer" title="日志记录" class="easyui-panel">
// </div>
// 2.后台Json数据: 需要包含CreateDate和Summary
//this.Model.Logs = new Needs.Ccs.Services.Views.OrderLogsView().Where(log => log.OrderID == "NL02020190420001")
//                  .Select(log => new
//                  {
//                      log.ID,
//                      log.CreateDate,
//                      log.Summary
//                  }).Json();
// 3.控件初始化:
// var logs = eval('(<%=this.Model.Logs%>)');
// $('#ccslogContainer').ccslog({
// data: logs
// });
// -----------------------------------------------------------------------
(function ($) {

    //插件扩展
    $.fn.ccslog = function (options) {
        var settings = $.extend({}, $.fn.ccslog.defaults, options);
        var data = settings.data;

        if (data.length > 0) {
            this.append('<div style="margin:10px">');
            //日志信息
            for (var index = 0; index < data.length; index++) {
                var createDate = new Date(data[index].CreateDate).format('yyyy-MM-dd hh:mm:ss');
                var summary = data[index].Summary;
                this.append('<p style="margin:5px">' + createDate + ' ' + summary + '</p>');
            }
            this.append('</div>');
        }

        return this;
    }

    //默认参数
    $.fn.ccslog.defaults = {
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