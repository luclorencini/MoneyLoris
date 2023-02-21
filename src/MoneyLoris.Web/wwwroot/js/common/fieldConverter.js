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
    },

    /*
     * Converte string no formato dinheiro (D.DDD,DD) em number decimal
     */
    stringToMoney(obj, att) {
        if (obj && obj[att]) {
            obj[att] = mascaras.unparseMoney(obj[att]);
        }
    },

    /*
     * Converte string no formato numérico em number inteiro
     */
    stringToInt(obj, att) {
        if (obj && obj[att]) {
            obj[att] = parseInt(obj[att]);
        }
    }

}