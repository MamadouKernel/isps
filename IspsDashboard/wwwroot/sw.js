// Service worker — Sûreté ISPS CIT
// Stratégie : network-first pour la navigation (données toujours fraîches),
// cache-first pour les assets statiques (CDN + images).
const CACHE = 'isps-cache-v1';
const STATIC_ASSETS = [
    '/images/logo.svg',
    '/images/logo-mark.svg',
    '/manifest.webmanifest'
];

self.addEventListener('install', (event) => {
    event.waitUntil(caches.open(CACHE).then((c) => c.addAll(STATIC_ASSETS)).catch(() => {}));
    self.skipWaiting();
});

self.addEventListener('activate', (event) => {
    event.waitUntil(
        caches.keys().then((keys) => Promise.all(keys.filter((k) => k !== CACHE).map((k) => caches.delete(k))))
    );
    self.clients.claim();
});

self.addEventListener('fetch', (event) => {
    const req = event.request;
    if (req.method !== 'GET') return;

    const url = new URL(req.url);
    // Ne pas mettre en cache les requêtes authentifiées / POST / API sensibles
    const isStatic = /\.(svg|png|jpg|jpeg|webp|css|woff2?)$/i.test(url.pathname)
        || url.origin.includes('cdn.') || url.origin.includes('fonts.');

    if (isStatic) {
        event.respondWith(
            caches.match(req).then((cached) => cached || fetch(req).then((res) => {
                const copy = res.clone();
                caches.open(CACHE).then((c) => c.put(req, copy)).catch(() => {});
                return res;
            }).catch(() => cached))
        );
    } else {
        // network-first pour les pages
        event.respondWith(fetch(req).catch(() => caches.match(req)));
    }
});
