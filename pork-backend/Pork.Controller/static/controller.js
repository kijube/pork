let socket;


function generateId(length = 16) {
    const pool = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    let id = '';
    for (let i = 0; i < length; i++) {
        id += pool[Math.floor(Math.random() * pool.length)];
    }
    return id;
}

function headersToJson(headers) {
    const obj = {};
    headers.forEach((value, key) => {
        obj[key] = value;
    });
    return obj;
}

function buildResponse({type, success, id = undefined, data = undefined, error = undefined, ...extra}) {
    return JSON.stringify({
        type,
        id,
        success,
        data,
        error,
        ...extra
    });
}


function connect() {
    // connect websocket
    socket = new WebSocket('ws://localhost:9092/connect');

    socket.onmessage = function (event) {
        const data = JSON.parse(event.data);
        const id = data.eventId;
        const payload = data.payload;
        let response;

        try {
            response = buildResponse({type: "eval", id, success: true, data: eval(payload)})
        } catch (e) {
            response = buildResponse({type: "eval", id, success: false, error: e.toString()})
        }

        socket.send(response);
    }


    socket.onopen = function (event) {

    }

    socket.onclose = function (event) {
        socket = undefined;
        connect();
    }
}

function hook() {
    // hook console log, error, etc.
    const methods = ['log', 'error', 'warn', 'info', 'debug'];
    methods.forEach(method => {
            const hookId = generateId();
            const original = console[method];
            console[method] = function (...args) {
                if (socket) {
                    socket.send(buildResponse({type: 'hook', success: true, hookId, method: `console.${method}`, args}));
                }
                original.apply(console, args);
            }
        }
    )
    ;

    // hook fetch
    const originalFetch = window.fetch;
    window.fetch = function (...args) {
        const hookId = generateId();
        if (socket) {
            socket.send(buildResponse({type: 'hook', success: true, hookId, method: "fetch", args}));
        }
        return originalFetch.apply(window, args)
            .then(response => {
                response.text().then(body => {
                    const b64body = btoa(body);
                    if (socket) {
                        socket.send(JSON.stringify({
                            type: 'hook', hookId, success: true, method: "fetch", result: {
                                status: response.status,
                                headers: headersToJson(response.headers),
                                body: b64body
                            }
                        }));
                    }
                })
            });
    }

    /*
    // hook xhr
    const originalXhr = window.XMLHttpRequest.prototype.send;
    window.XMLHttpRequest.prototype.send = function (...args) {
        if (socket) {
            socket.send(buildResponse({type: 'xhr', data: {type: "request", args}}));
        }
        return originalXhr.apply(this, args);
    }*/
}

(function () {
    connect();
    hook();
})();