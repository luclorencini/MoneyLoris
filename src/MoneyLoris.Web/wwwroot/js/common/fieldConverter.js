const fieldConverter = {

    /*
     * Field Converter - utilitário conversor de atributos de objetos
     * Seu diferencial é chamar o método que faz a conversão e já atualizar o campo informado,
     * Retirando a necessidade de fazer ifs e ternarios toda vez que precisar converter um campo.
     * 
     * obj - o objeto que contém o atributo a ser convertido
     * att - o nome do atributo, em formato string
     * 
     * exemplo de uso:
     *    converter.moneyToString(this.conta, 'limite');
     */

    /*
     * Converte number decimal em string formato dinheiro (D.DDD,DD)
     */
    moneyToString(obj, att) {
        if (obj && obj[att]) {
            obj[att] = utils.numberToString2CD(obj[att]);
        }
        else {
            obj[att] = null;
        }
    },

    /*
     * Converte string no formato dinheiro (D.DDD,DD) em number decimal
     */
    stringToMoney(obj, att) {
        if (obj && obj[att]) {
            obj[att] = mascaras.unparseMoney(obj[att]);
        }
        else {
            obj[att] = null;
        }
    },

    /*
     * Converte string no formato numérico em number inteiro
     */
    stringToInt(obj, att) {
        if (obj && obj[att]) {
            obj[att] = parseInt(obj[att]);
        }
        else {
            obj[att] = null;
        }
    },

    /*
     * Converte objeto Date no formato string YYYY-MM-DD proprio para campo <input type="date">
     */
    dateToInputString(obj, att) {
        if (obj && obj[att]) {
            obj[att] = utils.dateToInput(obj[att]);
        }
        else {
            obj[att] = null;
        }
    },

    /*
     * Converte string data serializada iso "2023-02-20T00:00:00-03:00" no formato string YYYY-MM-DD proprio para campo <input type="date">
     */
    dateStringIsoToInputString(obj, att) {
        if (obj && obj[att]) {
            let d = new Date(obj[att]);
            obj[att] = utils.dateToInput(d);
        }
        else {
            obj[att] = null;
        }
    }

}