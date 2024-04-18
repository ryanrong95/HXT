var selectedIndex;//存储选择业务的数组的下标
//帮助文件的url地址
var HelpUrl = "";

var businessJson = null;//左侧业务导航数据
//选择公司数据
var treeData = [{
    "id": 1,
    "text": "全球",
    "children": []
}];

//方法
// 获取json
function getJson(cb) {
    $.ajax({
        type: "get",
        url: "/confers/tsconfig.json",
        dataType: "json",
        success: function (data) {
            cb(data);
        },
        error: function (err) {
            console.log(err)
        }
    });
}

//获取公司数据
function getComponeyData(cb) {
    $.ajax({
        type: "get",
        url: "/Api/Leagues.ashx",
        success: function (data) {
            cb(data);
        },
        error: function (err) {
            console.log(err)
        }
    });
}
//设置菜单显示,以及右侧tabs页刷新
function setMenuShow(nodeID) {
    getJson(function (data) {
        var confers = []//可以隐藏的业务
        var noShowArr = []//不展示的业务数组
        var leagueArr = []//应该显示的公司
        var val = data[0].business;
        confers.push(val)
        for (var i = 1; i < data.length; i++) {
            if (confers.indexOf(data[i].business) == -1) {
                confers.push(data[i].business)
            }
        }
        if (nodeID == "1") {
            noShowArr = confers;
            leagueArr = []
        } else {
            leagueArr = data.filter(function (item) {
                return item.leagueID == nodeID
            })
            if (leagueArr.length) {
                var leagueArr2 = []
                for (var j = 0; j < leagueArr.length; j++) {
                    leagueArr2.push(leagueArr[j].business)
                }
                noShowArr = []//不展示的数组
                for (var j = 0; j < confers.length; j++) {
                    if (leagueArr2.indexOf(confers[j]) == -1) {
                        noShowArr.push(confers[j])
                    }
                }
            }
        }

        /*多系统样式修改2021-07-05-dialogue隐藏*/

        //$("#systemSwitch ul li").each(function () {
        //    $(this).show();
        //})
        $("#systemSwitch_select ul li").each(function () {
            $(this).show();
        })
        if (noShowArr.length) {
            for (var l = 0; l < noShowArr.length; l++) {
                hideSystem(noShowArr[l])
            }
            var txt = $('.header-left-business-single').css('display') === 'none' ? $('.header-left-business-multiple span').text() : $('.header-left-business-single span').text()
            if (noShowArr.indexOf(txt) != -1) {
                changeSystemFun(0)
            } else {
                refreshAllTabs()
            }
        } else {
            refreshAllTabs()
        }
    })
}
//nav添加indexId
function setIndexId(data) {
    var r = '_' + Math.random().toString().substring(2);
    var index = 0
    for (var i = 0; i < data.length; i++) {
        for (var j = 0; j < data[i].children.length; j++) {
            data[i].children[j].indexId = r + index++;
        }
    }
    return data;
}

