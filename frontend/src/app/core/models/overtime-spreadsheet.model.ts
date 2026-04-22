export interface OvertimeSpreadsheetRowPayload {
  securityGuardId: number | null;
  fullName: string;
  dni: string;
  fileNumber: string;
  hours: number;
  total: number;
  verified: boolean;
}

export interface OvertimeSpreadsheetCreatePayload {
  workplaceId: number;
  month: number;
  year: number;
  extraHourRate: number;
  rateValidFrom: string | null;
  rows: OvertimeSpreadsheetRowPayload[];
}

export interface OvertimeSpreadsheetSummary {
  id: number;
  workplaceId: number;
  workplaceName: string;
  month: number;
  year: number;
  extraHourRate: number;
  totalHours: number;
  grandTotal: number;
  rowsCount: number;
  verifiedCount: number;
  createdAt: string;
}

export interface OvertimeSpreadsheetRow {
  id: number;
  securityGuardId: number | null;
  fullName: string;
  dni: string;
  fileNumber: string;
  hours: number;
  total: number;
  verified: boolean;
}

export interface OvertimeSpreadsheetDetail {
  id: number;
  workplaceId: number;
  workplaceName: string;
  month: number;
  year: number;
  extraHourRate: number;
  rateValidFrom: string | null;
  totalHours: number;
  grandTotal: number;
  createdAt: string;
  rows: OvertimeSpreadsheetRow[];
}
