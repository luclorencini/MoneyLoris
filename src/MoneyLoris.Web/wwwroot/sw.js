
self.addEventListener('install', event => {
    //console.log('SW instalado');
});

self.addEventListener('activate', event => {
    //console.log('SW ativado');
});

self.addEventListener('fetch', event => {
    //console.log('fetch interceptado: ', event.request.url);
});