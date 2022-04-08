let current = 0;
let send_max = 4;
let last_packet = 10;
let sendings, recvings;

let rcv_ack = -1;
let snd_ack = -1;
let locked = false;

let states = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function init() {
    sendings = document.getElementById("sending");
    recvings = document.getElementById("recving");
    document.getElementById("window").setAttribute("style", `
        margin-left: 0 !important;
    `)
}

function setmove() {
    if (locked) return;
    if (current < send_max && current <= last_packet)
        send_next(current++);
}

function drop(i) {
    switch (states[i]) {
        case 1:
            sendings.children[i].setAttribute("style", `
                background-color: red !important
            `)
            break
        case 2:
            recvings.children[i].setAttribute("style", `
                background-color: red !important
            `)
    }
    if (states[i] !== 0 && states[i] !== 3) states[i]--;
}

async function send_next(i) {
    sendings.children[i].setAttribute("style", `
        transform: translateY(150px);background-color: yellow !important
    `)
    let now_state = ++states[i];
    await sleep(2000)
    if (now_state !== states[i]) {
        locked = true;
        await sleep(2000)
        locked = false;
        send_next(i)
        return;
    }
    now_state = ++states[i];
    console.log(now_state)
    recvings.children[i].setAttribute("style", `
        transform: translateY(-150px);background-color: green !important
    `)
    if (i - 1 === rcv_ack)
        rcv_ack++;
    recvings.children[i].textContent = 'ACK ' + rcv_ack
    let current_ack = rcv_ack;
    sendings.children[i].textContent = ' '
    if (i > current_ack) {
        recvings.children[i].setAttribute("style", `
            background-color: orange !important;transition: transform 0s !important;
        `)
        sendings.children[i].setAttribute("style", `
            background-color: gray !important;
        `)
        sendings.children[i].textContent = 'ACK ' + current_ack
        recvings.children[i].textContent = ' '
    }
    await sleep(2000)
    if (now_state !== states[i])
        return
    states[i]++;
    if (current_ack >= snd_ack)
        snd_ack = current_ack;
    if (i > current_ack) {
        current = current_ack + 2
    }
    document.getElementById("window").setAttribute("style", `
        margin-left: ${104 * (snd_ack + 1)}px !important;
    `)
    send_max = snd_ack + 5;
}