//添加nav图标
function addNavIcon(data) {
    var length = data.length;
    $.each(data, function (index, item) {
        $.each(item.Menu, function (inde, ite) {
            $.each(ite.children, function (ind, it) {
                it.iconCls = 'side-icon';
            })
        })
    })
    return data;
}
var sideData = [];
//初始化左侧业务以及导航
function init(data) {
    var index = 0;

    //默认是上次选择的系统模块
    if (getCookie('ydcx_Yahv.Erp.SelectedSystem')) {
        index = getCookie('ydcx_Yahv.Erp.SelectedSystem');
    }

    if (data.length <= index) {
        index = 0;
    }

    data = addNavIcon(data);
    //初始化数据
    sideData = data[index].Menu;
   
    $(".LogoUrl").attr('src', data[index].LogoUrl); //logo地址
    $(".FirstUrl").attr('src', data[index].FirstUrl); //首页地址
    $(".header-left-comName h1").text(data[index].Company);
    HelpUrl = businessJson[index].HelpUrl;//帮助地址
    //左侧导航组件配置
    $('#sm').sidemenu({
        data: setIndexId(sideData),
        multiple: false,
        onSelect: function (item) {
            addPanel(item);

        }
    });

    //-当左侧列表只有一组数据的时候，默认展开子菜单，去掉主菜单标题-
    if (sideData.length == 1) {
        //$('#sm').find('.panel-header').hide();//法一：隐藏一级菜单
        $('#sm').find('.panel-header').addClass("accordion-header-selected")//法二：给一级菜单背景添加蓝色样式
        $('#sm').find('.panel-body').show();
    }

    $(".header-left-business span").text(data[index].Name);

    //判断如果该用户只有一个业务，则不显示下箭头
    if (data.length == 1) {
        $(".header-left-business-multiple").hide();
        $(".header-left-business-single").show();
    } else if (data.length > 1) {
        $(".header-left-business-single").hide();
        $(".header-left-business-multiple").show();
    }

    if (data.length > 1) {
        for (var i = 0; i < data.length; i++) {
            //第一种切换系统的方法
            //var $li = $("<li><img src=" + data[i].IconUrl + " /><span>" + data[i].Name + "</span></li>")
            //$(".header-business-list").append($li);

            //第二种切换系统的方法
            var $li2 = $("<li><img src=" + data[i].LogoUrl + " /><span>" + data[i].Name + "</span></li>")

            /*多系统样式修改2021-07-05-dialogue隐藏*/
            //$("#systemSwitch ul").append($li2);

            $("#systemSwitch_select ul").append($li2);
        }
    }
    selectedIndex = index;
}

//查找左侧应该有选中状态的位置
function searchSideData(data, id) {
    var num;
    var num1 = 0;
    for (var i = 0; i < data.length; i++) {
        if (i > 0) {
            num1 += data[i - 1].children.length;
        }
        for (var j = 0; j < data[i].children.length; j++) {
            if (data[i].children[j].indexId == id) {
                if (i == 0) {
                    num = j
                } else {
                    num = num1 + j;
                }
                break;
            }

        }
    }
    return num;
}

//左侧nav打开或者关闭
function toggleFun() {
    if ($(".toggleSide").hasClass('open')) {
        $('#toplayer').layout('collapse', 'west');
        $(".toggleSide").addClass('close');
        $(".toggleSide").removeClass('open');
    } else {
        $('#toplayer').layout('expand', 'west');
        $(".toggleSide").addClass('open');
        $(".toggleSide").removeClass('close');
    }
}

//点击左侧导航或者点击tabs的某一项，刷新页面。
function updateTab(title) {
    var tab = $('#tabs').tabs('getSelected');  // 获取选择的面板
    $('#tabs').tabs('update', {
        tab: tab,
        options: {
            title: title
        }
    });
}

