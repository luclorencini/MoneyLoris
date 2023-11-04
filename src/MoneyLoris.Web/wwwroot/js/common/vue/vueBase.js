//
// Mixins
//

const BaseMixin = {

    data() {
        return {
            loading: false
        }
    },

    computed: {
        //expõe o objeto 'utils' (definido como const em utils.js), de forma a ele ser acessível dentro do componente, incluindo {{handlebars}} nos templates
        utils() {
            return utils;
        }
    },

    mounted() {
        //nextTick só é chamado quando a tela inteira (incluindo todos os subcomponentes) for renderizado na tela
        this.$nextTick(() => {
            this.reloadThirdPartyComponents();
        })
    },

    updated() {
        //nextTick só é chamado quando a tela inteira (incluindo todos os subcomponentes) for renderizado na tela
        this.$nextTick(() => {
            this.reloadThirdPartyComponents();
        })
    },

    methods: {

        /**
         * seta a variavel padrão 'this.loading' como true enquanto executa a função informada, e volta para false quando terminar
         * @param {any} f funcao a ser executada (de preferencia, uma arrow function)
         */
        async setLoadingAndExecute(f) {

            try {
                this.loading = true;

                await f();
            }
            finally {
                this.loading = false;
            }
        },

        /**
         * seta a variavel informada 'campo' como true enquanto executa a função informada, e volta para false quando terminar
         * o campo a ser informado deve ser string, e deve existir no componente. Ex: campo definido em data é loadingArea, então deve passar 'loadingArea' como string. Em javascript, this['loadingArea'] é equivalente a this.loadingArea
         * @param {any} f funcao a ser executada (de preferencia, uma arrow function)
         */
        async setLoadingCustomAndExecute(campo, f) {

            try {
                this[campo] = true;

                await f();
            }
            finally {
                this[campo] = false;
            }
        },

        /**
         * mostra a 'Loading Splash' (div opaca com ícone de 'carregando' enquanto executa a função informada, e a esconde quando terminar
         * @param {any} f funcao a ser executada (de preferencia, uma arrow function)
         */
        async showLoadingSplashAndExecute(f) {

            let splash = window.document.querySelector('#loading-splash');

            try {
                splash.classList.remove('d-none');

                await f();
            }
            finally {
                splash.classList.add('d-none');
            }
        },

        /**
        * @returns {boolean}
        */
        validarForm(form) {

            let isValido = form.checkValidity();

            if (!isValido) {
                form.classList.add('was-validated');
            }

            return isValido;
        },

        /**
         * limpa o formulário após validação dos campos, remove a class de validation do bootstrap (principalmente para formulários em modal)
         * @param {any} form objeto do formulário.
         */
        cleanValidationForm(form) {
            form.classList.remove('was-validated');
        },

        /**
         * Coloca foco no campo informado. É necessário criar um referência na tag, fazendo ref="nomeCampo"
         * Exemplo de uso:
         *   html: <input ref="nomeCampo"  ....>
         *   js:   this.setFocus('nomeCampo')
         * 
         * @param {string} ref string contendo o nome da referência (no exemplo dado, 'nomeCampo')
         * */
        setFocus(ref) {
            this.$nextTick(() => {
                let el = this.$refs[ref];

                if (el)
                    el.focus();
            });
        },

        reloadThirdPartyComponents() {

            ////Bootstrap 5 - setup de tooltips
            //let tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
            //var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            //    return new bootstrap.Tooltip(tooltipTriggerEl, {
            //        container: 'body',
            //        trigger: 'hover'
            //    });
            //})
        }
    }
}


//
// Diretivas
//

// v-ext-loading

const diretivaLoading = {
    mounted(el, binding) {
        diretivaLoadingUtils.init(el, binding);
    },
    updated(el, binding) {
        diretivaLoadingUtils.init(el, binding);
    }
}

const diretivaLoadingUtils = {
    init(el, binding) {
        if (binding.value == true)
            el.classList.add('ext-component-loading');
        else
            el.classList.remove('ext-component-loading');
    }
}

// v-ext-mask

const diretivaMask = {
    mounted(el, binding) {
        diretivaMaskUtils.init(el, binding);
    },
    updated(el, binding) {
        diretivaMaskUtils.init(el, binding);
    }
}

const diretivaMaskUtils = {
    init(el, binding) {

        const maskVal = binding.value;

        if (el.value) {

            switch (maskVal) {

                case 'alfa':
                    el.value = mascaras.parseAlfa(el.value);
                    break;

                case 'data':
                    el.value = mascaras.parseData(el.value);
                    break;

                case 'cpf':
                    el.value = mascaras.parseCpf(el.value);
                    break;

                case 'cnpj':
                    el.value = mascaras.parseCnpj(el.value);
                    break;

                case 'cep':
                    el.value = mascaras.parseCep(el.value);
                    break;

                case 'telefone':
                    el.value = mascaras.parseTelefoneDDD(el.value);
                    break;

                case 'dinheiro':
                    el.value = mascaras.parseMoney(el.value);
                    break;

                default:
                    break;
            }

            if (binding.value == true)
                el.classList.add('ext-component-loading');
            else
                el.classList.remove('ext-component-loading');

        }
    }
}


//
// Criação de app
//

const VueFactory = {

    createApp() {
        let app = Vue.createApp({});
        this._setupReusaveisAndDiretivas(app);
        return app;
    },

    /**
     * @param {any} componenteRaiz Vue a ser usado como raiz do app Vue
     * @param {string} el seletor do elemento onde o app será montado. Ex: '#app'
     */
    createAndMountApp(componenteRaiz, el) {

        let app = Vue.createApp(componenteRaiz);

        this._setupReusaveisAndDiretivas(app);

        //monta o app
        app.mount(el);

        return app;
    },

    _setupReusaveisAndDiretivas(app) {
        //registra componentes reusaveis
        app.component('confirmation-dialog', ConfirmationDialog)

        //registra diretivas

        // v-ext-loading
        app.directive('ext-loading', diretivaLoading);

        // v-ext-mask
        app.directive('ext-mask', diretivaMask);
    }
}