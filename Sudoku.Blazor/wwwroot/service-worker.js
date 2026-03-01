const CACHE_VERSION = '__BUILD_VERSION__';
const CACHE_NAME = `sudoku-static-${CACHE_VERSION}`;
const OFFLINE_URL = '/offline.html';

const STATIC_ASSETS = [
    OFFLINE_URL,
    '/manifest.json',
    '/favicon.ico',
    '/images/icons/icon-192x192.png',
    '/images/icons/icon-512x512.png',
    '/images/icons/apple-touch-icon.png'
];

self.addEventListener('install', event => {
    event.waitUntil(caches.open(CACHE_NAME).then(c => c.addAll(STATIC_ASSETS)));
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    event.waitUntil((async () => {
        const names = await caches.keys();
        await Promise.all(names.filter(n => n !== CACHE_NAME).map(n => caches.delete(n)));
        await self.clients.claim();
    })());
});

self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') return;

    const url = new URL(event.request.url);

    // Never intercept Blazor Server circuit traffic
    if (url.pathname.startsWith('/_blazor')) return;

    // Usually bypass APIs too
    if (url.pathname.startsWith('/api/')) return;

    // Network-first for navigations (HTML). Offline fallback is a static page.
    if (event.request.mode === 'navigate') {
        event.respondWith((async () => {
            try {
                return await fetch(event.request);
            } catch {
                return await caches.match(OFFLINE_URL);
            }
        })());
        return;
    }

    // Cache-first only for static assets
    const isStaticAsset = /\.(css|js|png|jpg|jpeg|gif|ico|woff|woff2|ttf|svg|webp)$/i.test(url.pathname);
    if (!isStaticAsset) return;

    event.respondWith((async () => {
        const cache = await caches.open(CACHE_NAME);
        const cached = await cache.match(event.request);
        if (cached) return cached;

        const res = await fetch(event.request);
        if (res && res.status === 200) cache.put(event.request, res.clone());
        return res;
    })());
});