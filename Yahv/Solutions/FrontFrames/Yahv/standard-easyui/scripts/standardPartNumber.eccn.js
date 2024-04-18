/// <reference path="../../jquery-easyui-1.7.6/jquery.easyui.min.js" />
/// <reference path="../../jquery-easyui-1.7.6/jquery.min.js" /> 

//初始化Eddn
(function () {
    //香港国内交货都不做
    var top1 = [
        "3A001.a.11",
        "3A001.a.11.a",
        "3A001.a.11.b",
        "3A001.a.3",
        "3A001.a.3.b",
        "3A001.a.4",
        "3A001.a.7",
        "3A001.a.7.a",
        "3A001.a.7.c",
        "3E991",
        "3A002.h.1.e"];

    //海关不禁运的情况下：可以香港交货,国内交货则需要客户签订民用声明（尽量香港交货）
    //1、问王文娟第三方可不可以报
    //2、问客户能不能接受单抬头发票
    //3、改第三方报关公司
    //4、公司单直接找王文娟改第三方
    var top2 = [
        "3A292",
        "4A994",
        "4A994B",
        "4A994.f",
        "4A994.A",
        "4A994.d.1",
        "4A994.g",
        "5A991",
        "5A991.e",
        "5a991.X",
        "5A991.b",
        "5A991.b.5",
        "5A991.c",
        "5A991.c.3",
        "5A991.c.7",
        "5A991.c.10.b",
        "5A991.b.1",
        "5A991.b.2",
        "5A991.B.4",
        "5A991.b.4.a",
        "5A991.b.4.b",
        "5A991.b.5.a",
        "5A991.B.5.B",
        "9A991",
        "9A991.A",
        "9A991.d",
        "5A991.g",
        "3A331A2",
        "5A991.B.7.D",
        "5A991.H",
        "5A991B4B",
        "5A991.C.1",
        "6A993A",
        "6A993",
        "4A994.k.2",
        "4A994.I",
        "4A994.J",
        "5A991C10A"
    ];

    //海关网不禁运的情况下，供应商能出货，可以做香港交货
    var top3 = [
        "5A991.a",
        "5A991.B.7",
        "5A991.B.7.C",
        "5A991.F",
        "7A994",
        "7B994",
        "3A292.D",
        "8A992",
        "5A991.B.3",
        "5A991.b.7.a",
        "3D991"
    ];

    //香港国内交货都不做
    var top4 = [
        "3A002",
        "5A002",
        "5D002，",
        "5A002.a.1.a",
        "5A002.a.1.enc",
        "6A998.a",
        "3A001.b.2",
        "3A001.a.5.a.4",
        "3A001.b.2.a.4",
        "3A001",
        "1C350",
        "1C351",
        "1C353",
        "1C395",
        "2B350",
        "2B351",
        "2B352",
        "3A228",
        "3B992",
        "3B992B",
        "3B992B4B1",
        "3A882",
        "3A882.c",
        "5D992",
        "5D992.c",
        "5B991"
    ];

    top1 = $.map(top1, function (item, index) {
        return item.toLowerCase();
    });
    top2 = $.map(top2, function (item, index) {
        return item.toLowerCase().replace('.', '');
    });
    top3 = $.map(top3, function (item, index) {
        return item.toLowerCase().replace('.', '');
    });
    top4 = $.map(top4, function (item, index) {
        return item.toLowerCase().replace('.', '');
    });

    top.$.baseData.Eccn = {
        top1: {
            data: top1,
            message: '不能接单',
            color: 'red'
        },
        top2: {
            data: top2,
            message: '国内交货需民用声明',
            color: 'purple'
        },
        top3: {
            data: top3,
            message: '可香港交货',
            color: 'red'
        },
        top4: {
            data: top4,
            message: '不能接单',
            color: 'yellow'
        }
    };

})();