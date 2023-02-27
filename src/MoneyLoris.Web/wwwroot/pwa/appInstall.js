const appInstall = {

    instalar() {

        let appInstaller = document.querySelector('#pwaAppInstaller');

        let deferredPrompt;

        if (appInstaller != null) {

            if (this.getPWADisplayMode() == 'browser') {
                appInstaller.classList.remove('d-none');
            }

            appInstaller.addEventListener('click', async (e) => {

                deferredPrompt.prompt();
                await deferredPrompt.userChoice;

                deferredPrompt = null;
            });
        }

        window.addEventListener('beforeinstallprompt', e => {
            e.preventDefault();

            //guarda o prompt de instalação automática, para ser mostrada só quando o usuário criar no botão de menu do sistema para instalar
            deferredPrompt = e;
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
    },

    isPwaInstalled() {
        return this.getPWADisplayMode() == 'standalone';
    }
}

appInstall.instalar();