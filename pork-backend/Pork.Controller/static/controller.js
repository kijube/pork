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

function buildResponse(type, flowId, {...data}) {
    return JSON.stringify({
        type,
        flowId,
        ...data
    });
}

function buildFailureResponse(flowId, error) {
    return buildResponse('f', flowId, {error});
}

function buildEvalResponse(flowId, data) {
    return buildResponse('e', flowId, {data});
}

function buildHookResponse(flowId, hookId, method, args = undefined, result = undefined) {
    return buildResponse('h', flowId, {method, hookId, args, result});
}


function connect() {
    // connect websocket
    socket = new WebSocket('ws://localhost:9092/connect');

    socket.onmessage = function (event) {
        const data = JSON.parse(event.data);
        const flowId = data.flowId;
        const code = data.code;
        let response;
        
        console.log(data);

        try {
            response = buildEvalResponse(flowId, eval(code))
        } catch (e) {
            response = buildFailureResponse(flowId, e.toString())
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
                    socket.send(buildHookResponse(undefined, hookId, `console.${method}`, args));
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
            socket.send(buildHookResponse(undefined, hookId, `fetch`, args));
        }
        return originalFetch.apply(window, args)
            .then(response => {
                    response.text().then(body => {
                            const b64body = btoa(body);
                            if (socket) {
                                socket.send(buildHookResponse(null, hookId, "fetch", undefined, {
                                    status: response.status,
                                    headers: headersToJson(response.headers),
                                    body: b64body
                                }));

                            }
                        }
                    )
                }
            )

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