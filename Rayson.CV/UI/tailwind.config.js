/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./src/components/**/*.{html,tsx,ts}",
    "./src/pages/**/*.{html,tsx,ts}",
    "./src/*.{html,tsx,ts}"
  ],
  theme: {
    extend: {},
  },
  daisyui: {
    themes: [
      {
        dark: {
          "primary": "#0ea5e9",
          "secondary": "#38bdf8",
          "accent": "#7dd3fc",
          "neutral": "#6b7280",
          "base-100": "#111827",
          "base-200": "#1f2937",
          "base-300": "#374151",
          "info": "#38bdf8",
          "success": "#bef264",
          "warning": "#fb923c",
          "error": "#ef4444",
        },
        light: {
          "primary": "#0ea5e9",
          "secondary": "#38bdf8",
          "accent": "#7dd3fc",
          "neutral": "#1f2937",
          "base-100": "#ffffff",
          "base-200": "#f3f4f6",
          "base-300": "#e5e7eb",
          "info": "#38bdf8",
          "success": "#22c55e",
          "warning": "#fb923c",
          "error": "#ef4444",
        },
      },
    ],
    darkTheme: "dark",
    base: true,
    styled: true,
    utils: true,
    themeRoot: ":root",
  },
  plugins: [
    require('daisyui'),
  ],
}

