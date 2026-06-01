import Axios, { type AxiosError} from "axios"
import env from "../config/env"

const axios = Axios.create({
  baseURL: env.apiBaseUrl,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
})

let isRefreshing = false;
let queue : Array<{
  resolve: () => void;
  reject: (error: unknown) => void;
}> = [];

const processQueue = (error : unknown) => {
  queue.forEach((p) => (error ? p.reject(error) : p.resolve()));
  queue = [];
};

axios.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as typeof error.config & { _retry?: boolean; };

    if (error.response?.status !== 401 || originalRequest._retry) {
      return Promise.reject(error);
    }

    if (originalRequest.url?.includes("/auth")) {
      return Promise.reject(error);
    }

    if (isRefreshing){
      return new Promise<void>((resolve, reject) => {
        queue.push({ resolve, reject });
      }).then(() => axios(originalRequest))
    }

    originalRequest._retry = true;
    isRefreshing = true;

    try{
      await axios.post("/auth/refresh");
      processQueue(null);
      return axios(originalRequest);
    }catch (refreshError){
      processQueue(refreshError);
      return Promise.reject(refreshError);
    }finally{
      isRefreshing = false;
    }
  }
);

export default axios
