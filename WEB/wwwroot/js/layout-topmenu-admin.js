// Counters timer with JWT authentication

const accessTokenKey = 'accessJwtToken';

async function getJwtTokenAsync(username, antiForgeryToken) {

    // получаем данные формы и фомируем объект для отправки
    const formData = new FormData();
    formData.append("grant_type", "password");
    formData.append("username", username);
    formData.append("__RequestVerificationToken", antiForgeryToken);

    // отправляет запрос и получаем ответ
    const response = await fetch("/token", {
        method: "POST",
        headers: { "Accept": "application/json" },
        body: formData
    });

    // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные 
        const data = await response.json();
        // сохраняем в хранилище sessionStorage токен доступа - not recommended as vulnerable
        // TODO: store in cookie
        sessionStorage.setItem(accessTokenKey, data.access_token);
    }
}

async function getCountersDataAsync(url, token) {

    const response = await fetch(url, {
        method: "GET",
        headers: {
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        }
    });

    if (response.ok === true) {
        const data = await response.json();
        $('#userCount').text(data.users);
        $('#commentCount').text(data.comments);
        $('#newsCount').text(data.news);
    }
}

function startTimer() {
    let token = sessionStorage.getItem(accessTokenKey);
    const url = 'api/counters';
    getCountersDataAsync(url, token);
    const delay = 60000;
    let timerId = setTimeout(function tickRecursive() {
        let token = sessionStorage.getItem(accessTokenKey);
        getCountersDataAsync(url, token);
        timerId = setTimeout(tickRecursive, delay);
    }, delay);
}

$(document).ready(function () {
    let $element = $('#counters-box');
    if ($element.length == 1) {
        const antiForgeryToken = $('input:hidden[name="__RequestVerificationToken"]').val();
        const username = $element.data('username');
        getJwtTokenAsync(username, antiForgeryToken).then(startTimer);
    }
});
