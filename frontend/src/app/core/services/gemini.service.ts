import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

/**
 * Estructura esperada del JSON que devuelve Gemini al analizar una planilla.
 */
export interface OcrAttendanceResult {
  /** Mes de la planilla (1-12 o texto como "Abril") */
  month?: string | number;
  /** Año de la planilla */
  year?: number;
  /** Nombre del vigilador encontrado en la imagen */
  guardName?: string;
  /** Nombre del objetivo/lugar de trabajo */
  workplace?: string;
  /** Filas diarias extraídas */
  rows: OcrRow[];
}

export interface OcrRow {
  /** Número de día (1-31) */
  day: number;
  /** Hora de entrada en formato HH:mm o vacío */
  entry: string;
  /** Hora de salida en formato HH:mm o vacío */
  exit: string;
  /** ¿Es franco/día libre? */
  isDayOff: boolean;
  /** Observaciones opcionales */
  notes: string;
}

const OCR_PROMPT = `Analizá esta planilla de control de horarios de vigilancia.
Extraé los datos de cabecera y el detalle de cada día.

Devolvé ESTRICTAMENTE solo un objeto JSON válido (sin markdown, sin bloques de código, sin texto adicional) con esta estructura exacta:
{
  "month": "<mes como número 1-12 o nombre en español>",
  "year": <año como número>,
  "guardName": "<apellido y nombre del vigilador si se ve>",
  "workplace": "<nombre del objetivo o lugar si se ve>",
  "rows": [
    {
      "day": <número de día>,
      "entry": "<hora entrada en formato HH:mm, o vacío si no hay>",
      "exit": "<hora salida en formato HH:mm, o vacío si no hay>",
      "isDayOff": <true si es franco/descanso, false si no>,
      "notes": "<observaciones si las hay, o cadena vacía>"
    }
  ]
}

Si un campo no se puede leer con claridad, dejalo como vacío ("") o null.
Los horarios deben estar en formato HH:mm de 24 horas.
No incluyas días que estén completamente en blanco si el mes tiene menos de 31 días.`;

const SYSTEM_INSTRUCTION = 'Sos un sistema experto en lectura óptica de planillas de control de asistencia y horarios. Tu única tarea es extraer datos y devolver JSON estructurado.';

@Injectable({
  providedIn: 'root'
})
export class GeminiService {

  private readonly apiUrl = 'https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent';

  /**
   * Procesa un archivo de imagen y extrae los datos de la planilla usando Gemini Vision.
   * @param imageFile Archivo de imagen de la planilla manuscrita
   * @returns Promise con los datos extraídos
   */
  async processAttendanceSheet(imageFile: File): Promise<OcrAttendanceResult> {
    const base64 = await this.fileToBase64(imageFile);

    const fullPrompt = `${SYSTEM_INSTRUCTION}\n\n${OCR_PROMPT}`;

    const body = {
      contents: [
        {
          role: 'user',
          parts: [
            { text: fullPrompt },
            {
              inlineData: {
                mimeType: imageFile.type,
                data: base64
              }
            }
          ]
        }
      ]
    };

    const url = `${this.apiUrl}?key=${environment.geminiApiKey}`;

    const response = await fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body)
    });

    if (!response.ok) {
      const errBody = await response.text();
      let detail = `Error ${response.status}`;
      try {
        const parsed = JSON.parse(errBody);
        detail = parsed?.error?.message || detail;
      } catch { /* ignore */ }
      throw new Error(`Gemini API respondió con error: ${detail}`);
    }

    const data = await response.json();
    const text = data?.candidates?.[0]?.content?.parts?.[0]?.text?.trim();

    if (!text) {
      throw new Error('Gemini no devolvió contenido de texto en la respuesta.');
    }

    // Limpiar posibles bloques markdown que el modelo agregue
    const jsonText = text
      .replace(/^```json\s*/i, '')
      .replace(/^```\s*/i, '')
      .replace(/\s*```$/i, '')
      .trim();

    try {
      return JSON.parse(jsonText) as OcrAttendanceResult;
    } catch {
      throw new Error(`La respuesta de la IA no es un JSON válido. Respuesta recibida:\n${text.substring(0, 300)}`);
    }
  }

  private fileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        const dataUrl = reader.result as string;
        // Extraer solo el contenido base64 sin el prefijo "data:image/...;base64,"
        resolve(dataUrl.split(',')[1]);
      };
      reader.onerror = () => reject(new Error('No se pudo leer el archivo de imagen.'));
      reader.readAsDataURL(file);
    });
  }
}
