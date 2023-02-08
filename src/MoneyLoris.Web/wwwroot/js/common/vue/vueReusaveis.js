const ConfirmationDialog = {

    name: 'ConfirmationDialog',

    data: () => ({

        // parametros de configuração do dialog
        titulo: undefined,
        mensagem: undefined,
        okLabel: 'Confirmar', //label do botão de confirmar
        cancelLabel: 'Cancelar', // label do botão de cancelar

        // interno: para resolver a promise
        resolvePromise: undefined,

        // referencia do modal bs
        modal: null
    }),

    methods: {

        aguardarConfirmacao(opts = {}) {

            this.titulo = opts.titulo;
            this.mensagem = opts.mensagem;
            this.okLabel = opts.okLabel;

            if (opts.cancelLabel) {
                this.cancelLabel = opts.cancelLabel;
            }

            this.modal = bootstrapHelper.openModal(this.$refs.confDialogModal);

            return new Promise((resolve, reject) => {
                this.resolvePromise = resolve;
            })
        },

        confirm() {
            bootstrapHelper.closeModal(this.modal);
            this.resolvePromise(true);
        },

        cancel() {
            bootstrapHelper.closeModal(this.modal);
            this.resolvePromise(false);
        }
    },

    template:
`<div class="modal" tabindex="-1" ref="confDialogModal">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <div class="d-flex align-items-center">
                        <i aria-hidden="true" class="fas fa-exclamation-triangle fa-3x text-warning me-2"></i>
                        <div class="fs-4 fw-bold">{{titulo}}</div>
                    </div>
                </h5>
                <button type="button" class="btn-close" aria-label="Close" v-on:click="cancel"></button>
            </div>
            <div class="modal-body">
                <div>{{mensagem}}</div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" v-on:click="cancel">{{cancelLabel}}</button>
                <button type="button" class="btn btn-primary" v-on:click="confirm">{{okLabel}}</button>
            </div>
        </div>
    </div>
</div>`
}