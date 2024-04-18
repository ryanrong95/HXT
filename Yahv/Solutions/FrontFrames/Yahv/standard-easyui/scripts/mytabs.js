(function ($) {
    //生成随机ID
    function randomID(model) {
        return model + '_' + parseInt(Math.random() * Math.pow(10, 10))
    }
    $.mytabs = {
        addPanel: function (options) {
            var options = $.extend(true, {}, $.mytabs.defaults, options);
            var selector = options.selector;
            var title = options.title;
            var url = options.url;
            var limit = options.limit;

            var indexIdFlag = false;
            var tabsLength = $(selector).tabs('tabs').length;
            if (tabsLength < limit) {
                var id = randomID("tab");
                $(selector).tabs('add', {
                    title: options.title,
                    content: '<div class="easyui-panel" data-options="fit:true,border:false" title="" style="padding: 10px;"><iframe class="workSpace" src="' + url + '" style="width:100%;height:100%;overflow:hidden;"></iframe></div>',
                    id: id,
                    closable: true
                });
            } else {
                $.timeouts.alert({
                    position: "CC",
                    msg: "选项卡不得超过" + limit + "个",
                    type: "warning"
                })
            }
        }
    }
    //默认配置
    $.mytabs.defaults = {
        title: '新页面',
        selector: "body",
        url: "",
        limit: 10
    }
})(jQuery);