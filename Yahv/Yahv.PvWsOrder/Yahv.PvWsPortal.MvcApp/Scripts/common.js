var navH;
$(".header").load("/head.html", function () {
    //获取要定位元素距离浏览器顶部的距离
    navH = $("#header").offset().top;
    $(".footer").load("/foot.html");
});
$(function () {
    //滚动条事件
    $(window).scroll(function () {
        //获取滚动条的滑动距离
        var scroH = $(this).scrollTop();
        //滚动条的滑动距离大于等于定位元素距离浏览器顶部的距离，就固定，反之就不固定
        if (scroH >= navH) {
            $(".header_top_box").addClass("transport");
            $(".header_main").addClass("transport");
        } else if (scroH < navH) {
            $(".header_top_box").removeClass("transport");
            $(".header_main").removeClass("transport");
        }
    })
    
})
