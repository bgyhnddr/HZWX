//  $.feat.nativeTouchScroll=true;
$(document).ready(function () {
    $("#afui").attr("style", "");
    order.getUserInfo(function () {
        if (order.userInfo) {
            $("#page_main_header h1").html(order.userInfo.name);
            $.ui.launch();
            order.bindEvents();
            order.getOrder(function (result) {
                order.updateOrderList(result);
            });
        }
        else {
            document.write("获取用户信息出错，请尝试重新进入应用");
        }

    });

});

var order = {
    userInfo: {},
    load: {
        page_main_load: function () {
        },
        page_detail_load: function () {
            $.ui.toggleNavMenu(false);
        }
    },
    bindEvents: function () {
        addOrder.addEventListener("touchstart", function (ev) {
            $.ui.loadContent("page_detail");
        });

        menu.addEventListener("touchstart", function (ev) {
            WXJS.imagePreview(
                [window.location.origin + "/weixintest/Ordering/menu.jpg"],
                window.location.origin + "/weixintest/Ordering/menu.jpg");
        });

        refreshOrder.addEventListener("touchstart", function (ev) {
            order.getOrder(function (result) {
                order.updateOrderList(result);
            });
        });

        confirmFood.addEventListener("touchstart", function (ev) {
            order.addOrder(function (result) {
                if (result) {
                    order.getOrder(function (list) {
                        order.updateOrderList(list);
                        $.ui.loadContent("page_main");
                    });
                }
                else {
                    alert("更新失败");
                }
            });
        });
    },
    getUserInfo: function (callback) {
        $.ui.unblockUI();
        $.ajax({
            type: "post",
            url: "ashx/GetUserInfoByCode.ashx",
            dataType: "json",
            data: {
                code: getUrlParam("code")
            },
            success: function (result) {
                order.userInfo = result.data;
                if (typeof callback == "function") {
                    callback();
                }
            },
            error: function (a, b, c) {
                document.write(a + b + c);
            }
        });
    },
    getOrder: function (callback) {
        $.ui.blockUI(0.5);
        $.ajax({
            type: "post",
            url: "ashx/GetOrder.ashx",
            dataType: "json",
            data: {
                user: order.userInfo.name ? order.userInfo.name : ""
            },
            success: function (result) {
                $.ui.unblockUI();
                if (result.data) {
                    if (typeof callback == "function") {
                        callback(result.data);
                    }

                }
            },
            error: function (a, b, c) {
                $.ui.unblockUI();
                document.write(a + b + c);
            }
        });
    },
    updateOrderList: function (data) {
        var list = $("#foodList");
        list.empty();
        if (data.length == 0) {
            list.append('<li style="text-align:center"><h4>无点餐</h4></li>');
        }
        else {
            $.each(data, function (index, val) {
                var li = $('<li></li>');
                var lia = $('<a><h2>' + val["OrderFood"] + '</h2><p>数量:' + val["Number"] + '</p><p>价格:' + val["Money"] + '</p></a>');
                lia.data("order", val);
                lia[0].addEventListener("touchstart", function (ev) {
                    $.ui.loadContent("page_detail");
                }, false);
                li.append(lia);
                list.append(li);
            });
        }
    },
    addOrder: function (callback) {
        //if (!order.currentOrder) {
        //    order.currentOrder = new Object();
        //}
        //order.currentOrder["Name"] = order.userInfo.name;
        //order.currentOrder["OrderFood"] = food.value;
        //order.currentOrder["Money"] = price.value;
        $.ui.blockUI(0.5);
        $.ajax({
            type: "post",
            url: "ashx/UpdateOrder.ashx",
            dataType: "json",
            data: {
                order: JSON.stringify({"Name":"","Store":"","Money":""})
            },
            success: function (result) {
                $.ui.unblockUI();
                if (typeof callback == "function") {
                    callback(result.data);
                }
            },
            error: function (a, b, c) {
                $.ui.unblockUI();
                document.write(a + b + c);
            }
        });
    }
}


var getUrlParam = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return "";
}

var getCookie = function (c_name) {
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

var setCookie = function (c_name, value, expiredays) {
    var exdate = new Date()
    exdate.setDate(exdate.getDate() + expiredays)
    document.cookie = c_name + "=" + escape(value) +
    ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString())
}

