﻿const utils = {

    /*
     * Redireciona para a url informada
     */
    redirecionar(url) {
        window.location.href = url;
    },

    /*
     * Obtém o valor do parametro na querystring na url atual do navegador
     */
    obterRequestParameter(param) {
        const queryString = window.location.search;
        const urlParams = new URLSearchParams(queryString);
        const val = urlParams.get(param);
        return val;
    },

    removerAcentuacao(str) {
        let t = str.normalize("NFD").replace(/\p{Diacritic}/gu, "");
        return t;
    },

    estaContido(dado, termoBusca) {

        // retira acentuação
        let t = removerAcentuacao(dado);

        return t.toUpperCase().includes(termoBusca.toUpperCase());
    },

    /*
     * Faz a cópia simples de atributos de um objeto.
     * Retorna um objeto novo contendo os atributos do informado.
     */
    copiaSimples(o) {
        const c = JSON.parse(JSON.stringify(o));
        return c;
    },

    /*
     * Testa se um objeto nulo (null, undefineded) ou se está vazio (ex: let a = {} )
     */
    isNullOrEmpty(obj) {

        if (!obj)
            return true;

        for (var key in obj) {
            if (obj.hasOwnProperty(key))
                return false;
        }
        return true;
    },

    /*
     * Utilitarios de data
     */

    dataCurtaMesAnoInputMonth(strDate) {

        let d = new Date(strDate);

        let mes = d.getMonth() + 1;
        if (mes <= 9) {
            mes = `0${mes}`;
        }

        let ano = d.getFullYear();

        let ext = `${ano}-${mes}`;

        return ext;
    },

    dataCurtaMesAno(strDate) {

        let d = new Date(strDate);

        let mes = d.getMonth() + 1;
        if (mes <= 9) {
            mes = `0${mes}`;
        }

        let ano = d.getFullYear();

        let ext = `${mes}/${ano}`;

        return ext;
    },

    dataCurta(strDate) {

        let d = new Date(strDate);

        let dia = d.getDate();
        if (dia <= 9) {
            dia = `0${dia}`;
        }

        let mes = d.getMonth() + 1;
        if (mes <= 9) {
            mes = `0${mes}`;
        }

        let ano = d.getFullYear();

        let ext = `${dia}/${mes}/${ano}`;

        return ext;
    },

    horaCurta(strDate) {

        let d = new Date(strDate);

        let hor = d.getHours();
        if (hor <= 9) hor = `0${hor}`;

        let min = d.getMinutes();
        if (min <= 9) min = `0${min}`;

        let ext = `${hor}:${min}`;

        return ext;
    },

    horaHMS(strDate) {

        let d = new Date(strDate);

        let hor = d.getHours();
        if (hor <= 9) hor = `0${hor}`;

        let min = d.getMinutes();
        if (min <= 9) min = `0${min}`;

        let seg = d.getSeconds();
        if (seg <= 9) seg = `0${seg}`;

        let ext = `${hor}:${min}.${seg}`;

        return ext;
    },

    /*
     * Validação de Documentos
     */

    validarCPF(cpf) {

        // Retira eventual mascara
        cpf = cpf.replace(/[^\d]+/g, '');

        if (cpf == '') return false;

        // Elimina CPFs invalidos conhecidos	
        if (cpf.length != 11 ||
            cpf == "00000000000" ||
            cpf == "11111111111" ||
            cpf == "22222222222" ||
            cpf == "33333333333" ||
            cpf == "44444444444" ||
            cpf == "55555555555" ||
            cpf == "66666666666" ||
            cpf == "77777777777" ||
            cpf == "88888888888" ||
            cpf == "99999999999")
            return false;

        // Valida 1o digito	
        add = 0;
        for (i = 0; i < 9; i++)
            add += parseInt(cpf.charAt(i)) * (10 - i);
        rev = 11 - (add % 11);
        if (rev == 10 || rev == 11)
            rev = 0;
        if (rev != parseInt(cpf.charAt(9)))
            return false;

        // Valida 2o digito	
        add = 0;
        for (i = 0; i < 10; i++)
            add += parseInt(cpf.charAt(i)) * (11 - i);
        rev = 11 - (add % 11);
        if (rev == 10 || rev == 11)
            rev = 0;
        if (rev != parseInt(cpf.charAt(10)))
            return false;

        return true;
    },

}