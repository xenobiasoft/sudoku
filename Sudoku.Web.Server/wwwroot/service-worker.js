// Service Worker for XenobiaSoft Sudoku PWA
const CACHE_NAME = 'sudoku-cache-v1';
const RUNTIME_CACHE = 'sudoku-runtime-v1';

// CDN URLs
const FONT_AWESOME_CDN = 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.2.0/css/all.min.css';
const MATERIAL_COMPONENTS_CDN = 'https://unpkg.com/material-components-web@latest/dist/material-components-web.min.css';

// Assets to cache on install
const PRECACHE_URLS = [
  '/',
  '/css/bootstrap/bootstrap.min.css',
  '/css/site.css',
  '/favicon.ico',
  '/images/logo.png',
  FONT_AWESOME_CDN,
  MATERIAL_COMPONENTS_CDN
];

// Install event - cache core assets
self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then((cache) => {
        return cache.addAll(PRECACHE_URLS);
      })
      .then(() => self.skipWaiting())
      .catch((error) => {
        console.error('Service worker install failed:', error);
      })
  );
});

// Activate event - clean up old caches
self.addEventListener('activate', (event) => {
  const currentCaches = [CACHE_NAME, RUNTIME_CACHE];
  event.waitUntil(
    caches.keys().then((cacheNames) => {
      return Promise.all(
        cacheNames.map((cacheName) => {
          if (!currentCaches.includes(cacheName)) {
            return caches.delete(cacheName);
          }
        })
      );
    }).then(() => self.clients.claim())
  );
});

// Fetch event - serve from cache, fallback to network
self.addEventListener('fetch', (event) => {
  // Skip cross-origin requests and browser extensions
  if (!event.request.url.startsWith(self.location.origin) && 
      !event.request.url.startsWith('https://cdnjs.cloudflare.com') &&
      !event.request.url.startsWith('https://unpkg.com')) {
    return;
  }

  // Skip Blazor SignalR connections
  if (event.request.url.includes('/_blazor')) {
    return;
  }

  event.respondWith(
    caches.match(event.request).then((cachedResponse) => {
      if (cachedResponse) {
        return cachedResponse;
      }

      return caches.open(RUNTIME_CACHE).then((cache) => {
        return fetch(event.request).then((response) => {
          // Cache successful GET requests
          if (event.request.method === 'GET' && response.status === 200) {
            cache.put(event.request, response.clone());
          }
          return response;
        }).catch((error) => {
          // Return a custom offline page if available
          console.error('Fetch failed:', error);
          throw error;
        });
      });
    })
  );
});
