import { defineConfig } from "vite";
import preact from "@preact/preset-vite";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [preact()],
  build: {
    outDir: "../NetBB/wwwroot",
    rollupOptions: {
      output: {
        entryFileNames: "assets/[name].js",
        assetFileNames: `assets/[name].[ext]`,
      },
    },
  },
});
