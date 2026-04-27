export interface AttendanceDayRow {
  day: number;
  entry: string;
  exit: string;
  isDayOff: boolean;
  workedHours: number;
  nightHours: number;
  notes: string;
}
