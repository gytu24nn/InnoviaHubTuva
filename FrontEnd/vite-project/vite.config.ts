import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5099',
        changeOrigin: true,
      },
      // SignalR
       '/NotificationHub': {
        target: 'http://localhost:5099',
        ws: true,
        changeOrigin: true,
      },
      '/BookingHub': {
        target: 'http://localhost:5099',
        ws: true,
        changeOrigin: true,
    }
  }
  }
})
