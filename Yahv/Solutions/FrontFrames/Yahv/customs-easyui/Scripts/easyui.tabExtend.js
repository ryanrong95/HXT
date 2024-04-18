/**
       * @author {kexb} easyui-tab扩展根据id切换页面
       */
$.extend($.fn.tabs.methods, {
    getTabById: function (jq, id) {
        var tabs = $.data(jq[0], 'tabs').tabs;
        for (var i = 0; i < tabs.length; i++) {
            var tab = tabs[i];
            if (tab.panel('options').id == id) {
                return tab;
            }
        }
        return null;
    },
    selectById: function (jq, id) {
        var tab;
        var tabs = $.data(jq[0], 'tabs').tabs;
        for (var i = 0; i < tabs.length; i++) {
            tab = tabs[i];
            if (tab.panel('options').id == id) {
                break;
            }
        }
        if (tab != undefined) {
            var curTabIndex = $("#tabs").tabs("getTabIndex", tab);
            $('#tabs').tabs('select', curTabIndex);
        }
    },
    existsById: function (jq, id) {
        return jq.tabs('getTabById', id) != null;
    }
});