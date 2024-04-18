//var ajaxPrexUrl = "http://pv.standard.api.cn";
var ajaxPrexUrl = "http://standarddata.ic360.cn";
var v_RFQ_3_0 = true;
var currentVersion = "";
getFixedVersion();
//获取资源文件版本
function getFixedVersion() {
    $.ajax({
        async: false,
        type: "get",
        url: "/Api/FixedVersions.ashx",
        success: function (data) {
            currentVersion = data;
        },
        error: function (err) {
            console.log(err);
        }
    })
}
function writeScript(url) {
    document.write('<script src="' + url + '?v=' + currentVersion + '"></script' + '>');
}