//添加tab，以及在已经添加的情况下打开对应的tab
function addPanel(item) {
    var indexIdFlag = false;
    var m = $('#tabs').tabs('tabs');
    var tabsLength = m.length;
    //当前的索引
    var currentIndex;
    for (var i = 0; i < tabsLength; i++) {
        if (m[i].panel('options').id == item.indexId) {
            indexIdFlag = false;
            currentIndex = i;
            break;
        } else {
            indexIdFlag = true;
        }
    }

    if (!indexIdFlag) {
        $('#tabs').tabs('selectById', item.indexId);

        //增加强制刷新逻辑

        //菜单
        var wcontent = window.frames[currentIndex].window;
        if (!!wcontent) {

            //约定示例
            //top.$.backLiebiao = {
            //    kkk: 1,
            //    Ruturn: function () {
            //        this.kkk = ''
            //    }
            //};

            //top.$.backLiebiao 为内容也控制回转的 方法

            if (!!top.$.backLiebiao && !!top.$.backLiebiao.Ruturn) {
                top.$.backLiebiao.Ruturn();
                delete top.$.backLiebiao;
            } else {
                wcontent.location.reload();
            }
        }
        //alert(JSON.stringify({
        //    '标签:': item.indexId,
        //    'indexIdFlag:': indexIdFlag,
        //}));

        //放宽到30个
    } else if (tabsLength < 30 && indexIdFlag) {
        $('#tabs').tabs('add', {
            title: item.text,
            content: '<div class="easyui-panel" data-options="fit:true,border:false" title="" style="padding: 1px;"><script>//alert(1111);</script><iframe class="workSpace" src="' + item.url + '" style="background-color:white;width:100%;height:100%;"></iframe></div>',
            id: item.indexId,
            closable: true
        });

        //放宽到30个
    } else if (tabsLength >= 30 && indexIdFlag) {
        var idVal = $('#tabs').tabs('getSelected').panel("options").id;
        var num = searchSideData(sideData, idVal);

        $('#sm').find('.sidemenu-tree').find('li').find(".tree-node").removeClass("tree-node-selected");
        $('#sm').find('.sidemenu-tree').find('li').eq(num).find(".tree-node").addClass("tree-node-selected");

        $.timeouts.alert({
            position: "CC",
            msg: "选项卡不得超过10个",
            type: "warning"
        })
        //$.messager.alert('警告', '选项卡不得超过10个!', 'warning');
    }
    tabsLength = $('#tabs').tabs('tabs').length;
}

//打开选择公司的界面
function selectCom() {
    $('#selectCom').dialog('open')
}

//打开消息阅读界面
function openReadList() {
    $.myDialog({
        url: 'msg.aspx',
        isHaveOk: false,
        title: '消息列表',
        width: "70%"
    });
}

//打开修改密码的界面
function openChangePsd() {
    $.myDialog({
        url: '/Changes/Passwords.aspx',
        isHaveOk: false,
        isHaveCancel: false,
        title: '修改密码',
        width: 600,
        height: 400
    });
}

//退出
function logout() {
    location.replace('/Logout.aspx');
}



var heartsFlag;//心跳标志
var lastTime;//发出公告最近的时间
var requestInterval = 60 * 1000;//公告请求间隔时间
var HeartbeatInterval = 30 * 1000;//心跳请求间隔时间
var intervaltwinkle = 800;//闪烁时间

var noReadedCount = 0;//没有阅读的公告的数量
//设置指示灯颜色
function setLightColor(ele, bg) {
    $(ele).css("background", bg);
}

//获取后台公告数据
function hearts(cb) {
    $.ajax({
        type: "get",
        url: "/Api/hearts.ashx",
        success: function (data) {
            cb(data);
        },
        error: function () {
            heartsFlag = 3;//代表没有获取到数据
        }
    });
}

function useHearts() {
    hearts(function (data) {
        if (data.status && data.status === 200) {
            heartsFlag = 1;//代表正常数据
        } else if (data.count) {
            heartsFlag = 2;//代表繁忙数据
        }
    })
}

//获取消息列表数据
function getMsgList(cb) {
    $.ajax({
        type: "get",
        url: "/api/Notices.ashx",
        success: function (data) {
            cb(data);
        },
        error: function (err) {
            console.log(err);
        }
    })
}

