// Minimal service worker: app shell cache.
// Blazor Server'da SignalR connection canlı verisi tarayıcıdan akar; offline yetenekleri
// sınırlıdır. Bu SW sadece basic asset cache + offline yedek sayfa sağlar.
const CACHE = 'refleks360-shell-v1';
const SHELL_ASSETS = [
    '/',
    '/login',
    '/manifest.webmanifest',
    '/favicon.png',
];

self.addEventListener('install', (event) => {
    event.waitUntil(caches.open(CACHE).then((c) => c.addAll(SHELL_ASSETS).catch(() => null)));
    self.skipWaiting();
});

self.addEventListener('activate', (event) => {
    event.waitUntil((async () => {
        const keys = await caches.keys();
        await Promise.all(keys.filter(k => k !== CACHE).map(k => caches.delete(k)));
        self.clients.claim();
    })());
});

self.addEventListener('fetch', (event) => {
    if (event.request.method !== 'GET') return;
    const url = new URL(event.request.url);
    if (url.origin !== self.location.origin) return;
    // API ve auth response'ları cache'leme
    if (url.pathname.startsWith('/api/') || url.pathname.startsWith('/auth/') || url.pathname.startsWith('/_blazor')) return;

    event.respondWith((async () => {
        try {
            const resp = await fetch(event.request);
            if (resp.ok) {
                const cache = await caches.open(CACHE);
                cache.put(event.request, resp.clone());
            }
            return resp;
        } catch {
            const cached = await caches.match(event.request);
            if (cached) return cached;
            // Offline durumda anasayfayı geri ver
            const fallback = await caches.match('/');
            return fallback ?? new Response('Offline', { status: 503 });
        }
    })());
});
