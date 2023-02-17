const fieldConverter = {

    /*
     * Field Converter - conversor de atributos de objetos
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
    },

    /*
     * Converte string formato dinheiro (D.DDD,DD) number decimal
     */
    stringToMoney(obj, att) {
        if (obj && obj[att]) {
            obj[att] = mascaras.unparseMoney(obj[att]);
        }
    },

}