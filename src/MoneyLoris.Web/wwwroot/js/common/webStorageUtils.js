const localStorageUtils = {

    salvarObjeto(chave, o) {
        const jsonObj = JSON.stringify(o)
        localStorage.setItem(chave, jsonObj);
    },

    obterObjeto(chave) {
        const jsonObj = localStorage.getItem(chave);
        const o = JSON.parse(jsonObj);
        return o;
    },

    removeObjeto(chave) {
        localStorage.removeItem(chave);
    }
}

const sessionStorageUtils = {

    salvarObjeto(chave, o) {
        const jsonObj = JSON.stringify(o)
        sessionStorage.setItem(chave, jsonObj);
    },

    obterObjeto(chave) {
        const jsonObj = sessionStorage.getItem(chave);
        const o = JSON.parse(jsonObj);
        return o;
    },

    removeObjeto(chave) {
        sessionStorage.removeItem(chave);
    }
}