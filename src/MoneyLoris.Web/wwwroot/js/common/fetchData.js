/**
 * 
 * Modulo: fetchData
 * trata lógica de fetch (Ajax) do sistema e trata fluxos de tela
 *
 **/

const fetchData = {

    async fetchPostJson(url, dados) {
        return this._fetchDataPost(url, dados, 'json');
    },

    async fetchPostText(url, dados) {
        return this._fetchDataPost(url, dados, 'text');
    },

    async fetchGetJson(url, options) {
        return this._fetchData(url, options, 'json');
    },

    async fetchGetText(url, options) {
        return this._fetchData(url, options, 'text');
    },



    async _fetchDataPost(url, dados, type) {
        
        let requestToken = window.document.querySelector("#RequestVerificationToken").value;

        let options = {
            method: 'POST',
            body: JSON.stringify(dados),
            headers: {
                "X-ANTI-FORGERY-TOKEN": requestToken,
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        }

        return this._fetchData(url, options, type);
    },

    async _fetchData(url, options, type) {

        let dados = undefined;

        try {
            let response = await fetch(url, options);
            dados = await this._handleResponse(response, type);

        } catch (error) {
            throw error;
        }

        return dados;
    },

    async _handleResponse(response, type) {

        //se tipo não for informado, tenta inferir pelo content-type
        if (type == undefined) {
            type = response.headers.get('content-type');
        }

        return this._parseResponse(response, type);
    },

    async _parseResponse(response, type) {

        if (response.ok) {

            let dado = undefined;

            if (type.includes('json')) {
                dado = await response.json();
            }
            else if (type.includes('text')) {
                dado = await response.text();
            }
            else {
                throw new Error(`content-type ${type} não suportado`);
            }

            return dado;
        }
        else {
            throw new Error(`${response.status} - ${response.statusText}`);
        }
    }
}