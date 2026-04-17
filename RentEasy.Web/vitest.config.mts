import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'
import tsconfigPaths from 'vite-tsconfig-paths'
import path from 'path'

export default defineConfig({
  plugins: [tsconfigPaths(), react()],
  test: {
    environment: 'jsdom',
  },
  resolve: {
    alias: {
      // Next.js 16 ESM compatibility: resolve bare 'next/X' imports to their .js files
      'next/server': path.resolve('./node_modules/next/server.js'),
      'next/headers': path.resolve('./node_modules/next/headers.js'),
      'next/navigation': path.resolve('./node_modules/next/navigation.js'),
      'next/experimental/testing/server': path.resolve(
        './node_modules/next/dist/experimental/testing/server/index.js'
      ),
    },
  },
})
