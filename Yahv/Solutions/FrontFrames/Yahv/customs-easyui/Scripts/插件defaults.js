$.fn.layout.defaults.border = false;
$.fn.panel.defaults.border = false;

//validatebox验证密码一致的扩展方法

$.extend($.fn.validatebox.defaults.rules, {
    equalTo: { validator: function (value, param) { return $(param[0]).val() == value; }, message: '字段不匹配' }
});

//阻止默认事件
function stopPropagation(e) {
    if (e.stopPropagation)
        e.stopPropagation();
    else
        e.cancelBubble = true;
}

//form统一调用
$(function () {
    var sender = $('form');
    sender.each(function (index, elem) {
        var subber = $(elem);
        subber.submit(function () {
            //可以接收事件，未来开发
            return subber.form('enableValidation').form('validate');

            //alert(vaild);

            //return false;

            //if (subber.form('enableValidation').form('validate')) {
            //    return true;
            //} else {
            //    return false;
            //}
        });
    });
    //帮助王俊丽触发
    $(document.body).bind('click', function () {
        top.document.body.click();
    });
});

//获取地址参数
function queryString(name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result == null || result.length < 1) {
        return "";
    }
    return result[1];
}

//easyui 控件默认宽高

$.fn.accordion.defaults = {
    width: "auto", height: "auto", fit: false, border: true, animate: true, multiple: false, selected: 0, halign: "top", onSelect: function (_5e, _5f) {
    }, onUnselect: function (_60, _61) {
    }, onAdd: function (_62, _63) {
    }, onBeforeRemove: function (_64, _65) {
    }, onRemove: function (_66, _67) {
    }
};

$.fn.calendar.defaults = {
    Date: Date, width: 180, height: 180, fit: false, border: true, showWeek: false, firstDay: 0, weeks: ["S", "M", "T", "W", "T", "F", "S"], months: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], year: new Date().getFullYear(), month: new Date().getMonth() + 1, current: (function () {
        var d = new Date();
        return new Date(d.getFullYear(), d.getMonth(), d.getDate());
    })(), weekNumberHeader: "", getWeekNumber: function (_45) {
        var _46 = new Date(_45.getTime());
        _46.setDate(_46.getDate() + 4 - (_46.getDay() || 7));
        var _47 = _46.getTime();
        _46.setMonth(0);
        _46.setDate(1);
        return Math.floor(Math.round((_47 - _46) / 86400000) / 7) + 1;
    }, formatter: function (_48) {
        return _48.getDate();
    }, styler: function (_49) {
        return "";
    }, validator: function (_4a) {
        return true;
    }, onSelect: function (_4b) {
    }, onChange: function (_4c, _4d) {
    }, onNavigate: function (_4e, _4f) {
    }
};


$.fn.checkbox.defaults = {
    width: 20, height: 20, value: null, disabled: false, checked: false, label: null, labelWidth: "auto", labelPosition: "before", labelAlign: "left", onChange: function (_2b) {
    }
};


$.fn.combo.defaults = $.extend({}, $.fn.textbox.defaults, {
    inputEvents: { click: _16, keydown: _1b, paste: _1b, drop: _1b, blur: _1f }, panelEvents: {
        mousedown: function (e) {
            e.preventDefault();
            e.stopPropagation();
        }
    },
    panelWidth: null,
    panelHeight: 300,
    panelMinWidth: null,
    panelMaxWidth: null,
    panelMinHeight: null,
    panelMaxHeight: null,
    panelAlign: "left", panelValign: "auto", reversed: false, multiple: false, multivalue: true, selectOnNavigation: true, separator: ",", hasDownArrow: true, delay: 200, keyHandler: {
        up: function (e) {
        }, down: function (e) {
        }, left: function (e) {
        }, right: function (e) {
        }, enter: function (e) {
        }, query: function (q, e) {
        }
    }, onShowPanel: function () {
    }, onHidePanel: function () {
    }, onChange: function (_5b, _5c) {
    }
});


$.fn.combobox.defaults = $.extend({}, $.fn.combo.defaults, {
    valueField: "value",
    textField: "text",
    groupPosition: "static",
    groupField: null,
    groupFormatter: function (_7c) {
        return _7c;
    },
    mode: "local",
    method: "post",
    url: null,
    data: null,
    queryParams: {},
    showItemIcon: false,
    limitToList: false,
    unselectedValues: [],
    mappingRows: [],
    view: _74,
    keyHandler: {
        up: function (e) {
            _b(this, "prev");
            e.preventDefault();
        }, down: function (e) {
            _b(this, "next");
            e.preventDefault();
        }, left: function (e) {
        }, right: function (e) {
        }, enter: function (e) {
            _41(this);
        }, query: function (q, e) {
            _36(this, q);
        }
    },
    inputEvents: $.extend({}, $.fn.combo.defaults.inputEvents, {
        blur: function (e) {
            $.fn.combo.defaults.inputEvents.blur(e);
            var _7d = e.data.target;
            var _7e = $(_7d).combobox("options");
            if (_7e.reversed || _7e.limitToList) {
                if (_7e.blurTimer) {
                    clearTimeout(_7e.blurTimer);
                }
                _7e.blurTimer = setTimeout(function () {
                    var _7f = $(_7d).parent().length;
                    if (_7f) {
                        if (_7e.reversed) {
                            $(_7d).combobox("setValues", $(_7d).combobox("getValues"));
                        } else {
                            if (_7e.limitToList) {
                                var vv = [];
                                $.map($(_7d).combobox("getValues"), function (v) {
                                    var _80 = $.easyui.indexOfArray($(_7d).combobox("getData"), _7e.valueField, v);
                                    if (_80 >= 0) {
                                        vv.push(v);
                                    }
                                });
                                $(_7d).combobox("setValues", vv);
                            }
                        }
                        _7e.blurTimer = null;
                    }
                }, 50);
            }
        }
    }),
    panelEvents: {
        mouseover: _4b, mouseout: _4d, mousedown: function (e) {
            e.preventDefault();
            e.stopPropagation();
        }, click: _4e, scroll: _53
    },
    filter: function (q, row) {
        var _81 = $(this).combobox("options");
        return row[_81.textField].toLowerCase().indexOf(q.toLowerCase()) >= 0;
    },
    formatter: function (row) {
        var _82 = $(this).combobox("options");
        return row[_82.textField];
    },
    loader: function (_83, _84, _85) {
        var _86 = $(this).combobox("options");
        if (!_86.url) {
            return false;
        }
        $.ajax({
            type: _86.method, url: _86.url, data: _83, dataType: "json", success: function (_87) {
                _84(_87);
            }, error: function () {
                _85.apply(this, arguments);
            }
        });
    },
    loadFilter: function (_88) {
        return _88;
    },
    finder: {
        getEl: function (_89, _8a) {
            var _8b = _1(_89, _8a);
            var id = $.data(_89, "combobox").itemIdPrefix + "_" + _8b;
            return $("#" + id);
        }, getGroupEl: function (_8c, _8d) {
            var _8e = $.data(_8c, "combobox");
            var _8f = $.easyui.indexOfArray(_8e.groups, "value", _8d);
            var id = _8e.groupIdPrefix + "_" + _8f;
            return $("#" + id);
        }, getGroup: function (_90, p) {
            var _91 = $.data(_90, "combobox");
            var _92 = p.attr("id").substr(_91.groupIdPrefix.length + 1);
            return _91.groups[parseInt(_92)];
        }, getRow: function (_93, p) {
            var _94 = $.data(_93, "combobox");
            var _95 = (p instanceof $) ? p.attr("id").substr(_94.itemIdPrefix.length + 1) : _1(_93, p);
            return _94.data[parseInt(_95)];
        }
    },
    onBeforeLoad: function (_96) {
    },
    onLoadSuccess: function (_97) {
    },
    onLoadError: function () {
    },
    onSelect: function (_98) {
    },
    onUnselect: function (_99) {
    },
    onClick: function (_9a) {
    }
});


