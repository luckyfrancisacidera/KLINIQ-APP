const VERSION = "kliniq-v2"
const STATIC_CACHE = `${VERSION}-static`
const RUNTIME_CACHE = `${VERSION}-runtime`
const APP_SHELL = [
  "/offline.html",
  "/manifest.webmanifest",
  "/logo.png",
  "/icons/icon-192.png",
  "/icons/icon-512.png",
  "/fonts/Geist-Regular.woff2",
  "/fonts/Geist-SemiBold.woff2",
  "/fonts/Geist-Bold.woff2",
  "/fonts/Geist-ExtraBold.woff2"
]

self.addEventListener("install", (event) => {
  event.waitUntil(caches.open(STATIC_CACHE).then((cache) => cache.addAll(APP_SHELL)))
})

self.addEventListener("activate", (event) => {
  event.waitUntil(
    caches.keys()
      .then((keys) => Promise.all(keys.filter((key) => key.startsWith("kliniq-") && ![STATIC_CACHE, RUNTIME_CACHE].includes(key)).map((key) => caches.delete(key))))
      .then(() => self.clients.claim())
  )
})

const isSensitiveApi = (url) => url.pathname.startsWith("/api/")
const isStaticAsset = (request, url) =>
  request.destination === "style" ||
  request.destination === "script" ||
  request.destination === "font" ||
  request.destination === "image" ||
  /\.(?:css|js|woff2|png|jpg|jpeg|svg|webp)$/.test(url.pathname)

self.addEventListener("fetch", (event) => {
  const { request } = event
  if (request.method !== "GET") return

  const url = new URL(request.url)
  if (url.origin !== self.location.origin || isSensitiveApi(url)) return

  if (request.mode === "navigate") {
    event.respondWith(
      fetch(request)
        .then((response) => response)
        .catch(() => caches.match("/offline.html"))
    )
    return
  }

  if (isStaticAsset(request, url)) {
    event.respondWith(
      caches.match(request).then((cached) => cached || fetch(request).then((response) => {
        if (!response || response.status !== 200 || response.type !== "basic") return response
        const copy = response.clone()
        caches.open(RUNTIME_CACHE).then((cache) => cache.put(request, copy))
        return response
      }))
    )
  }
})

self.addEventListener("message", (event) => {
  if (event.data?.type === "SKIP_WAITING") self.skipWaiting()
  if (event.data?.type === "CLEAR_USER_CACHES") {
    event.waitUntil(caches.delete(RUNTIME_CACHE).then(() => caches.open(RUNTIME_CACHE)))
  }
})
