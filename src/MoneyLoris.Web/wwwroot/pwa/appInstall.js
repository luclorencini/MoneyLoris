const appInstall = {

    instalar() {

        let appInstaller = document.querySelector('#pwaAppInstaller');

        let deferredPrompt;

        if (this.getPWADisplayMode() == 'browser') {
            appInstaller.classList.remove('d-none');
        }

        window.addEventListener('beforeinstallprompt', e => {
            e.preventDefault();

            //guarda o prompt de instalação automática, para ser mostrada só quando o usuário criar no botão de menu do sistema para instalar
            deferredPrompt = e;
        });

        appInstaller.addEventListener('click', async (e) => {

            deferredPrompt.prompt();
            await deferredPrompt.userChoice;

            deferredPrompt = null;
        });

        window.addEventListener('appinstalled', e => {
            appInstaller.classList.add('d-none');
            alert("Instalação iniciada.");
        });

    },

    getPWADisplayMode() {
        const isStandalone = window.matchMedia('(display-mode: standalone)').matches;
        if (document.referrer.startsWith('android-app://')) {
            return 'twa';
        } else if (navigator.standalone || isStandalone) {
            return 'standalone';
        }
        return 'browser';
    }
}

appInstall.instalar();