const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.trim() || "http://localhost:5178/api"

const env = {
  apiBaseUrl: apiBaseUrl.replace(/\/$/, ""),
  googleMapsApiKey: import.meta.env.VITE_GOOGLE_MAPS_API_KEY?.trim() || "",
  appName: "KLINIQ",
}

export default env
