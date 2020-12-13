var selected = [];
var selectedId = [];
window.onload = function () {
    $('.dropdown-menu').on('click', function (e) {
        e.stopPropagation();
    });
}
function drop(id,url) {
    var content = document.getElementById("content").value;
    $.ajax({
        type: 'GET',
        url: url,
        data: {
            id: id,
            content: content,
            },
    success:
    function (response) {
        // Generate HTML table.
        $('#empList').html(response);
    },
    error:
        function (response) {
        console.log("Error: " + response, response.responseText);
    }
    });
}
function change(e) {
    console.log(e);
    if (e.target.parentElement.className == "list-group-item list-group-item-warning")
        return;
    if (document.getElementsByClassName("list-group-item list-group-item-warning")[0] != undefined)
        document.getElementsByClassName("list-group-item list-group-item-warning")[0].className = "list-group-item";
    e.target.parentElement.className = "list-group-item list-group-item-warning";
    console.log(e.target.parentElement)
    var x = document.getElementsByName("UserId")[0];
    x.value = e.target.getAttribute("id");
}