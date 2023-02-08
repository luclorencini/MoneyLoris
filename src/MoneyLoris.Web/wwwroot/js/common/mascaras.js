const mascaras = {

    parseAlfa(txt) {
        let msk = '';
        if (txt) {
            let x = txt.match(/[a-zA-ZÁáÉéÍíÓóÚúÀàÈèÌìÒòÙùâÂêÊîÎôÔûÛüÜãÃõÕÇçºª ]{0,}/);
            msk = x;
        }
        return msk;
    },

    parseData(data) {
        let msk = '';
        if (data) {
            let x = data.replace(/\D/g, '').match(/(\d{0,2})(\d{0,2})(\d{0,4})/);
            msk = !x[2] ? x[1] : x[1] + '/' + x[2] + '/' + x[3];
        }
        return msk;
    },

    parseCpf(cpf) {
        let msk = '';
        if (cpf) {
            let x = cpf.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,3})(\d{0,2})/);
            msk = !x[2] ? x[1] : x[1] + '.' + x[2] + '.' + x[3] + '-' + x[4];
        }
        return msk;
    },

    parseCnpj(cnpj) {
        let msk = '';
        if (cnpj) {
            let x = cnpj.replace(/\D/g, '').match(/(\d{0,2})(\d{0,3})(\d{0,3})(\d{0,4})(\d{0,2})/);
            msk = !x[2] ? x[1] : x[1] + '.' + x[2] + '.' + x[3] + '/' + x[4] + (x[5] ? '-' + x[5] : '');
        }
        return msk;
    },

    parseCep(cep) {
        let msk = '';
        if (cep) {
            let x = cep.replace(/\D/g, '').match(/(\d{0,5})(\d{0,3})/);
            msk = !x[2] ? x[1] : x[1] + '-' + x[2];
        }
        return msk;
    },

    parseTelefoneDDD(tel) {
        let msk = '';
        if (tel) {
            let x = undefined;
            let telne = tel.replace(/\D/g, '');

            if (telne.length < 11)
                x = telne.match(/(\d{0,2})(\d{0,4})(\d{0,4})/);
            else
                x = telne.match(/(\d{0,2})(\d{0,5})(\d{0,4})/);

            msk = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + '-' + x[3];
        }
        return msk;
    },

    parseCrp(crp) {
        let msk = '';
        if (crp) {
            let x = crp.replace(/\D/g, '').match(/(\d{0,2})(\d{0,6})/);
            msk = !x[2] ? x[1] : x[1] + '/' + x[2];
        }
        return msk;
    },

    removeMascara(valor) {
        //retira espaços e os seguintes caracteres: '(', ')', '-', '/', '.' 
        valor = valor
            .replace(/\D/g, '')
            .replace(/\(/g, '')
            .replace(/\)/g, '')
            .replace(/-/g, '')
            .replace(/\//g, '')
            .replace(/\./g, '');
        return valor;
    }
}