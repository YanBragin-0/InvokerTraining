//async function apiCall(url, method = 'GET', body = null) {
//    const options = {
//        method: method,
//        headers: {
//            'Content-Type': 'application/json'
//        }
//    };

//    if (body) {
//        options.body = JSON.stringify(body);
//    }
//    let response = await fetch(url, options);
//    if (response.status === 401) {
//        console.warn("Access протух, иду за новым...");
//        const refreshResponse = await fetch('/api/Auth/refresh', { method: 'POST' });

//        if (refreshResponse.ok) {
//            console.log("Рефреш прошел успешно, повторяю запрос");
//            return await fetch(url, options);
//        } else {
//            console.error("Рефреш не удался, сессия истекла");
//            window.location.href = '/Auth/login';
//            return response;
//        }
//    }
//    return response;
//}