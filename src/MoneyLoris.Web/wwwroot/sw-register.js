async function registerSW() {

    if ("serviceWorker" in navigator) {
        try {
            let reg = await navigator.serviceWorker.register("sw.js");            
            console.log(reg);

        } catch (e) {            
            console.log(e);
        }
    }
}

registerSW();