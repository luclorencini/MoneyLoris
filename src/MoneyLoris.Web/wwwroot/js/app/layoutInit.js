const layoutInit = {

    setGlobalErrorHandler() {

        window.addEventListener('error',
            event => {
                mensagemSistema.showMensagemErro(`ERRO: Algo inesperado ocorreu: ${event.error.message}`);
                console.error(event.stack);
            });

        window.addEventListener('unhandledrejection',
            event => {
                mensagemSistema.showMensagemErro(`ERRO: Algo inesperado ocorreu: ${event.reason}`);
                console.error(event.promise);
            });
    },
}

layoutInit.setGlobalErrorHandler();