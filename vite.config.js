import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig(({ command, mode }) => {
    return {
        base: mode === 'production' ? '/tour/' : '/',
        plugins: [react({ jsxRuntime: 'classic' })],
    }
})