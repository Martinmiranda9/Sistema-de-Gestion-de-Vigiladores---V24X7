/**
 * generate-env.js
 *
 * Este script se ejecuta ANTES del build en Vercel (node generate-env.js && npm run build).
 * Lee las variables de entorno de Vercel y genera el archivo environment.prod.ts
 * que Angular usa en el build de producción.
 *
 * El archivo generado NO se sube a GitHub (.gitignore).
 * Los valores reales viven únicamente en Vercel → Settings → Environment Variables.
 */

const fs = require('fs');
const path = require('path');

// ─── Lee las variables de entorno de Vercel ───────────────────────────────────
const apiUrl       = process.env.API_URL        || '';
const geminiApiKey = process.env.GEMINI_API_KEY || '';

// ─── Advertencias si faltan valores ──────────────────────────────────────────
if (!apiUrl) {
  console.warn('⚠️  API_URL no está definida. El frontend no podrá conectarse al backend.');
}
if (!geminiApiKey) {
  console.warn('⚠️  GEMINI_API_KEY no está definida. Las funciones de IA no funcionarán.');
}

// ─── Genera el contenido del archivo ─────────────────────────────────────────
const content = `// ============================================================
// ARCHIVO AUTO-GENERADO — NO EDITAR MANUALMENTE
// Generado por generate-env.js durante el build en Vercel.
// NO está en git (.gitignore). Los valores vienen de Vercel
// Settings → Environment Variables.
// ============================================================
export const environment = {
  production: true,
  apiUrl: '${apiUrl}',
  geminiApiKey: '${geminiApiKey}'
};
`;

// ─── Escribe el archivo ───────────────────────────────────────────────────────
const outputPath = path.join(__dirname, 'src', 'environments', 'environment.prod.ts');

fs.writeFileSync(outputPath, content, 'utf8');

console.log('✅  environment.prod.ts generado correctamente');
console.log('    API_URL:        ', apiUrl        ? '✓ configurada' : '⚠  vacía');
console.log('    GEMINI_API_KEY: ', geminiApiKey  ? '✓ configurada' : '⚠  vacía');