$.fn.combogrid.defaults = $.extend({}, $.fn.combo.defaults, $.fn.datagrid.defaults, {
    loadMsg: null, idField: null, textField: null, unselectedValues: [], mappingRows: [], mode: "local", keyHandler: {
        up: function (e) {
            nav(this, "prev");
            e.preventDefault();
        }, down: function (e) {
            nav(this, "next");
            e.preventDefault();
        }, left: function (e) {
        }, right: function (e) {
        }, enter: function (e) {
            _48(this);
        }, query: function (q, e) {
            _3d(this, q);
        }
    }, inputEvents: $.extend({}, $.fn.combo.defaults.inputEvents, {
        blur: function (e) {
            $.fn.combo.defaults.inputEvents.blur(e);
            var _5a = e.data.target;
            var _5b = $(_5a).combogrid("options");
            if (_5b.reversed) {
                $(_5a).combogrid("setValues", $(_5a).combogrid("getValues"));
            }
        }
    }), panelEvents: {
        mousedown: function (e) {
        }
    }, filter: function (q, row) {
        var _5c = $(this).combogrid("options");
        return (row[_5c.textField] || "").toLowerCase().indexOf(q.toLowerCase()) >= 0;
    }
});


$.fn.combotree.defaults = $.extend({}, $.fn.combo.defaults, $.fn.tree.defaults, {
    editable: false, textField: null, unselectedValues: [], mappingRows: [], keyHandler: {
        up: function (e) {
        }, down: function (e) {
        }, left: function (e) {
        }, right: function (e) {
        }, enter: function (e) {
            _2e(this);
        }, query: function (q, e) {
            _29(this, q);
        }
    }
});


$.fn.combotreegrid.defaults = $.extend({}, $.fn.combo.defaults, $.fn.treegrid.defaults, {
    editable: false, singleSelect: true, limitToGrid: false, unselectedValues: [], mappingRows: [], mode: "local", textField: null, keyHandler: {
        up: function (e) {
        }, down: function (e) {
        }, left: function (e) {
        }, right: function (e) {
        }, enter: function (e) {
            _2a(this);
        }, query: function (q, e) {
            _23(this, q);
        }
    }, inputEvents: $.extend({}, $.fn.combo.defaults.inputEvents, {
        blur: function (e) {
            $.fn.combo.defaults.inputEvents.blur(e);
            var _3a = e.data.target;
            var _3b = $(_3a).combotreegrid("options");
            if (_3b.limitToGrid) {
                _2a(_3a);
            }
        }
    }), filter: function (q, row) {
        var _3c = $(this).combotreegrid("options");
        return (row[_3c.treeField] || "").toLowerCase().indexOf(q.toLowerCase()) >= 0;
    }
});


