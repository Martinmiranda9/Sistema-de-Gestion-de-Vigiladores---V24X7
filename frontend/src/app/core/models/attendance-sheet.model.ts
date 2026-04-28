export interface AttendanceSheetRow {
  id?: number;
  day: number;
  entry: string | null;
  exit: string | null;
  isDayOff: boolean;
  workedHours: number;
  nightHours: number;
  notes: string | null;
}

export interface AttendanceSheet {
  id: number;
  securityGuardId: number;
  securityGuardName: string;
  securityGuardDNI: string;
  workplaceId: number;
  workplaceName: string;
  month: number;
  year: number;
  totalWorkedHours: number;
  totalNightHours: number;
  totalExtraHours: number;
  createdAt: string;
  rows: AttendanceSheetRow[];
}

export interface AttendanceSheetCreatePayload {
  securityGuardId: number;
  workplaceId: number;
  month: number;
  year: number;
  totalWorkedHours: number;
  totalNightHours: number;
  totalExtraHours: number;
  rows: Omit<AttendanceSheetRow, 'id'>[];
}
