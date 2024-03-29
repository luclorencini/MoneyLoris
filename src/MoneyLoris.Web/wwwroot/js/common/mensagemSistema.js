const mensagemSistema = {

    /**
    * @param {string} mensagem
    */
    showMensagemSucesso(mensagem) {

        let html = `
<div class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="2000">
    <div class="toast-header text-white bg-success">
        <i aria-hidden="true" class="fas fa-check text-white me-2"></i>
        <strong class="me-auto">Tudo certo!</strong>
        <button type="button" class="btn-close text-white" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
    <div class="toast-body">
        ${mensagem}
    </div>
</div>
`;

        let toastEls = bootstrapHelper.htmlToElement(html);
        bootstrapHelper.showToast(toastEls[0]);
    },

    /**
    * @param {string} mensagem
    */
    showMensagemErro(mensagem) {

        let html = `
<div id="erroModal" class="modal" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <div class="d-flex align-items-center">
                        <i aria-hidden="true" class="fas fa-exclamation-circle fa-3x text-warning me-2"></i>
                        <div class="fs-4 fw-bold">Alerta</div>
                    </div>
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div>${mensagem}</div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" data-bs-dismiss="modal">Fechar</button>
            </div>
        </div>
    </div>
</div>
`;

        //se j� existir o modal de erro no dom, remove-o
        let elModalJaExistente = document.getElementById('erroModal');
        if (elModalJaExistente)
            elModalJaExistente.remove();

        //insere o modal calculado acima, o obt�m, e abre o modal
        document.body.insertAdjacentHTML('beforeend', html);
        let el = document.getElementById('erroModal');
        bootstrapHelper.openModal(el);
    },


    /**
    * @param {string} mensagem
    */
    showSplashSucesso(mensagem) {

        if (!mensagem)
            mensagem = 'Tudo certo!';

        let html = `
<div id="splashSucesso" class="splash-message">
    <div class="container">
        <img src="./img/moneyloris-mascot-piscando-transparent.svg">
        <div class="message">${mensagem}</div>
    </div>
</div>
`;

        //se j� existir o splash no dom, remove-o
        let elSplashJaExistente = document.getElementById('splashSucesso');
        if (elSplashJaExistente)
            elSplashJaExistente.remove();

        //insere o splash
        document.body.insertAdjacentHTML('beforeend', html);
    }
}