$.fn.datagrid.defaults = $.extend({}, $.fn.panel.defaults, {
    sharedStyleSheet: false,
    frozenColumns: undefined,
    columns: undefined,
    fitColumns: false,
    resizeHandle: "right",
    resizeEdge: 5,
    autoRowHeight: true,
    toolbar: null,
    striped: false,
    method: "post",
    nowrap: true,
    idField: null,
    url: null,
    data: null,
    loadMsg: "Processing, please wait ...",
    emptyMsg: "",
    rownumbers: false,
    singleSelect: false,
    ctrlSelect: false,
    selectOnCheck: true,
    checkOnSelect: true,
    pagination: false,
    pagePosition: "bottom",
    pageNumber: 1,
    pageSize: 10,
    pageList: [10, 20, 30, 40, 50],
    queryParams: {},
    sortName: null,
    sortOrder: "asc",
    multiSort: false,
    remoteSort: true,
    showHeader: true,
    showFooter: false,
    scrollOnSelect: true,
    scrollbarSize: 18,
    rownumberWidth: 30,
    editorHeight: 31,
    headerEvents: { mouseover: _84(true), mouseout: _84(false), click: _88, dblclick: _8f, contextmenu: _95 },
    rowEvents: { mouseover: _98(true), mouseout: _98(false), click: _a0, dblclick: _ab, contextmenu: _b0 },
    rowStyler: function (_255, _256) {
    },
    loader: function (_257, _258, _259) {
        var opts = $(this).datagrid("options");
        if (!opts.url) {
            return false;
        }
        $.ajax({
            type: opts.method, url: opts.url, data: _257, dataType: "json", success: function (data) {
                _258(data);
            }, error: function () {
                _259.apply(this, arguments);
            }
        });
    },
    loadFilter: function (data) {
        return data;
    },
    editors: _1c6, finder: {
        getTr: function (_25a, _25b, type, _25c) {
            type = type || "body";
            _25c = _25c || 0;
            var _25d = $.data(_25a, "datagrid");
            var dc = _25d.dc;
            var opts = _25d.options;
            if (_25c == 0) {
                var tr1 = opts.finder.getTr(_25a, _25b, type, 1);
                var tr2 = opts.finder.getTr(_25a, _25b, type, 2);
                return tr1.add(tr2);
            } else {
                if (type == "body") {
                    var tr = $("#" + _25d.rowIdPrefix + "-" + _25c + "-" + _25b);
                    if (!tr.length) {
                        tr = (_25c == 1 ? dc.body1 : dc.body2).find(">table>tbody>tr[datagrid-row-index=" + _25b + "]");
                    }
                    return tr;
                } else {
                    if (type == "footer") {
                        return (_25c == 1 ? dc.footer1 : dc.footer2).find(">table>tbody>tr[datagrid-row-index=" + _25b + "]");
                    } else {
                        if (type == "selected") {
                            return (_25c == 1 ? dc.body1 : dc.body2).find(">table>tbody>tr.datagrid-row-selected");
                        } else {
                            if (type == "highlight") {
                                return (_25c == 1 ? dc.body1 : dc.body2).find(">table>tbody>tr.datagrid-row-over");
                            } else {
                                if (type == "checked") {
                                    return (_25c == 1 ? dc.body1 : dc.body2).find(">table>tbody>tr.datagrid-row-checked");
                                } else {
                                    if (type == "editing") {
                                        return (_25c == 1 ? dc.body1 : dc.body2).find(">table>tbody>tr.datagrid-row-editing");
                                    } else {
                                        if (type == "last") {
                                            return (_25c == 1 ? dc.body1 : dc.body2).find(">table>tbody>tr[datagrid-row-index]:last");
                                        } else {
                                            if (type == "allbody") {
                                                return (_25c == 1 ? dc.body1 : dc.body2).find(">table>tbody>tr[datagrid-row-index]");
                                            } else {
                                                if (type == "allfooter") {
                                                    return (_25c == 1 ? dc.footer1 : dc.footer2).find(">table>tbody>tr[datagrid-row-index]");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }, getRow: function (_25e, p) {
            var _25f = (typeof p == "object") ? p.attr("datagrid-row-index") : p;
            return $.data(_25e, "datagrid").data.rows[parseInt(_25f)];
        }, getRows: function (_260) {
            return $(_260).datagrid("getRows");
        }
    }, view: _20f, onBeforeLoad: function (_261) {
    }, onLoadSuccess: function () {
    }, onLoadError: function () {
    }, onClickRow: function (_262, _263) {
    }, onDblClickRow: function (_264, _265) {
    }, onClickCell: function (_266, _267, _268) {
    }, onDblClickCell: function (_269, _26a, _26b) {
    }, onBeforeSortColumn: function (sort, _26c) {
    }, onSortColumn: function (sort, _26d) {
    }, onResizeColumn: function (_26e, _26f) {
    }, onBeforeSelect: function (_270, _271) {
    }, onSelect: function (_272, _273) {
    }, onBeforeUnselect: function (_274, _275) {
    }, onUnselect: function (_276, _277) {
    }, onSelectAll: function (rows) {
    }, onUnselectAll: function (rows) {
    }, onBeforeCheck: function (_278, _279) {
    }, onCheck: function (_27a, _27b) {
    }, onBeforeUncheck: function (_27c, _27d) {
    }, onUncheck: function (_27e, _27f) {
    }, onCheckAll: function (rows) {
    }, onUncheckAll: function (rows) {
    }, onBeforeEdit: function (_280, _281) {
    }, onBeginEdit: function (_282, _283) {
    }, onEndEdit: function (_284, _285, _286) {
    }, onAfterEdit: function (_287, _288, _289) {
    }, onCancelEdit: function (_28a, _28b) {
    }, onHeaderContextMenu: function (e, _28c) {
    }, onRowContextMenu: function (e, _28d, _28e) {
    }
});


$.fn.datalist.defaults = $.extend({}, $.fn.datagrid.defaults, {
    fitColumns: true, singleSelect: true, showHeader: false, checkbox: false, lines: false, valueField: "value", textField: "text", groupField: "", view: _7, textFormatter: function (_2c, row) {
        return _2c;
    }, groupFormatter: function (_2d, _2e) {
        return _2d;
    }
});



$.fn.datebox.defaults = $.extend({}, $.fn.combo.defaults, {
    panelWidth: 250, panelHeight: "auto", sharedCalendar: null, keyHandler: {
        up: function (e) {
        }, down: function (e) {
        }, left: function (e) {
        }, right: function (e) {
        }, enter: function (e) {
            _19(this);
        }, query: function (q, e) {
            _16(this, q);
        }
    }, currentText: "Today", closeText: "Close", okText: "Ok", buttons: [{
        text: function (_30) {
            return $(_30).datebox("options").currentText;
        }, handler: function (_31) {
            var _32 = $(_31).datebox("options");
            var now = new Date();
            var _33 = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            $(_31).datebox("calendar").calendar({ year: _33.getFullYear(), month: _33.getMonth() + 1, current: _33 });
            _32.onSelect.call(_31, _33);
            _19(_31);
        }
    }, {
        text: function (_34) {
            return $(_34).datebox("options").closeText;
        }, handler: function (_35) {
            $(this).closest("div.combo-panel").panel("close");
        }
    }], formatter: function (_36) {
        var y = _36.getFullYear();
        var m = _36.getMonth() + 1;
        var d = _36.getDate();
        return (m < 10 ? ("0" + m) : m) + "/" + (d < 10 ? ("0" + d) : d) + "/" + y;
    }, parser: function (s) {
        var _37 = $(this).datebox("calendar").calendar("options");
        if (!s) {
            return new _37.Date();
        }
        var ss = s.split("/");
        var m = parseInt(ss[0], 10);
        var d = parseInt(ss[1], 10);
        var y = parseInt(ss[2], 10);
        if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
            return new _37.Date(y, m - 1, d);
        } else {
            return new _37.Date();
        }
    }, onSelect: function (_38) {
    }
});

$.fn.datetimebox.defaults = $.extend({}, $.fn.datebox.defaults, {
    spinnerWidth: "100%", showSeconds: true, timeSeparator: ":", hour12: false, panelEvents: {
        mousedown: function (e) {
        }
    }, keyHandler: {
        up: function (e) {
        }, down: function (e) {
        }, left: function (e) {
        }, right: function (e) {
        }, enter: function (e) {
            _e(this);
        }, query: function (q, e) {
            _b(this, q);
        }
    }, buttons: [{
        text: function (_27) {
            return $(_27).datetimebox("options").currentText;
        }, handler: function (_28) {
            var _29 = $(_28).datetimebox("options");
            _d(_28, _29.formatter.call(_28, new Date()));
            $(_28).datetimebox("hidePanel");
        }
    }, {
        text: function (_2a) {
            return $(_2a).datetimebox("options").okText;
        }, handler: function (_2b) {
            _e(_2b);
        }
    }, {
        text: function (_2c) {
            return $(_2c).datetimebox("options").closeText;
        }, handler: function (_2d) {
            $(_2d).datetimebox("hidePanel");
        }
    }], formatter: function (_2e) {
        if (!_2e) {
            return "";
        }
        return $.fn.datebox.defaults.formatter.call(this, _2e) + " " + $.fn.timespinner.defaults.formatter.call($(this).datetimebox("spinner")[0], _2e);
    }, parser: function (s) {
        s = $.trim(s);
        if (!s) {
            return new Date();
        }
        var dt = s.split(" ");
        var _2f = $.fn.datebox.defaults.parser.call(this, dt[0]);
        if (dt.length < 2) {
            return _2f;
        }
        var _30 = $.fn.timespinner.defaults.parser.call($(this).datetimebox("spinner")[0], dt[1] + (dt[2] ? " " + dt[2] : ""));
        return new Date(_2f.getFullYear(), _2f.getMonth(), _2f.getDate(), _30.getHours(), _30.getMinutes(), _30.getSeconds());
    }
});

$.fn.datetimespinner.defaults = $.extend({}, $.fn.timespinner.defaults, {
    formatter: function (_a) {
        if (!_a) {
            return "";
        }
        return $.fn.datebox.defaults.formatter.call(this, _a) + " " + $.fn.timespinner.defaults.formatter.call(this, _a);
    }, parser: function (s) {
        s = $.trim(s);
        if (!s) {
            return null;
        }
        var dt = s.split(" ");
        var _b = $.fn.datebox.defaults.parser.call(this, dt[0]);
        if (dt.length < 2) {
            return _b;
        }
        var _c = $.fn.timespinner.defaults.parser.call(this, dt[1] + (dt[2] ? " " + dt[2] : ""));
        return new Date(_b.getFullYear(), _b.getMonth(), _b.getDate(), _c.getHours(), _c.getMinutes(), _c.getSeconds());
    }, selections: [[0, 2], [3, 5], [6, 10], [11, 13], [14, 16], [17, 19], [20, 22]]
});


$.fn.dialog.defaults = $.extend({}, $.fn.window.defaults, { title: "New Dialog", collapsible: false, minimizable: false, maximizable: false, resizable: false, toolbar: null, buttons: null });


$.fn.draggable.defaults = {
    proxy: null, revert: false, cursor: "move", deltaX: null, deltaY: null, handle: null, disabled: false, edge: 0, axis: null, delay: 100, onBeforeDrag: function (e) {
    }, onStartDrag: function (e) {
    }, onDrag: function (e) {
    }, onEndDrag: function (e) {
    }, onStopDrag: function (e) {
    }
};

$.fn.droppable.defaults = {
    accept: null, disabled: false, onDragEnter: function (e, _b) {
    }, onDragOver: function (e, _c) {
    }, onDragLeave: function (e, _d) {
    }, onDrop: function (e, _e) {
    }
};

$.fn.filebox.defaults = $.extend({}, $.fn.textbox.defaults, { buttonIcon: null, buttonText: "Choose File", buttonAlign: "right", inputEvents: {}, accept: "", capture: "", separator: ",", multiple: false });


$.fn.form.defaults = {
    fieldTypes: ["tagbox", "combobox", "combotree", "combogrid", "combotreegrid", "datetimebox", "datebox", "combo", "datetimespinner", "timespinner", "numberspinner", "spinner", "slider", "searchbox", "numberbox", "passwordbox", "filebox", "textbox", "switchbutton", "radiobutton", "checkbox"], novalidate: false, ajax: true, iframe: true, dirty: false, dirtyFields: [], url: null, queryParams: {}, onSubmit: function (_55) {
        return $(this).form("validate");
    }, onProgress: function (_56) {
    }, success: function (_57) {
    }, onBeforeLoad: function (_58) {
    }, onLoadSuccess: function (_59) {
    }, onLoadError: function () {
    }, onChange: function (_5a) {
    }
};

$.fn.layout.defaults = {
    fit: false, onExpand: function (_72) {
    }, onCollapse: function (_73) {
    }, onAdd: function (_74) {
    }, onRemove: function (_75) {
    }
};

$.fn.layout.paneldefaults = $.extend({}, $.fn.panel.defaults, {
    region: null,
    split: false,
    collapsedSize: 32,
    expandMode: "float",
    hideExpandTool: false,
    hideCollapsedContent: true,
    collapsedContent: function (_77) {
        var p = $(this);
        var _78 = p.panel("options");
        if (_78.region == "north" || _78.region == "south") {
            return _77;
        }
        var cc = [];
        if (_78.iconCls) {
            cc.push("<div class=\"panel-icon " + _78.iconCls + "\"></div>");
        }
        cc.push("<div class=\"panel-title layout-expand-title");
        cc.push(" layout-expand-title-" + _78.titleDirection);
        cc.push(_78.iconCls ? " layout-expand-with-icon" : "");
        cc.push("\">");
        cc.push(_77);
        cc.push("</div>");
        return cc.join("");
    },
    minWidth: 10,
    minHeight: 10,
    maxWidth: 10000,
    maxHeight: 10000
});

$.fn.linkbutton.defaults = {
    id: null, disabled: false, toggle: false, selected: false, outline: false, group: null, plain: false, text: "", iconCls: null, iconAlign: "left", size: "small", onClick: function () {
    }
};

$.fn.maskedbox.defaults = $.extend({}, $.fn.textbox.defaults, { mask: "", promptChar: "_", masks: { "9": "[0-9]", "a": "[a-zA-Z]", "*": "[0-9a-zA-Z]" }, inputEvents: { keydown: _26 } });

$.fn.menu.defaults = {
    zIndex: 110000, left: 0, top: 0, alignTo: null, align: "left", minWidth: 150, itemHeight: 32, duration: 100, hideOnUnhover: true, inline: false, fit: false, noline: false, events: { mouseenter: _25, mouseleave: _28, mouseover: _2c, mouseout: _30, click: _33 }, position: function (_77, _78, top) {
        return { left: _78, top: top };
    }, onShow: function () {
    }, onHide: function () {
    }, onClick: function (_79) {
    }
};

$.fn.menubutton.defaults = $.extend({}, $.fn.linkbutton.defaults, { plain: true, hasDownArrow: true, menu: null, menuAlign: "left", duration: 100, showEvent: "mouseenter", hideEvent: "mouseleave", cls: { btn1: "m-btn-active", btn2: "m-btn-plain-active", arrow: "m-btn-downarrow", trigger: "m-btn" } });


$.messager.defaults = $.extend({}, $.fn.dialog.defaults, {
    ok: "Ok", cancel: "Cancel", width: 300, height: "auto", minHeight: 150, modal: true, collapsible: false, minimizable: false, maximizable: false, resizable: false, fn: function () {
    }
});


$.fn.numberbox.defaults = $.extend({}, $.fn.textbox.defaults, {
    inputEvents: {
        keypress: function (e) {
            var _17 = e.data.target;
            var _18 = $(_17).numberbox("options");
            return _18.filter.call(_17, e);
        }, blur: function (e) {
            $(e.data.target).numberbox("fix");
        }, keydown: function (e) {
            if (e.keyCode == 13) {
                $(e.data.target).numberbox("fix");
            }
        }
    }, min: null, max: null, precision: 0, decimalSeparator: ".", groupSeparator: "", prefix: "", suffix: "", filter: function (e) {
        var _19 = $(this).numberbox("options");
        var s = $(this).numberbox("getText");
        if (e.metaKey || e.ctrlKey) {
            return true;
        }
        if ($.inArray(String(e.which), ["46", "8", "13", "0"]) >= 0) {
            return true;
        }
        var tmp = $("<span></span>");
        tmp.html(String.fromCharCode(e.which));
        var c = tmp.text();
        tmp.remove();
        if (!c) {
            return true;
        }
        if (c == "-" || c == _19.decimalSeparator) {
            return (s.indexOf(c) == -1) ? true : false;
        } else {
            if (c == _19.groupSeparator) {
                return true;
            } else {
                if ("0123456789".indexOf(c) >= 0) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }, formatter: function (_1a) {
        if (!_1a) {
            return _1a;
        }
        _1a = _1a + "";
        var _1b = $(this).numberbox("options");
        var s1 = _1a, s2 = "";
        var _1c = _1a.indexOf(".");
        if (_1c >= 0) {
            s1 = _1a.substring(0, _1c);
            s2 = _1a.substring(_1c + 1, _1a.length);
        }
        if (_1b.groupSeparator) {
            var p = /(\d+)(\d{3})/;
            while (p.test(s1)) {
                s1 = s1.replace(p, "$1" + _1b.groupSeparator + "$2");
            }
        }
        if (s2) {
            return _1b.prefix + s1 + _1b.decimalSeparator + s2 + _1b.suffix;
        } else {
            return _1b.prefix + s1 + _1b.suffix;
        }
    }, parser: function (s) {
        s = s + "";
        var _1d = $(this).numberbox("options");
        if (_1d.prefix) {
            s = $.trim(s.replace(new RegExp("\\" + $.trim(_1d.prefix), "g"), ""));
        }
        if (_1d.suffix) {
            s = $.trim(s.replace(new RegExp("\\" + $.trim(_1d.suffix), "g"), ""));
        }
        if (parseFloat(s) != _1d.value) {
            if (_1d.groupSeparator) {
                s = $.trim(s.replace(new RegExp("\\" + _1d.groupSeparator, "g"), ""));
            }
            if (_1d.decimalSeparator) {
                s = $.trim(s.replace(new RegExp("\\" + _1d.decimalSeparator, "g"), "."));
            }
            s = s.replace(/\s/g, "");
        }
        var val = parseFloat(s).toFixed(_1d.precision);
        if (isNaN(val)) {
            val = "";
        } else {
            if (typeof (_1d.min) == "number" && val < _1d.min) {
                val = _1d.min.toFixed(_1d.precision);
            } else {
                if (typeof (_1d.max) == "number" && val > _1d.max) {
                    val = _1d.max.toFixed(_1d.precision);
                }
            }
        }
        return val;
    }
});

$.fn.numberspinner.defaults = $.extend({}, $.fn.spinner.defaults, $.fn.numberbox.defaults, {
    spin: function (_e) {
        _4(this, _e);
    }
});

$.fn.pagination.defaults = {
    total: 1, pageSize: 10, pageNumber: 1, pageList: [10, 20, 30, 50], loading: false, buttons: null, showPageList: true, showPageInfo: true, showRefresh: true, links: 10, layout: ["list", "sep", "first", "prev", "sep", "manual", "sep", "next", "last", "sep", "refresh", "info"], onSelectPage: function (_29, _2a) {
    }, onBeforeRefresh: function (_2b, _2c) {
    }, onRefresh: function (_2d, _2e) {
    }, onChangePageSize: function (_2f) {
    }, beforePageText: "Page", afterPageText: "of {pages}", displayMsg: "Displaying {from} to {to} of {total} items", nav: {
        first: {
            iconCls: "pagination-first", handler: function () {
                var _30 = $(this).pagination("options");
                if (_30.pageNumber > 1) {
                    $(this).pagination("select", 1);
                }
            }
        }, prev: {
            iconCls: "pagination-prev", handler: function () {
                var _31 = $(this).pagination("options");
                if (_31.pageNumber > 1) {
                    $(this).pagination("select", _31.pageNumber - 1);
                }
            }
        }, next: {
            iconCls: "pagination-next", handler: function () {
                var _32 = $(this).pagination("options");
                var _33 = Math.ceil(_32.total / _32.pageSize);
                if (_32.pageNumber < _33) {
                    $(this).pagination("select", _32.pageNumber + 1);
                }
            }
        }, last: {
            iconCls: "pagination-last", handler: function () {
                var _34 = $(this).pagination("options");
                var _35 = Math.ceil(_34.total / _34.pageSize);
                if (_34.pageNumber < _35) {
                    $(this).pagination("select", _35);
                }
            }
        }, refresh: {
            iconCls: "pagination-refresh", handler: function () {
                var _36 = $(this).pagination("options");
                if (_36.onBeforeRefresh.call(this, _36.pageNumber, _36.pageSize) != false) {
                    $(this).pagination("select", _36.pageNumber);
                    _36.onRefresh.call(this, _36.pageNumber, _36.pageSize);
                }
            }
        }
    }
};

$.fn.panel.defaults = {
    id: null, title: null, iconCls: null, width: "auto", height: "auto", left: null, top: null, cls: null, headerCls: null, bodyCls: null, style: {}, href: null, cache: true, fit: false, border: true, doSize: true, noheader: false, content: null, halign: "top", titleDirection: "down", collapsible: false, minimizable: false, maximizable: false, closable: false, collapsed: false, minimized: false, maximized: false, closed: false, openAnimation: false, openDuration: 400, closeAnimation: false, closeDuration: 400, tools: null, footer: null, header: null, queryParams: {}, method: "get", href: null, loadingMessage: "Loading...", loader: function (_85, _86, _87) {
        var _88 = $(this).panel("options");
        if (!_88.href) {
            return false;
        }
        $.ajax({
            type: _88.method, url: _88.href, cache: false, data: _85, dataType: "html", success: function (_89) {
                _86(_89);
            }, error: function () {
                _87.apply(this, arguments);
            }
        });
    }, extractor: function (_8a) {
        var _8b = /<body[^>]*>((.|[\n\r])*)<\/body>/im;
        var _8c = _8b.exec(_8a);
        if (_8c) {
            return _8c[1];
        } else {
            return _8a;
        }
    }, onBeforeLoad: function (_8d) {
    }, onLoad: function () {
    }, onLoadError: function () {
    }, onBeforeOpen: function () {
    }, onOpen: function () {
    }, onBeforeClose: function () {
    }, onClose: function () {
    }, onBeforeDestroy: function () {
    }, onDestroy: function () {
    }, onResize: function (_8e, _8f) {
    }, onMove: function (_90, top) {
    }, onMaximize: function () {
    }, onRestore: function () {
    }, onMinimize: function () {
    }, onBeforeCollapse: function () {
    }, onBeforeExpand: function () {
    }, onCollapse: function () {
    }, onExpand: function () {
    }
};

$.fn.passwordbox.defaults = $.extend({}, $.fn.textbox.defaults, {
    passwordChar: "%u25CF", checkInterval: 200, lastDelay: 500, revealed: false, showEye: true, inputEvents: {
        focus: _14, blur: _19, keydown: function (e) {
            var _24 = $(e.data.target).data("passwordbox");
            return !_24.converting;
        }
    }, val: function (_25) {
        return $(_25).parent().prev().passwordbox("getValue");
    }
});


$.fn.progressbar.defaults = {
    width: "auto", height: 22, value: 0, text: "{value}%", onChange: function (_13, _14) {
    }
};

$.fn.propertygrid.defaults = $.extend({}, $.fn.datagrid.defaults, {
    groupHeight: 28, expanderWidth: 20, singleSelect: true, remoteSort: false, fitColumns: true, loadMsg: "", frozenColumns: [[{ field: "f", width: 20, resizable: false }]], columns: [[{ field: "name", title: "Name", width: 100, sortable: true }, { field: "value", title: "Value", width: 100, resizable: false }]], showGroup: false, groupView: _18, groupField: "group", groupStyler: function (_80, _81) {
        return "";
    }, groupFormatter: function (_82, _83) {
        return _82;
    }
});

$.fn.radiobutton.defaults = {
    width: 20, height: 20, value: null, disabled: false, checked: false, label: null, labelWidth: "auto", labelPosition: "before", labelAlign: "left", onChange: function (_2b) {
    }
};

$.fn.resizable.defaults = {
    disabled: false, handles: "n, e, s, w, ne, se, sw, nw, all", minWidth: 10, minHeight: 10, maxWidth: 10000, maxHeight: 10000, edge: 5, onStartResize: function (e) {
    }, onResize: function (e) {
    }, onStopResize: function (e) {
    }
};

$.fn.searchbox.defaults = $.extend({}, $.fn.textbox.defaults, {
    inputEvents: $.extend({}, $.fn.textbox.defaults.inputEvents, {
        keydown: function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                var t = $(e.data.target);
                var _1b = t.searchbox("options");
                t.searchbox("setValue", $(this).val());
                _1b.searcher.call(e.data.target, t.searchbox("getValue"), t.searchbox("getName"));
                return false;
            }
        }
    }), buttonAlign: "left", menu: null, searcher: function (_1c, _1d) {
    }
});

$.fn.sidemenu.defaults = {
    width: 200, height: "auto", border: true, animate: true, multiple: true, collapsed: false, data: null, floatMenuWidth: 200, floatMenuPosition: "right", onSelect: function (_3f) {
    }
};

$.fn.slider.defaults = {
    width: "auto", height: "auto", mode: "h", reversed: false, showTip: false, disabled: false, range: false, value: 0, separator: ",", min: 0, max: 100, step: 1, rule: [], tipFormatter: function (_4d) {
        return _4d;
    }, converter: {
        toPosition: function (_4e, _4f) {
            var _50 = $(this).slider("options");
            var p = (_4e - _50.min) / (_50.max - _50.min) * _4f;
            return p;
        }, toValue: function (pos, _51) {
            var _52 = $(this).slider("options");
            var v = _52.min + (_52.max - _52.min) * (pos / _51);
            return v;
        }
    }, onChange: function (_53, _54) {
    }, onSlideStart: function (_55) {
    }, onSlideEnd: function (_56) {
    }, onComplete: function (_57) {
    }
};

$.fn.spinner.defaults = $.extend({}, $.fn.textbox.defaults, {
    min: null, max: null, increment: 1, spinAlign: "right", reversed: false, spin: function (_1d) {
    }, onSpinUp: function () {
    }, onSpinDown: function () {
    }
});

$.fn.splitbutton.defaults = $.extend({}, $.fn.linkbutton.defaults, {
    plain: true, menu: null, duration: 100, cls: {
        btn1: "m-btn-active s-btn-active",
        btn2: "m-btn-plain-active s-btn-plain-active",
        arrow: "m-btn-downarrow",
        trigger: "m-btn-line"
    }
});

$.fn.switchbutton.defaults = {
    handleWidth: "auto", width: 60, height: 30, checked: false, disabled: false, readonly: false, reversed: false, onText: "ON", offText: "OFF", handleText: "", value: "on", label: null, labelWidth: "auto", labelPosition: "before", labelAlign: "left", onChange: function (_3c) {
    }
};

$.fn.tabs.defaults = {
    width: "auto", height: "auto", headerWidth: 150, tabWidth: "auto", tabHeight: 32, selected: 0, showHeader: true, plain: false, fit: false, border: true, justified: false, narrow: false, pill: false, tools: null, toolPosition: "right", tabPosition: "top", scrollIncrement: 100, scrollDuration: 400, onLoad: function (_9e) {
    }, onSelect: function (_9f, _a0) {
    }, onUnselect: function (_a1, _a2) {
    }, onBeforeClose: function (_a3, _a4) {
    }, onClose: function (_a5, _a6) {
    }, onAdd: function (_a7, _a8) {
    }, onUpdate: function (_a9, _aa) {
    }, onContextMenu: function (e, _ab, _ac) {
    }
};

$.fn.tagbox.defaults = $.extend({}, $.fn.combobox.defaults, {
    hasDownArrow: false, multiple: true, reversed: true, selectOnNavigation: false, tipOptions: $.extend({}, $.fn.textbox.defaults.tipOptions, { showDelay: 200 }), val: function (_3d) {
        var vv = $(_3d).parent().prev().tagbox("getValues");
        if ($(_3d).is(":focus")) {
            vv.push($(_3d).val());
        }
        return vv.join(",");
    }, inputEvents: $.extend({}, $.fn.combo.defaults.inputEvents, {
        blur: function (e) {
            var _3e = e.data.target;
            var _3f = $(_3e).tagbox("options");
            if (_3f.limitToList) {
                _2c(_3e);
            }
        }
    }), keyHandler: $.extend({}, $.fn.combobox.defaults.keyHandler, {
        enter: function (e) {
            _2c(this);
        }, query: function (q, e) {
            var _40 = $(this).tagbox("options");
            if (_40.limitToList) {
                $.fn.combobox.defaults.keyHandler.query.call(this, q, e);
            } else {
                $(this).combobox("hidePanel");
            }
        }
    }), tagFormatter: function (_41, row) {
        var _42 = $(this).tagbox("options");
        return row ? row[_42.textField] : _41;
    }, tagStyler: function (_43, row) {
        return "";
    }, onClickTag: function (_44) {
    }, onBeforeRemoveTag: function (_45) {
    }, onRemoveTag: function (_46) {
    }
});

$.fn.textbox.defaults = $.extend({}, $.fn.validatebox.defaults, {
    doSize: true,
    width: "auto",
    height: "auto",
    cls: null,
    prompt: "",
    value: "",
    type: "text",
    multiline: false,
    icons: [],
    iconCls: null,
    iconAlign: "right",
    iconWidth: 26,
    buttonText: "",
    buttonIcon: null,
    buttonAlign: "right",
    label: null,
    labelWidth: "auto",
    labelPosition: "before",
    labelAlign: "left",
    inputEvents: {
        blur: function (e) {
            var t = $(e.data.target);
            var _6a = t.textbox("options");
            if (t.textbox("getValue") != _6a.value) {
                t.textbox("setValue", _6a.value);
            }
        }, keydown: function (e) {
            if (e.keyCode == 13) {
                var t = $(e.data.target);
                t.textbox("setValue", t.textbox("getText"));
            }
        }
    }, onChange: function (_6b, _6c) {
    }, onResizing: function (_6d, _6e) {
    }, onResize: function (_6f, _70) {
    }, onClickButton: function () {
    }, onClickIcon: function (_71) {
    }
});

$.fn.timespinner.defaults = $.extend({}, $.fn.spinner.defaults, {
    inputEvents: $.extend({}, $.fn.spinner.defaults.inputEvents, {
        click: function (e) {
            _5.call(this, e);
        }, blur: function (e) {
            var t = $(e.data.target);
            t.timespinner("setValue", t.timespinner("getText"));
        }, keydown: function (e) {
            if (e.keyCode == 13) {
                var t = $(e.data.target);
                t.timespinner("setValue", t.timespinner("getText"));
            }
        }
    }), formatter: function (_26) {
        if (!_26) {
            return "";
        }
        var _27 = $(this).timespinner("options");
        var _28 = _26.getHours();
        var _29 = _26.getMinutes();
        var _2a = _26.getSeconds();
        var _2b = "";
        if (_27.hour12) {
            _2b = _28 >= 12 ? _27.ampm[1] : _27.ampm[0];
            _28 = _28 % 12;
            if (_28 == 0) {
                _28 = 12;
            }
        }
        var tt = [_2c(_28), _2c(_29)];
        if (_27.showSeconds) {
            tt.push(_2c(_2a));
        }
        var s = tt.join(_27.separator) + " " + _2b;
        return $.trim(s);
        function _2c(_2d) {
            return (_2d < 10 ? "0" : "") + _2d;
        };
    }, parser: function (s) {
        var _2e = $(this).timespinner("options");
        var _2f = _30(s);
        if (_2f) {
            var min = _30(_2e.min);
            var max = _30(_2e.max);
            if (min && min > _2f) {
                _2f = min;
            }
            if (max && max < _2f) {
                _2f = max;
            }
        }
        return _2f;
        function _30(s) {
            if (!s) {
                return null;
            }
            var ss = s.split(" ");
            var tt = ss[0].split(_2e.separator);
            var _31 = parseInt(tt[0], 10) || 0;
            var _32 = parseInt(tt[1], 10) || 0;
            var _33 = parseInt(tt[2], 10) || 0;
            if (_2e.hour12) {
                var _34 = ss[1];
                if (_34 == _2e.ampm[1] && _31 < 12) {
                    _31 += 12;
                } else {
                    if (_34 == _2e.ampm[0] && _31 == 12) {
                        _31 -= 12;
                    }
                }
            }
            return new Date(1900, 0, 0, _31, _32, _33);
        };
    }, selections: [[0, 2], [3, 5], [6, 8], [9, 11]], separator: ":", showSeconds: false, highlight: 0, hour12: false, ampm: ["AM", "PM"], spin: function (_35) {
        _14(this, _35);
    }
});


$.fn.tooltip.defaults = {
    position: "bottom", valign: "middle", content: null, trackMouse: false, deltaX: 0, deltaY: 0, showEvent: "mouseenter", hideEvent: "mouseleave", showDelay: 200, hideDelay: 100, onShow: function (e) {
    }, onHide: function (e) {
    }, onUpdate: function (_2d) {
    }, onPosition: function (_2e, top) {
    }, onDestroy: function () {
    }
};


$.fn.tree.defaults = {
    url: null, method: "post", animate: false, checkbox: false, cascadeCheck: true, onlyLeafCheck: false, lines: false, dnd: false, editorHeight: 26, data: null, queryParams: {}, formatter: function (node) {
        return node.text;
    }, filter: function (q, node) {
        var qq = [];
        $.map($.isArray(q) ? q : [q], function (q) {
            q = $.trim(q);
            if (q) {
                qq.push(q);
            }
        });
        for (var i = 0; i < qq.length; i++) {
            var _12c = node.text.toLowerCase().indexOf(qq[i].toLowerCase());
            if (_12c >= 0) {
                return true;
            }
        }
        return !qq.length;
    }, loader: function (_12d, _12e, _12f) {
        var opts = $(this).tree("options");
        if (!opts.url) {
            return false;
        }
        $.ajax({
            type: opts.method, url: opts.url, data: _12d, dataType: "json", success: function (data) {
                _12e(data);
            }, error: function () {
                _12f.apply(this, arguments);
            }
        });
    }, loadFilter: function (data, _130) {
        return data;
    }, view: _120, onBeforeLoad: function (node, _131) {
    }, onLoadSuccess: function (node, data) {
    }, onLoadError: function () {
    }, onClick: function (node) {
    }, onDblClick: function (node) {
    }, onBeforeExpand: function (node) {
    }, onExpand: function (node) {
    }, onBeforeCollapse: function (node) {
    }, onCollapse: function (node) {
    }, onBeforeCheck: function (node, _132) {
    }, onCheck: function (node, _133) {
    }, onBeforeSelect: function (node) {
    }, onSelect: function (node) {
    }, onContextMenu: function (e, node) {
    }, onBeforeDrag: function (node) {
    }, onStartDrag: function (node) {
    }, onStopDrag: function (node) {
    }, onDragEnter: function (_134, _135) {
    }, onDragOver: function (_136, _137) {
    }, onDragLeave: function (_138, _139) {
    }, onBeforeDrop: function (_13a, _13b, _13c) {
    }, onDrop: function (_13d, _13e, _13f) {
    }, onBeforeEdit: function (node) {
    }, onAfterEdit: function (node) {
    }, onCancelEdit: function (node) {
    }
};

$.fn.treegrid.defaults = $.extend({}, $.fn.datagrid.defaults, {
    treeField: null, checkbox: false, cascadeCheck: true, onlyLeafCheck: false, lines: false, animate: false, singleSelect: true, view: _e6, rowEvents: $.extend({}, $.fn.datagrid.defaults.rowEvents, { mouseover: _22(true), mouseout: _22(false), click: _24 }), loader: function (_128, _129, _12a) {
        var opts = $(this).treegrid("options");
        if (!opts.url) {
            return false;
        }
        $.ajax({
            type: opts.method, url: opts.url, data: _128, dataType: "json", success: function (data) {
                _129(data);
            }, error: function () {
                _12a.apply(this, arguments);
            }
        });
    }, loadFilter: function (data, _12b) {
        return data;
    }, finder: {
        getTr: function (_12c, id, type, _12d) {
            type = type || "body";
            _12d = _12d || 0;
            var dc = $.data(_12c, "datagrid").dc;
            if (_12d == 0) {
                var opts = $.data(_12c, "treegrid").options;
                var tr1 = opts.finder.getTr(_12c, id, type, 1);
                var tr2 = opts.finder.getTr(_12c, id, type, 2);
                return tr1.add(tr2);
            } else {
                if (type == "body") {
                    var tr = $("#" + $.data(_12c, "datagrid").rowIdPrefix + "-" + _12d + "-" + id);
                    if (!tr.length) {
                        tr = (_12d == 1 ? dc.body1 : dc.body2).find("tr[node-id=\"" + id + "\"]");
                    }
                    return tr;
                } else {
                    if (type == "footer") {
                        return (_12d == 1 ? dc.footer1 : dc.footer2).find("tr[node-id=\"" + id + "\"]");
                    } else {
                        if (type == "selected") {
                            return (_12d == 1 ? dc.body1 : dc.body2).find("tr.datagrid-row-selected");
                        } else {
                            if (type == "highlight") {
                                return (_12d == 1 ? dc.body1 : dc.body2).find("tr.datagrid-row-over");
                            } else {
                                if (type == "checked") {
                                    return (_12d == 1 ? dc.body1 : dc.body2).find("tr.datagrid-row-checked");
                                } else {
                                    if (type == "last") {
                                        return (_12d == 1 ? dc.body1 : dc.body2).find("tr:last[node-id]");
                                    } else {
                                        if (type == "allbody") {
                                            return (_12d == 1 ? dc.body1 : dc.body2).find("tr[node-id]");
                                        } else {
                                            if (type == "allfooter") {
                                                return (_12d == 1 ? dc.footer1 : dc.footer2).find("tr[node-id]");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }, getRow: function (_12e, p) {
            var id = (typeof p == "object") ? p.attr("node-id") : p;
            return $(_12e).treegrid("find", id);
        }, getRows: function (_12f) {
            return $(_12f).treegrid("getChildren");
        }
    }, onBeforeLoad: function (row, _130) {
    }, onLoadSuccess: function (row, data) {
    }, onLoadError: function () {
    }, onBeforeCollapse: function (row) {
    }, onCollapse: function (row) {
    }, onBeforeExpand: function (row) {
    }, onExpand: function (row) {
    }, onClickRow: function (row) {
    }, onDblClickRow: function (row) {
    }, onClickCell: function (_131, row) {
    }, onDblClickCell: function (_132, row) {
    }, onContextMenu: function (e, row) {
    }, onBeforeEdit: function (row) {
    }, onAfterEdit: function (row, _133) {
    }, onCancelEdit: function (row) {
    }, onBeforeCheckNode: function (row, _134) {
    }, onCheckNode: function (row, _135) {
    }
});

$.fn.validatebox.defaults = {
    required: false,
    validType: null,
    validParams: null,
    delay: 200,
    interval: 200,
    missingMessage: "This field is required.",
    invalidMessage: null,
    tipPosition: "right",
    deltaX: 0,
    deltaY: 0,
    novalidate: false,
    editable: true,
    disabled: false,
    readonly: false,
    validateOnCreate: true,
    validateOnBlur: false,
    events: {
        focus: _a, blur: _f, mouseenter: _13, mouseleave: _16, click: function (e) {
            var t = $(e.data.target);
            if (t.attr("type") == "checkbox" || t.attr("type") == "radio") {
                t.focus().validatebox("validate");
            }
        }
    },
    val: function (_3e) {
        return $(_3e).val();
    },
    err: function (_3f, _40, _41) {
        _19(_3f, _40, _41);
    },
    tipOptions: {
        showEvent: "none",
        hideEvent: "none",
        showDelay: 0,
        hideDelay: 0,
        zIndex: "",
        onShow: function () {
            $(this).tooltip("tip").css({ color: "#000", borderColor: "#CC9933", backgroundColor: "#FFFFCC" });
        }, onHide: function () {
            $(this).tooltip("destroy");
        }
    }, rules: {
        email: {
            validator: function (_42) {
                return /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i.test(_42);
            }, message: "Please enter a valid email address."
        }, url: {
            validator: function (_43) {
                return /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i.test(_43);
            }, message: "Please enter a valid URL."
        }, length: {
            validator: function (_44, _45) {
                var len = $.trim(_44).length;
                return len >= _45[0] && len <= _45[1];
            }, message: "Please enter a value between {0} and {1}."
        }, remote: {
            validator: function (_46, _47) {
                var _48 = {};
                _48[_47[1]] = _46;
                var _49 = $.ajax({ url: _47[0], dataType: "json", data: _48, async: false, cache: false, type: "post" }).responseText;
                return _49 == "true";
            }, message: "Please fix this field."
        }
    }, onBeforeValidate: function () {
    }, onValidate: function (_4a) {
    }
};

$.fn.window.defaults = $.extend({}, $.fn.panel.defaults, { zIndex: 9000, draggable: true, resizable: true, shadow: true, modal: false, border: true, inline: false, title: "New Window", collapsible: true, minimizable: true, maximizable: true, closable: true, closed: false, constrain: false });