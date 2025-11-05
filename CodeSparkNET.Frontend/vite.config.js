import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

const isProd = process.env.NODE_ENV === 'production'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    server: {
        port: 50333,
    },
})