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

    parseMoney(value) {

        const separadorDecimal = ",";
        const separadorMilesimo = ".";

        //if (!value.includes(',')) {
        //    var value = parseFloat(value).toFixed(2);
        //}        

        ////fail-safe: se valor vem inteiro, insere zeros ao final
        //if (!value.includes('.')) {
        //    value = value + '.00';
        //}
        //else if (value.match(/\.(\d{1})/g)) {
        //    value = value + '0';
        //}
        

        let len = value.length;
        let key = '';
        let i = 0;
        const strCheck = '0123456789';
        let aux = '';

        for (i = 0; i < len; i += 1) {
            if ((value.charAt(i) !== '0') && (value.charAt(i) !== separadorDecimal)) {
                break;
            }
        }
        for (; i < len; i += 1) {
            if (strCheck.indexOf(value.charAt(i)) !== -1) {
                aux += value.charAt(i);
            }
        }
        aux += key;
        len = aux.length;
        if (len === 0) {
            value = '';
        }
        if (len === 1) {
            value = `0${separadorDecimal}0${aux}`;
        }
        if (len === 2) {
            value = `0${separadorDecimal}${aux}`;
        }
        if (len > 2) {
            let aux2 = '';
            let len2 = 0;
            let j = 0;
            for (j = 0, i = len - 3; i >= 0; i -= 1) {
                if (j === 3) {
                    aux2 += separadorMilesimo;
                    j = 0;
                }
                aux2 += aux.charAt(i);
                j += 1;
            }
            value = '';
            len2 = aux2.length;
            for (i = len2 - 1; i >= 0; i -= 1) {
                value += aux2.charAt(i);
            }
            value += separadorDecimal + aux.substr(len - 2, len);
        }
        return value;
    },

    unparseMoney(str) {

        //hack
        str = mascaras.parseMoney(str);

        //retira possiveis pontos separadores de milhar, e transforma a virgula dos centavos em ponto
        str = str.replace(/\./g, '');
        str = str.replace(/\,/g, '.');
        return str;
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