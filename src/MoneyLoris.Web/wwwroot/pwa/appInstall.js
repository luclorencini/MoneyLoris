const appInstall = {

    instalar() {

        let appInstall = document.querySelector('#pwaAppInstaller');

        let deferredPrompt;
     
        window.addEventListener('beforeinstallprompt', e => {
            e.preventDefault();
            deferredPrompt = e;

            if (this.showAppHint()) {
                appInstall.classList.remove('d-none');
            }
        });
      
        appInstall.addEventListener('click', async (e) => {

            if (this.showAppHint()) {

                deferredPrompt.prompt();
                await deferredPrompt.userChoice;

                deferredPrompt = null;
            }
        });
      
        window.addEventListener('appinstalled', e => {
            appInstall.classList.add('d-none');
            alert("App instalado com sucesso na tela inicial!");
        });

    },

    showAppHint() {
        //só mostra prompt de instalação se não veio do link do pwa
        const urlParams = new URLSearchParams(window.location.search);
        const source = urlParams.get('source');
        return (!source || source != 'pwa');
    }
}

appInstall.instalar();