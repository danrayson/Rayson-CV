import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'
import path from "path";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  build: {
    outDir: 'dist',
    chunkSizeWarningLimit: 1000,
    minify: false,
  },
  base: './',
//  server: {
//    host: '0.0.0.0',
//    port: 5176,
//  },
  define: {
    'import.meta.env.VITE_API_BASE_URL': JSON.stringify(process.env.VITE_API_BASE_URL),
  },
  resolve:{
    alias:{
      "@styles": path.resolve(__dirname, "/src/styles"),
    }
  }
})
