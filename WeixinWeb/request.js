$(function () {
    $("#getToken").click(function () {
        var password = prompt("请输入密码");
        $.ajax({
            type: "get",
            url: "../Interface/GetAccessToken.ashx",
            dataType: "json",
            data: {
                password: password
            },
            success: function (result) {
                $("#token").text(result.access_token);
            },
            error: function (a, b, c) {
                alert(a + b + c);
            }
        });
    });

    $("#customMenu").click(function () {
        var menuValue = $("#menu").val();
        var tokenText = $("#token").text();
        try {
            JSON.parse(menuValue);
            $.ajax({
                type: "post",
                url: "../Interface/CreateMenu.ashx",
                dataType: "json",
                data: {
                    token: tokenText,
                    menu: menuValue
                },
                success: function (result) {
                    alert(result.errmsg);
                },
                error: function (a, b, c) {
                    alert(a + b + c);
                }
            });
        }
        catch (e) {
            alert("菜单格式错误");
        }
    });

    $("#loadPage").load("LoadPage.html");
});