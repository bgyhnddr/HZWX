$(function () {
    $("body").height(document.documentElement.clientHeight);
    var giftDiv = $("div.gift");
    var width = giftDiv.width();
    giftDiv.height(width);


    canvas = document.getElementById("snow");
    canvas.width = width;
    canvas.height = width;

    $.getUrlParam("promoteId");
    makeSnow("snow");
    //window.location.href = "../../Interface/Promote.ashx?promoteId=" + $.getUrlParam("promoteId") + "&redirect=1";
    alert(document.cookie);
    $.ajax({
        type: "post",
        url: "../../Interface/Promote.ashx",
        dataType: "json",
        data: {
            promoteId: $.getUrlParam("promoteId"),
            redirect: "true"
        },
        success: function (result) {
            window.location.href = result.data;
        },
        error: function (a, b, c) {
            alert(a + b + c);
        }
    });
});

(function ($) {
    $.getUrlParam = function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]);
        return "";
    }

    $.getCookie = function (c_name) {
        if (document.cookie.length > 0) {
            c_start = document.cookie.indexOf(c_name + "=")
            if (c_start != -1) {
                c_start = c_start + c_name.length + 1
                c_end = document.cookie.indexOf(";", c_start)
                if (c_end == -1) c_end = document.cookie.length
                return unescape(document.cookie.substring(c_start, c_end))
            }
        }
        return ""
    }

    $.setCookie = function (c_name, value, expiredays) {
        var exdate = new Date()
        exdate.setDate(exdate.getDate() + expiredays)
        document.cookie = c_name + "=" + escape(value) +
        ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString())
    }
})(jQuery);


