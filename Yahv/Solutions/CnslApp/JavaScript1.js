$.classifies.open({});


$.classifies.open = function (option) {
    var options = $.extend(option, $.classifies.open);
    var urlQuery = $.param(options);


    // 调用我们的dialog后相当于有一个frame
    // frame.window.window.document.write();
    // 调用以上代码实现  对 打开传递 Post 到 王增超开发的页面的功能。实际上就是拼写：<form action>
    //具体的产出使用 <input  hiden> 实现
    //</form>
    //window.document.write();
    //request[""]

    //当前的系统与芯达通使用的框架均有弹出窗体的  easyui 用法，
    //建议曾超：在开发中直接使用我们新的框架中 esayui 我们封装的 myDialog 弹出窗体开发统一操作页面
    // myDialog.open();
    //其中Open方法的Url 如下：

    //其中有部署的 http或是https+域名的前缀我们新的框架也有标准，建议询问王亚进行开发
    var openUrl = '你的地址前缀从Http开始或是从/相对绝对地址开始' + '?' + urlQuery;

    //真实 myDialog.open();
    // 把callBackUrl 以  input hidden 方式放到form 中，在点击按钮 post 到后台代码中拼写正确的url + 参数
    // 直接使用  apiHelper.Get 方式进行 请求（返回通知）,参考地址如下：
    // D:\Projects_vs2015\Yahv\Solutions\Yahv.Utils\Http\ApiHelper.cs
};

/*
列表还是有各个子系统开发，这个列表中增加归类按钮
这个按钮就调用$.classifies.open({}); 以下简称：归类窗体
归类窗体根据子系统中提供的参数暂时相应的Ui
Ui需要提供实际的归类功能与公司规定的日志功能
例如：归类页面发现OrderItem已经被锁定就直接提示不进行弹出了。
Ui的功能需要提供回发Post，在Post中 写入我们已经商定好的功能 并  按照 callBackUrl 进行反向通知
*/

$.classifies.open = {
    enterCode: '',
    ClientName: '',//公司名称

    OtherParams: '代表其他所需要的参数',

    callBackUrl: ''//归类完成后post到后台C#代码中  用于请求通知的地址，参数就包涵：mainID与ItemID，ItemID 可空
};

//如果有OrderID的传值与OrderItemID的传值，就使用 ：mainID与ItemID
//如果只有OrderItemID ，那么就直接使用mainID = OrederItemID
//如果只有OrderID，那么就直接使用mianID=OrderID