//获取消息列表数据
function useGetMsgList() {
    getMsgList(function (data) {
        noReadedCount = 0;
        for (var i = 0; i < data.length; i++) {
            if (data[i].Readed == false) {
                noReadedCount++;
            }
        }

        $(".header-right-msg").find("span").find("b").text(noReadedCount);
    })
}
//打开弹出框系统
function openSystemSwitch() {

    /*多系统样式修改2021-07-05-dialogue隐藏*/
    //$('#systemSwitch').dialog('open');

    $('#systemSwitch_select').stop().show();
}
//更换业务
function changeSystemFun(element) {
    var index = $(element).index();
    if (typeof (element) == "number") {
        index = element;
    }
    if (index != selectedIndex) {
        $("#tabs .tabs li").each(function (i, o) {
            //获取所有可关闭的选项卡
            var tab = $(".tabs-closable", this).text();
            $("#tabs").tabs('close', tab);
        });
        $(".header-left-business span").text(businessJson[index].Name);
        $(".LogoUrl").attr("src", businessJson[index].LogoUrl);
        $(".FirstUrl").attr('src', businessJson[index].FirstUrl); //首页地址
        $(".header-left-comName h1").text(businessJson[index].Company);
        HelpUrl = businessJson[index].HelpUrl;//帮助地址
        sideData = businessJson[index].Menu;
        $('#sm').sidemenu({
            data: setIndexId(sideData),
            onSelect: function (item) {
                addPanel(item);
            }
        })

        //-当左侧列表只有一组数据的时候，默认展开子菜单，去掉主菜单标题-
        if (sideData.length == 1) {
            //$('#sm').find('.panel-header').hide();//法一：隐藏一级菜单
            $('#sm').find('.panel-header').addClass("accordion-header-selected")//法二：给一级菜单背景添加蓝色样式
            $('#sm').find('.panel-body').show();
        }

        $(".header-business-list").removeClass("open");
        selectedIndex = index;
        setCookie('ydcx_Yahv.Erp.SelectedSystem', index);       //记录选择系统的Index
    }
}
//刷新所有打开的tab
function refreshAllTabs() {
    var tabs = $("#tabs").tabs("tabs");
    for (var i = 0; i < tabs.length; i++) {
        $('#tabs').tabs('update', {
            tab: tabs[i],
            options: {
                title: tabs[i].panel('options').title
            }
        });
    }
}
//隐藏不需要显示的业务
function hideSystem(systemName) {

    /*多系统样式修改2021-07-05-dialogue隐藏*/
    //$("#systemSwitch ul li").each(function () {
    //    if ($(this).find('span').text() == systemName) {
    //        $(this).hide();
    //    }
    //})

    $("#systemSwitch_select ul li").each(function () {
        if ($(this).find('span').text() == systemName) {
            $(this).hide();
        }
    })
}
// 设置cookie
function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + value + ";expires=" + exp.toGMTString();
    //document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}
