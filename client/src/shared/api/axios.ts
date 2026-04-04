import Axios from "axios"

import env from "../config/env"

const axios = Axios.create({
  baseURL: env.apiBaseUrl,
  headers: {
    "Content-Type": "application/json",
  },
})

export default axios
