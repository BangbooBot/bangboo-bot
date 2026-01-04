/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./index.html",
        "./src/**/*.{vue,js,ts,jsx,tsx}",
    ],
    darkMode: 'class',
    theme: {
        extend: {
            screens: {
                'mobs': '320px',   // mobile small
                'mobm': '375px',   // mobile medium
                'mobl': '425px',   // mobile large
                'tb': '768px',     // tablet
                'lt': '1024px',    // laptop
                'll': '1440px',    // large laptop
            }
        }
    },
    future: {
        hoverOnlyWhenSupported: true,
    },
    plugins: [],
} 