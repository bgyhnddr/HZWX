// #region app framework config
$.ui.autoLaunch = true;

$.ui.useOSThemes = false;

$.ui.resetScrollers = false;

$.ui.splitview = false;

$.ui.blockPageScroll();

$.ajaxSettings.error = function (a, b, c) {
    CFUNC.msgDialog({
        title: b,
        text: c ? c.toLocaleString() : "?",
        hiddenText: c ? c.stack : "?"
    });
};

$.ajaxSettings.timeout = 1e4;

if (!(window.DocumentTouch && document instanceof DocumentTouch || "ontouchstart" in window)) {
    var script = document.createElement("script");
    script.src = "inc/js/plugins/jqmobi/af.desktopBrowsers.js";
    var tag = $("head").append(script);
}

Date.prototype.Format = function (fmt) { //author: meizz   
    var o = {
        "M+": this.getMonth() + 1, //月份   
        "d+": this.getDate(), //日   
        "h+": this.getHours(), //小时   
        "m+": this.getMinutes(), //分   
        "s+": this.getSeconds(), //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds() //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
};

// #endregion
$.grep = function (elems, callback, inv) {
    var retVal,
        ret = [],
        i = 0,
        length = elems.length;
    inv = !!inv;

    // Go through the array, only saving the items
    // that pass the validator function
    for (; i < length; i++) {
        retVal = !!callback(elems[i], i);
        if (inv !== retVal) {
            ret.push(elems[i]);
        }
    }

    return ret;
};





$(document).ready(function () {
    var path = window.location.href.substring(0, window.location.href.lastIndexOf("/"));
    $("#imagePreviewPath").val(path + "/themes/default/images/hashiqi.jpg");
    $("#imagePreview").bind("tap", function () {
        var onBridgeReady = function () {
            WeixinJSBridge.invoke('imagePreview', {
                'current': path + "/themes/default/images/hashiqi.jpg",
                'urls': [path + "/themes/default/images/hashiqi.jpg"]
            });
        }

        if (typeof WeixinJSBridge == "undefined") {
            if (document.addEventListener) {
                document.addEventListener('WeixinJSBridgeReady', onBridgeReady, false);
            } else if (document.attachEvent) {
                document.attachEvent('WeixinJSBridgeReady', onBridgeReady);
                document.attachEvent('onWeixinJSBridgeReady', onBridgeReady);
            }
        } else {
            onBridgeReady();
        }

    });

    $("#getNetworkType").bind("tap", function () {
        var onBridgeReady = function () {
            WeixinJSBridge.invoke('getNetworkType', {},
                   function (e) {
                       $("#networkType").val(e.err_msg);
                   });
        }

        if (typeof WeixinJSBridge == "undefined") {
            if (document.addEventListener) {
                document.addEventListener('WeixinJSBridgeReady', onBridgeReady, false);
            } else if (document.attachEvent) {
                document.attachEvent('WeixinJSBridgeReady', onBridgeReady);
                document.attachEvent('onWeixinJSBridgeReady', onBridgeReady);
            }
        } else {
            onBridgeReady();
        }
    });
});