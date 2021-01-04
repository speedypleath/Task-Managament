var selected = [];
var selectedId = [];
window.onload = function () {
    $('.dropdown-menu').on('click', function (e) {
        e.stopPropagation();
    });

    var input = document.querySelectorAll('.invform'); // get the input element
    input.forEach(i => {
        i.addEventListener('input', resizeInput); // bind the "resizeInput" callback on "input" event
        resizeInput.call(i); // immediately call the function
    });
    function resizeInput() {
        this.style.width = this.value.length + 'ch';
    }
    $(".selectpicker").change(function (e) {
        console.log("da");
        var selected = $(this).children("option:selected");
        console.log(selected.html());
        var color = selected.attr('class').substr(0, selected.attr('class').indexOf(' '));
        var but = document.querySelector("[role=combobox]");
        but.className = "btn "+color;
    });
    selectval();
    init();
    if (document.title == "Tasks")
        drop(null, '/Task/RenderTasks', null, `List`);
    else if (document.title.slice(0, 7) == "Project") {
        console.log(document.title.split(" ").slice(-1)[0]);
        drop(document.title.split(" ").slice(-1)[0], '/Task/RenderTasks', null, `List`);
    }
    else
        console.log(document.title);
}
function selectval() {
    x = $('.selected').attr('value');
    console.log("aici");
    console.log(x);
    console.log("aici");
    $(function () {
        function show_popup() {
            $(".selectpicker").val(x).trigger('change');
        };
        window.setTimeout(show_popup, 50); // yaaaaaaaaaaaaaaaaaaaaay
    });
}

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
}

function down(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    ev.target.appendChild(document.getElementById(data));
    var id = document.getElementById(data).id;
    var content = ev.target.id;
    drop(id.slice(-1), '/Task/ModifyStatus', content);
}

var observe;
if (window.attachEvent) {
    observe = function (element, event, handler) {
        element.attachEvent('on' + event, handler);
    };
}
else {
    observe = function (element, event, handler) {
        element.addEventListener(event, handler, false);
    };
}
function init() {
    var text = document.getElementById('text');
    if (text == undefined)
        return;
    console.log(text);
    function resize() {
        text.style.height = 'auto';
        text.style.height = text.scrollHeight + 'px';
    }
    /* 0-timeout to get the already changed text */
    function delayedResize() {
        window.setTimeout(resize, 0);
    }
    observe(text, 'change', resize);
    observe(text, 'cut', delayedResize);
    observe(text, 'paste', delayedResize);
    observe(text, 'drop', delayedResize);
    observe(text, 'keydown', delayedResize);

    text.focus();
    text.select();
    resize();
}

function activate(e, action, content) {
    var aux = e.parentElement.childNodes[1];
    console.log(e.parentElement.parentElement.innerHTML)
    if (aux.id == "state1") {
        aux.id = "state2"
        aux.innerHTML = `<form method="post" action="${action}">
        <input name="X-HTTP-Method-Override" type="hidden" value="PUT">
        <input class="form-control smallform" name="Content" value="${content}"/>
        </form>
        <small> press edit again to cancel</small >`;
    }
    else {

        aux.id = "state1";
        aux.innerHTML = `<p>${content}</p>`
    }

}


function resizeInput() {
    this.style.width = this.value.length + "ch";
}


function drop(id, url, content, how) {
    if (content == undefined) {
        var content = document.getElementById("content");
        if (content != null)
            content = content.value;
    }
    console.log(how);
    $.ajax({
        type: 'GET',
        url: url,
        data: {
            id: id,
            content: content,
            how: how,
            },
    success:
        function (response) {
            if (url == "/Profile/ShowInbox")
                $('#inbox').html(response);
            else
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
