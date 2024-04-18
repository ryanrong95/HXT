
/*
* 权限颗粒化配置
* 说明:如果是 easyui-datagrid 列,必须写成 v_name='grid_fieldValue',否则无法识别. field 大小写敏感. 
* grid_后的 fieldValue 是 <th> data-options="field: value", 的 value
* 示例如下:
*    <table id="tab1" class="easyui-dagagrid">
*       <th v_name="grid_RoleName" v_title="权限" data-options="field:RoleName"></th>
*    </table>
*/

; (function ($) {

    /* 默认值 */
    var defaults = {
        menu: '_menu', /* 菜单名称*/
        rawurl: document.URL.replace(/\?.*?$/ig, '').toLowerCase(), /* 当前页面url*/
        actionurl: '/ga.js.aspx?action=roleunites', /* 写入action */
        readurl: '/ga.js.aspx?action=readunites', /* 配置读取action */
        enterSuccess: function () { }, /* callback */
        readSuccess: function () { } /* callback */
    };
    /* 标签属性名称 */
    var uniteLabel = {
        name: 'v_name',
        title: 'v_title'
    };
    /* 配置类型 */
    var roleUniteType = {
        /* 正常 */
        normal: 1,
        /* grid */
        grid: 2
    };

    $.extend({
        roleUnites: function (options) {

            options = options || {};

            var obj = new Object();

            /* 项 */
            obj.options = $.extend({}, defaults, options);

            /* 标签集合 */
            obj.labels = $('[' + uniteLabel.name + ']');

            /* roleUnite 对象集合 */
            obj.options.rawurl = getRelativeUrl(obj.options.rawurl);
            obj.sets = getSets(obj.options.menu, obj.options.rawurl, obj.labels);

            /* 读取入库 */
            obj.enter = function () {
                /* 验证 menu 必需项 */
                if ($.trim(obj.options.menu) == '' || obj.options.menu == defaults.menu) {
                    throw new Error("roleUnites enter options.menu undefined!");
                    return;
                }
                /* 验证 action 必需项 */
                if ($.trim(obj.options.actionurl) == '') {
                    throw new Error("roleUnites enter options.actionurl undefined!");
                    return;
                }
                /* 写入读取的配置项 */
                if (obj.sets.length > 0) {
                    $.ajax({
                        type: 'post',
                        url: obj.options.actionurl,
                        data: { sets: JSON.stringify(obj.sets) },
                        dataType: 'json',
                        success: function (data) {
                            /* callback */
                            obj.options.enterSuccess();
                        }
                    });
                }

                return obj; /* 返回当前对象，支持链式调用 */
            };

            /* 配置 */
            obj.read = function () {
                /* 验证 menu 必需项 */
                if ($.trim(obj.options.menu) == '' || obj.options.menu == defaults.menu) {
                    throw new Error("roleUnites read options.menu undefined!");
                    return;
                }
                /* 验证 action 必需项 */
                if ($.trim(obj.options.actionurl) == '') {
                    throw new Error("roleUnites read options.actionurl undefined!");
                    return;
                }

                $.ajax({
                    type: 'post',
                    url: obj.options.readurl,
                    data: { menu: obj.options.menu, rawurl: obj.options.rawurl },
                    success: function (json) {
                        var arry = JSON.parse(json);
                        if (arry != null || arry.length > 0) {
                            $.each(arry, function (index, item) {
                                if (item.Type == 1) {  /* normal */
                                    $('[' + uniteLabel.name + '="' + item.Name + '"]').hide();
                                }
                                else if (item.Type == 2) { /* easyui-datagrid cloumn */
                                    /* 切割 vname 获取 grid_name_tableid */
                                    var cells = item.Name.split('_');
                                    if ($.inArray('grid', cells) == 0 && cells.length >= 3) {
                                        var tid = cells[cells.length - 1]; /* table 的 id */
                                        /* field 获取方式一 v_name值 中间部分是field 区分大小写 */
                                        var field = cells.length == 3 ? cells[1] : cells.slice(1, cells.length - 2).join("_"); /* 掐头去尾 中间是fieldName */
                                        /* field 获取方式二 field 是直接属性 */
                                        /*
                                        var col = $('[' + uniteLabel.name + '="' + cells.slice(0, cells.length - 1).join('_') + '"]');
                                        if (col) {
                                             field = col.attr("field");
                                        }
                                        */
                                        /* 隐藏列方式一 hidden 失败的*/
                                        /*
                                        $("#" + tid).datagrid('getColumnOption', field).hidden = true;
                                        */

                                        /* 隐藏列方式二 成功的  */
                                        $("#" + tid).datagrid('hideColumn', field);

                                    }
                                }
                            });
                        }
                        /* callback */
                        obj.options.readSuccess();
                    }
                });
            };

            return obj; /* 返回当前对象，支持链式调用 */
        }
    });



    /* 根据标签生成 roleUnite 对象集合 */
    var getSets = function (menu, url, labels) {
        var arry = [];
        labels.each(function () {
            var vname = $(this).attr(uniteLabel.name);
            var vtype = vname.indexOf('grid_') >= 0 ? roleUniteType.grid : roleUniteType.normal; /* 根据 v_name 规范确定类型 */
            /* 把 table id 写到v_name中 */
            if (vtype == roleUniteType.grid) {
                var tid = $(this).parents('table').attr('id');
                vname = vname + '_' + tid;
            }

            arry.push({
                Menu: menu, /* 菜单 */
                Type: vtype, /* 类型 grid 还是正常 */
                Name: vname, /* 标识名称 */
                Title: $(this).attr(uniteLabel.title), /* 标识title */
                Url: url, /* 当前页面url */
            });
        });
        return arry;
    };

    /* 根据标签名称获取类型 已作废 */
    /* var getType = function (vname) {
         var result = 1;
         switch (node.prop("nodeName").toLowerCase()) {
             case 'th':
                 result = 2;
                 break;
                     case 'td':
                     result = 2;
                     break;
             default:
                 result = 1;
                 break;
         }
         return result;
     };
    */

    /*
    * 获取当前相对路径
    * 比如 http://www.xx.com/public/item.aspx?t=110 结果为：/public/item.aspx
    */
    function getRelativeUrl(url) {
        var arrUrl = url.split("//");

        var start = arrUrl[1].indexOf("/");
        var relUrl = arrUrl[1].substring(start);/* stop省略，截取从start开始到结尾的所有字符 */

        if (relUrl.indexOf("?") != -1) {
            relUrl = relUrl.split("?")[0];
        }
        return relUrl;
    }
})(jQuery);


