function importJs(src) {
    var ele = document.createElement("script");
    ele.setAttribute("type", "text/javascript");
    ele.setAttribute("src", src);
    document.body.appendChild(ele);
}

function srcJs(src) {
    document.write('<script src="' + src + '"></script>');
}
function srcCss(src) {
    document.write('<link rel="stylesheet" href="' + src + '">');
}

var jScripts = {
    axios: 'https://unpkg.com/axios/dist/axios.min.js',
    jquery: 'https://code.jquery.com/jquery-3.5.0.min.js',
    'vue': 'https://unpkg.com/vue/dist/vue.js',
    'elementJs': 'https://unpkg.com/element-ui/lib/index.js',
    'elementCss': 'https://unpkg.com/element-ui/lib/theme-chalk/index.css'
};


(function () {
    srcCss(jScripts.elementCss);
    srcJs(jScripts.vue);
    srcJs(jScripts.elementJs);
    srcJs(jScripts.jquery);

    var footer = "";
    footer += "<div style=\"height:110px;\">";
    footer += "    <img style=\"position: absolute; top:8px;\" src=\"..\/Images\/logo.png\" \/>";
    footer += "    <div style=\"text-align:center ; padding:35px;\"><span class=\"title-1\">员工考勤系统<\/span><\/div>";
    footer += "    <div class=\"line\"><\/div>";
    footer += "<\/div>";
    footer += '';

    var html = document.createElement('div');
    html.innerHTML = footer;
    var allmain = document.getElementsByClassName('allmain')[0];
    allmain.insertBefore(html.children[0], allmain.firstChild);
})();

