import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

/**
 * Estructura esperada del JSON que devuelve el backend al analizar una planilla.
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

@Injectable({
  providedIn: 'root'
})
export class GeminiService {

  /**
   * Procesa un archivo de imagen y extrae los datos de la planilla enviándolo al backend.
   * @param imageFile Archivo de imagen de la planilla manuscrita
   * @returns Promise con los datos extraídos
   */
  async processAttendanceSheet(imageFile: File): Promise<OcrAttendanceResult> {
    const formData = new FormData();
    formData.append('image', imageFile);

    const url = `${environment.apiUrl}/Gemini/ProcessAttendanceSheet`;

    const response = await fetch(url, {
      method: 'POST',
      body: formData
    });

    if (!response.ok) {
      let detail = `Error ${response.status}`;
      try {
        const errBody = await response.json();
        detail = errBody?.message || detail;
      } catch {
        try {
          detail = await response.text() || detail;
        } catch { /* ignore */ }
      }
      throw new Error(`El servidor respondió con error al procesar la imagen: ${detail}`);
    }

    const data = await response.json();
    return data as OcrAttendanceResult;
  }
}
