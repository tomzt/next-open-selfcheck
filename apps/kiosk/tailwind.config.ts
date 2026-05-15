import type { Config } from 'tailwindcss'

const config: Config = {
  content: ['./src/**/*.{js,ts,jsx,tsx,mdx}'],
  darkMode: 'class',
  theme: {
    extend: {
      // Font sizes for accessibility controls
      fontSize: {
        'kiosk-sm':   ['1rem',    { lineHeight: '1.5rem' }],
        'kiosk-md':   ['1.25rem', { lineHeight: '1.75rem' }],
        'kiosk-lg':   ['1.5rem',  { lineHeight: '2rem' }],
        'kiosk-xl':   ['2rem',    { lineHeight: '2.5rem' }],
      },
      // Kiosk touch-friendly sizing
      spacing: {
        'touch': '3rem',      // minimum touch target size
        'touch-lg': '4rem',
      },
      colors: {
        // Primary color — overridden by Setup Wizard via CSS variable
        primary: 'rgb(var(--color-primary) / <alpha-value>)',
        'primary-foreground': 'rgb(var(--color-primary-foreground) / <alpha-value>)',
      },
      animation: {
        'fade-in': 'fadeIn 0.3s ease-in-out',
        'slide-up': 'slideUp 0.3s ease-out',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        slideUp: {
          '0%': { transform: 'translateY(1rem)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
      },
    },
  },
  plugins: [],
}

export default config
