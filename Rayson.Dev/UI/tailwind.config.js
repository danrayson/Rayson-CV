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
          "info": "#38bdf8",
          "success": "#bef264",
          "warning": "#fb923c",
          "error": "#ef4444",
        },
        light: {
          "primary": "#0ea5e9",
          "secondary": "#38bdf8",
          "accent": "#7dd3fc",
          "neutral": "#6b7280",
          "base-100": "#111827",
          "info": "#38bdf8",
          "success": "#bef264",
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