//读取cookies 
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg))
        return arr[2];
    else
        return null;
}
//删除cookies 
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}
$(function () {
    //禁用
    function bans() {
        //禁用右键
        $('*').bind("contextmenu", function () { return false; });
        //文本选择功能
        $('*').bind("selectstart", function () { return false; });
        //$('*').draggable("disable");
    }
    $(document).ajaxSuccess(function () { bans() });

    bans();

    //获取左侧数据
    $.ajax({
        type: "get",
        url: "/Api/Menus.ashx",
        success: function (data) {
            console.log(data)
            businessJson = data;
            init(data);
        },
        error: function (err) {
            console.log(err);
        }
    });
    $(".header-left-business").on("click", function (e) {
        //第一种切换系统的方式
        //判断如果该用户只有一个业务，则不进行切换业务
        if (businessJson.length == 1) {
            return false;
        }
        //if ($(".header-business-list").hasClass("open")) {
        //    $(".header-business-list").removeClass("open");
        //} else {
        //    $(".header-business-list").addClass("open");
        //}
        //stopPropagation(e);
        //
        //第二种切换系统的方式
        openSystemSwitch();
    })
    //
    $("#tabs").tabs({
        fit: true,
        border: true,
        plain: true,
        //tools: [
        //    {//强制刷新按钮
        //        iconCls: 'icon-reload',
        //        handler: function () {
        //            var tab = $("#tabs").tabs("getSelected");
        //            var title = tab.panel('options').title;//获取面板标题
        //            updateTab(title);
        //        }
        //    }
        //]
    })
    //切换系统
    $(".header-business-list").on("click", "li", function () {
        changeSystemFun(this);
    })
    $(document).bind('click', function () {
        $(".header-business-list").removeClass("open");
    });

    $(".HelpUrl").click(function () {
        if (!HelpUrl) {
            HelpUrl = "javascript:void(0);"
        }
        $.myDialog({
            url: 'help.aspx', isHaveOk: false, title: '帮助文档', width: "70%"
        });
    })

    /*多系统样式修改2021-07-05-dialogue隐藏*/
    //初始化切换系统方式2切换弹出框
    //$('#systemSwitch').dialog({
    //    maximizable: true,
    //    title: '系统切换',
    //    width: 808,
    //    height: 520,
    //    closed: true,
    //    modal: true,
    //    buttons: [{
    //        text: '关闭',
    //        handler: function () {
    //            $('#systemSwitch').dialog("close");
    //        }
    //    }]
    //});

    /*多系统样式修改2021-07-05-dialogue隐藏*/
    //切换系统方式2的切换函数
    //$("#systemSwitch ul").on("click", "li", function () {
    //    changeSystemFun(this);
    //    $("#systemSwitch").dialog('close');
    //})

    $("#systemSwitch_select ul").on("click", "li", function () {
        changeSystemFun(this);
        $("#systemSwitch_select").stop().hide();
    })
    $("#systemSwitch_select").mouseleave(function(){
        $("#systemSwitch_select").stop().hide();
    })
    //消息列表数据
    useGetMsgList();
    var msglistTimer = setInterval(function () {
        useGetMsgList();
    }, requestInterval);

    //消息列表指示灯的变化
    setInterval(function () {
        //如果有未读的，则显示灰蓝相间的灯
        if (noReadedCount && noReadedCount > 0) {
            $(".header-right-msg").toggleClass("gray blue");
        } else {
            //若没有未读的，则是灰色
            $(".header-right-msg").removeClass("blue");
            $(".header-right-msg").addClass("gray");
        }
    }, intervaltwinkle / 2);

    var getHearts;
    //心跳指示灯
    useHearts();
    setInterval(function () {
        useHearts();
    }, HeartbeatInterval)
    //写操作
    //正常数据显示绿色闪烁
    setInterval(function () {
        if (heartsFlag == 1) {
            $('.msg-light').removeClass("yellow");
            $('.msg-light').removeClass("red");
            $('.msg-light').toggleClass("green white");
        }
    }, intervaltwinkle);
    //繁忙数据显示橙绿色闪烁
    setInterval(function () {
        if (heartsFlag == 2) {
            $('.msg-light').removeClass("white");
            $('.msg-light').removeClass("red");
            $('.msg-light').toggleClass("yellow green");
        }
    }, intervaltwinkle / 2);
    //错误数据显示红色
    setInterval(function () {
        if (heartsFlag == 3) {
            $('.msg-light').removeClass("yellow");
            $('.msg-light').removeClass("green");
            $('.msg-light').toggleClass("red white");
        }
    }, intervaltwinkle);


    //选择职位
    var nodeID, nodeText, confers = [];
    //初始化选择职位的弹出框
    $('#selectCom').dialog({
        title: '选择公司',
        width: 700,
        height: 500,
        closed: true,
        modal: true,
        buttons: [{
            text: '确认',
            iconCls: 'icon-yg-confirm',
            handler: function () {
                $(".selectCom").text(nodeText);
                setMenuShow(nodeID)
                if (nodeID == '1') {
                    delCookie('ydcx_Yahv.Erp.League');
                } else {
                    setCookie('ydcx_Yahv.Erp.League', nodeID);
                }
                $('#selectCom').dialog("close");
                //alert(nodeID);
            }
        }, {
            text: '取消',
            iconCls: 'icon-yg-cancel',
            handler: function () {
                var nodeIDCookie = getCookie('ydcx_Yahv.Erp.League')
                if (!nodeIDCookie) {
                    var node3 = $('#comMsg').tree('find', '1');
                    $('#comMsg').tree('select', node3.target);
                } else {
                    var node3 = $('#comMsg').tree('find', nodeIDCookie);
                    $('#comMsg').tree('select', node3.target);
                }
                $('#selectCom').dialog("close");
            }
        }],
        onClose: function () {
            if (!getCookie('ydcx_Yahv.Erp.League')) {
                var node3 = $('#comMsg').tree('find', '1');
                $('#comMsg').tree('select', node3.target);
            } else {
                var node3 = $('#comMsg').tree('find', getCookie('ydcx_Yahv.Erp.League'));
                $('#comMsg').tree('select', node3.target);
            }
        }
    });

    //默认为：芯达通
    setCookie('ydcx_Yahv.Erp.League', 'B456AEEE0BED035A7CE6DF5373E8C59A');
    //隐藏公司选择
    $('.selectCom,.header-right-componey').hide();

    getComponeyData(function (data) {
        if (data == null || data.length === 0) {
            $('.header-right-componey').hide();
            $(".selectCom").text('');
            if (getCookie('ydcx_Yahv.Erp.League')) {
                delCookie('ydcx_Yahv.Erp.League')
            }
        } else if (data.length) {
            $('.header-right-componey').show();
            $(".selectCom").text('全球');
            var data2 = [];
            for (var i = 0; i < data.length; i++) {
                var el = {}
                el.text = data[i].Name
                el.id = data[i].ID
                data2.push(el)
            }
            treeData[0].children = data2
            //初始化公司职位数据
            $('#comMsg').tree({
                data: treeData,
                animate: true,
                onSelect: function (node) {
                    nodeText = node.text;
                    nodeID = node.id;
                },
                onLoadSuccess: function () {
                    var ID;
                    var cookie = getCookie('ydcx_Yahv.Erp.League')
                    if (cookie) {
                        var isHasCookie = data2.filter(function (item) {
                            return item.id == cookie
                        })
                        if (isHasCookie.length) {
                            var node2 = $('#comMsg').tree('find', isHasCookie[0].id);
                            $('#comMsg').tree('select', node2.target);
                            $(".selectCom").text(node2.text);
                            ID = cookie;
                        } else {
                            ID = "1";
                            delCookie('ydcx_Yahv.Erp.League')
                        }
                    } else {
                        var node3 = $('#comMsg').tree('find', '1');
                        $('#comMsg').tree('select', node3.target);
                        ID = "1";
                    }
                    setMenuShow(ID)
                }
            });
        }
    })
    //全屏功能
    $(".header-right-fullscreen-close").hide();
    $(".header-right-fullscreen").click(function () {
        $(document).toggleFullScreen();
        if ($(document).fullScreen() == false) {
            $(".header-right-fullscreen-close").show();
            $(".header-right-fullscreen-open").hide();
        } else {
            $(".header-right-fullscreen-close").hide();
            $(".header-right-fullscreen-open").show();
        }
    })

    //鼠标滑过用户名展示用户信息
    var left = $(".header-right-person").offset().left;
    var width = $(".header-right-person").width();
    var dis1 = left + width / 2;
    var width2 = $(".user-box").width();
    var dis2 = dis1 - width2 / 2;
    $(".user-box").css('left', dis2);
    $(".header-right-person").hover(function () {
        $(".user-box").show();
    }, function (e) {
        if (e.clientY >= 43 && e.clientY <= 151 && e.clientX >= dis2 && e.clientY <= (dis2 + width2)) {
        } else {
            $(".user-box").hide();
        }
    });
    $(".user-box").hover(function () {

    }, function () {
        $(".user-box").hide();
    });
});

//退出登录
function exitLogin() {
    window.location.replace("/Logout.aspx")
}

//绑定微信
function bindWx() {
    //console.log(model);

    if (!model.StaffID) {
        top.$.messager.alert('提示', '您没有员工ID，不能绑定微信');
        return;
    }

    //该员工是否绑定微信
    if (model.IsBind) {
        $.messager.confirm("操作提示", "您已经绑定微信，确定要更新绑定吗？", function (data) {
            if (data) {
                //do 确定
                window.location.replace("/Outsets/wxBind.aspx");
            }
        });
    }
    else {
        window.location.replace("/Outsets/wxBind.aspx");
    }
}

