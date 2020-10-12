$(document).ready(function () {
    $('#AsdComment').on('click', function (event) {
        let $feed = $('input:hidden[name="userId"]');
        let value = $feed.val();
        console.log(value)
        if (!value) {
            let $url = $('input:hidden[name="loginUrl"]');
            let valueURL = $url.val();
            console.log(valueURL)
            $.get(valueURL);
            //$('#FormCC');
            event.preventDefault();
        }
    });
});
