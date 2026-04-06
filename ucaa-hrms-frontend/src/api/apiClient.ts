import axios from "axios";

const apiClient = axios.create({
  baseURL: process.env.REACT_APP_API_BASE_URL ?? "https://localhost:7044/api",
  timeout: 20000,
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("ucaa_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default apiClient;
