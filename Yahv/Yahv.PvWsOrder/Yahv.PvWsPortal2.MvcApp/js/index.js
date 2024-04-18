$(function () {
    //服务项目模块
    $(".n_mol_zw li").hover(function () {
        $(this).children(".n_mol_desc").stop().animate({ height: "100%" }, 500).end().siblings().children(".n_mol_desc").stop().animate({ height: "125px" }, 300);
        $(this).children("div").addClass("active").end().siblings().children("div").removeClass("active");
    }, function () {
        $(this).children(".n_mol_desc").stop().animate({ height: "125px" }, 300);
        $(".n_mol_zw li").children("div").removeClass("active");
    })

    //芯达通助手模块
    $(".n_mol_assit li").hover(function () {
        $(this).find(".n_fade_bg").stop().fadeIn().end().siblings().find(".n_fade_bg").stop().fadeOut();
        $(this).children("a").addClass("active").end().siblings().children("a").removeClass("active");
    }, function () {
        $(this).find(".n_fade_bg").stop().fadeOut();
        $(".n_mol_assit li").children("a").removeClass("active");
    })
})