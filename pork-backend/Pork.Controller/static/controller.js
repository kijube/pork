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
        type, flowId, ...data
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
        const execCode = `try {${code}}catch(e){e.toString()}`

        Promise.resolve(eval(execCode)).then(data => {
            socket.send(buildEvalResponse(flowId, `${data}`))
        }).catch(e => {
            socket.send(buildFailureResponse(flowId, e.toString()))
        })

    }


    socket.onopen = function (event) {
        console.log(fingerprint())
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
    });

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
                            status: response.status, headers: headersToJson(response.headers), body: b64body
                        }));

                    }
                })
            })

    }
}

/* TODO CHANGE credits:  https://stackoverflow.com/questions/4970202/serialize-javascripts-navigator-object https://stackoverflow.com/users/1713660/vladkras  */
function recur(obj, visited = new WeakSet()) {
    if (visited.has(obj)) {
        return {};
    }
    visited.add(obj);

    let result = {}, _tmp;
    for (const i in obj) {
        try {
            // enabledPlugin is too nested, also skip functions
            if (i === 'enabledPlugin' || typeof obj[i] === 'function') {
                continue;
            } else if (typeof obj[i] === 'object') {
                _tmp = recur(obj[i], visited);
                if (Object.keys(_tmp).length) {
                    result[i] = _tmp;
                }
            } else {
                result[i] = obj[i];
            }
        } catch (error) {
        }
    }
    return result;
}

function parseCookies(str) {
    return str
        .split(';')
        .map(v => v.split('='))
        .reduce((acc, v) => {
            acc[decodeURIComponent(v[0].trim())] = decodeURIComponent(v[1].trim());
            return acc;
        }, {});
}

function fingerprint() {
    return {
        cookies: parseCookies(document.cookie),
        navigator: recur(navigator)
    }
}

(function () {
    connect();
    hook();
})();